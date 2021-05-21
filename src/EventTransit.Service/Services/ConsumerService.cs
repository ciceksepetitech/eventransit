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

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScope.CreateScope();
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            eventConsumer.Consume();

            return Task.CompletedTask;
        }
    }
}