using EventTransit.Core.Enums;

namespace EventTransit.Core.Dto
{
    public class EventLogDto
    {
        public string EventName { get; set; }
        public string ServiceName { get; set; }
        public LogType LogType { get; set; }
        public EventDetailDto Details { get; set; }
    }

    public class EventDetailDto
    {
        public HttpRequestDto Request { get; set; }
        public object Response { get; set; }
        public string Message { get; set; }
    }
}