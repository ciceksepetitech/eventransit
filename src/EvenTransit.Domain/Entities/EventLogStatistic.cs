
namespace EvenTransit.Domain.Entities;

public class EventLogStatistic : BaseEntity
{
    public string EventName { get; set; }
    public long SuccessCount { get; set; }
    public long FailCount { get; set; }
    public DateTime CreatedOn { get; set; }
}
