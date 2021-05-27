using System;
using System.Text.Json;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Constants;
using StackExchange.Redis;

namespace EvenTransit.Core.Domain.Common
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;
        
        public CacheService()
        {
            var redisConnectionString = Environment.GetEnvironmentVariable(CacheConstants.RedisConnectionStringKey);
            var connection = ConnectionMultiplexer.Connect(redisConnectionString);
            _db = connection.GetDatabase();
        }
        
        public async Task SetAsync<T>(string key, T data, int expireMinutes = 60)
        {
            var expireTime = TimeSpan.FromMinutes(expireMinutes);
            var body = JsonSerializer.Serialize(data);
            
            await _db.StringSetAsync(key, body, expireTime);
        }

        public async Task<T> GetAsync<T>(string key, int expireMinutes, Func<Task<T>> acquire)
        {
            var data = await _db.StringGetAsync(key);
            if (string.IsNullOrEmpty(data))
            {
                var functionResult = await acquire();
                await SetAsync(key, functionResult, expireMinutes);

                return functionResult;
            }

            var body = JsonSerializer.Deserialize<T>(data);
            return body ?? default;
        }

        public async Task DeleteAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}