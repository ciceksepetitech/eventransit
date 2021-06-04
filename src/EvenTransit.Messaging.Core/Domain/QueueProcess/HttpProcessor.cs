using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.QueueProcess;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Enums;

namespace EvenTransit.Messaging.Core.Domain.QueueProcess
{
    public class HttpProcessor : IHttpProcessor
    {
        private readonly IHttpRequestSender _httpRequestSender;
        private readonly IEventLog _eventLog;

        public HttpProcessor(
            IHttpRequestSender httpRequestSender,
            IEventLog eventLog)
        {
            _httpRequestSender = httpRequestSender;
            _eventLog = eventLog;
        }

        public async Task ProcessAsync(string eventName, ServiceDto service, string message)
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
        }
    }
}