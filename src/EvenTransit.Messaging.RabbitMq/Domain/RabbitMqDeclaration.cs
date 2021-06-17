using System.Threading.Tasks;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain
{
    public class RabbitMqDeclaration : IRabbitMqDeclaration
    {
        private readonly IEventsRepository _eventsRepository;

        public RabbitMqDeclaration(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task DeclareQueuesAsync(IModel channel)
        {
            var events = await _eventsRepository.GetEventsAsync();
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

            channel.ExchangeDeclare(MessagingConstants.NewServiceQueue, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(MessagingConstants.NewServiceQueue, false, false, false, null);
            channel.QueueBind(MessagingConstants.NewServiceQueue, MessagingConstants.NewServiceQueue, string.Empty);
        }
    }
}