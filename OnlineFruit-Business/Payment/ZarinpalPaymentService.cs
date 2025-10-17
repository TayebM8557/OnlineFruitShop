using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace OnlineFruit_Business.Payment
{
    public class ZarinpalPaymentService : IPaymentService
    {
        private readonly PaymentGatewaySettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ZarinpalPaymentService> _logger;

        public ZarinpalPaymentService(IOptions<PaymentGatewaySettings> settings, HttpClient httpClient, ILogger<ZarinpalPaymentService> logger)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PaymentRequestResult> CreatePaymentRequestAsync(decimal amount, string description, string callbackUrl, int orderId)
        {
            try
            {
                var requestData = new
                {
                    merchant_id = _settings.MerchantId,
                    amount = (int)(amount * 10), // تبدیل به تومان
                    description = description,
                    callback_url = callbackUrl,
                    metadata = new { order_id = orderId }
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_settings.PaymentUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ZarinpalResponse>(responseContent);

                if (result?.data?.code == 100)
                {
                    var paymentUrl = _settings.IsSandbox 
                        ? $"https://sandbox.zarinpal.com/pg/StartPay/{result.data.authority}"
                        : $"https://zarinpal.com/pg/StartPay/{result.data.authority}";

                    return new PaymentRequestResult
                    {
                        IsSuccess = true,
                        PaymentUrl = paymentUrl,
                        Authority = result.data.authority
                    };
                }

                return new PaymentRequestResult
                {
                    IsSuccess = false,
                    ErrorMessage = result?.errors?.message ?? "خطا در ایجاد درخواست پرداخت"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد درخواست پرداخت");
                return new PaymentRequestResult
                {
                    IsSuccess = false,
                    ErrorMessage = "خطا در ارتباط با درگاه پرداخت"
                };
            }
        }

        public async Task<PaymentVerificationResult> VerifyPaymentAsync(string authority, decimal amount)
        {
            try
            {
                var requestData = new
                {
                    merchant_id = _settings.MerchantId,
                    amount = (int)(amount * 10), // تبدیل به تومان
                    authority = authority
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_settings.VerificationUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ZarinpalResponse>(responseContent);

                if (result?.data?.code == 100)
                {
                    return new PaymentVerificationResult
                    {
                        IsSuccess = true,
                        RefId = result.data.ref_id,
                        Amount = amount
                    };
                }

                return new PaymentVerificationResult
                {
                    IsSuccess = false,
                    ErrorMessage = result?.errors?.message ?? "خطا در تایید پرداخت"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در تایید پرداخت");
                return new PaymentVerificationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "خطا در ارتباط با درگاه پرداخت"
                };
            }
        }
    }

    // کلاس‌های کمکی برای JSON
    public class ZarinpalResponse
    {
        public ZarinpalData? data { get; set; }
        public ZarinpalError? errors { get; set; }
    }

    public class ZarinpalData
    {
        public int code { get; set; }
        public string message { get; set; } = string.Empty;
        public string authority { get; set; } = string.Empty;
        public string ref_id { get; set; } = string.Empty;
    }

    public class ZarinpalError
    {
        public string message { get; set; } = string.Empty;
    }
}
