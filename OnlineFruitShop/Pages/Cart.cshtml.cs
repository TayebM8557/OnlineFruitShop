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
        private readonly IPaymentService _service;
        private readonly DatabaseContext _databaseContext;

        public CartModel(ILogger<CartModel> logger,ICartItemRepository cartItemRepository ,IPaymentService service, DatabaseContext databaseContext)
        {
            _cartItemRepository = cartItemRepository;
            _service = service;
            _databaseContext = databaseContext;
            _logger = logger;
        }

        public IEnumerable<CartItem> cartItems { get; set; }
        public int UserId { get; set; }
        public decimal totalPrice { get; set; }
        public decimal TotalPrice { get; set; }
        // شناسه سفارش جاری
        public int CurrentOrderId { get; set; }
        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                UserId = int.Parse(userId);
            }
            CurrentOrderId = 123;
            cartItems = await _cartItemRepository.GetAllInUserId(UserId,cancellationToken);
            totalPrice = await _databaseContext.CartItems
                .Where(x => x.UserId == UserId) // اگر می‌خواهید برای کاربر خاصی محاسبه کنید
                .SumAsync(x => x.Product.Price * x.Quantity, cancellationToken);
            ViewData["TotalPrice"] = totalPrice;
            return Page();
        }

        public async Task<IActionResult> OnGetDelete(int Id, CancellationToken cancellationToken)
        {
            await _cartItemRepository.Delete(Id, cancellationToken);

            return LocalRedirect("/Cart");
        }

        //public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
        //{
           
        //    using var transaction = await _databaseContext.Database.BeginTransactionAsync(cancellationToken);
        //    try
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (userId == null) return Unauthorized();

        //        if (!int.TryParse(userId, out var userIdParsed))
        //            return Unauthorized();

        //        var userCartItems = await _databaseContext.CartItems
        //            .Include(ci => ci.Product)
        //            .Where(ci => ci.UserId == userIdParsed)
        //            .ToListAsync(cancellationToken);

        //        if (!userCartItems.Any())
        //            return BadRequest("Your cart is empty.");

        //        foreach (var cartItem in userCartItems)
        //        {
        //            if (cartItem.Product == null)
        //            {
        //                return BadRequest($"Product with ID {cartItem.ProductId} does not exist.");
        //            }

        //            if (cartItem.Product.Stock < cartItem.Quantity)
        //            {
        //                return BadRequest($"Not enough stock for product {cartItem.Product.Name}.");
        //            }

        //            cartItem.Product.Stock -= cartItem.Quantity;

        //            // Set OriginalValues for RowVersion to enable optimistic locking
        //            _databaseContext.Entry(cartItem.Product).OriginalValues["RowVersion"] = cartItem.Product.RowVersion;
        //        }

        //        var totalPrice = userCartItems.Sum(ci => ci.Product.Price * ci.Quantity);

        //        var order = new Order
        //        {
        //            UserId = userIdParsed,
        //            OrderDate = DateTime.Now,
        //            TotalAmount = totalPrice,
        //            Status = OrderStatus.Pending
        //        };
        //        _databaseContext.Orders.Add(order);
        //        await _databaseContext.SaveChangesAsync(cancellationToken);

        //        foreach (var cartItem in userCartItems)
        //        {
        //            var orderItem = new OrderItem
        //            {
        //                OrderId = order.Id,
        //                ProductId = cartItem.ProductId,
        //                Quantity = cartItem.Quantity,
        //                UnitPrice = cartItem.Product.Price,
        //                Discount = cartItem.Product.Discount
        //            };
        //            _databaseContext.OrderItems.Add(orderItem);
        //        }

        //        var payment = new Payment
        //        {
        //            OrderId = order.Id,
        //            PaymentDate = DateTime.Now,
        //            Amount = totalPrice,
        //            PaymentMethod = "CreditCard",
        //            Status = PaymentStatus.Pending,
        //            TransactionId = Guid.NewGuid().ToString()
        //        };
        //        _databaseContext.Payments.Add(payment);

        //        _databaseContext.CartItems.RemoveRange(userCartItems);

        //        try
        //        {
        //            await _databaseContext.SaveChangesAsync(cancellationToken);
        //            await transaction.CommitAsync(cancellationToken);

        //            return LocalRedirect("/");
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            foreach (var entry in ex.Entries)
        //            {
        //                if (entry.Entity is Product)
        //                {
        //                    var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
        //                    if (databaseValues == null)
        //                    {
        //                        return NotFound("The product was deleted by another user.");
        //                    }

        //                    var databaseProduct = (Product)databaseValues.ToObject();

        //                    return new StatusCodeResult(StatusCodes.Status409Conflict);
        //                }
        //            }
        //            throw;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Transaction failed.");
        //        await transaction.RollbackAsync(cancellationToken);
        //        throw;
        //    }
        //}
        //// درخواست پرداخت
        public async Task<IActionResult> OnPostAsync([FromBody] dynamic request)
        {
            int orderId = request?.orderId ?? 0;
            if (orderId == 0)
                return new JsonResult(new { isSuccess = false, errorMessage = "شناسه سفارش نامعتبر است" });

            decimal totalAmount = TotalPrice;

            var paymentResult = await _service.CreatePaymentRequestAsync(
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

            var verification = await _service.VerifyPaymentAsync(authority, TotalPrice);

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
