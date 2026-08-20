using IOGKFExams.Server.Models.Sms;
using System.Net.Http.Json;

namespace IOGKFExams.Client
{
    public partial class SmsService
    {
        private readonly HttpClient _httpClient;

        public SmsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SendSmsResponse> SendSms(
            string phoneNumber,
            string message)
        {
            var request = new SendSmsRequest
            {
                To = phoneNumber,
                Message = message
            };

            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/sms/send",
                    request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content
                           .ReadFromJsonAsync<SendSmsResponse>()
                       ?? new SendSmsResponse
                       {
                           Success = false,
                           ErrorMessage =
                               "Invalid response from SMS service."
                       };
            }

            var error =
                await response.Content
                    .ReadFromJsonAsync<SendSmsResponse>();

            return error ?? new SendSmsResponse
            {
                Success = false,
                ErrorMessage =
                    $"SMS request failed: {response.StatusCode}"
            };
        }
    }
}