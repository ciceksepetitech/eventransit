using System.Threading.Tasks;

namespace EventTransit.Core.Abstractions.Common
{
    public interface IEventConsumer
    {
        void Consume();
    }
}