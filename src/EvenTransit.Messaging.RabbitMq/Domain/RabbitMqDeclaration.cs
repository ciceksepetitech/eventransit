using System.Collections.Generic;
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
                var retryExchangeName = @event.Name.GetRetryExchangeName();
                channel.ExchangeDeclare(@event.Name, ExchangeType.Direct, true, false, null);
                channel.ExchangeDeclare(retryExchangeName, ExchangeType.Direct, true, false, null);

                foreach (var service in @event.Services)
                {
                    channel.QueueDeclare(queue: service.Name,
                        true,
                        false,
                        false,
                        null);
                    channel.QueueBind(service.Name, @event.Name, @event.Name);

                    var retryQueueName = service.Name.GetRetryQueueName(@event.Name);
                    var retryQueueArguments = new Dictionary<string, object>
                    {
                        {"x-dead-letter-exchange", @event.Name},
                        {"x-dead-letter-routing-key", service.Name},
                        {"x-message-ttl", MessagingConstants.DeadLetterQueueTTL}
                    };
                    channel.QueueDeclare(queue: retryQueueName,
                        true,
                        false,
                        false,
                        retryQueueArguments);
                    channel.QueueBind(retryQueueName, retryExchangeName, service.Name);
                    channel.QueueBind(service.Name, @event.Name, service.Name);
                }
            }

            channel.ExchangeDeclare(MessagingConstants.NewServiceExchange, ExchangeType.Fanout, false, false, null);
        }
    }
}