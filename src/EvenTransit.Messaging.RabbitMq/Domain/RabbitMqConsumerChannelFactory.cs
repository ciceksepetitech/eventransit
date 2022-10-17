using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqConsumerChannelFactory : IRabbitMqChannelFactory, IDisposable
{
    private bool _disposed;
    private Lazy<IModel> _channel;
    private readonly object _guard = new();
    private readonly IRabbitMqConnectionFactory _connection;
    private readonly IList<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitMqConsumerChannelFactory> _logger;

    private const int _retryToConnectAfterSeconds = 5;
    private const ushort _disposeReasonCodeSuccess = 200;

    public ChannelTypes ChannelType
    {
        get => ChannelTypes.Consumer;
    }

    public IModel Channel
    {
        get
        {
            lock (_guard)
            {
                if (_channel.IsValueCreated && _channel.Value.IsOpen)
                    return _channel.Value;

                _channel = new Lazy<IModel>(_connection.ConsumerConnection.CreateModel());

                ChannelFailureScenario(_channel.Value);

                return _channel.Value;
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
        _channel = new Lazy<IModel>(connection.ConsumerConnection.CreateModel());

        ChannelFailureScenario(_channel.Value);
    }

    private void ChannelFailureScenario(IModel channel)
    {
        channel.ModelShutdown += (sender, args) =>
        {
            if (args.ReplyCode == _disposeReasonCodeSuccess)
                return;

            _logger.LogError($"Channel lost --> {args.ReplyText} -- {args.Cause.Serialize()}");

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
                            //todo:check it 
                            if (channel.IsClosed)
                            {
                                channel = _connection.ConsumerConnection.CreateModel();
                                _logger.LogError("Channel opened...");
                            }
                            //todo:check it 

                            if (channel.IsOpen)
                            {
                                using var scope = _serviceScopeFactory.CreateScope();
                                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                                eventConsumer.ConsumeAsync().GetAwaiter().GetResult();

                                break;
                            }
                            _logger.LogError($"Channel waiting --> ${args.Cause.Serialize()}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"{ex.Message} --> ${args.Cause.Serialize()}");
                        }
                    }

                    _logger.LogError($"Connection waiting --> ${args.Cause.Serialize()}");

                    Thread.Sleep(1000 * _retryToConnectAfterSeconds);
                }
            }, token);
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            if (!_channel.IsValueCreated)
                return;

            if (!Channel.IsOpen)
                return;

            Channel.Close();

            foreach (var source in _cancellationTokenSources)
            {
                source.Cancel();
            }

            _disposed = true;
            GC.SuppressFinalize(this);

            _logger.LogInformation("Consumer channel closed successfully.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }


}
