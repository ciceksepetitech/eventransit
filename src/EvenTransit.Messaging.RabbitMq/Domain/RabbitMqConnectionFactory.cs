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
        private readonly Lazy<IModel> _producerChannel;
        private readonly Lazy<IModel> _consumerChannel;
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

        public IModel ProducerChannel
        {
            get
            {
                lock (_guard)
                {
                    return _producerChannel.Value;
                }
            }
        }

        public IModel ConsumerChannel
        {
            get
            {
                lock (_guard)
                {
                    return _consumerChannel.Value;
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
            _producerChannel = new Lazy<IModel>(ProducerConnection.CreateModel);
            _consumerChannel = new Lazy<IModel>(ConsumerConnection.CreateModel);
            _logger = logger;
        }

        public void Dispose()
        {
            if (_producerChannel.IsValueCreated) TryCloseChannel(() => ProducerChannel);
            if (_consumerChannel.IsValueCreated) TryCloseChannel(() => ConsumerChannel);
            if (_producerConnection.IsValueCreated) TryCloseConnection(() => ProducerConnection);
            if (_consumerConnection.IsValueCreated) TryCloseConnection(() => ConsumerConnection);
        }

        private void TryCloseChannel(Func<IModel> channel)
        {
            try
            {
                var c = channel();
                if (c.IsOpen) c.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Channel close error!");
            }
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