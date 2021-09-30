using System;
using System.Threading;
using System.Threading.Tasks;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Service.Locker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EvenTransit.Service.BackgroundServices
{
    public class LogStatisticsService : IHostedService
    {
        private const string ServiceName = "LogStatisticsService";
        
        private readonly ILogStatisticsRepository _logStatisticsRepository;
        private readonly ILogsRepository _logsRepository;
        private readonly IDistributedLocker _distributedLocker;

        public LogStatisticsService(IServiceScopeFactory serviceScopeFactory)
        {
            using var scope = serviceScopeFactory.CreateScope();
            _logStatisticsRepository = scope.ServiceProvider.GetRequiredService<ILogStatisticsRepository>();
            _logsRepository = scope.ServiceProvider.GetRequiredService<ILogsRepository>();
            _distributedLocker = scope.ServiceProvider.GetRequiredService<IDistributedLocker>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    CalculateStatistics();

                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }, cancellationToken);
        }

        private void CalculateStatistics()
        {
            if (!_distributedLocker.Acquire(ServiceName)) return;
            
            var now = DateTime.UtcNow;
            var startDate = DateTime.Today;
            var endDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            
            var successCount = _logsRepository.GetLogsCount(startDate, endDate, LogType.Success);
            var failCount = _logsRepository.GetLogsCount(startDate, endDate, LogType.Fail);
            var statistic = _logStatisticsRepository.GetStatistic(startDate);
            
            if (statistic == null)
            {
                var logStatistic = new LogStatistic
                {
                    Id = Guid.NewGuid(),
                    Date = startDate,
                    FailCount = failCount,
                    SuccessCount = successCount
                }; 
                _logStatisticsRepository.AddStatistic(logStatistic);
            }
            else
            {
                _logStatisticsRepository.UpdateStatistic(statistic.Id, successCount, failCount);
            }
            
            _distributedLocker.Release();
        } 

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _distributedLocker.Release();
            
            return Task.CompletedTask;
        }
    }
}