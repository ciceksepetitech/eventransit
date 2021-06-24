using System.Reflection;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Domain;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Locker;
using EvenTransit.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Service
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddScoped<IEventLog, EventLog>();
            services.AddScoped<IHttpRequestSender, HttpRequestSender>();
            services.AddScoped<IHttpProcessor, HttpProcessor>();
            services.AddScoped<IDistributedLocker, DistributedLocker>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ILogService, LogService>();
            
            return services;
        }
    }
}