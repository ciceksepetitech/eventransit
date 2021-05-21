using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.QueueProcess;
using EventTransit.Core.Domain.Common;
using EventTransit.Messaging.Core.Domain.QueueProcess;
using Microsoft.Extensions.DependencyInjection;

namespace EventTransit.Service.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddScoped<IEventLog, EventLog>();
            services.AddScoped<IHttpRequestSender, HttpRequestSender>();
            services.AddScoped<IHttpProcessor, HttpProcessor>();

            return services;
        }
    }
}