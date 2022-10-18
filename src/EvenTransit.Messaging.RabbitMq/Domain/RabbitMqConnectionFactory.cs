using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IDisposable
{
    private readonly Lazy<IConnection> _producerConnection;
    private readonly Lazy<IConnection> _consumerConnection;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;

    public IConnection ProducerConnection => _producerConnection.Value;

    public IConnection ConsumerConnection => _consumerConnection.Value;

    public RabbitMqConnectionFactory(IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqConnectionFactory> logger)
    {
        using var scope = scopeFactory.CreateScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();

        _producerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
        _consumerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
        _logger = logger;
    }

    public void Dispose()
    {
        if (_producerConnection.IsValueCreated) TryCloseConnection(() => ProducerConnection);
        if (_consumerConnection.IsValueCreated) TryCloseConnection(() => ConsumerConnection);
        
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
