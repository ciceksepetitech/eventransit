using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        public async Task PublishAsync(string name, dynamic payload)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            var queues = await GetQueues(name);

            foreach (var queue in queues)
            {
                _channel.BasicPublish(name, queue, false, _properties, body);
            }
        }

        public void RegisterNewServiceAsync(NewServiceDto data)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

            _channel.BasicPublish(RabbitMqConstants.NewServiceQueue, RabbitMqConstants.NewServiceQueue, false,
                _properties, body);
        }

        private async Task<List<string>> GetQueues(string eventName)
        {
            return await _eventsDataService.GetQueueNamesByEventAsync(eventName);
        }
    }
}