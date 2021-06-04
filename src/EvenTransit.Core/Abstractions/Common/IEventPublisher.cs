using System.Threading.Tasks;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Abstractions.Common
{
    public interface IEventPublisher
    {
        void Publish(string eventName, dynamic payload);
        void RegisterNewService(NewServiceDto data);
    }
}