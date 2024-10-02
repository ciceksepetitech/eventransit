using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Abstractions;

public interface IRabbitMqChannelPool
{
    IModel Channel(IConnection connection);
    void ReturnChannel(IConnection connection, IModel channel);
}
