using EvenTransit.Domain.Entities;
using EvenTransit.Messaging.Core.Abstractions;

namespace EvenTransit.Messaging.Core.InternalEvents;

public class StatisticsUpdatedEvent : IInternalAsyncEvent
{
    public Logs Details { get; set; }
}
