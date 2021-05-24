using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.QueueProcess;
using EvenTransit.Core.Constants;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;
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
        private AsyncEventingBasicConsumer _eventConsumer;
        private EventingBasicConsumer _newServiceConsumer;

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

        public async Task ConsumeAsync()
        {
            _eventConsumer = new AsyncEventingBasicConsumer(_channel);
            _newServiceConsumer = new EventingBasicConsumer(_channel);
            
            _eventConsumer.Received += OnReceiveMessageAsync;
            _newServiceConsumer.Received += OnNewServiceCreatedAsync;
            
            await BindQueues();
        }

        private async Task OnReceiveMessageAsync(object sender, BasicDeliverEventArgs ea)
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

        private void OnNewServiceCreatedAsync(object sender, BasicDeliverEventArgs ea)
        {
            var messageBody = ea.Body;
            var message = Encoding.UTF8.GetString(messageBody.ToArray());

            if (string.IsNullOrEmpty(message)) return;

            var serviceInfo = JsonSerializer.Deserialize<NewServiceDto>(message);

            if (serviceInfo == null) return;
            
            var eventName = serviceInfo.EventName;
            var serviceName = serviceInfo.ServiceName;
            
            try
            {
                _channel.ExchangeDeclare(eventName, ExchangeType.Direct, true, false, null);
                _channel.QueueDeclare(serviceName, false, false, false, null);
                _channel.QueueBind(serviceName, eventName, string.Empty);
                _channel.BasicConsume(serviceName, false, _eventConsumer);
                
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.LogError("New service creation fail!", e);

                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        }
        
        private async Task BindQueues()
        {
            var events = await _eventsRepository.GetEvents();

            foreach (var @event in events)
            {
                var eventName = @event.Name;
                foreach (var service in @event.Services)
                {
                    var serviceName = service.Name;
                    _channel.QueueBind(serviceName, eventName, serviceName);
                    _channel.BasicConsume(serviceName, false, _eventConsumer);
                }
            }
            
            _channel.QueueBind(RabbitMqConstants.NewServiceQueue, RabbitMqConstants.NewServiceQueue, string.Empty);
            _channel.BasicConsume(RabbitMqConstants.NewServiceQueue, false, _newServiceConsumer);
        }
    }
}