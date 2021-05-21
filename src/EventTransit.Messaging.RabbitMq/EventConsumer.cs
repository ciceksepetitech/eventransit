using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Abstractions.QueueProcess;
using EventTransit.Core.Dto;
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
        private readonly IHttpProcessor _httpProcessor;
        private readonly IEventLog _eventLog;
        private readonly ILogger<EventConsumer> _logger;

        public EventConsumer(
            IRabbitMqConnectionFactory connection,
            IHttpProcessor httpProcessor,
            IEventsMongoRepository eventsRepository,
            IEventLog eventLog,
            ILogger<EventConsumer> logger)
        {
            _logger = logger;
            _eventLog = eventLog;
            _httpProcessor = httpProcessor;
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
                _channel.QueueDeclare(queue, false, false, false, null);
                _channel.QueueBind(queue, "order_created", queue);
                _channel.BasicConsume(queue, false, consumer);
            }
        }

        private void ReceiveMessage(object sender, BasicDeliverEventArgs ea)
        {
            var messageBody = ea.Body;
            var message = Encoding.UTF8.GetString(messageBody.ToArray());
            var eventName = ea.Exchange;
            var serviceName = ea.RoutingKey;
            
            try
            {
                _httpProcessor.ProcessAsync(eventName, serviceName, message).GetAwaiter().GetResult();

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.LogError("Message consume fail!", e);

                _channel.BasicNack(ea.DeliveryTag, false, false);

                _eventLog.Log(new EventLogDto
                {
                    EventName = eventName,
                    ServiceName = serviceName,
                    LogType = LogType.Fail,
                    Details = new EventDetailDto
                    {
                        Request = new HttpRequestDto
                        {
                            Body = message
                        },
                        Message = e.Message
                    }
                });
            }
        }
    }
}