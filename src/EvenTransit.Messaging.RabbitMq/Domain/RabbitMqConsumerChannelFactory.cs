using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace EvenTransit.Messaging.RabbitMq.Domain;

public class RabbitMqConsumerChannelFactory : IRabbitMqChannelFactory, IDisposable
{
    private readonly ConcurrentDictionary<string, IModel> _channels = new();
    private readonly IRabbitMqConnectionFactory _connection;
    private readonly IList<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();
    private readonly ILogger<RabbitMqConsumerChannelFactory> _logger;

    private const int RetryToConnectAfterSeconds = 5;
    private const ushort DisposeReasonCodeSuccess = 200;

    public RabbitMqConsumerChannelFactory(IRabbitMqConnectionFactory connection,
        ILogger<RabbitMqConsumerChannelFactory> logger)
    {
        _connection = connection;
        _logger = logger;
    }
    
    public IModel ChannelForRecover(Action<IModel> recover)
    {
        var channel = _connection.ConsumerConnection.CreateModel();
        
        var id = Guid.NewGuid().ToString();
        _channels.TryAdd(id, channel);
        
        ChannelFailureScenario(id, channel, recover);
        
        return channel;
    }

    private void ChannelFailureScenario(string id, IModel channel, Action<IModel> recover)
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
                                channel = _connection.ConsumerConnection.CreateModel();
                            }

                            if (channel.IsOpen)
                            {
                                if (_channels.ContainsKey(id))
                                {
                                    _channels[id] = channel;
                                }
                                recover(channel);
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
        foreach (var pair in _channels)
        {
            pair.Value?.Close();
        }
        
        foreach (var source in _cancellationTokenSources)
        {
            source.Cancel();
        }

        GC.SuppressFinalize(this);

        _logger.ChannelState("Consumer channel closed successfully.");
    }
}
