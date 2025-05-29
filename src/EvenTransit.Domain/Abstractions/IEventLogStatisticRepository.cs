using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions;

public interface IEventLogStatisticRepository
{
    Task<List<EventLogStatistic>> ListAsync();
    Task<List<EventLogStatistic>> ListAsync(string eventName);
    Task<EventLogStatistic> GetAsync(string eventName, string serviceName, DateTime date);
    Task InsertAsync(EventLogStatistic data);
    Task UpdateAsync(Guid id, long successCount, long failCount);
}
