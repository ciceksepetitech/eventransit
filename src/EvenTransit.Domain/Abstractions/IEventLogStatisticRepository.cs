﻿using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions;

public interface IEventLogStatisticRepository
{
    Task<List<EventLogStatistic>> GetAllAsync();
    List<EventLogStatistic> GetAll();
    Task<EventLogStatistic> GetAsync(Guid eventId);
    Task<EventLogStatistic> GetAsync(string name);
    Task InsertAsync(EventLogStatistic data);
    void Update(Guid id, EventLogStatistic data);
    Task UpdateAsync(Guid id, EventLogStatistic data);
    Task DeleteAsync(Guid id);
    Task UpdateStatisticAsync(Guid id, long successCount, long failCount);
}
