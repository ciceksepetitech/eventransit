using System;
using System.Threading;
using System.Threading.Tasks;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EvenTransit.Service.BackgroundServices
{
    public class LogStatisticsService : IHostedService, IDisposable
    {
        private readonly ILogStatisticsRepository _logStatisticsRepository;
        private readonly ILogsRepository _logsRepository;
        private Timer _timer;

        public LogStatisticsService(IServiceProvider serviceProvider)
        {
            _logStatisticsRepository = serviceProvider.GetRequiredService<ILogStatisticsRepository>();
            _logsRepository = serviceProvider.GetRequiredService<ILogsRepository>();
        }
        

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CalculateStatistics, null, TimeSpan.Zero, 
                TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        private void CalculateStatistics(object state)
        {
            var now = DateTime.Now;
            var startDate = DateTime.Today;
            var endDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            
            var successCount = _logsRepository.GetLogsCount(startDate, endDate, LogType.Success);
            var failCount = _logsRepository.GetLogsCount(startDate, endDate, LogType.Success);
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
        } 

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}