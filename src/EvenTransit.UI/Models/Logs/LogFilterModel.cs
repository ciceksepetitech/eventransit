using System;
using EvenTransit.Core.Enums;

namespace EvenTransit.UI.Models.Logs
{
    public class LogFilterModel
    {
        public DateTime LogDateFrom { get; set; }
        public DateTime LogDateTo { get; set; }
        public LogType LogType { get; set; }
        public string EventName { get; set; }
        public string ServiceName { get; set; }
        public string Keyword { get; set; }
        public int Page { get; set; }
    }
}