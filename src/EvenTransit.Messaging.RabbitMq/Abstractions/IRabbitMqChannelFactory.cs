using EvenTransit.Domain.Enums;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Abstractions;

public interface IRabbitMqChannelFactory
{
    ChannelTypes ChannelType { get; }
    IModel Channel { get; }
}
