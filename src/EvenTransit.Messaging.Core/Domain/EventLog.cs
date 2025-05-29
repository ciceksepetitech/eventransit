using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core.Abstractions;

namespace EvenTransit.Messaging.Core.Domain;

public class EventLog : IEventLog
{
    private readonly ILogsRepository _logsRepository;
    private readonly ILogStatisticsRepository _logStatisticsRepository;
    private readonly IEventLogStatisticRepository _eventLogStatisticRepository;

    public EventLog
    (
        ILogsRepository logsRepository,
        ILogStatisticsRepository logStatisticsRepository,
        IEventLogStatisticRepository eventLogStatisticRepository
    )
    {
        _logsRepository = logsRepository;
        _logStatisticsRepository = logStatisticsRepository;
        _eventLogStatisticRepository = eventLogStatisticRepository;
    }

    public async Task LogAsync(Logs details)
    {
        await _logsRepository.InsertLogAsync(details);
        await UpdateStatisticsAsync(details);
        await UpdateEventStatisticsAsync(details);
    }

    private async Task UpdateStatisticsAsync(Logs details)
    {
        var startDate = DateTime.Today;
        var statistic = await _logStatisticsRepository.GetStatisticAsync(startDate);

        var failCount = details.LogType == LogType.Fail ? 1 : 0;
        var successCount = details.LogType == LogType.Success ? 1 : 0;

        if (statistic == null)
        {
            var logStatistic = new LogStatistic
            {
                Id = Guid.NewGuid(),
                Date = startDate,
                FailCount = failCount,
                SuccessCount = successCount
            };
            await _logStatisticsRepository.AddStatisticAsync(logStatistic);
        }
        else
            await _logStatisticsRepository.UpdateStatisticAsync(statistic.Id, successCount, failCount);
    }

    private async Task UpdateEventStatisticsAsync(Logs details)
    {
        var startDate = DateTime.Today;
        
        var eventStatistic = await _eventLogStatisticRepository.GetAsync(details.EventName, startDate);

        var successCount = details.LogType == LogType.Success ? 1 : 0;
        var failCount = details.LogType == LogType.Fail ? 1 : 0;

        if (eventStatistic == null)
        {
            eventStatistic = new EventLogStatistic
            {
                CreatedOn = startDate,
                EventName = details.EventName,
                SuccessCount = successCount,
                FailCount = failCount
            };
            await _eventLogStatisticRepository.InsertAsync(eventStatistic);
        }
        else
        {
            eventStatistic.SuccessCount += successCount;
            eventStatistic.FailCount += failCount;
            await _eventLogStatisticRepository.UpdateAsync(eventStatistic.Id, successCount, failCount);
        }
    }
}
