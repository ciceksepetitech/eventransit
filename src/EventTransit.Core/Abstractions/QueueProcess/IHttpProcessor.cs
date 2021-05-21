using System.Threading.Tasks;

namespace EventTransit.Core.Abstractions.QueueProcess
{
    public interface IHttpProcessor
    {
        Task ProcessAsync(string eventName, string serviceName, string message);
    }
}