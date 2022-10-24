using System.Reflection;
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
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IRabbitMqChannelFactory, RabbitMqProducerChannelFactory>();

        if (modeConsumer)
        {
            services.AddScoped<IEventConsumer, EventConsumer>();
            services.AddScoped<IRabbitMqChannelFactory, RabbitMqConsumerChannelFactory>();
        }

        services.AddScoped<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();

        services.AddSingleton(typeof(IConnectionFactory), _ =>
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
