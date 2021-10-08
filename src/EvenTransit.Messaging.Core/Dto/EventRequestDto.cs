using System;
using System.Collections.Generic;

namespace EvenTransit.Messaging.Core.Dto
{
    public class EventRequestDto
    {
        public string EventName { get; set; }

        public dynamic Payload { get; set; }

        public Guid CorrelationId { get; set; }
        
        public Dictionary<string,string> Fields { get; set; }

        public EventRequestDto()
        {
            CorrelationId = Guid.NewGuid();
        }
    }
}