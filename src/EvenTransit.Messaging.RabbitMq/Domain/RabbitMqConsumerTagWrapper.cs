using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public static class RabbitMqConsumerTagWrapper
{
    public static readonly ConcurrentDictionary<string, ConsumerTag> _tags = new();
}

public class ConsumerTag
{
    public string Value { get; set; }
    public IModel Channel { get; set; }
}
