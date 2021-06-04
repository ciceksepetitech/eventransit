using System.Collections.Generic;

namespace EvenTransit.UI.Models.Events
{
    public class ServiceModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Timeout { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}