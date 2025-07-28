
using System.Threading.Tasks;

namespace Service.NotificationService
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string channel, string message);
    }
}
