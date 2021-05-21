using System;
using System.Text;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.QueueProcess;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Enums;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EvenTransit.Messaging.RabbitMq
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
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += ReceiveMessageAsync;

            BindQueues(consumer);
        }

        private async Task ReceiveMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            var messageBody = ea.Body;
            var message = Encoding.UTF8.GetString(messageBody.ToArray());
            var eventName = ea.Exchange;
            var serviceName = ea.RoutingKey;

            try
            {
                await _httpProcessor.ProcessAsync(eventName, serviceName, message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.LogError("Message consume fail!", e);

                _channel.BasicNack(ea.DeliveryTag, false, false);

                await _eventLog.LogAsync(new EventLogDto
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
        
        private void BindQueues(AsyncEventingBasicConsumer consumer)
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