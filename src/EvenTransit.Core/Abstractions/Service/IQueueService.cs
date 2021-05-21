using System.Threading.Tasks;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Abstractions.Service
{
    public interface IQueueService
    {
        Task<bool> PublishAsync(QueueRequestDto requestDto);
    }
}