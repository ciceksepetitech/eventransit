using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions
{
    public interface ILogStatisticsRepository
    {
        LogStatistic GetStatistic(DateTime date);
        void AddStatistic(LogStatistic logStatistic);
        void UpdateStatistic(Guid id, long successCount, long failCount);
        Task<List<LogStatistic>> GetStatisticsAsync(DateTime startDate, DateTime endDate);
    }
}