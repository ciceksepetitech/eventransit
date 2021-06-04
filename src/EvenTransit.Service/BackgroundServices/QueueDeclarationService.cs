using System.Threading;
using System.Threading.Tasks;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EvenTransit.Service.BackgroundServices
{
    public class QueueDeclarationService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScope;

        public QueueDeclarationService(IServiceScopeFactory serviceScope)
        {
            _serviceScope = serviceScope;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScope.CreateScope();
            var rabbitMqConnection = scope.ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var rabbitMqDeclaration = scope.ServiceProvider.GetRequiredService<IRabbitMqDeclaration>();

            using var channel = rabbitMqConnection.ProducerConnection.CreateModel();
            await rabbitMqDeclaration.DeclareQueuesAsync(channel);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}