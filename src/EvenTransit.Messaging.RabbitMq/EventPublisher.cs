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
        private readonly IModel _channel;
        private readonly IBasicProperties _properties;

        public EventPublisher(IRabbitMqConnectionFactory connection)
        {
            _channel = connection.ProducerConnection.CreateModel();
            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;
        }

        public void Publish(string eventName, dynamic payload)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            _channel.BasicPublish(eventName, eventName, false, _properties, body);
        }

        public void RegisterNewService(NewServiceDto data)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

            _channel.BasicPublish(MessagingConstants.NewServiceQueue, MessagingConstants.NewServiceQueue, false,
                _properties, body);
        }
    }
}