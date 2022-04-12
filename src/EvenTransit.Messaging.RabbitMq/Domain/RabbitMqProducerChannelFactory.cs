using System;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqProducerChannelFactory : IRabbitMqChannelFactory, IDisposable
{
    private bool _disposed;
    private Lazy<IModel> _channel;
    private readonly object _guard = new();
    private readonly IRabbitMqConnectionFactory _connection;
    private readonly ILogger<RabbitMqProducerChannelFactory> _logger;

    public ChannelTypes ChannelType
    {
        get => ChannelTypes.Producer;
    }

    public IModel Channel
    {
        get
        {
            lock (_guard)
            {
                if (_channel.IsValueCreated && _channel.Value.IsOpen) return _channel.Value;

                _channel = new Lazy<IModel>(_connection.ProducerConnection.CreateModel());

                return _channel.Value;
            }
        }
    }

    public RabbitMqProducerChannelFactory(IRabbitMqConnectionFactory connection,
        ILogger<RabbitMqProducerChannelFactory> logger)
    {
        _connection = connection;
        _logger = logger;
        _channel = new Lazy<IModel>(connection.ProducerConnection.CreateModel());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            if (!_channel.IsValueCreated) return;

            if (!Channel.IsOpen) return;

            Channel.Close();
            _disposed = true;
            GC.SuppressFinalize(this);

            _logger.LogInformation("Producer channel closed successfully.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }
}
