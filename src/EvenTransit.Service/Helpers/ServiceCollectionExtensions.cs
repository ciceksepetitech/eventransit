using System.Reflection;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.QueueProcess;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Domain.Common;
using EvenTransit.Messaging.Core.Domain.QueueProcess;
using EvenTransit.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Service.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, CacheService>();

            return services;
        }
        
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddScoped<IEventLog, EventLog>();
            services.AddScoped<IHttpRequestSender, HttpRequestSender>();
            services.AddScoped<IHttpProcessor, HttpProcessor>();

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