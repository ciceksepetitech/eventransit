using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.QueueProcess;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Enums;

namespace EvenTransit.Messaging.Core.Domain.QueueProcess
{
    public class HttpProcessor : IHttpProcessor
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IHttpRequestSender _httpRequestSender;
        private readonly IEventLog _eventLog;

        public HttpProcessor(
            IHttpRequestSender httpRequestSender, 
            IEventsRepository eventsRepository, 
            IEventLog eventLog)
        {
            _httpRequestSender = httpRequestSender;
            _eventsRepository = eventsRepository;
            _eventLog = eventLog;
        }

        public async Task ProcessAsync(string eventName, string serviceName, string message)
        {
            var services = await _eventsRepository.GetServicesByEventAsync(eventName, serviceName);
            foreach (var service in services)
            {
                var request = new HttpRequestDto
                {
                    Url = service.Url,
                    Method = service.Method,
                    Timeout = service.Timeout,
                    Body = message,
                    Headers = service.Headers
                };

                var result = await _httpRequestSender.SendAsync(request);

                var logData = new EventLogDto
                {
                    EventName = eventName,
                    ServiceName = serviceName,
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
}