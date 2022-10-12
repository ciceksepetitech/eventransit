using System.Text;
using System.Text.Json;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Domain;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq;

public class EventPublisher : IEventPublisher
{
    private readonly IModel _channel;
    private readonly RetryQueueHelper _retryQueueHelper;
    private const int _maxRetryCount = 5;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IEnumerable<IRabbitMqChannelFactory> channelFactories,
        RetryQueueHelper retryQueueHelper,
        ILogger<EventPublisher> logger)
    {
        _retryQueueHelper = retryQueueHelper;
        _logger = logger;
        var channelFactory = channelFactories.Single(x => x.ChannelType == ChannelTypes.Producer);
        _channel = channelFactory.Channel;
    }

    public void Publish(EventRequestDto request)
    {
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        var data = new EventPublishDto
        {
            Payload = request.Payload,
            Fields = request.Fields,
            CorrelationId = request.CorrelationId
        };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

        _channel.BasicPublish(request.EventName, request.EventName, false, properties, body);
    }

    public void PublishToRetry(string eventName, string serviceName, byte[] payload, long retryCount)
    {
        _logger.LogInformation($"PublishToRetry step 1 --> event name : {eventName} - service name : {serviceName} and retry count : {retryCount}");

        if (retryCount > _maxRetryCount) return;

        _logger.LogInformation($"PublishToRetry step 2 --> event name : {eventName} - service name : {serviceName} and retry count : {retryCount}");

        var newRetryCount = retryCount + 1;

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Headers = new Dictionary<string, object> { { MessagingConstants.RetryHeaderName, newRetryCount } };

        var retryQueueName = serviceName.GetRetryQueueName(eventName, _retryQueueHelper.GetRetryQueue(retryCount));
        _channel.BasicPublish(eventName.GetRetryExchangeName(), retryQueueName, false, properties, payload);

        _logger.LogInformation($"PublishToRetry step 3 --> event name : {eventName} - service name : {serviceName} and retry count : {retryCount}");
    }

    public void RegisterNewService(NewServiceDto data)
    {
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

        _channel.BasicPublish(MessagingConstants.NewServiceExchange, string.Empty, false, properties, body);
    }
}
