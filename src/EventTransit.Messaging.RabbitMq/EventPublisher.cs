using System.Text;
using System.Text.Json;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Messaging.RabbitMq.Abstractions;

namespace EventTransit.Messaging.RabbitMq
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IRabbitMqConnectionFactory _connection;

        public EventPublisher(IRabbitMqConnectionFactory connection)
        {
            _connection = connection;
        }
        
        public void Publish(string name, dynamic payload)
        {
            using var channel = _connection.ProducerConnection.CreateModel();

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            // Publish.
            channel.BasicPublish(name, name, false, properties, body);
        }
    }
}