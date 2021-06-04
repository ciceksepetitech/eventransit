using System.Threading;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EvenTransit.Service.BackgroundServices
{
    public class ConsumerBinderService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScope;

        public ConsumerBinderService(IServiceScopeFactory serviceScope)
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