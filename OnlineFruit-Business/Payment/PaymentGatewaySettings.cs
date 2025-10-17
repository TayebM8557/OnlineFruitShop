namespace OnlineFruit_Business.Payment
{
    public class PaymentGatewaySettings
    {
        public string MerchantId { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string VerificationUrl { get; set; } = string.Empty;
        public bool IsSandbox { get; set; } = true; // برای تست
    }
}
