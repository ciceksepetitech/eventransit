using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Messaging.RabbitMq.Abstractions;

namespace EvenTransit.Messaging.RabbitMq
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IRabbitMqConnectionFactory _connection;
        private readonly IEventsMongoRepository _eventsRepository;

        public EventPublisher(IRabbitMqConnectionFactory connection, IEventsMongoRepository eventsRepository)
        {
            _connection = connection;
            _eventsRepository = eventsRepository;
        }
        
        public async Task PublishAsync(string name, dynamic payload)
        {
            using var channel = _connection.ProducerConnection.CreateModel();

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            var queues = await GetQueues(name);

            foreach (var queue in queues)
            {
                channel.BasicPublish(name, queue, false, properties, body);
            }
        }

        private async Task<List<string>> GetQueues(string eventName)
        {
            // TODO Cache queues
            var queues = await _eventsRepository.GetEvent(eventName);
            return queues.Services.Select(x => x.Name).ToList();
        }
    }
}