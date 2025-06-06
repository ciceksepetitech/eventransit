﻿using System.Text;
using System.Text.Json;
using AutoMapper;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Configuration;
using EvenTransit.Domain.Constants;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Messaging.RabbitMq.Abstractions;
using EvenTransit.Messaging.RabbitMq.Domain;
using EvenTransit.Messaging.RabbitMq.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    private readonly ICustomObjectMapper _customObjectMapper;
    private readonly IRabbitMqPooledChannelProvider _channelProvider;
    private readonly IRabbitMqChannelFactory _channelFactory;
    private readonly int _maxRetryCount;

    public EventConsumer(
        IRabbitMqChannelFactory channelFactory,
        IHttpProcessor httpProcessor,
        IEventsRepository eventsRepository,
        IEventLog eventLog,
        ILogger<EventConsumer> logger,
        IMapper mapper,
        IEventPublisher eventPublisher,
        ICustomObjectMapper customObjectMapper,
        IOptions<EvenTransitConfig> config,
        IRabbitMqPooledChannelProvider channelProvider)
    {
        _httpProcessor = httpProcessor;
        _eventsRepository = eventsRepository;
        _eventLog = eventLog;
        _logger = logger;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
        _customObjectMapper = customObjectMapper;
        _channelProvider = channelProvider;
        _channelFactory = channelFactory;
        _maxRetryCount = config.Value.RetryCount;
    }

    public async Task ConsumeAsync()
    {
        #region New Service Registration Queue

        void BindNewServiceConsumer(IModel c)
        {
            var newServiceConsumer = new AsyncEventingBasicConsumer(c);
            newServiceConsumer.Received += async (sender, ea) =>
            {
                await OnNewServiceCreated(c, sender, ea);
            };

            c.ExchangeDeclare(MessagingConstants.NewServiceExchange, ExchangeType.Fanout, false, false, null);
            var queueName = c.QueueDeclare().QueueName;
            c.QueueBind(queueName, MessagingConstants.NewServiceExchange, string.Empty);
            c.BasicConsume(queueName, false, newServiceConsumer);
        }

        var channel = _channelFactory.ChannelForRecover(BindNewServiceConsumer);
        BindNewServiceConsumer(channel);

        #endregion

        #region Event Service Registration

        var events = await _eventsRepository.GetEventsAsync();

        foreach (var @event in events)
        {
            var eventName = @event.Name;

            var ch = _channelFactory.ChannelForRecover(c =>
            {
                var e = _eventsRepository.GetEvent(x => x.Name == eventName);

                foreach (var s in e.Services)
                    Bind(c, eventName, s);
            });

            foreach (var service in @event.Services)
                Bind(ch, eventName, service);
        }

        #endregion
    }

    public void DeleteQueue(string eventName, string serviceName)
    {
        var queueName = serviceName.GetQueueName(eventName);

        var channel = _channelProvider.Channel();

        channel.QueueDelete(queueName, false, false);

        var retryQueueName = serviceName.GetRetryQueueName(eventName);
        channel.QueueDelete(retryQueueName, false, false);

        var delayQueueName = serviceName.GetDelayQueueName(eventName);
        channel.QueueDelete(delayQueueName, false, false);
    }

    public void DeleteExchange(string eventName)
    {
        var channel = _channelProvider.Channel();
        channel.ExchangeDelete(eventName);
        channel.ExchangeDelete(eventName.GetRetryExchangeName());
        channel.ExchangeDelete(eventName.GetRetryExchangeName());
    }

    private async Task OnReceiveMessageAsync(IModel channel, string eventName, ServiceDto serviceData, BasicDeliverEventArgs ea)
    {
        var serviceName = serviceData.Name;
        var bodyArray = ea.Body.ToArray();
        var consumeDate = DateTime.UtcNow;

        if (serviceData.DelaySeconds > 0 && !IsAlreadyDelayed(ea.BasicProperties))
        {
            _eventPublisher.PublishToDelay(eventName, serviceName, bodyArray, serviceData.DelaySeconds);
            channel.BasicAck(ea.DeliveryTag, false);
            return;
        }

        var body = JsonSerializer.Deserialize<EventPublishDto>(Encoding.UTF8.GetString(bodyArray));
        var retryCount = GetRetryCount(ea.BasicProperties);

        var replacedUrl = serviceData.Url.ReplaceDynamicFieldValues(body?.Fields);
        var requestHeaders = new Dictionary<string, string>();
        foreach (var header in serviceData.Headers)
            requestHeaders.Add(header.Key, header.Value);

        if (!requestHeaders.ContainsKey(HeaderConstants.RequestIdHeader))
            requestHeaders.Add(HeaderConstants.RequestIdHeader, body?.CorrelationId);

        foreach (var header in requestHeaders)
            requestHeaders[header.Key] = header.Value.ReplaceDynamicFieldValues(body?.Fields);

        if (body != null)
            body.ConsumeDate = consumeDate;

        try
        {
            if (serviceData.CustomBodyMap != null && serviceData.CustomBodyMap.Any() && body is { Payload: JsonElement element })
            {
                body.Payload = _customObjectMapper.Map(element, serviceData.CustomBodyMap);
            }

            var requestData = serviceData with { Url = replacedUrl, Headers = requestHeaders };
            var processResult = await _httpProcessor.ProcessAsync(eventName, requestData, body, retryCount);

            if (!processResult)
                _eventPublisher.PublishToRetry(eventName, serviceName, bodyArray, retryCount);

            channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.AckFailed($"Message consume fail! - event name : {eventName} - service name : {serviceData.Name} - retry : {retryCount} ", e, retryCount, _maxRetryCount);

            _eventPublisher.PublishToRetry(eventName, serviceName, bodyArray, retryCount);

            channel.BasicAck(ea.DeliveryTag, false);

            var logData = new EventLogDto
            {
                EventName = eventName,
                ServiceName = serviceData.Name,
                LogType = LogType.Fail,
                Details = new EventDetailDto
                {
                    Request = new HttpRequestDto
                    {
                        Body = body?.Payload,
                        Fields = body?.Fields,
                        Url = replacedUrl,
                        Timeout = serviceData.Timeout,
                        DelaySeconds = serviceData.DelaySeconds,
                        Headers = requestHeaders
                    },
                    Response = new EventLogHttpResponseDto
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status500InternalServerError
                    },
                    Message = e.Message,
                    CorrelationId = body?.CorrelationId,
                    PublishDate = body?.PublishDate,
                    ConsumeDate = body?.ConsumeDate,
                    Retry = retryCount
                }
            };

            await _eventLog.LogAsync(_mapper.Map<Logs>(logData));
        }
    }

    private async Task OnNewServiceCreated(IModel channel, object sender, BasicDeliverEventArgs ea)
    {
        var messageBody = ea.Body;
        var message = Encoding.UTF8.GetString(messageBody.ToArray());

        if (string.IsNullOrEmpty(message))
            return;

        var serviceInfo = JsonSerializer.Deserialize<NewServiceDto>(message);

        if (serviceInfo == null)
            return;

        var eventName = serviceInfo.EventName;
        var queueName = serviceInfo.ServiceName.GetQueueName(eventName);

        try
        {
            var service = await _eventsRepository.GetServiceByEventAsync(eventName, serviceInfo.ServiceName);

            Bind(channel, eventName, service);

            channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.ConsumerFailed($"New service creation fail! - queue name {queueName} ", e);

            channel.BasicNack(ea.DeliveryTag, false, false);
        }
    }

    private void Bind(IModel channel, string eventName, Service service)
    {
        var serviceData = _mapper.Map<ServiceDto>(service);

        var queueName = service.Name.GetQueueName(eventName);

        if (RabbitMqConsumerTagWrapper._tags.TryGetValue(queueName, out var t) && t.Channel.IsOpen)
        {
            try
            {
                t.Channel.BasicCancel(t.Value);
            }
            catch (Exception e)
            {
                _logger.ConsumerWarning($"New service creation issue! Unable to cancel the old receiver. queue: {queueName} ", e);
            }
            RabbitMqConsumerTagWrapper._tags.TryRemove(queueName, out _);
        }

        var eventConsumer = new AsyncEventingBasicConsumer(channel);

        eventConsumer.Received += async (_, ea) =>
        {
            await OnReceiveMessageAsync(channel, eventName, serviceData, ea);
        };

        channel.ExchangeDeclare(eventName, ExchangeType.Direct, true, false, null);
        channel.QueueDeclare(queueName, true, false, false, null);
        channel.QueueBind(queueName, eventName, eventName); // bind routing key events.
        channel.QueueBind(queueName, eventName, queueName); // bind routing key for retries and delayed ones.

        BindRetryQueues(channel, eventName, service.Name);

        if (service.DelaySeconds > 0)
        {
            BindDelayQueues(channel, eventName, service.Name);
        }

        var tag = channel.BasicConsume(queueName, false, eventConsumer);

        RabbitMqConsumerTagWrapper._tags.TryAdd(queueName, new ConsumerTag
        {
            Value = tag,
            Channel = channel
        });
    }

    private static void BindRetryQueues(IModel channel, string eventName, string serviceName)
    {
        var queueName = serviceName.GetQueueName(eventName);
        var retryExchangeName = eventName.GetRetryExchangeName();
        channel.ExchangeDeclare(retryExchangeName, ExchangeType.Direct, true, false, null);

        var retryQueueName = serviceName.GetRetryQueueName(eventName);
        var retryQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", eventName },
            { "x-dead-letter-routing-key", queueName }
        };

        channel.QueueDeclare(retryQueueName,
            true,
            false,
            false,
            retryQueueArguments);

        channel.QueueBind(retryQueueName, retryExchangeName, retryQueueName);
    }

    private static void BindDelayQueues(IModel channel, string eventName, string serviceName)
    {
        var queueName = serviceName.GetQueueName(eventName);
        var delayExchangeName = eventName.GetDelayExchangeName();
        channel.ExchangeDeclare(delayExchangeName, ExchangeType.Direct, true, false, null);

        var delayQueueName = serviceName.GetDelayQueueName(eventName);
        var delayQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", eventName },
            { "x-dead-letter-routing-key", queueName },
            { "x-queue-mode", "lazy" } // save to disk.
        };

        channel.QueueDeclare(delayQueueName,
            true,
            false,
            false,
            delayQueueArguments);

        channel.QueueBind(delayQueueName, delayExchangeName, delayQueueName);
    }

    private static long GetRetryCount(IBasicProperties properties)
    {
        if (properties?.Headers == null ||
            !properties.Headers.TryGetValue(MessagingConstants.RetryHeaderName, out var header))
        {
            return 0;
        }

        return (long)header;
    }

    private static bool IsAlreadyDelayed(IBasicProperties properties)
    {
        if (properties?.Headers == null ||
            !properties.Headers.TryGetValue(MessagingConstants.CustomDelayHeaderName, out var header))
        {
            return false;
        }

        return (bool)header;
    }
}
