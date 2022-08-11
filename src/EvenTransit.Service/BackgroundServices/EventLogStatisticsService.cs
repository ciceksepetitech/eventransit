using EvenTransit.Domain.Abstractions;
using EvenTransit.Service.Locker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EvenTransit.Service.BackgroundServices;

public class EventLogStatisticsService : IHostedService
{
    private const string ServiceName = "EventLogStatisticsService";

    private readonly IEventLogStatisticRepository _eventLogStatisticRepository;
    private readonly ILogsRepository _logsRepository;
    private readonly IDistributedLocker _distributedLocker;

    public EventLogStatisticsService(IServiceScopeFactory serviceScopeFactory)
    {
        using var scope = serviceScopeFactory.CreateScope();
        _eventLogStatisticRepository = scope.ServiceProvider.GetRequiredService<IEventLogStatisticRepository>();
        _logsRepository = scope.ServiceProvider.GetRequiredService<ILogsRepository>();
        _distributedLocker = scope.ServiceProvider.GetRequiredService<IDistributedLocker>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                CalculateStatistics();

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
        }, cancellationToken);
        
        return Task.CompletedTask;
    }

    private void CalculateStatistics()
    {
        if (!_distributedLocker.Acquire(ServiceName)) return;

        var events = _eventLogStatisticRepository.GetAll();

        foreach (var @event in events)
        {
            var logCounts = _logsRepository.GetLogsCountByEvent(@event.EventName);
            @event.SuccessCount = logCounts.Item1;
            @event.FailCount = logCounts.Item2;

            _eventLogStatisticRepository.Update(@event.EventId, @event);
        }

        _distributedLocker.Release();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _distributedLocker.Release();

        return Task.CompletedTask;
    }
}
