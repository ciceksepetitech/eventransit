using System.Threading.Tasks;
using EvenTransit.Core.Enums;
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

            var logData = new EventLogDto
            {
                EventName = eventName,
                ServiceName = service.Name,
                LogType = result.IsSuccess ? LogType.Success : LogType.Fail,
                Details = new EventDetailDto
                {
                    Request = request,
                    Response = result
                }
            };

            await _eventLog.LogAsync(logData);

            return result.IsSuccess;
        }
    }
}