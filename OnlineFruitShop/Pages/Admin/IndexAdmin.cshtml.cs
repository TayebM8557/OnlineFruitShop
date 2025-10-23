using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages.Admin
{
    public class IndexAdminModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public IndexAdminModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Login/LoginPage");
        }
    }
}
