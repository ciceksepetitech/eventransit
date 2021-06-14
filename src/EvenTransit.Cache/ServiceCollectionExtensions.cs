using EvenTransit.Cache.Abstractions;
using EvenTransit.Cache.Domain;
using EvenTransit.Cache.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Cache
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICacheService, RedisCacheService>();
            services.Configure<RedisCacheSettings>(configuration.GetSection("Redis"));

            return services;
        }
    }
}