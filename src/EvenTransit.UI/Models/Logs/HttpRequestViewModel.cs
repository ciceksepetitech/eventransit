using System.Collections.Generic;

namespace EvenTransit.UI.Models.Logs
{
    public class HttpRequestViewModel
    {
        public string Url { get; set; }
        public int Timeout { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}