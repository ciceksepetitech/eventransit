using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Abstractions;

public interface IRabbitMqConnectionFactory
{
    IConnection ProducerConnection { get; }
    IConnection ConsumerConnection { get; }
}
