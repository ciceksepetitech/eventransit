using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.QueueProcess;
using EvenTransit.Core.Constants;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Dto.Service.Event;
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
        private readonly IEventsRepository _eventsRepository;
        private readonly IEventLog _eventLog;
        private readonly IModel _channel;
        private readonly ILogger<EventConsumer> _logger;
        private readonly IMapper _mapper;
        private EventingBasicConsumer _newServiceConsumer;

        public EventConsumer(
            IRabbitMqConnectionFactory connection,
            IHttpProcessor httpProcessor,
            IEventsRepository eventsRepository,
            IEventLog eventLog,
            ILogger<EventConsumer> logger, 
            IMapper mapper)
        {
            _httpProcessor = httpProcessor;
            _eventsRepository = eventsRepository;
            _eventLog = eventLog;
            _logger = logger;
            _mapper = mapper;
            _channel = connection.ConsumerConnection.CreateModel();
        }

        public async Task ConsumeAsync()
        {
            #region New Service Registration Queue

            _newServiceConsumer = new EventingBasicConsumer(_channel);
            _newServiceConsumer.Received += OnNewServiceCreated;

            _channel.QueueBind(RabbitMqConstants.NewServiceQueue, RabbitMqConstants.NewServiceQueue, string.Empty);
            _channel.BasicConsume(RabbitMqConstants.NewServiceQueue, false, _newServiceConsumer);

            #endregion

            #region Event Service Registration

            var events = await _eventsRepository.GetEventsAsync();

            foreach (var @event in events)
            {
                var eventName = @event.Name;
                foreach (var service in @event.Services)
                {
                    BindQueue(eventName, service);
                }
            }

            #endregion
        }

        private async Task OnReceiveMessageAsync(string eventName, Service serviceInfo, BasicDeliverEventArgs ea)
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            try
            {
                var serviceData = _mapper.Map<ServiceDto>(serviceInfo);
                
                await _httpProcessor.ProcessAsync(eventName, serviceData, message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.LogError("Message consume fail!", e);

                _channel.BasicNack(ea.DeliveryTag, false, false);

                await _eventLog.LogAsync(new EventLogDto
                {
                    EventName = eventName,
                    ServiceName = serviceInfo.Name,
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

        private void OnNewServiceCreated(object sender, BasicDeliverEventArgs ea)
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

                var service = _eventsRepository.GetServiceByEvent(eventName, serviceName);
                BindQueue(eventName, service);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.LogError("New service creation fail!", e);

                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        }

        private void BindQueue(string eventName, Service service)
        {
            var eventConsumer = new EventingBasicConsumer(_channel);
            eventConsumer.Received += (sender, ea) =>
            {
                // TODO Inspect
                OnReceiveMessageAsync(eventName, service, ea);
            };

            _channel.QueueBind(service.Name, eventName, eventName);
            _channel.BasicConsume(service.Name, false, eventConsumer);
        }
    }
}