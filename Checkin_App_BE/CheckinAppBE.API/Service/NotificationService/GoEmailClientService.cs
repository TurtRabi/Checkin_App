using Common;
using Dto.Notification;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Service.NotificationService
{
    public class GoEmailClientService : IGoEmailClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _goNotificationServiceBaseUrl;

        public GoEmailClientService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _goNotificationServiceBaseUrl = _configuration["GoNotificationService:BaseUrl"] ?? "http://localhost:8080";
            _httpClient.BaseAddress = new Uri(_goNotificationServiceBaseUrl);
        }

        public async Task<ServiceResult> SendEmailToGOServiceAsync(GoEmailRequestDto request)
        {
            var jsonContent = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/send-email", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult.Success("Email đã được gửi thành công qua dịch vụ Go.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return ServiceResult.Fail($"Gửi email đến dịch vụ Go thất bại. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                return ServiceResult.Fail($"Lỗi kết nối đến dịch vụ Go khi gửi email: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Lỗi không xác định khi gửi email đến dịch vụ Go: {ex.Message}");
            }
        }
    }
}
