using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Email;
using OnlineFruit_Data.Entity;
using OnlineFruitShop.Model;
using System.ComponentModel.DataAnnotations;

namespace OnlineFruitShop.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<APP.User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ForgotPasswordModel(UserManager<APP.User> userManager, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //    return Page();

            //var user = await _userManager.FindByEmailAsync(Input.Email);
            ////if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            ////{
            ////    return RedirectToPage("/ForgotPasswordConfirmation");
            ////}



            //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var resetLink = Url.Page("/ResetPassword", null, new { token, email = Input.Email }, Request.Scheme);

            //await _emailService.SendEmail(Input.Email, "بازیابی رمز عبور", $"<a href='{resetLink}'>اینجا کلیک کنید</a>");

            //return RedirectToPage("/ForgotPasswordConfirmation");
            var user = await _userManager.FindByEmailAsync(Input.email);
            if (user == null)
            {
                return BadRequest("ایمیل یافت نشد.");
            }
            // دریافت دامنه سایت از تنظیمات یا مقدار پیش‌فرض
            string domain = _configuration.GetValue<string>("EMAIL_CONFIGORATION:LOCAL") ?? "https://localhost:7282";
            // تولید توکن تغییر رمز عبور
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // ایجاد لینک تغییر رمز عبور
            string resetLink = $"{domain}/ResetPassword?email={Input.email}&token={resetToken}";

            // قالب ایمیل
            string emailBody = $@"
        <html>
        <body>
            <h2>درخواست تغییر رمز عبور</h2>
            <p>برای تغییر رمز عبور روی لینک زیر کلیک کنید:</p>
            <a href='{resetLink}'>تغییر رمز عبور</a>
            <p>اگر این درخواست از طرف شما نبوده، این ایمیل را نادیده بگیرید.</p>
        </body>
        </html>";

            // ارسال ایمیل
            await _emailService.SendEmail("بازیابی رمز عبور", emailBody, Input.email);

            return RedirectToPage("/ForgotPasswordConfirmation");
        }
    }
}
