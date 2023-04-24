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

    public async Task<bool> ProcessAsync(string eventName, ServiceDto service, EventPublishDto message)
    {
        var request = new HttpRequestDto
        {
            Url = service.Url,
            Timeout = service.Timeout,
            Body = message.Payload,
            Method = service.Method,
            Headers = service.Headers
        };

        var result = await _httpRequestSender.SendAsync(request);

        await LogResult(eventName, service.Name, result, request, message.CorrelationId);

        return result.IsSuccess;
    }

    private async Task LogResult(string eventName, string serviceName, HttpResponseDto result, HttpRequestDto request, string correlationId)
    {
        var logType = result.IsSuccess ? LogType.Success : LogType.Fail;

        switch (result.StatusCode)
        {
            case 400:
            case 404:
                result.IsSuccess = false;
                break;

        }

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
                    Body = body,
                    Headers = request.Headers
                },
                Response = new LogDetailResponse
                {
                    Response = result.Response,
                    IsSuccess = result.IsSuccess,
                    StatusCode = result.StatusCode
                },
                CorrelationId = correlationId
            }
        };

        await _eventLog.LogAsync(logData);
    }
}
