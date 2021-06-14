using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Abstractions
{
    public interface IEventPublisher
    {
        void Publish(string eventName, dynamic payload);
        void RegisterNewService(NewServiceDto data);
    }
}