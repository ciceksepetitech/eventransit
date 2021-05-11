using System.Threading.Tasks;

namespace EventTransit.Core.Abstractions.Common
{
    public interface IEventPublisher
    {
        void Publish(string name, dynamic payload);    
    }
}