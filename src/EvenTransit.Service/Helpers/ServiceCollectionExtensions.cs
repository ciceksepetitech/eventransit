using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.QueueProcess;
using EvenTransit.Core.Domain.Common;
using EvenTransit.Messaging.Core.Domain.QueueProcess;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Service.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddScoped<IEventLog, EventLog>();
            services.AddScoped<IHttpRequestSender, HttpRequestSender>();
            services.AddScoped<IHttpProcessor, HttpProcessor>();
            services.AddScoped<ICacheService, CacheService>();

            return services;
        }
    }
}