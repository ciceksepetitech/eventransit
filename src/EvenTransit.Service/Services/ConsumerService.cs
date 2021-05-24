using System.Threading;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EvenTransit.Service.Services
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
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            
            await eventConsumer.ConsumeAsync();
        }
    }
}