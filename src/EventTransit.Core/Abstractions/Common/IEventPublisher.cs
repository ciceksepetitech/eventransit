using System.Threading.Tasks;

namespace EventTransit.Core.Abstractions.Common
{
    public interface IEventPublisher
    {
        Task PublishAsync(string name, dynamic payload);    
    }
}