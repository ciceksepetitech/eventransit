using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IDisposable
{
    private readonly ILogger<RabbitMqConnectionFactory> _logger;

    public IConnection ProducerConnection { get; }
    public IConnection ConsumerConnection { get; }

    public RabbitMqConnectionFactory(ILogger<RabbitMqConnectionFactory> logger, IConnectionFactory connectionFactory)
    {
        ProducerConnection = connectionFactory.CreateConnection();
        ConsumerConnection = connectionFactory.CreateConnection();

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
