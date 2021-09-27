using EvenTransit.Domain.Enums;

namespace EvenTransit.UI.Models.Logs
{
    public class LogFilterModel
    {
        public string LogDateFrom { get; set; }
        public string LogDateTo { get; set; }
        public LogType LogType { get; set; }
        public string EventName { get; set; }
        public string ServiceName { get; set; }
        public int Page { get; set; }
    }
}