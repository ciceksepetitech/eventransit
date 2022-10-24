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

    public EventLog(ILogsRepository logsRepository,
        ILogStatisticsRepository logStatisticsRepository,
        IEventLogStatisticRepository eventLogStatisticRepository)
    {
        _logsRepository = logsRepository;
        _logStatisticsRepository = logStatisticsRepository;
        _eventLogStatisticRepository = eventLogStatisticRepository;
    }

    public async Task LogAsync(Logs details)
    {
        await _logsRepository.InsertLogAsync(details);

        //TODO: do async or event
        await UpdateStatisticsAsync(details);
        await UpdateEventStatisticsAsync(details);
        //TODO: do async or event
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
        var eventStatistic = await _eventLogStatisticRepository.GetAsync(details.EventName);

        var successCount = details.LogType == LogType.Success ? 1 : 0;
        var failCount = details.LogType == LogType.Fail ? 1 : 0;

        await _eventLogStatisticRepository.UpdateStatisticAsync(eventStatistic.Id, successCount, failCount);
    }
}
