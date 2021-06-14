using System.Collections.Generic;

namespace EvenTransit.Messaging.Core.Dto
{
    public class HttpRequestDto
    {
        public string Url { get; set; }
        public int Timeout { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}