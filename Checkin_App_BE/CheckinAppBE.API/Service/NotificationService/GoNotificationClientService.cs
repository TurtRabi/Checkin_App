using Dto.Notification;
using Common;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Service.NotificationService
{
    public class GoNotificationClientService : IGoNotificationClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _goNotificationServiceBaseUrl;

        public GoNotificationClientService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _goNotificationServiceBaseUrl = _configuration["GoNotificationService:BaseUrl"] ?? "http://localhost:8080";
            _httpClient.BaseAddress = new Uri(_goNotificationServiceBaseUrl);
        }

        public async Task<ServiceResult> SendNotificationToGOServiceAsync(string channel, string message)
        {
            var requestDto = new GoNotificationRequestDto
            {
                Channel = channel,
                Message = message
            };

            var jsonContent = JsonSerializer.Serialize(requestDto);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/send", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult.Success("Thông báo đã được gửi thành công qua dịch vụ Go.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return ServiceResult.Fail($"Gửi thông báo đến dịch vụ Go thất bại. Status: {response.StatusCode}, Error: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                return ServiceResult.Fail($"Lỗi kết nối đến dịch vụ Go: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Lỗi không xác định khi gửi thông báo đến dịch vụ Go: {ex.Message}");
            }
        }
    }
}
