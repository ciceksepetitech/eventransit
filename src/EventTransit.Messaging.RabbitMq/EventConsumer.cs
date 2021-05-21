using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Abstractions.QueueProcess;
using EventTransit.Core.Enums;
using EventTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventTransit.Messaging.RabbitMq
{
    public class EventConsumer : IEventConsumer
    {
        private readonly IModel _channel;
        private readonly List<string> _queues;
        private readonly IQueueProcessResolver _queueProcessResolver;
        private readonly ILogger<EventConsumer> _logger;

        public EventConsumer(
            IRabbitMqConnectionFactory connection,
            IQueueProcessResolver queueProcessResolver,
            IEventsMongoRepository eventsRepository,
            ILogger<EventConsumer> logger)
        {
            _logger = logger;
            _queueProcessResolver = queueProcessResolver;
            _channel = connection.ConsumerConnection.CreateModel();

            var events = eventsRepository.GetEvents().Result;

            // TODO Fix fetch queue name
            _queues = events.SelectMany(x => x.Services.Select(y => y.Name)).ToList();
        }

        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += ReceiveMessage;

            foreach (var queue in _queues)
            {
                // TODO Refactor here
                _channel.QueueDeclare(queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                _channel.QueueBind(queue, "order_created", queue);

                _channel.BasicConsume(queue,
                    autoAck: false,
                    consumer: consumer);
            }
        }

        private void ReceiveMessage(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                var messageBody = ea.Body;
                var message = Encoding.UTF8.GetString(messageBody.ToArray());
                var eventName = ea.Exchange;
                var serviceName = ea.RoutingKey;

                var queueProcess = _queueProcessResolver.Resolve(QueueProcessType.HttpRequest);
                var result = queueProcess.Process(eventName, serviceName, message);

                _channel.BasicAck(ea.DeliveryTag, false);

                // TODO Log success info to RabbitMq
            }
            catch (Exception e)
            {
                _logger.LogError("Message consume fail!", e);

                _channel.BasicNack(ea.DeliveryTag, false, false);

                // TODO Log Exceptions to RabbitMq
            }
        }
    }
}