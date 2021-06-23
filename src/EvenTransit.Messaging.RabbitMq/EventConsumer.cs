using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
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
        private readonly IEventPublisher _eventPublisher;

        public EventConsumer(
            IRabbitMqConnectionFactory connection,
            IHttpProcessor httpProcessor,
            IEventsRepository eventsRepository,
            IEventLog eventLog,
            ILogger<EventConsumer> logger,
            IMapper mapper, 
            IEventPublisher eventPublisher)
        {
            _httpProcessor = httpProcessor;
            _eventsRepository = eventsRepository;
            _eventLog = eventLog;
            _logger = logger;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
            _channel = connection.ConsumerConnection.CreateModel();
        }

        public async Task ConsumeAsync()
        {
            #region New Service Registration Queue

            var newServiceConsumer = new EventingBasicConsumer(_channel);
            newServiceConsumer.Received += OnNewServiceCreated;

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, MessagingConstants.NewServiceExchange, string.Empty);
            _channel.BasicConsume(queueName, false, newServiceConsumer);

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
            var bodyArray = ea.Body.ToArray();
            try
            {
                var serviceData = _mapper.Map<ServiceDto>(serviceInfo);
                var processResult = await _httpProcessor.ProcessAsync(eventName, serviceData, bodyArray);

                var retryCount = GetRetryCount(ea.BasicProperties);
                _logger.LogInformation(retryCount?.ToString() ?? "-1");
                
                _channel.BasicAck(ea.DeliveryTag, false);

                if (!processResult)
                {
                    _eventPublisher.PublishToRetry(eventName, serviceData.Name, bodyArray);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Message consume fail!", e);

                var retryCount = GetRetryCount(ea.BasicProperties);
                _logger.LogInformation(retryCount?.ToString() ?? "-1");
                
                _channel.BasicAck(ea.DeliveryTag, false);
                _eventPublisher.PublishToRetry(eventName, serviceInfo.Name, bodyArray);
                
                var logData = new EventLogDto
                {
                    EventName = eventName,
                    ServiceName = serviceInfo.Name,
                    LogType = LogType.Fail,
                    Details = new EventDetailDto
                    {
                        Request = new HttpRequestDto
                        {
                            Body = bodyArray
                        },
                        Message = e.Message
                    }
                };
                
                await _eventLog.LogAsync(_mapper.Map<Logs>(logData));
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
                _channel.QueueBind(serviceName, eventName, eventName);

                var service = _eventsRepository.GetServiceByEvent(eventName, serviceName);
                BindQueue(eventName, service);
                BindRetryQueue(eventName, serviceName);

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
            // TODO Map Service Entity to ServiceDto
            eventConsumer.Received += (sender, ea) => { OnReceiveMessageAsync(eventName, service, ea); };

            _channel.QueueBind(service.Name, eventName, eventName);
            _channel.BasicConsume(service.Name, false, eventConsumer);
        }

        private void BindRetryQueue(string eventName, string serviceName)
        {
            var retryExchangeName = eventName.GetRetryExchangeName();
            _channel.ExchangeDeclare(retryExchangeName, ExchangeType.Direct, false, false, null);
            
            var retryQueueName = serviceName.GetRetryQueueName();
            var retryQueueArguments = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", eventName},
                {"x-dead-letter-routing-key", serviceName},
                {"x-message-ttl", MessagingConstants.DeadLetterQueueTTL}
            };
            
            _channel.QueueDeclare(queue: retryQueueName,
                false,
                false,
                false,
                retryQueueArguments);
            
            _channel.QueueBind(retryQueueName, retryExchangeName, serviceName);
            _channel.QueueBind(serviceName, eventName, serviceName);
        }
        
        private long? GetRetryCount(IBasicProperties properties)
        {
            if (properties.Headers.ContainsKey("x-death"))
            {
                var deathProperties = (List<object>)properties.Headers["x-death"];
                var lastRetry = (Dictionary<string, object>)deathProperties[0];
                var count = lastRetry["count"];
                return (long)count;
            }
            else
            {
                return null;
            }
        }
    }
}