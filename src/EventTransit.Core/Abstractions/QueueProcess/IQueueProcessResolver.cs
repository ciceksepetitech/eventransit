
using EventTransit.Core.Enums;

namespace EventTransit.Core.Abstractions.QueueProcess
{
    public interface IQueueProcessResolver
    {
        IQueueProcess Resolve(QueueProcessType processType);
    }
}