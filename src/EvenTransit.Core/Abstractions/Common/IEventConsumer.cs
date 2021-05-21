using System.Threading.Tasks;

namespace EvenTransit.Core.Abstractions.Common
{
    public interface IEventConsumer
    {
        void Consume();
    }
}