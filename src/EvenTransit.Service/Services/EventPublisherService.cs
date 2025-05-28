using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;

namespace EvenTransit.Service.Services;

public class EventPublisherService : IEventPublisherService
{
    private readonly IEventPublisher _eventPublisher;

    public EventPublisherService(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public void Publish(EventRequestDto requestDto)
    {
        if (string.IsNullOrEmpty(requestDto.CorrelationId))
            requestDto.CorrelationId = Guid.NewGuid().ToString();
        
        _eventPublisher.Publish(requestDto);
    }
}
