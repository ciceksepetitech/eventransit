using System;
using System.Threading;
using System.Threading.Tasks;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Service.Locker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EvenTransit.Service.BackgroundServices
{
    public class EventLogStatisticsService : IHostedService, IDisposable
    {
        private const string ServiceName = "EventLogStatisticsService";
        
        private readonly IEventLogStatisticRepository _eventLogStatisticRepository;
        private readonly ILogsRepository _logsRepository;
        private readonly IDistributedLocker _distributedLocker;
        private Timer _timer;

        public EventLogStatisticsService(IServiceScopeFactory serviceScopeFactory)
        {
            using var scope = serviceScopeFactory.CreateScope();
            _eventLogStatisticRepository = scope.ServiceProvider.GetRequiredService<IEventLogStatisticRepository>();
            _logsRepository = scope.ServiceProvider.GetRequiredService<ILogsRepository>();
            _distributedLocker = scope.ServiceProvider.GetRequiredService<IDistributedLocker>();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CalculateStatistics, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }
        
        private void CalculateStatistics(object state)
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
            _timer?.Change(Timeout.Infinite, 0);
            _distributedLocker.Release();
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}