using EvenTransit.Core.Enums;

namespace EvenTransit.Core.Dto
{
    public class LogsDto
    {
        public LogType LogType { get; set; }
        public string ServiceName { get; set; }
        public string EventName { get; set; }
        public LogDetailDto Details { get; set; }
    }
}