using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Domain;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IModel _channel;
        private readonly RetryQueueHelper _retryQueueHelper;

        public EventPublisher(IEnumerable<IRabbitMqChannelFactory> channelFactories, RetryQueueHelper retryQueueHelper)
        {
            _retryQueueHelper = retryQueueHelper;
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
                CorrelationId = request.CorrelationId,
                OutboxEventId = request.OutboxEventId
            };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
            
            _channel.BasicPublish(request.EventName, request.EventName, false, properties, body);
        }

        public void PublishToRetry(string eventName, string serviceName, byte[] payload, long retryCount)
        {
            var newRetryCount = retryCount + 1;
            
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = new Dictionary<string, object>
            {
                {MessagingConstants.RetryHeaderName, newRetryCount}
            };

            var retryQueueName = serviceName.GetRetryQueueName(eventName, _retryQueueHelper.GetRetryQueue(retryCount));
            _channel.BasicPublish(eventName.GetRetryExchangeName(), retryQueueName, false, properties, payload);
        }

        public void RegisterNewService(NewServiceDto data)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

            _channel.BasicPublish(MessagingConstants.NewServiceExchange, string.Empty, false,
                properties, body);
        }
    }
}