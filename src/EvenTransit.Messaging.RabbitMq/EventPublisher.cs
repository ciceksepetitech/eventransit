using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IRabbitMqConnectionFactory _connection;

        public EventPublisher(IRabbitMqConnectionFactory connection)
        {
            _connection = connection;
        }

        public void Publish(string eventName, object payload)
        {
            var properties = _connection.ProducerChannel.CreateBasicProperties();
            properties.Persistent = true;
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            _connection.ProducerChannel.BasicPublish(eventName, eventName, false, properties, body);
        }

        public void PublishToRetry(string eventName, string serviceName, byte[] payload, long retryCount)
        {
            var newRetryCount = retryCount + 1;
            
            var properties = _connection.ProducerChannel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = new Dictionary<string, object>
            {
                {MessagingConstants.RetryHeaderName, newRetryCount}
            };
            
            _connection.ProducerChannel.BasicPublish(eventName.GetRetryExchangeName(), serviceName, false, properties, payload);
        }

        public void RegisterNewService(NewServiceDto data)
        {
            var properties = _connection.ProducerChannel.CreateBasicProperties();
            properties.Persistent = true;
            
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

            _connection.ProducerChannel.BasicPublish(MessagingConstants.NewServiceExchange, string.Empty, false,
                properties, body);
        }
    }
}