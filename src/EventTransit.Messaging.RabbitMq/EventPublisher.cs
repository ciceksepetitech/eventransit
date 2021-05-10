using System.Text;
using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Common;
using RabbitMQ.Client;

namespace EventTransit.Messaging.RabbitMq
{
    public class EventPublisher : IEventPublisher
    {
        
        public async Task PublishAsync(string name, dynamic payload)
        {
            //TODO: Refactor this
            //TODO: Service name can be given as a parameter for declaring queue names
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq", Port = 5672, UserName = "guest", Password = "guest"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(name, ExchangeType.Direct, true, false,
                null);
            channel.QueueDeclare(queue: "cargo",
                false,
                false,
                false,
                null);
            channel.QueueBind("cargo", name, name);
            
            var body = Encoding.UTF8.GetBytes(payload.Serialize());
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            
            // Publish.
            channel.BasicPublish(name, name, false, properties, body);
        }
    }
}