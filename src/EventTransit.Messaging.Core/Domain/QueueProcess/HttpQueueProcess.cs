using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Abstractions.QueueProcess;
using EventTransit.Core.Dto;

namespace EventTransit.Messaging.Core.Domain.QueueProcess
{
    public class HttpQueueProcess : IQueueProcess
    {
        private readonly IEventsMongoRepository _eventsRepository;
        private readonly IHttpRequestSender _httpRequestSender;

        public HttpQueueProcess(IHttpRequestSender httpRequestSender, IEventsMongoRepository eventsRepository)
        {
            _httpRequestSender = httpRequestSender;
            _eventsRepository = eventsRepository;
        }

        public async Task Process(string eventName, string serviceName, string message)
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

                var result = _httpRequestSender.SendAsync(request).Result;
                    
                // TODO Log response
            }
        }
    }
}