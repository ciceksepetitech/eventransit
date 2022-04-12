using System;

namespace EvenTransit.Domain.Entities;

public class EventLogStatistic : BaseEntity
{
    public Guid EventId { get; set; }
    public string EventName { get; set; }
    public int ServiceCount { get; set; }
    public long SuccessCount { get; set; }
    public long FailCount { get; set; }
}
