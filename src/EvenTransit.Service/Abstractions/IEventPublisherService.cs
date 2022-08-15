using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Service.Abstractions;

public interface IEventPublisherService
{
    void Publish(EventRequestDto requestDto);
}