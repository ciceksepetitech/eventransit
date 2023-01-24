using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Messaging.Core.Domain;

public class EventLog : IEventLog
{
    private readonly ILogsRepository _logsRepository;
    private readonly ILogStatisticsRepository _logStatisticsRepository;
    private readonly IEventLogStatisticRepository _eventLogStatisticRepository;
    private readonly IServiceScope _scope;

    public EventLog
    (
        ILogsRepository logsRepository,
        IServiceProvider serviceProvider,
        ILogStatisticsRepository logStatisticsRepository,
        IEventLogStatisticRepository eventLogStatisticRepository
    )
    {
        _logsRepository = logsRepository;
        _scope = serviceProvider.CreateScope(); //don't dispose that scope. rabbitmq consumes 1 per lifetime
        _logStatisticsRepository = logStatisticsRepository;
        _eventLogStatisticRepository = eventLogStatisticRepository;
    }

    public async Task LogAsync(Logs details)
    {
        await _logsRepository.InsertLogAsync(details);

        using var scope = _scope.ServiceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEventLog>();
        Task.Run(async () =>
        {
            await service.UpdateStatisticsAsync(details);
        });
        Task.Run(async () =>
        {
            await service.UpdateEventStatisticsAsync(details);
        });
    }

    public async Task UpdateStatisticsAsync(Logs details)
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


    public async Task UpdateEventStatisticsAsync(Logs details)
    {
        var eventStatistic = await _eventLogStatisticRepository.GetAsync(details.EventName);

        var successCount = details.LogType == LogType.Success ? 1 : 0;
        var failCount = details.LogType == LogType.Fail ? 1 : 0;

        await _eventLogStatisticRepository.UpdateStatisticAsync(eventStatistic.Id, successCount, failCount);
    }
}
