using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Service.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EvenTransit.Service.BackgroundServices;

public class ConsumerBinderService : BackgroundService
{
    private const int DelayMs = 5000;
    private const int OneSecond = 1000;
    
    private readonly IServiceScopeFactory _serviceScope;
    private readonly ILogger<ConsumerBinderService> _logger;

    public ConsumerBinderService(IServiceScopeFactory serviceScope,
        ILogger<ConsumerBinderService> logger)
    {
        _serviceScope = serviceScope;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScope.CreateScope();
                    var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                    
                    await eventConsumer.ConsumeAsync();
                    break;
                }
                catch (Exception e)
                {
                    _logger.ConsumerBinderFailed(null,
                        $"{e.GetType().Name}. {e.Message}. Retrying in {DelayMs / OneSecond} seconds...");
                    
                    await Task.Delay(DelayMs, cancellationToken);
                }
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }
}
