using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Entity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OnlineFruit_Data.Context;
using OnlineFruit_Data.Entity.Dtos;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly DatabaseContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SignInManager<User> _signInManager;

        public ProfileModel(DatabaseContext db, UserManager<User> userManager, IHttpContextAccessor httpContext, SignInManager<User> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContext = httpContext;
            _signInManager = signInManager;
        }



        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
        {
            var userIdStr = _userManager.GetUserId(_httpContext.HttpContext.User);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            User = await _db.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            //var userIdStr = _userManager.GetUserId(_httpContext.HttpContext.User);
            //if (!int.TryParse(userIdStr, out var userId))
            //    return Unauthorized();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
                return NotFound();

            ViewData["Title"] = "پروفایل کاربر";
            ViewData["FirstName"] = user.FirstName;
            ViewData["LastName"] = user.LastName;
            ViewData["PhoneNumber"] = user.PhoneNumber;
            ViewData["CreatedAt"] = user.CreatedAt?.ToString("yyyy/MM/dd");

            return Page();
        }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            //if (!ModelState.IsValid)
            //    return Page();

            var userIdStr = _userManager.GetUserId(_httpContext.HttpContext.User);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var existingUser = await _db.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (existingUser == null)
                return NotFound();

            // اطلاعات کاربر
            existingUser.FirstName = User.FirstName;
            existingUser.LastName = User.LastName;
            existingUser.PhoneNumber = User.PhoneNumber;
            existingUser.Email = User.Email;
            existingUser.UserName = User.UserName;
            existingUser.UpdatedAt = DateTime.UtcNow;

            // اطلاعات آدرس
            if (existingUser.Address == null)
                existingUser.Address = new Address();

            if (User.Address != null)
            {
                existingUser.Address.Street = User.Address.Street;
                existingUser.Address.City = User.Address.City;
                existingUser.Address.State = User.Address.State;
                existingUser.Address.Country = User.Address.Country;
                existingUser.Address.PostalCode = User.Address.PostalCode;
                existingUser.Address.plaque = User.Address.plaque;
                existingUser.Address.UpdatedAt = DateTime.UtcNow;
            }

            _db.Users.Update(existingUser);
            await _db.SaveChangesAsync(cancellationToken);

            return RedirectToPage("/Profile");
        }
    }
}
