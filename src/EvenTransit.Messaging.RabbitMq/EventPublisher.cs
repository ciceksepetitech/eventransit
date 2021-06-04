using System.Text;
using System.Text.Json;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Constants;
using EvenTransit.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IModel _channel;
        private readonly IBasicProperties _properties;
        private readonly IEventsDataService _eventsDataService;

        public EventPublisher(IRabbitMqConnectionFactory connection, IEventsDataService eventsDataService)
        {
            _eventsDataService = eventsDataService;

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

            _channel.BasicPublish(RabbitMqConstants.NewServiceQueue, RabbitMqConstants.NewServiceQueue, false,
                _properties, body);
        }
    }
}