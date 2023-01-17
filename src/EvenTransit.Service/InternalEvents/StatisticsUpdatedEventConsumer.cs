using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.InternalEvents;

namespace EvenTransit.Service.InternalEvents;

public class StatisticsUpdatedEventConsumer : IAsyncConsumer<StatisticsUpdatedEvent>
{
    private readonly ILogStatisticsRepository _logStatisticsRepository;
    private readonly IEventLogStatisticRepository _eventLogStatisticRepository;

    public StatisticsUpdatedEventConsumer(IEventLogStatisticRepository eventLogStatisticRepository, ILogStatisticsRepository logStatisticsRepository)
    {
        _eventLogStatisticRepository = eventLogStatisticRepository;
        _logStatisticsRepository = logStatisticsRepository;
    }

    public async Task HandleEventAsync(StatisticsUpdatedEvent eventMessage)
    {
        await UpdateStatisticsAsync(eventMessage.Details);
        await UpdateEventStatisticsAsync(eventMessage.Details);
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
