using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqProducerChannelFactory : IRabbitMqChannelFactory, IDisposable
{
    private IModel _channel;
    private readonly object _guard = new();
    private readonly IRabbitMqConnectionFactory _connection;
    private readonly ILogger<RabbitMqProducerChannelFactory> _logger;

    public ChannelTypes ChannelType => ChannelTypes.Producer;

    public IModel Channel
    {
        get
        {
            if (_channel is {IsOpen: true}) return _channel;
            
            lock (_guard)
            {
                if (_channel is {IsOpen: true}) return _channel;

                _channel = _connection.ProducerConnection.CreateModel();
                
                return _channel;
            }
        }
    }

    public RabbitMqProducerChannelFactory(IRabbitMqConnectionFactory connection,
        ILogger<RabbitMqProducerChannelFactory> logger)
    {
        _connection = connection;
        _logger = logger;
    }


    public void Dispose()
    {
        _channel?.Close();
        
        GC.SuppressFinalize(this);
        
        _logger.ChannelState("Producer channel closed successfully.");
    }
}
