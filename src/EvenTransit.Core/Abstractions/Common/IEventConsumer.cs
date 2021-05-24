using System.Threading.Tasks;

namespace EvenTransit.Core.Abstractions.Common
{
    public interface IEventConsumer
    {
        Task ConsumeAsync();
    }
}