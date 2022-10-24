using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions;

public interface ILogStatisticsRepository
{
    Task<LogStatistic> GetStatisticAsync(DateTime date);
    Task AddStatisticAsync(LogStatistic logStatistic);
    Task UpdateStatisticAsync(Guid id, long successCount, long failCount);
    Task<List<LogStatistic>> GetStatisticsAsync(DateTime startDate, DateTime endDate);
}
