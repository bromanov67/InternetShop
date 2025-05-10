using InternetShop.Application.BusinessLogic.Product;
using StackExchange.Redis;
using System.Text.Json;

namespace InternetShop.Database.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _redisDb;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var value = await _redisDb.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null,
            CancellationToken cancellationToken = default)
        {
            await _redisDb.StringSetAsync(
                key,
                JsonSerializer.Serialize(value),
                expiry);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _redisDb.KeyDeleteAsync(key);
        }
    }
}