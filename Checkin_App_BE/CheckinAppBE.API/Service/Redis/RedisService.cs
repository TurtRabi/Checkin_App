using StackExchange.Redis;
using System.Text.Json;

namespace Service.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer redisConnection)
        {
            _database = redisConnection.GetDatabase();
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            return await _database.StringSetAsync(key, serializedValue, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var serializedValue = await _database.StringGetAsync(key);
            if (serializedValue.IsNullOrEmpty)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(serializedValue!); // Sử dụng null-forgiving operator
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }
    }
}
