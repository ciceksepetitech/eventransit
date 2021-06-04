using System;
using System.Reflection;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Constants;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Domain;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            // TODO Check lifetime
            services.AddScoped<IEventConsumer, EventConsumer>();
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            services.AddScoped<IRabbitMqDeclaration, RabbitMqDeclaration>();
            services.AddScoped(typeof(IConnectionFactory), provider =>
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = Environment.GetEnvironmentVariable(RabbitMqConstants.Host),
                    UserName = Environment.GetEnvironmentVariable(RabbitMqConstants.UserName),
                    Password = Environment.GetEnvironmentVariable(RabbitMqConstants.Password),
                    AutomaticRecoveryEnabled = true
                };

                return connectionFactory;
            });

            return services;
        }
    }
}