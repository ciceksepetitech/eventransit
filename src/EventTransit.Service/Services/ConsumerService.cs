using System.Threading;
using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventTransit.Service.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScope;

        public ConsumerService(IServiceScopeFactory serviceScope)
        {
            _serviceScope = serviceScope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScope.CreateScope();
            while (!stoppingToken.IsCancellationRequested)
            {
                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                eventConsumer.Consume();
                
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}