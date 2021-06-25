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
                    var queueName = service.Name.GetQueueName(@event.Name);
                    channel.QueueDeclare(queue: queueName,
                        true,
                        false,
                        false,
                        null);
                    channel.QueueBind(queueName, @event.Name, @event.Name);

                    var retryQueueName = service.Name.GetRetryQueueName(@event.Name);
                    var retryQueueArguments = new Dictionary<string, object>
                    {
                        {"x-dead-letter-exchange", @event.Name},
                        {"x-dead-letter-routing-key", queueName},
                        {"x-message-ttl", MessagingConstants.DeadLetterQueueTTL}
                    };
                    channel.QueueDeclare(queue: retryQueueName,
                        true,
                        false,
                        false,
                        retryQueueArguments);
                    channel.QueueBind(retryQueueName, retryExchangeName, queueName);
                    channel.QueueBind(queueName, @event.Name, queueName);
                }
            }

            channel.ExchangeDeclare(MessagingConstants.NewServiceExchange, ExchangeType.Fanout, false, false, null);
        }
    }
}