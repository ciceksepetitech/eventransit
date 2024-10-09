using EvenTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqChannelPool : IRabbitMqChannelPool, IDisposable
{
    private readonly ConcurrentBag<IModel> _channels = new();
    private const int _maxChannels = 100;
    
    private static IModel CreateChannel(IConnection connection)
    {
        return connection.CreateModel();
    }

    public IModel Channel(IConnection connection)
    {
        if (!_channels.TryTake(out var channel)) return CreateChannel(connection);
        
        if (channel.IsOpen)
            return channel;
        
        channel.Close();
        
        return CreateChannel(connection);
    }

    public void ReturnChannel(IConnection connection, IModel channel)
    {
        if (channel == null) return;
        
        var channelCountExceedsMax = _channels.Count >= _maxChannels;
        
        switch (channel.IsOpen)
        {
            case true when !channelCountExceedsMax:
                _channels.Add(channel);
                return;
            case true:
                channel.Close();
                break;
        }

        if (!channelCountExceedsMax) 
            _channels.Add(CreateChannel(connection));
    }

    public void Dispose()
    {
        while (_channels.TryTake(out var channel)) channel.Close();

        GC.SuppressFinalize(this);
    }
}
