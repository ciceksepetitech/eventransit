using System;
using EvenTransit.Core.Enums;

namespace EvenTransit.Core.Entities
{
    public class Logs : BaseEntity
    {
        public string EventName { get; set; }
        public string ServiceName { get; set; }
        public LogType LogType { get; set; }
        public LogDetail Details { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}