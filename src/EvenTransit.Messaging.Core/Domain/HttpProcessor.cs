using System.Text.Json;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Domain;

public class HttpProcessor : IHttpProcessor
{
    private readonly IHttpRequestSender _httpRequestSender;
    private readonly IEventLog _eventLog;

    public HttpProcessor(IHttpRequestSender httpRequestSender, IEventLog eventLog)
    {
        _httpRequestSender = httpRequestSender;
        _eventLog = eventLog;
    }

    public async Task<bool> ProcessAsync(string eventName, ServiceDto service, EventPublishDto message, long retry)
    {
        var request = new HttpRequestDto
        {
            Url = service.Url,
            Timeout = service.Timeout,
            DelaySeconds = service.DelaySeconds,
            Body = message.Payload,
            Method = service.Method,
            Headers = service.Headers
        };

        var result = await _httpRequestSender.SendAsync(request);

        await LogResult(eventName, service.Name, result, request, message, retry);

        return result.IsSuccess;
    }

    private async Task LogResult(string eventName, string serviceName, HttpResponseDto result, HttpRequestDto request, EventPublishDto message, long retry)
    {
        var logSuccess = result.StatusCode is >= 200 and <= 299;
        var logType = logSuccess ? LogType.Success : LogType.Fail;

        var body = JsonSerializer.Serialize(request.Body);
        var logData = new Logs
        {
            EventName = eventName,
            ServiceName = serviceName,
            LogType = logType,
            Details = new LogDetail
            {
                Request = new LogDetailRequest
                {
                    Url = request.Url,
                    Timeout = request.Timeout,
                    DelaySeconds = request.DelaySeconds,
                    Body = body,
                    Headers = request.Headers,
                    Method = request.Method
                },
                Response = new LogDetailResponse
                {
                    Response = result.Response,
                    IsSuccess = logSuccess,
                    StatusCode = result.StatusCode
                },
                CorrelationId = message.CorrelationId,
                PublishDate = message.PublishDate,
                ConsumeDate = message.ConsumeDate,
                Retry = retry
            }
        };

        await _eventLog.LogAsync(logData);
    }
}
