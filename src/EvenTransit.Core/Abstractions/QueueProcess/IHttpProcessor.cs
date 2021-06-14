using System.Threading.Tasks;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Dto.Service.Event;

namespace EvenTransit.Core.Abstractions.QueueProcess
{
    public interface IHttpProcessor
    {
        Task<bool> ProcessAsync(string eventName, ServiceDto service, string message);
    }
}