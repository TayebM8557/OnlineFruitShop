using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using OnlineFruit_Data.Entity;

namespace OnlineFruitShop.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<APP.User> _userManager;

        public ResetPasswordModel(UserManager<APP.User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public ResetPasswordInputModel Input { get; set; }

        public class ResetPasswordInputModel
        {
            [Required]
            public string Token { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [MinLength(6)]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("NewPassword")]
            public string ConfirmPassword { get; set; }
        }

        //public void OnGet(string token, string email)
        //{
        //    Input = new ResetPasswordInputModel { Token = token, Email = email };
        //}
        [BindProperty]
        public string Token { get; set; }

        public async Task<IActionResult> OnGetAsync(string token = null, string email = null)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("توکن یا ایمیل نامعتبر است.");
            }

            // ذخیره مقادیر در مدل Input برای استفاده در فرم
            Input = new ResetPasswordInputModel
            {
                Token = token,
                Email = email
            };

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //    return Page();
            Console.WriteLine($"🔹 مقدار دریافت شده در POST - Email: {Input.Email}, Token: {Input.Token}");

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "ایمیل یافت نشد.");
                return Page();
            }

            // بررسی اینکه آیا توکن موجود است یا خیر
            if (string.IsNullOrEmpty(Input.Token))
            {
                ModelState.AddModelError(string.Empty, "توکن تغییر رمز عبور نامعتبر است.");
                return Page();
            }

            // تلاش برای تغییر رمز عبور
            var result = await _userManager.ResetPasswordAsync(user, Input.Token, Input.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToPage("/Account/ResetPasswordConfirmation");
            }

            // اگر خطا رخ دهد، نمایش آن‌ها در صفحه
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
