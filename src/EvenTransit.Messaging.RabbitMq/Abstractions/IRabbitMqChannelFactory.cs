using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Abstractions;

public interface IRabbitMqChannelFactory
{
    IModel ChannelForRecover(Action<IModel> recover);
}
