using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Entity.Dtos;
using System.Threading;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages.Login
{
    public class LoginPageModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User>? _userManager;

        public LoginPageModel(SignInManager<User> signInManager, IUserRepository userRepository, IMapper mapper, UserManager<User>? userManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostLogin(UserDto model, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepository.Login(model, cancellationToken);

                if (result.Succeeded)
                {
                    var user = await _userRepository.GetByEmailOrPhone(model.EmailOrPhone, cancellationToken);
                    if (user == null)
                    {
                        TempData["notUser"] = "کاربر یافت نشد";
                        return RedirectToPage(); // <- اصلاح شد
                    }

                    var roles = await _signInManager.UserManager.GetRolesAsync(user);
                    var roleName = roles.SingleOrDefault();

                    if (roleName == "Customer")
                    {
                        return LocalRedirect("/Index/");
                    }
                    else
                    {
                        return LocalRedirect("/Admin/IndexAdmin");
                    }
                }
                else
                {
                    TempData["notUser"] = "نام کاربری یا رمز عبور اشتباه است";
                    return RedirectToPage(); // <- اصلاح شد
                }
            }
            catch (Exception ex)
            {
                TempData["notUser"] = "خطا در ورود به سیستم";
                return RedirectToPage(); // <- اصلاح شد
            }
        }

        //public async Task<IActionResult> OnPostLogin(UserDto model, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var result = await _userRepository.Login(model, cancellationToken);

        //        if (result.Succeeded)
        //        {
        //            // پیدا کردن کاربر برای دریافت نقش
        //            var user = await _userRepository.GetByEmailOrPhone(model.EmailOrPhone, cancellationToken);
        //            if (user == null)
        //            {
        //                TempData["notUser"] = "کاربر یافت نشد";
        //                return RedirectToAction("OnGet");
        //            }

        //            var roles = await _signInManager.UserManager.GetRolesAsync(user);
        //            var roleName = roles.SingleOrDefault();

        //            if (roleName == "Customer")
        //            {
        //                return LocalRedirect("/Index/");
        //            }
        //            else
        //            {
        //                return LocalRedirect("/Admin/IndexAdmin");
        //            }
        //        }
        //        else
        //        {
        //            TempData["notUser"] = "نام کاربری یا رمز عبور اشتباه است";
        //            return RedirectToAction("OnGet");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["notUser"] = "خطا در ورود به سیستم";
        //        return RedirectToAction("OnGet");
        //    }
        //}
    }
}