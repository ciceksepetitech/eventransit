using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Domain;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq.Domain
{
    public class RabbitMqDeclaration : IRabbitMqDeclaration
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly RetryQueueHelper _retryQueueHelper;

        public RabbitMqDeclaration(IEventsRepository eventsRepository, RetryQueueHelper retryQueueHelper)
        {
            _eventsRepository = eventsRepository;
            _retryQueueHelper = retryQueueHelper;
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

                    foreach (var retryQueue in _retryQueueHelper.RetryQueueInfo)
                    {
                        var retryQueueName = service.Name.GetRetryQueueName(@event.Name, retryQueue.RetryTime);
                        var retryQueueArguments = new Dictionary<string, object>
                        {
                            {"x-dead-letter-exchange", @event.Name},
                            {"x-dead-letter-routing-key", queueName},
                            {"x-message-ttl", retryQueue.TTL}
                        };
                        channel.QueueDeclare(queue: retryQueueName,
                            true,
                            false,
                            false,
                            retryQueueArguments);
                        channel.QueueBind(retryQueueName, retryExchangeName, retryQueueName);
                    }
                    
                    channel.QueueBind(queueName, @event.Name, queueName);
                }
            }

            channel.ExchangeDeclare(MessagingConstants.NewServiceExchange, ExchangeType.Fanout, false, false, null);
        }
    }
}