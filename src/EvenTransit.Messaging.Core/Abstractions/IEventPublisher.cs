using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Abstractions
{
    public interface IEventPublisher
    {
        void Publish(string eventName, object payload);
        void PublishToRetry(string eventName, string serviceName, byte[] payload);
        void RegisterNewService(NewServiceDto data);
    }
}