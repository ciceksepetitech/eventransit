using System;
using EventTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventTransit.Messaging.RabbitMq.Domain
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

        public RabbitMqConnectionFactory(IConnectionFactory connectionFactory,
            ILogger<RabbitMqConnectionFactory> logger)
        {
            _producerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
            _consumerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
            _logger = logger;
        }

        public void Dispose()
        {
            if (_producerConnection.IsValueCreated) TryClose(() => ProducerConnection);
            if (_consumerConnection.IsValueCreated) TryClose(() => ConsumerConnection);
        }

        private void TryClose(Func<IConnection> connection)
        {
            try
            {
                connection().Close();
            }
            catch (Exception ex)
            {
                _logger.LogError("Connection close error!", ex);
            }
        }
    }
}