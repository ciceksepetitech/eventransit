using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Entities;
using EventTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;

namespace EventTransit.Messaging.RabbitMq.Domain
{
    public class RabbitMqDeclaration : IRabbitMqDeclaration
    {
        private readonly IMongoRepository<Events> _eventsRepository;

        public RabbitMqDeclaration(IMongoRepository<Events> eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task DeclareQueuesAsync(IModel channel)
        {
            var events = await _eventsRepository.GetEvents();
            foreach (var @event in events)
            {
                channel.ExchangeDeclare(@event.Name, ExchangeType.Direct, true, false, null);

                foreach (var service in @event.Services)
                {
                    channel.QueueDeclare(queue: service.Name,
                        false,
                        false,
                        false,
                        null);
                    channel.QueueBind(service.Name, @event.Name, @event.Name);
                }
            }
        }
    }
}