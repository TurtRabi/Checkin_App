using Common;
using Dto.Notification;

namespace Service.NotificationService
{
    public interface IGoEmailClientService
    {
        Task<ServiceResult> SendEmailToGOServiceAsync(GoEmailRequestDto request);
    }
}
