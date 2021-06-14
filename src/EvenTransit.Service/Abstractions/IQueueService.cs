using System.Threading.Tasks;
using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Service.Abstractions
{
    public interface IQueueService
    {
        Task<bool> PublishAsync(QueueRequestDto requestDto);
    }
}