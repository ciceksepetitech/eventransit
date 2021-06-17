using System;
using EvenTransit.Domain.Enums;

namespace EvenTransit.Domain.Entities
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