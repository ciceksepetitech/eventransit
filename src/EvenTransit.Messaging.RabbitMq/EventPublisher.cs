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
    private readonly ILogger<EventPublisher> _logger;
    private readonly IRabbitMqPooledChannelProvider _channelProvider;

    public EventPublisher(ILogger<EventPublisher> logger,
        IOptions<EvenTransitConfig> config,
        IRabbitMqPooledChannelProvider channelProvider)
    {
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
            CorrelationId = request.CorrelationId,
            PublishDate = request.PublishDate
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
        var expiration = retryCount switch
        {
            //0,1 --> 5 sec
            >= 0 and <= 1 => "5000",
            //2,3 --> 30 sec
            > 1 and <= 3 => "30000",
            //4,5 --> 60 sec
            _ => "60000"
        };
        properties.Expiration = expiration;
        properties.Headers = new Dictionary<string, object> { { MessagingConstants.RetryHeaderName, newRetryCount } };

        var retryQueueName = serviceName.GetRetryQueueName(eventName);
        channel.BasicPublish(eventName.GetRetryExchangeName(), retryQueueName, false, properties, payload);
    }
    
    public void PublishToDelay(string eventName, string serviceName, byte[] payload, int delaySeconds)
    {
        var channel = _channelProvider.Channel();
        
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        const int msInSec = 1000;
        properties.Expiration = (delaySeconds * msInSec).ToString();
        properties.Headers = new Dictionary<string, object> { { MessagingConstants.CustomDelayHeaderName, true } };

        var delayQueueName = serviceName.GetDelayQueueName(eventName);
        channel.BasicPublish(eventName.GetDelayExchangeName(), delayQueueName, false, properties, payload);
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
