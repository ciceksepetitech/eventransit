using System.Collections.Generic;

namespace EvenTransit.Messaging.Core.Dto
{
    public class BaseHttpRequestDto
    {
        public string Url { get; set; }
        public int Timeout { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
    
    public class HttpRequestDto : BaseHttpRequestDto
    {
        public byte[] Body { get; set; }
    }

    public class EventLogHttpRequestDto : BaseHttpRequestDto
    {
        public string Body { get; set; }
    }
}