using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Business.Repository.Reposi;
using OnlineFruit_Data.Context;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OnlineFruit_Business.Payment;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages
{
    public class CartModel : PageModel
    {
        private readonly ILogger<CartModel> _logger;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IPaymentService _paymentService;
        private readonly DatabaseContext _databaseContext;
        private readonly IPaymentRepository _paymentRepository;

        public CartModel(
            ILogger<CartModel> logger,
            ICartItemRepository cartItemRepository,
            IPaymentService paymentService,
            DatabaseContext databaseContext,
            IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _cartItemRepository = cartItemRepository;
            _paymentService = paymentService;
            _databaseContext = databaseContext;
            _paymentRepository = paymentRepository;
        }

        // لیست آیتم‌های سبد خرید
        public IEnumerable<CartItem> CartItems { get; set; } = Enumerable.Empty<CartItem>();
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice => TotalPrice - Discount;
        public int CurrentOrderId { get; set; }

        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            try
            {
                UserId = GetUserIdFromClaims();
                if (UserId == 0)
                {
                    _logger.LogWarning("کاربر احراز هویت نشده یا شناسه نامعتبر دارد.");
                    return RedirectToPage("/Account/Login");
                }

                CartItems = await _cartItemRepository.GetAllInUserId(UserId, cancellationToken);

                if (!CartItems.Any())
                {
                    _logger.LogInformation("سبد خرید کاربر {UserId} خالی است.", UserId);
                }
                
                decimal total = CartItems.Sum(item => item.Product.Price * item.Quantity);
                decimal discount = 3; // یا محاسبه‌شده
                decimal final = total - discount;

                ViewData["TotalPrice"] = total.ToString("N0");
                ViewData["Discount"] = discount.ToString("N0");
                ViewData["FinalPrice"] = final.ToString("N0");
                TotalPrice = await _databaseContext.CartItems
                    .Include(x => x.Product)
                    .Where(x => x.UserId == UserId)
                    .SumAsync(x => x.Product.Price * x.Quantity, cancellationToken);

                Discount = 3; // یا محاسبه پویا

                CurrentOrderId = await _paymentRepository.GetCurrentOrderId(UserId, cancellationToken);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در بارگذاری صفحه سبد خرید برای کاربر {UserId}", UserId);
                return RedirectToPage("/Error");
            }
        }

        public async Task OnPostDelete(int id, CancellationToken cancellationToken)
        {
            var item = await _databaseContext.CartItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (item == null)
                throw new KeyNotFoundException("آیتم یافت نشد");

            _databaseContext.CartItems.Remove(item);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }



        private int GetUserIdFromClaims()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }

        //// درخواست پرداخت
        public async Task<IActionResult> OnPostAsync([FromBody] dynamic request)
        {
            int orderId = request?.orderId ?? 0;
            if (orderId == 0)
                return new JsonResult(new { isSuccess = false, errorMessage = "شناسه سفارش نامعتبر است" });

            decimal totalAmount = TotalPrice;

            var paymentResult = await _paymentService.CreatePaymentRequestAsync(
                totalAmount,
                "پرداخت سفارش فروشگاه",
                Url.Page("/Cart", "PaymentCallback", null, Request.Scheme),
                orderId
            );

            if (!paymentResult.IsSuccess)
                return new JsonResult(new { isSuccess = false, errorMessage = paymentResult.ErrorMessage });

            return new JsonResult(new { isSuccess = true, paymentUrl = paymentResult.PaymentUrl });
        }

        // بررسی وضعیت پرداخت بعد از بازگشت از زرین‌پال
        public async Task<IActionResult> OnGetCheckPaymentAsync(string authority, string status)
        {
            if (status != "OK")
                return new JsonResult(new { isSuccess = false, message = "پرداخت انجام نشد یا لغو شد." });

            var verification = await _paymentService.VerifyPaymentAsync(authority, TotalPrice);

            if (verification.IsSuccess)
            {
                return new JsonResult(new
                {
                    isSuccess = true,
                    message = $"پرداخت موفق! شماره پیگیری: {verification.RefId}"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = $"پرداخت ناموفق: {verification.ErrorMessage}"
                });
            }
        }

    }
}
