﻿using AutoMapper.Internal;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Domain;
using EvenTransit.Service.Abstractions;
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

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, bool modeConsumer)
    {
        services.AddAutoMapper(cfg => cfg.Internal().MethodMappingEnabled = false, Assembly.GetExecutingAssembly());

        if (modeConsumer)
        {
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ILogService, LogService>();
        }

        services.AddTransient<ICustomObjectMapper, CustomObjectMapper>();
        services.AddScoped<IEventPublisherService, EventPublisherService>();

        return services;
    }
}
