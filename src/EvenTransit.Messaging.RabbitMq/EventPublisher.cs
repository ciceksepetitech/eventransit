using EvenTransit.Domain.Configuration;
using System.Text;
using System.Text.Json;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq;

public class EventPublisher : IEventPublisher
{
    private readonly int _maxRetryCount;
    private readonly IRetryQueueHelper _retryQueueHelper;
    private readonly ILogger<EventPublisher> _logger;
    private readonly IRabbitMqPooledChannelProvider _channelProvider;

    public EventPublisher(IRetryQueueHelper retryQueueHelper,
        ILogger<EventPublisher> logger,
        IOptions<EvenTransitConfig> config,
        IRabbitMqPooledChannelProvider channelProvider)
    {
        _retryQueueHelper = retryQueueHelper;
        _logger = logger;
        _channelProvider = channelProvider;
        _maxRetryCount = config.Value.RetryCount;
    }

    public void Publish(EventRequestDto request)
    {
        var channel = _channelProvider.Channel();
        
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        var data = new EventPublishDto
        {
            Payload = request.Payload,
            Fields = request.Fields,
            CorrelationId = request.CorrelationId
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
        
        channel.ExchangeDeclare(request.EventName, ExchangeType.Direct, true, false, null);
        
        channel.BasicPublish(request.EventName, request.EventName, false, properties, body);
    }

    public void PublishToRetry(string eventName, string serviceName, byte[] payload, long retryCount)
    {
        if (_maxRetryCount == 0)
            return;

        if (retryCount >= _maxRetryCount)
        {
            _logger.MaxRetryReached($" event name : {eventName} - service name : {serviceName} - retry : {retryCount} ");
            return;
        }

        var newRetryCount = retryCount + 1;

        var channel = _channelProvider.Channel();
        
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Headers = new Dictionary<string, object> { { MessagingConstants.RetryHeaderName, newRetryCount } };

        var retryQueueName = serviceName.GetRetryQueueName(eventName, _retryQueueHelper.GetRetryQueue(retryCount));
        channel.BasicPublish(eventName.GetRetryExchangeName(), retryQueueName, false, properties, payload);
    }

    public void RegisterNewService(NewServiceDto data)
    {
        var channel = _channelProvider.Channel();
        
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

        channel.BasicPublish(MessagingConstants.NewServiceExchange, string.Empty, false, properties, body);
    }
}
