using System.Threading.Tasks;

namespace EvenTransit.Core.Abstractions.Common
{
    public interface IEventPublisher
    {
        Task PublishAsync(string name, dynamic payload);    
    }
}