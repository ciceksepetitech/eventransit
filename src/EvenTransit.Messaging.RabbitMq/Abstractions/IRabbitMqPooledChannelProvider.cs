using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Abstractions;

public interface IRabbitMqPooledChannelProvider
{
    IModel Channel();
}
