using System.Threading.Tasks;

namespace EvenTransit.Messaging.Core.Abstractions;

public interface IEventConsumer
{
    Task ConsumeAsync();

    void DeleteQueue(string eventName, string serviceName);

    void DeleteExchange(string eventName);
}
