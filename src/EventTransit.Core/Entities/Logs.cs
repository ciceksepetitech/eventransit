using System;
using EventTransit.Core.Enums;

namespace EventTransit.Core.Entities
{
    public class Logs : BaseEntity
    {
        public string EventName { get; set; }
        public string ServiceName { get; set; }
        public LogType LogType { get; set; }
        public object Details { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}