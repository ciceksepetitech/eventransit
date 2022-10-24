using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IDisposable
{
    private readonly IConnection _producerConnection;
    private readonly IConnection _consumerConnection;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;

    public IConnection ProducerConnection => _producerConnection;

    public IConnection ConsumerConnection => _consumerConnection;

    public RabbitMqConnectionFactory(IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqConnectionFactory> logger)
    {
        using var scope = scopeFactory.CreateScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();

        _producerConnection = connectionFactory.CreateConnection();
        _consumerConnection = connectionFactory.CreateConnection();
        _logger = logger;
    }

    public void Dispose()
    {
        TryCloseConnection(() => ProducerConnection);
        TryCloseConnection(() => ConsumerConnection);

        GC.SuppressFinalize(this);
    }

    private void TryCloseConnection(Func<IConnection> connection)
    {
        try
        {
            connection().Close();
        }
        catch (Exception ex)
        {
            _logger.ConnectionStateFailed("Connection close error!", ex);
        }
    }
}
