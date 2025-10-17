using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineFruit_Business.Payment;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Context;
using OnlineFruit_Data.Entity.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoMapper;
using OnlineFruit_Data.Entity;

namespace OnlineFruitShop.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly DatabaseContext _context;
        private readonly ILogger<PaymentController> _logger;
        private readonly IMapper _mapper;

        public PaymentController(
            IPaymentService paymentService,
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            DatabaseContext context,
            ILogger<PaymentController> logger,
            IMapper mapper)
        {
            _paymentService = paymentService;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> InitiatePayment(int orderId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userId, out var userIdParsed))
                {
                    return Json(new { success = false, message = "کاربر یافت نشد" });
                }

                var order = await _context.Orders
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userIdParsed);

                if (order == null)
                {
                    return Json(new { success = false, message = "سفارش یافت نشد" });
                }

                if (order.Status != APP.OrderStatus.Pending)
                {
                    return Json(new { success = false, message = "این سفارش قابل پرداخت نیست" });
                }

                var callbackUrl = Url.Action("VerifyPayment", "Payment", null, Request.Scheme);
                var description = $"پرداخت سفارش شماره {order.Id}";

                var paymentRequest = await _paymentService.CreatePaymentRequestAsync(
                    order.TotalAmount, 
                    description, 
                    callbackUrl, 
                    order.Id);

                if (paymentRequest.IsSuccess)
                {
                    // ذخیره authority در دیتابیس
                    var payment = new APP.Payment
                    {
                        OrderId = order.Id,
                        PaymentDate = DateTime.UtcNow,
                        Amount = order.TotalAmount,
                        PaymentMethod = "Zarinpal",
                        Status = APP.PaymentStatus.Pending,
                        TransactionId = paymentRequest.Authority
                    };

                    var paymentDto = _mapper.Map<PaymentDto>(payment);
                    await _paymentRepository.Create(paymentDto, CancellationToken.None);

                    return Json(new { 
                        success = true, 
                        paymentUrl = paymentRequest.PaymentUrl 
                    });
                }

                return Json(new { 
                    success = false, 
                    message = paymentRequest.ErrorMessage 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در شروع پرداخت");
                return Json(new { success = false, message = "خطا در پردازش درخواست" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPayment(string Authority, string Status)
        {
            try
            {
                if (Status != "OK")
                {
                    TempData["PaymentError"] = "پرداخت لغو شد";
                    return RedirectToAction("Index", "OrderProfile");
                }

                var payment = await _context.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.TransactionId == Authority);

                if (payment == null)
                {
                    TempData["PaymentError"] = "اطلاعات پرداخت یافت نشد";
                    return RedirectToAction("Index", "OrderProfile");
                }

                var verificationResult = await _paymentService.VerifyPaymentAsync(Authority, payment.Amount);

                if (verificationResult.IsSuccess)
                {
                    // بروزرسانی وضعیت پرداخت
                    payment.Status = APP.PaymentStatus.Completed;
                    payment.TransactionId = verificationResult.RefId;

                    // بروزرسانی وضعیت سفارش
                    payment.Order.Status = APP.OrderStatus.Processing;

                    await _context.SaveChangesAsync();

                    TempData["PaymentSuccess"] = $"پرداخت با موفقیت انجام شد. کد رهگیری: {verificationResult.RefId}";
                    return RedirectToAction("PaymentSuccess", "Payment", new { orderId = payment.OrderId });
                }
                else
                {
                    payment.Status = APP.PaymentStatus.Failed;
                    await _context.SaveChangesAsync();

                    TempData["PaymentError"] = verificationResult.ErrorMessage;
                    return RedirectToAction("PaymentFailed", "Payment", new { orderId = payment.OrderId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در تایید پرداخت");
                TempData["PaymentError"] = "خطا در پردازش پرداخت";
                return RedirectToAction("Index", "OrderProfile");
            }
        }

        [HttpGet]
        public IActionResult PaymentSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        [HttpGet]
        public IActionResult PaymentFailed(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
