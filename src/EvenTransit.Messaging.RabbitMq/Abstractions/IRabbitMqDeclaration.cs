using System.Threading.Tasks;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Abstractions
{
    public interface IRabbitMqDeclaration
    {
        Task DeclareQueuesAsync(IModel channel);
    }
}