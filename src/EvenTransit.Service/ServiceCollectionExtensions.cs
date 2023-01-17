using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Domain;
using EvenTransit.Messaging.Core.InternalEvents;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Abstractions.Events;
using EvenTransit.Service.BackgroundServices;
using EvenTransit.Service.InternalEvents;
using EvenTransit.Service.Locker;
using EvenTransit.Service.Mappers;
using EvenTransit.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EvenTransit.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, bool modeConsumer)
    {
        if (modeConsumer)
        {
            services.AddScoped<IEventLog, EventLog>();
            services.AddScoped<IHttpRequestSender, HttpRequestSender>();
            services.AddScoped<IHttpProcessor, HttpProcessor>();
            services.AddScoped<IDistributedLocker, DistributedLocker>();
        }

        services.AddSingleton<IRetryQueueHelper, RetryQueueHelper>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, bool modeConsumer)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        if (modeConsumer)
        {
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ILogService, LogService>();
            services.AddTransient<ICustomObjectMapper, CustomObjectMapper>();
        }

        services.AddScoped<IEventPublisherService, EventPublisherService>();
        services.AddAsyncRunners();

        return services;
    }

    /// <summary>
    /// Register async runner that runs actions with new lifetimescope.
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>Collection of service descriptors</returns>
    private static void AddAsyncRunners(this IServiceCollection services)
    {
        services.AddScoped<IAsyncRunner, ScopeSafeAsyncRunner>();
        services.AddScoped<IInternalEventPublisher, InternalEventPublisher>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IAsyncConsumer<StatisticsUpdatedEvent>, StatisticsUpdatedEventConsumer>();
    }
}
