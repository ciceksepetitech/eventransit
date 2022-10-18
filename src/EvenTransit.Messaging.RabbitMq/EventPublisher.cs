using System.Text;
using System.Text.Json;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Extensions;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq;

public class EventPublisher : IEventPublisher
{
    private const int _maxRetryCount = 5;
    private IModel Channel => _channelFactory.Channel;
    private readonly IRetryQueueHelper _retryQueueHelper;
    private readonly IRabbitMqChannelFactory _channelFactory;

    public EventPublisher(IEnumerable<IRabbitMqChannelFactory> channelFactories, 
        IRetryQueueHelper retryQueueHelper)
    {
        _retryQueueHelper = retryQueueHelper;
        _channelFactory = channelFactories.Single(x => x.ChannelType == ChannelTypes.Producer);
    }

    public void Publish(EventRequestDto request)
    {
        var properties = Channel.CreateBasicProperties();
        properties.Persistent = true;
        
        var data = new EventPublishDto
        {
            Payload = request.Payload,
            Fields = request.Fields,
            CorrelationId = request.CorrelationId
        };
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

        Channel.BasicPublish(request.EventName, request.EventName, false, properties, body);
    }

    public void PublishToRetry(string eventName, string serviceName, byte[] payload, long retryCount)
    {
        if (retryCount > _maxRetryCount)
            return;

        var newRetryCount = retryCount + 1;

        var properties = Channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Headers = new Dictionary<string, object> { { MessagingConstants.RetryHeaderName, newRetryCount } };

        var retryQueueName = serviceName.GetRetryQueueName(eventName, _retryQueueHelper.GetRetryQueue(retryCount));
        Channel.BasicPublish(eventName.GetRetryExchangeName(), retryQueueName, false, properties, payload);
    }

    public void RegisterNewService(NewServiceDto data)
    {
        var properties = Channel.CreateBasicProperties();
        properties.Persistent = true;

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

        Channel.BasicPublish(MessagingConstants.NewServiceExchange, string.Empty, false,
            properties, body);
    }
}
