using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqConsumerChannelFactory : IRabbitMqChannelFactory, IDisposable
{
    private IModel _channel;
    private static readonly object _guard = new();
    private readonly IRabbitMqConnectionFactory _connection;
    private readonly IList<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitMqConsumerChannelFactory> _logger;

    private const int RetryToConnectAfterSeconds = 5;
    private const ushort DisposeReasonCodeSuccess = 200;

    public ChannelTypes ChannelType => ChannelTypes.Consumer;

    public IModel Channel
    {
        get
        {
            if (_channel is { IsOpen: true })
                return _channel;

            lock (_guard)
            {
                if (_channel is { IsOpen: true })
                    return _channel;

                _channel = _connection.ConsumerConnection.CreateModel();

                ChannelFailureScenario(_channel);

                return _channel;
            }
        }
    }

    public RabbitMqConsumerChannelFactory(IRabbitMqConnectionFactory connection,
        ILogger<RabbitMqConsumerChannelFactory> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _connection = connection;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    private void ChannelFailureScenario(IModel channel)
    {
        channel.ModelShutdown += (sender, args) =>
        {
            if (args.ReplyCode == DisposeReasonCodeSuccess)
                return;

            _logger.ChannelStateFailed($"Channel lost. {args.ReplyText}", args.Cause, null);

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            _cancellationTokenSources.Add(cts);
            Task.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (_connection.ConsumerConnection.IsOpen)
                    {
                        try
                        {
                            if (channel.IsClosed)
                            {
                                using var scope = _serviceScopeFactory.CreateScope();
                                var connectionFac = scope.ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
                                channel = connectionFac.ConsumerConnection.CreateModel();
                            }

                            if (channel.IsOpen)
                            {
                                using var scope = _serviceScopeFactory.CreateScope();
                                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                                eventConsumer.ConsumeAsync().GetAwaiter().GetResult();

                                break;
                            }
                            _logger.ChannelStateFailed("Channel waiting...", args.Cause, null);
                        }
                        catch (Exception ex)
                        {
                            _logger.ChannelStateFailed(ex.Message, string.Empty, ex);
                        }
                    }

                    _logger.ChannelStateFailed("Connection waiting...", args.Cause, null);

                    Thread.Sleep(1000 * RetryToConnectAfterSeconds);
                }
            }, token);
        };
    }


    public void Dispose()
    {
        _channel?.Close();

        foreach (var source in _cancellationTokenSources)
        {
            source.Cancel();
        }

        GC.SuppressFinalize(this);

        _logger.ChannelState("Consumer channel closed successfully.");
    }
}
