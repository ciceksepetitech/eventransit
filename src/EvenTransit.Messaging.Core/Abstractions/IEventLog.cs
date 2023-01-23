using EvenTransit.Domain.Entities;

namespace EvenTransit.Messaging.Core.Abstractions;

public interface IEventLog
{
    Task LogAsync(Logs details);
    Task UpdateStatisticsAsync(Logs details);
    Task UpdateEventStatisticsAsync(Logs details);
}
