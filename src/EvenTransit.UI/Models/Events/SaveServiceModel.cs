using System.Collections.Generic;

namespace EvenTransit.UI.Models.Events
{
    public class SaveServiceModel
    {
        public string EventId { get; set; }
        public string ServiceName { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public int Timeout { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}