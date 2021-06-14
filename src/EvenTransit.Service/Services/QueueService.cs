using System.Threading.Tasks;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;

namespace EvenTransit.Service.Services
{
    public class QueueService : IQueueService
    {
        private readonly IEventPublisher _eventPublisher;

        public QueueService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public Task<bool> PublishAsync(QueueRequestDto requestDto)
        {
            _eventPublisher.Publish(requestDto.EventName, requestDto.Payload);
            return Task.FromResult(true);
        }
    }
}