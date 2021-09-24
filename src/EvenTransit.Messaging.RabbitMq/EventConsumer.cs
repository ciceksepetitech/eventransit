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
        private readonly ILogger<EventConsumer> _logger;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRabbitMqConnectionFactory _connection;

        public EventConsumer(
            IRabbitMqConnectionFactory connection,
            IHttpProcessor httpProcessor,
            IEventsRepository eventsRepository,
            IEventLog eventLog,
            ILogger<EventConsumer> logger,
            IMapper mapper,
            IEventPublisher eventPublisher)
        {
            _connection = connection;
            _httpProcessor = httpProcessor;
            _eventsRepository = eventsRepository;
            _eventLog = eventLog;
            _logger = logger;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task ConsumeAsync()
        {
            #region New Service Registration Queue
            
            var newServiceConsumer = new EventingBasicConsumer(_connection.ConsumerChannel);
            newServiceConsumer.Received += OnNewServiceCreated;

            var queueName = _connection.ConsumerChannel.QueueDeclare().QueueName;
            _connection.ConsumerChannel.QueueBind(queueName, MessagingConstants.NewServiceExchange, string.Empty);
            _connection.ConsumerChannel.BasicConsume(queueName, false, newServiceConsumer);

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
            var retryCount = GetRetryCount(ea.BasicProperties);
            var serviceData = _mapper.Map<ServiceDto>(serviceInfo);
            var serviceName = serviceData.Name;
            var queueName = serviceName.GetQueueName(eventName);

            try
            {
                var processResult = await _httpProcessor.ProcessAsync(eventName, serviceData, bodyArray);
                
                _connection.ConsumerChannel.BasicAck(ea.DeliveryTag, false);

                if (!processResult && retryCount < MessagingConstants.MaxRetryCount)
                    _eventPublisher.PublishToRetry(eventName, queueName, bodyArray, retryCount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Message consume fail!");

                _connection.ConsumerChannel.BasicAck(ea.DeliveryTag, false);

                if (retryCount < MessagingConstants.MaxRetryCount)
                    _eventPublisher.PublishToRetry(eventName, queueName, bodyArray, retryCount);

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
            var queueName = serviceInfo.ServiceName.GetQueueName(eventName);

            try
            {
                _connection.ConsumerChannel.ExchangeDeclare(eventName, ExchangeType.Direct, true, false, null);
                _connection.ConsumerChannel.QueueDeclare(queueName, true, false, false, null);

                var service = _eventsRepository.GetServiceByEvent(eventName, serviceInfo.ServiceName);
                BindQueue(eventName, service);
                BindRetryQueue(eventName, service.Name);

                _connection.ConsumerChannel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.LogError("New service creation fail!", e);

                _connection.ConsumerChannel.BasicNack(ea.DeliveryTag, false, false);
            }
        }

        private void BindQueue(string eventName, Service service)
        {
            var queueName = service.Name.GetQueueName(eventName);
            var eventConsumer = new EventingBasicConsumer(_connection.ConsumerChannel);
            // TODO Map Service Entity to ServiceDto
            eventConsumer.Received += (_, ea) =>
            {
                _logger.LogInformation($"EventName: {eventName} ServiceName: {queueName}");
                OnReceiveMessageAsync(eventName, service, ea);
            };

            _connection.ConsumerChannel.QueueBind(queueName, eventName, eventName);
            _connection.ConsumerChannel.BasicConsume(queueName, false, eventConsumer);
        }

        private void BindRetryQueue(string eventName, string serviceName)
        {
            var queueName = serviceName.GetQueueName(eventName);
            var retryExchangeName = eventName.GetRetryExchangeName();
            _connection.ConsumerChannel.ExchangeDeclare(retryExchangeName, ExchangeType.Direct, true, false, null);

            var retryQueueName = serviceName.GetRetryQueueName(eventName);
            var retryQueueArguments = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", eventName},
                {"x-dead-letter-routing-key", queueName},
                {"x-message-ttl", MessagingConstants.DeadLetterQueueTTL}
            };

            _connection.ConsumerChannel.QueueDeclare(queue: retryQueueName,
                true,
                false,
                false,
                retryQueueArguments);

            _connection.ConsumerChannel.QueueBind(retryQueueName, retryExchangeName, queueName);
            _connection.ConsumerChannel.QueueBind(queueName, eventName, queueName);
        }

        private long GetRetryCount(IBasicProperties properties)
        {
            if (properties?.Headers == null || !properties.Headers.ContainsKey(MessagingConstants.RetryHeaderName)) 
                return 0;

            return (long) properties.Headers[MessagingConstants.RetryHeaderName];
        }
    }
}