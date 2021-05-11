using RabbitMQ.Client;

namespace EventTransit.Messaging.RabbitMq.Abstractions
{
    public interface IRabbitMqConnectionFactory
    {
        IConnection ProducerConnection { get; }
        IConnection ConsumerConnection { get; }
    }
}