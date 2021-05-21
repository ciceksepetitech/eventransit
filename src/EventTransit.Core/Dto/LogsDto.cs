using EventTransit.Core.Enums;

namespace EventTransit.Core.Dto
{
    public class LogsDto
    {
        public LogType LogType { get; set; }
        public string ServiceName { get; set; }
        public string EventName { get; set; }
        public object Details { get; set; }
    }
}