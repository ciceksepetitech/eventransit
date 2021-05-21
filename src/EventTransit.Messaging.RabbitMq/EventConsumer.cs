using System;
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
        private readonly IHttpProcessor _httpProcessor;
        private readonly IEventsMongoRepository _eventsRepository;
        private readonly IEventLog _eventLog;
        private readonly IModel _channel;
        private readonly ILogger<EventConsumer> _logger;

        public EventConsumer(
            IRabbitMqConnectionFactory connection,
            IHttpProcessor httpProcessor,
            IEventsMongoRepository eventsRepository,
            IEventLog eventLog,
            ILogger<EventConsumer> logger)
        {
            _httpProcessor = httpProcessor;
            _eventsRepository = eventsRepository;
            _eventLog = eventLog;
            _logger = logger;
            _channel = connection.ConsumerConnection.CreateModel();
        }

        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ReceiveMessage;

            BindQueues(consumer);
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
        
        private void BindQueues(EventingBasicConsumer consumer)
        {
            var events = _eventsRepository.GetEvents().Result;

            foreach (var @event in events)
            {
                var eventName = @event.Name;
                foreach (var service in @event.Services)
                {
                    var serviceName = service.Name;
                    _channel.QueueBind(serviceName, eventName, serviceName);
                    _channel.BasicConsume(serviceName, false, consumer);
                }
            }
        }
    }
}