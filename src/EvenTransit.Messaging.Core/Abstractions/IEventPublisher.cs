using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Abstractions;

public interface IEventPublisher
{
    void Publish(EventRequestDto request);
    void PublishToRetry(string eventName, string serviceName, byte[] payload, long retryCount);
    void RegisterNewService(NewServiceDto data);
}
