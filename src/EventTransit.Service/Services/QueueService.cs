using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.Service;
using EventTransit.Core.Dto;

namespace EventTransit.Service.Services
{
    public class QueueService : IQueueService
    {
        private readonly IEventPublisher _eventPublisher;

        public QueueService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public bool Publish(QueueRequestDto requestDto)
        {
            _eventPublisher.Publish(requestDto.Name, requestDto.Payload);
            return true;
        }
    }
}