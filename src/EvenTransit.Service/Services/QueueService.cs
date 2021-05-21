using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto;

namespace EvenTransit.Service.Services
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