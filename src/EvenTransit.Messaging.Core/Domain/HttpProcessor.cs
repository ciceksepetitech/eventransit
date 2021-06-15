using System.Threading.Tasks;
using EvenTransit.Core.Enums;
using EvenTransit.Data.Entities;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Domain
{
    public class HttpProcessor : IHttpProcessor
    {
        private readonly IHttpRequestSender _httpRequestSender;
        private readonly IEventLog _eventLog;

        public HttpProcessor(IHttpRequestSender httpRequestSender, IEventLog eventLog)
        {
            _httpRequestSender = httpRequestSender;
            _eventLog = eventLog;
        }

        public async Task<bool> ProcessAsync(string eventName, ServiceDto service, string message)
        {
            var request = new HttpRequestDto
            {
                Url = service.Url,
                Timeout = service.Timeout,
                Body = message,
                Headers = service.Headers
            };

            var result = await _httpRequestSender.SendAsync(request);

            await LogResult(eventName, service, result, request);

            return result.IsSuccess;
        }

        private async Task LogResult(string eventName, ServiceDto service, HttpResponseDto result, HttpRequestDto request)
        {
            var logData = new Logs
            {
                EventName = eventName,
                ServiceName = service.Name,
                LogType = result.IsSuccess ? LogType.Success : LogType.Fail,
                Details = new LogDetail
                {
                    Request = new LogDetailRequest
                    {
                        Url = request.Url,
                        Timeout = request.Timeout,
                        Body = request.Body,
                        Headers = request.Headers
                    },
                    Response = new LogDetailResponse
                    {
                        Response = result.Response,
                        IsSuccess = result.IsSuccess,
                        StatusCode = result.StatusCode
                    }
                }
            };

            await _eventLog.LogAsync(logData);
        }
    }
}