using System.Threading.Tasks;

namespace EvenTransit.Messaging.Core.Abstractions
{
    public interface IEventConsumer
    {
        Task ConsumeAsync();
    }
}