using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EvenTransit.Service.BackgroundServices;

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
        var rabbitMqChannelFactories = scope.ServiceProvider.GetRequiredService<IEnumerable<IRabbitMqChannelFactory>>();
        var rabbitMqDeclaration = scope.ServiceProvider.GetRequiredService<IRabbitMqDeclaration>();

        using var channel = rabbitMqChannelFactories.Single(x => x.ChannelType == ChannelTypes.Producer).Channel;
        await rabbitMqDeclaration.DeclareQueuesAsync(channel);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
