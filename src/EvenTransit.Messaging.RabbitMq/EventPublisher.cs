using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IModel _channel;

        public EventPublisher(IEnumerable<IRabbitMqChannelFactory> channelFactories)
        {
            var channelFactory = channelFactories.Single(x => x.ChannelType == ChannelTypes.Producer);
            _channel = channelFactory.Channel;
        }

        public void Publish(string eventName, object payload)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            _channel.BasicPublish(eventName, eventName, false, properties, body);
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
            
            _channel.BasicPublish(eventName.GetRetryExchangeName(), serviceName, false, properties, payload);
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