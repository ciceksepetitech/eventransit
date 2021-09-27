using System;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain
{
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IDisposable
    {
        private readonly object _guard = new();
        private readonly Lazy<IConnection> _producerConnection;
        private readonly Lazy<IConnection> _consumerConnection;
        private readonly ILogger<RabbitMqConnectionFactory> _logger;

        public IConnection ProducerConnection
        {
            get
            {
                lock (_guard)
                {
                    return _producerConnection.Value;
                }
            }
        }

        public IConnection ConsumerConnection
        {
            get
            {
                lock (_guard)
                {
                    return _consumerConnection.Value;
                }
            }
        }

        public RabbitMqConnectionFactory(IServiceScopeFactory scopeFactory,
            ILogger<RabbitMqConnectionFactory> logger)
        {
            using var scope = scopeFactory.CreateScope();
            var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();

            _producerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
            _consumerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
            _logger = logger;
        }

        public void Dispose()
        {
            if (_producerConnection.IsValueCreated) TryCloseConnection(() => ProducerConnection);
            if (_consumerConnection.IsValueCreated) TryCloseConnection(() => ConsumerConnection);
        }

        private void TryCloseConnection(Func<IConnection> connection)
        {
            try
            {
                connection().Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection close error!");
            }
        }
    }
}