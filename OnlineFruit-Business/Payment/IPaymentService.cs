using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Payment
{
    public interface IPaymentService
    {
        Task<PaymentRequestResult> CreatePaymentRequestAsync(decimal amount, string description, string callbackUrl, int orderId);
        Task<PaymentVerificationResult> VerifyPaymentAsync(string authority, decimal amount);
    }

    public class PaymentRequestResult
    {
        public bool IsSuccess { get; set; }
        public string PaymentUrl { get; set; }
        public string Authority { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class PaymentVerificationResult
    {
        public bool IsSuccess { get; set; }
        public string RefId { get; set; }
        public string ErrorMessage { get; set; }
        public decimal Amount { get; set; }
    }
}
