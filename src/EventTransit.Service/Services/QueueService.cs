using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.Service;
using EventTransit.Core.Domain;

namespace EventTransit.Service.Services
{
    public class QueueService : IQueueService
    {
        private readonly IEventPublisher _eventPublisher;

        public QueueService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> PublishAsync(QueueRequestDto requestDto)
        {
            await _eventPublisher.PublishAsync(requestDto.Name, requestDto.Payload);
            return true;
        }
    }
}