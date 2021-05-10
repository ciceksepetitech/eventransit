using System.Threading.Tasks;
using EventTransit.Core.Domain;

namespace EventTransit.Core.Abstractions.Service
{
    public interface IQueueService
    {
        Task<bool> PublishAsync(QueueRequestDto requestDto);
    }
}