using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Constants;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Domain;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EvenTransit.Messaging.RabbitMq;

public class EventConsumer : IEventConsumer
{
    private readonly IHttpProcessor _httpProcessor;
    private readonly IEventsRepository _eventsRepository;
    private readonly IEventLog _eventLog;
    private readonly ILogger<EventConsumer> _logger;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly IModel _channel;
    private readonly RetryQueueHelper _retryQueueHelper;

    public EventConsumer(
        IEnumerable<IRabbitMqChannelFactory> channelFactories,
        IHttpProcessor httpProcessor,
        IEventsRepository eventsRepository,
        IEventLog eventLog,
        ILogger<EventConsumer> logger,
        IMapper mapper,
        IEventPublisher eventPublisher,
        RetryQueueHelper retryQueueHelper)
    {
        _httpProcessor = httpProcessor;
        _eventsRepository = eventsRepository;
        _eventLog = eventLog;
        _logger = logger;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
        _retryQueueHelper = retryQueueHelper;
        var channelFactory = channelFactories.Single(x => x.ChannelType == ChannelTypes.Consumer);
        _channel = channelFactory.Channel;
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
            foreach (var service in @event.Services) BindQueue(eventName, service);
        }

        #endregion
    }

    public void DeleteQueue(string eventName, string serviceName)
    {
        var queueName = serviceName.GetQueueName(eventName);

        _channel.QueueDelete(queueName, false, false);

        foreach (var retryQueue in _retryQueueHelper.RetryQueueInfo)
        {
            var retryQueueName = serviceName.GetRetryQueueName(eventName, retryQueue.RetryTime);
            _channel.QueueDelete(retryQueueName, false, false);
        }
    }

    public void DeleteExchange(string eventName)
    {
        _channel.ExchangeDelete(eventName);
        _channel.ExchangeDelete(eventName.GetRetryExchangeName());
    }

    private async Task OnReceiveMessageAsync(string eventName, Service serviceInfo, BasicDeliverEventArgs ea)
    {
        var bodyArray = ea.Body.ToArray();
        var body = JsonSerializer.Deserialize<EventPublishDto>(Encoding.UTF8.GetString(bodyArray));
        var retryCount = GetRetryCount(ea.BasicProperties);
        var serviceData = _mapper.Map<ServiceDto>(serviceInfo);
        var serviceName = serviceData.Name;

        serviceData.Url = serviceData.Url.ReplaceDynamicFieldValues(body.Fields);
        serviceData.Headers ??= new Dictionary<string, string>();

        if (!serviceData.Headers.ContainsKey(HeaderConstants.RequestIdHeader))
            serviceData.Headers.Add(HeaderConstants.RequestIdHeader, body.CorrelationId);

        foreach (var header in serviceData.Headers)
            serviceData.Headers[header.Key] = header.Value.ReplaceDynamicFieldValues(body.Fields);

        try
        {
            var processResult = await _httpProcessor.ProcessAsync(eventName, serviceData, body);

            _channel.BasicAck(ea.DeliveryTag, false);

            if (!processResult)
                _eventPublisher.PublishToRetry(eventName, serviceName, bodyArray, retryCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Message consume fail!");

            _channel.BasicAck(ea.DeliveryTag, false);

            _eventPublisher.PublishToRetry(eventName, serviceName, bodyArray, retryCount);

            var logData = new EventLogDto
            {
                EventName = eventName,
                ServiceName = serviceInfo.Name,
                LogType = LogType.Fail,
                Details = new EventDetailDto
                {
                    Request = new HttpRequestDto
                    {
                        Body = body.Payload,
                        Fields = body.Fields,
                        Url = serviceData.Url,
                        Timeout = serviceData.Timeout,
                        Headers = serviceData.Headers
                    },
                    Response = new EventLogHttpResponseDto
                    {
                        IsSuccess = false, StatusCode = StatusCodes.Status500InternalServerError
                    },
                    Message = e.Message,
                    CorrelationId = body.CorrelationId
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
            _channel.ExchangeDeclare(eventName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);

            var service = _eventsRepository.GetServiceByEvent(eventName, serviceInfo.ServiceName);

            BindQueue(eventName, service);

            foreach (var retryQueue in _retryQueueHelper.RetryQueueInfo)
                BindRetryQueue(eventName, service.Name, retryQueue);

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
        var queueName = service.Name.GetQueueName(eventName);
        var eventConsumer = new EventingBasicConsumer(_channel);
        // TODO Map Service Entity to ServiceDto
        eventConsumer.Received += (_, ea) =>
        {
            OnReceiveMessageAsync(eventName, service, ea);
        };

        _channel.QueueBind(queueName, eventName, eventName);
        _channel.BasicConsume(queueName, false, eventConsumer);
    }

    private void BindRetryQueue(string eventName, string serviceName, RetryQueueInfo retryQueue)
    {
        var queueName = serviceName.GetQueueName(eventName);
        var retryExchangeName = eventName.GetRetryExchangeName();
        _channel.ExchangeDeclare(retryExchangeName, ExchangeType.Direct, true, false, null);

        var retryQueueName = serviceName.GetRetryQueueName(eventName, retryQueue.RetryTime);
        var retryQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", eventName },
            { "x-dead-letter-routing-key", queueName },
            { "x-message-ttl", retryQueue.TTL }
        };

        _channel.QueueDeclare(retryQueueName,
            true,
            false,
            false,
            retryQueueArguments);

        _channel.QueueBind(retryQueueName, retryExchangeName, queueName);
        _channel.QueueBind(queueName, eventName, queueName);
    }

    private long GetRetryCount(IBasicProperties properties)
    {
        if (properties?.Headers == null || !properties.Headers.ContainsKey(MessagingConstants.RetryHeaderName))
            return 0;

        return (long)properties.Headers[MessagingConstants.RetryHeaderName];
    }
}
