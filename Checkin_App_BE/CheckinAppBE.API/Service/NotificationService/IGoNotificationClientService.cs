using Dto.Notification;
using Common;

namespace Service.NotificationService
{
    public interface IGoNotificationClientService
    {
        Task<ServiceResult> SendNotificationToGOServiceAsync(string channel, string message);
    }
}
