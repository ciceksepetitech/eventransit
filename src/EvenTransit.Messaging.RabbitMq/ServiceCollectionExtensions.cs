using System.Reflection;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            services.AddScoped<IEventConsumer, EventConsumer>();
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            services.AddScoped<IRabbitMqDeclaration, RabbitMqDeclaration>();
            services.AddScoped(typeof(IConnectionFactory), provider =>
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMq:Host"],
                    UserName = configuration["RabbitMq:UserName"],
                    Password = configuration["RabbitMq:Password"],
                    AutomaticRecoveryEnabled = true
                };

                return connectionFactory;
            });

            return services;
        }
    }
}