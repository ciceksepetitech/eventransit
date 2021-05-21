using System.Threading.Tasks;

namespace EventTransit.Core.Abstractions.QueueProcess
{
    public interface IQueueProcess
    {
        Task Process(string eventName, string serviceName, string message);
    }
}