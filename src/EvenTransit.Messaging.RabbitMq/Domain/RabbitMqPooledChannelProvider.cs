using EvenTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqPooledChannelProvider : IRabbitMqPooledChannelProvider, IDisposable
{
    private IModel _channel;
    
    private readonly IRabbitMqConnectionFactory _connection;
    private readonly IRabbitMqChannelPool _channelPool;

    public RabbitMqPooledChannelProvider(IRabbitMqConnectionFactory connection,
        IRabbitMqChannelPool channelPool)
    {
        _connection = connection;
        _channelPool = channelPool;
    }
    
    public IModel Channel()
    {
        if (_channel != null) return _channel;
        
        _channel = _channelPool.Channel(_connection.ProducerConnection);

        return _channel;
    }

    public void Dispose()
    {
        _channelPool.ReturnChannel(_connection.ProducerConnection, _channel);
        
        GC.SuppressFinalize(this);
    }
}
