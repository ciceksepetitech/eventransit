using System.Threading.Tasks;
using EvenTransit.Core.Dto.Service;

namespace EvenTransit.Core.Abstractions.QueueProcess
{
    public interface IHttpProcessor
    {
        Task ProcessAsync(string eventName, ServiceDto service, string message);
    }
}