using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Entity;
using System.Security.Claims;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;

        public ProfileModel(SignInManager<User> signInManager, UserManager<User> userManager, IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContext = httpContext;
            _userRepository = userRepository;
        }

        
        public APP.User User { get; set; } 
        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            var getUserId = _userManager.GetUserId(_httpContext.HttpContext.User);
            var GetId = Convert.ToInt32(getUserId);
            User = await _userRepository.GetById(GetId,cancellationToken);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
