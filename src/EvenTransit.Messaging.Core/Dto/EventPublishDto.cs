using System;
using System.Collections.Generic;

namespace EvenTransit.Messaging.Core.Dto
{
    public class EventPublishDto
    {
        public object Payload { get; set; }
        public Guid CorrelationId { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }
}