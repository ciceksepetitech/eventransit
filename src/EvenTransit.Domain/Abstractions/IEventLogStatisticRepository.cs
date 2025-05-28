using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions;

public interface IEventLogStatisticRepository
{
    Task<List<EventLogStatistic>> GetAllAsync();
    Task<EventLogStatistic> GetAsync(string name, DateTime date);
    Task InsertAsync(EventLogStatistic data);
    Task UpdateAsync(Guid id, long successCount, long failCount);
}
