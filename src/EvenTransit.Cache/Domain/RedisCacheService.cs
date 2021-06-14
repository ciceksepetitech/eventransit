using System;
using System.Text.Json;
using System.Threading.Tasks;
using EvenTransit.Cache.Abstractions;
using EvenTransit.Cache.Settings;
using EvenTransit.Core.Enums;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace EvenTransit.Cache.Domain
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        
        public RedisCacheService(IOptions<RedisCacheSettings> redisCacheSettings)
        {
            var connection = ConnectionMultiplexer.Connect(redisCacheSettings.Value.ConnectionString);
            _db = connection.GetDatabase();
        }
        
        public async Task SetAsync<T>(string key, T data, ExpireTimes expireTimes = ExpireTimes.OneHour)
        {
            var expireTime = TimeSpan.FromMinutes((int)expireTimes);
            var body = JsonSerializer.Serialize(data);
            
            await _db.StringSetAsync(key, body, expireTime);
        }

        public async Task<T> GetAsync<T>(string key, ExpireTimes expireTimes, Func<Task<T>> acquire)
        {
            var data = await _db.StringGetAsync(key);
            if (string.IsNullOrEmpty(data))
            {
                var functionResult = await acquire();
                await SetAsync(key, functionResult, expireTimes);

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