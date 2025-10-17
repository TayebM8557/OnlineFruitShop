using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Entity.Dtos;

namespace OnlineFruitShop.Pages.Login
{
    public class RegisterModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public RegisterModel(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCreate(UserDto model, CancellationToken cancellationToken)
        {
            try
            {
                // اعتبارسنجی
                if (string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.PhoneNumber))
                {
                    TempData["ErrorMessage"] = "حداقل یکی از ایمیل یا شماره موبایل الزامی است";
                    return Page();
                }

                if (string.IsNullOrEmpty(model.Password) || model.Password.Length < 6)
                {
                    TempData["ErrorMessage"] = "رمز عبور باید حداقل 6 کاراکتر باشد";
                    return Page();
                }

                // بررسی تکراری بودن ایمیل
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var existingUserByEmail = await _userRepository.GetByEmail(model.Email, cancellationToken);
                    if (existingUserByEmail != null)
                    {
                        TempData["ErrorMessage"] = "این ایمیل قبلاً ثبت شده است";
                        return Page();
                    }
                }

                // بررسی تکراری بودن شماره موبایل
                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    var existingUserByPhone = await _userRepository.GetByPhoneNumber(model.PhoneNumber, cancellationToken);
                    if (existingUserByPhone != null)
                    {
                        TempData["ErrorMessage"] = "این شماره موبایل قبلاً ثبت شده است";
                        return Page();
                    }
                }

                if (ModelState.IsValid)
                {
                    model.Role = "Customer";

                    var result = await _userRepository.Create(model, cancellationToken);
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "ثبت نام با موفقیت انجام شد. اکنون می‌توانید وارد شوید";
                        return LocalRedirect("/Login/LoginPage");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            TempData["ErrorMessage"] = item.Description;
                            return Page();
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "لطفاً تمام فیلدهای الزامی را پر کنید";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "خطا در ثبت نام";
            }
            
            return Page();
        }
    }
}