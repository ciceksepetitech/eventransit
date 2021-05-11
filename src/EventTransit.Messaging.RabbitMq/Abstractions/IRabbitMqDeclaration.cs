using System.Threading.Tasks;
using RabbitMQ.Client;

namespace EventTransit.Messaging.RabbitMq.Abstractions
{
    public interface IRabbitMqDeclaration
    {
        Task DeclareQueuesAsync(IModel channel);
    }
}