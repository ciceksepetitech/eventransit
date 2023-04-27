using System.Text;
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
    private readonly IRetryQueueHelper _retryQueueHelper;
    private readonly ICustomObjectMapper _customObjectMapper;
    private readonly IRabbitMqChannelFactory _channelFactory;
    private readonly int _maxRetryCount;

    private IModel Channel => _channelFactory.Channel;

    public EventConsumer(
        IEnumerable<IRabbitMqChannelFactory> channelFactories,
        IHttpProcessor httpProcessor,
        IEventsRepository eventsRepository,
        IEventLog eventLog,
        ILogger<EventConsumer> logger,
        IMapper mapper,
        IEventPublisher eventPublisher,
        IRetryQueueHelper retryQueueHelper,
        ICustomObjectMapper customObjectMapper,
        IOptions<EvenTransitConfig> config)
    {
        _httpProcessor = httpProcessor;
        _eventsRepository = eventsRepository;
        _eventLog = eventLog;
        _logger = logger;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
        _retryQueueHelper = retryQueueHelper;
        _customObjectMapper = customObjectMapper;
        _channelFactory = channelFactories.Single(x => x.ChannelType == ChannelTypes.Consumer);
        _maxRetryCount = config.Value.RetryCount;
    }

    public async Task ConsumeAsync()
    {
        #region New Service Registration Queue

        var newServiceConsumer = new AsyncEventingBasicConsumer(Channel);
        newServiceConsumer.Received += OnNewServiceCreated;

        var channel = Channel;

        channel.ExchangeDeclare(MessagingConstants.NewServiceExchange, ExchangeType.Fanout, false, false, null);
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName, MessagingConstants.NewServiceExchange, string.Empty);
        channel.BasicConsume(queueName, false, newServiceConsumer);

        #endregion

        #region Event Service Registration

        var events = await _eventsRepository.GetEventsAsync();

        foreach (var @event in events)
        {
            var eventName = @event.Name;
            foreach (var service in @event.Services)
                Bind(eventName, service);
        }

        #endregion
    }

    public void DeleteQueue(string eventName, string serviceName)
    {
        var queueName = serviceName.GetQueueName(eventName);

        Channel.QueueDelete(queueName, false, false);

        foreach (var retryQueue in _retryQueueHelper.RetryQueueInfo)
        {
            var retryQueueName = serviceName.GetRetryQueueName(eventName, retryQueue.RetryTime);
            Channel.QueueDelete(retryQueueName, false, false);
        }
    }

    public void DeleteExchange(string eventName)
    {
        Channel.ExchangeDelete(eventName);
        Channel.ExchangeDelete(eventName.GetRetryExchangeName());
    }

    private async Task OnReceiveMessageAsync(string eventName, ServiceDto serviceData, BasicDeliverEventArgs ea)
    {
        var bodyArray = ea.Body.ToArray();
        var body = JsonSerializer.Deserialize<EventPublishDto>(Encoding.UTF8.GetString(bodyArray));
        var retryCount = GetRetryCount(ea.BasicProperties);
        var serviceName = serviceData.Name;

        var replacedUrl = serviceData.Url.ReplaceDynamicFieldValues(body?.Fields);
        var requestHeaders = new Dictionary<string, string>();
        foreach (var header in serviceData.Headers)
            requestHeaders.Add(header.Key, header.Value);

        if (!requestHeaders.ContainsKey(HeaderConstants.RequestIdHeader))
            requestHeaders.Add(HeaderConstants.RequestIdHeader, body?.CorrelationId);

        foreach (var header in requestHeaders)
            requestHeaders[header.Key] = header.Value.ReplaceDynamicFieldValues(body?.Fields);

        try
        {
            if (serviceData.CustomBodyMap != null && serviceData.CustomBodyMap.Any() && body is { Payload: JsonElement element })
            {
                body.Payload = _customObjectMapper.Map(element, serviceData.CustomBodyMap);
            }

            var requestData = serviceData with { Url = replacedUrl, Headers = requestHeaders };
            var processResult = await _httpProcessor.ProcessAsync(eventName, requestData, body);

            if (!processResult)
                _eventPublisher.PublishToRetry(eventName, serviceName, bodyArray, retryCount);

            Channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.AckFailed($"Message consume fail! - event name : {eventName} - service name : {serviceData.Name} - retry : {retryCount} ", e, retryCount, _maxRetryCount);

            _eventPublisher.PublishToRetry(eventName, serviceName, bodyArray, retryCount);

            Channel.BasicAck(ea.DeliveryTag, false);

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
                        Headers = requestHeaders
                    },
                    Response = new EventLogHttpResponseDto
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status500InternalServerError
                    },
                    Message = e.Message,
                    CorrelationId = body?.CorrelationId
                }
            };

            await _eventLog.LogAsync(_mapper.Map<Logs>(logData));
        }
    }

    private Task OnNewServiceCreated(object sender, BasicDeliverEventArgs ea)
    {
        var messageBody = ea.Body;
        var message = Encoding.UTF8.GetString(messageBody.ToArray());

        if (string.IsNullOrEmpty(message))
            return Task.CompletedTask;

        var serviceInfo = JsonSerializer.Deserialize<NewServiceDto>(message);

        if (serviceInfo == null)
            return Task.CompletedTask;

        var eventName = serviceInfo.EventName;
        var queueName = serviceInfo.ServiceName.GetQueueName(eventName);

        try
        {
            var service = _eventsRepository.GetServiceByEvent(eventName, serviceInfo.ServiceName);

            Bind(eventName, service);

            Channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.ConsumerFailed($"New service creation fail! - queue name {queueName} ", e);

            Channel.BasicNack(ea.DeliveryTag, false, false);
        }

        return Task.CompletedTask;
    }

    private void Bind(string eventName, Service service)
    {
        var serviceData = _mapper.Map<ServiceDto>(service);

        var channel = Channel;

        var queueName = service.Name.GetQueueName(eventName);
        var eventConsumer = new AsyncEventingBasicConsumer(channel);

        eventConsumer.Received += async (_, ea) =>
        {
            await OnReceiveMessageAsync(eventName, serviceData, ea);
        };

        channel.ExchangeDeclare(eventName, ExchangeType.Direct, true, false, null);
        channel.QueueDeclare(queueName, true, false, false, null);
        channel.QueueBind(queueName, eventName, eventName); // bind routing key events.
        channel.QueueBind(queueName, eventName, queueName); // bind routing key for retries.

        BindRetryQueues(eventName, service.Name);

        channel.BasicConsume(queueName, false, eventConsumer);
    }

    private void BindRetryQueues(string eventName, string serviceName)
    {
        var channel = Channel;

        var queueName = serviceName.GetQueueName(eventName);
        var retryExchangeName = eventName.GetRetryExchangeName();
        channel.ExchangeDeclare(retryExchangeName, ExchangeType.Direct, true, false, null);

        foreach (var retryQueue in _retryQueueHelper.RetryQueueInfo)
        {
            var retryQueueName = serviceName.GetRetryQueueName(eventName, retryQueue.RetryTime);
            var retryQueueArguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", eventName },
                { "x-dead-letter-routing-key", queueName },
                { "x-message-ttl", retryQueue.TTL }
            };

            channel.QueueDeclare(retryQueueName,
                true,
                false,
                false,
                retryQueueArguments);

            channel.QueueBind(retryQueueName, retryExchangeName, retryQueueName);
        }
    }

    private static long GetRetryCount(IBasicProperties properties)
    {
        if (properties?.Headers == null || !properties.Headers.ContainsKey(MessagingConstants.RetryHeaderName))
            return 0;

        return (long)properties.Headers[MessagingConstants.RetryHeaderName];
    }
}
