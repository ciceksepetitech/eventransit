using System.Text.Json;
using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Abstractions.QueueProcess;
using EventTransit.Core.Dto;
using EventTransit.Core.Enums;
using Microsoft.Extensions.Logging;

namespace EventTransit.Messaging.Core.Domain.QueueProcess
{
    public class HttpProcessor : IHttpProcessor
    {
        private readonly IEventsMongoRepository _eventsRepository;
        private readonly IHttpRequestSender _httpRequestSender;
        private readonly IEventLog _eventLog;

        public HttpProcessor(
            IHttpRequestSender httpRequestSender, 
            IEventsMongoRepository eventsRepository, 
            IEventLog eventLog)
        {
            _httpRequestSender = httpRequestSender;
            _eventsRepository = eventsRepository;
            _eventLog = eventLog;
        }

        public async Task ProcessAsync(string eventName, string serviceName, string message)
        {
            var services = await _eventsRepository.GetServicesByEvent(eventName, serviceName);
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
                        Response = result.Response
                    }
                };

                await _eventLog.Log(logData);
            }
        }
    }
}