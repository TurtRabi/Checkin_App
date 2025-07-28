
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Service.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IConnectionMultiplexer _redis;

        public NotificationService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task SendNotificationAsync(string channel, string message)
        {
            var db = _redis.GetDatabase();
            await db.PublishAsync(channel, message);
        }
    }
}
