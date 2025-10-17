using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Entity.Dtos;
using System.Security.Claims;
using OnlineFruit_Data.Context;
using static OnlineFruit_Data.Entity.APP;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;

namespace OnlineFruitShop.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly IProductRepository _productRepository;
        private readonly DatabaseContext _databaseContext;
        private readonly ICartItemRepository _cartItemRepository;

        public IndexModel(SignInManager<User> signInManager,ILogger<IndexModel> logger, IProductRepository productRepository, ICartItemRepository cartItemRepository)
        {
            _signInManager = signInManager;
            _logger = logger;
            _productRepository = productRepository;
            _cartItemRepository = cartItemRepository;
        }

        [BindProperty]
        public int id { get; set; }
        [BindProperty]
        public int quantity {  get; set; }
        public IEnumerable<Product> products { get; set; }
        public int cartItems { get; set; }
        public int UserId { get; set; }

        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null) 
            { 
               UserId = int.Parse(userId);
            }
            
            products = await _productRepository.GetAll(cancellationToken);
            cartItems = await _cartItemRepository.GetUserCartItemTotalQuantity(UserId,cancellationToken);
            ViewData["CartItemCount"] = cartItems; // ذخیره مقدار در ViewData
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostAddToCart(int id, int quantity, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            products = await _productRepository.GetAll(cancellationToken);
            if (userId == null)
            {
                TempData["Error"] = "لطفاً ابتدا وارد شوید.";
                return LocalRedirect("/Login/LoginPage");
            }
            var UserId = int.Parse(userId);
            var product = await _productRepository.GetBy(id, cancellationToken);

            if (product == null)
            {
                TempData["Error"] = "محصول موردنظر یافت نشد.";
                return Page();
            }
            if (quantity <= 0)
            {
                TempData["Error"] = "تعداد باید بیشتر از صفر باشد.";
                return Page();
            }
            if (product.Stock < quantity)
            {
                TempData["Error"] = "موجودی کافی برای این محصول وجود ندارد.";
                return Page();
            }

            // افزودن محصول به سبد خرید
            var cartItem = new CartItemDto
            {
                UserId = UserId,
                ProductId = product.Id,
                Quantity = quantity,
                
            };

            await _cartItemRepository.Create(cartItem, cancellationToken);

            TempData["SuccessMessage"] = "محصول با موفقیت به سبد خرید اضافه شد.";
            return RedirectToPage("Index");
        }


    }
}
