using System.Reflection;
using AutoMapper.Internal;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq;

public static class ServiceCollectionExtensions
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration, bool modeConsumer)
    {
        services.AddAutoMapper(cfg => cfg.Internal().MethodMappingEnabled = false, Assembly.GetExecutingAssembly());


        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IRabbitMqPooledChannelProvider, RabbitMqPooledChannelProvider>();
        services.AddSingleton<IRabbitMqChannelPool, RabbitMqChannelPool>();

        if (modeConsumer)
        {
            services.AddScoped<IEventConsumer, EventConsumer>();
            services.AddSingleton<IRabbitMqChannelFactory, RabbitMqConsumerChannelFactory>();
        }

        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();

        services.AddScoped(typeof(IConnectionFactory), _ =>
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMq:Endpoint"]),
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true
            };

            return connectionFactory;
        });

        services.AddHealthChecks().AddCheck<RabbitMqHealthCheck>("rabbitmq");
    }
}
