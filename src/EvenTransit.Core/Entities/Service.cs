using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace EvenTransit.Core.Entities
{
    public class Service
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public int Timeout { get; set; }
        
        [BsonElement]
        public Dictionary<string, string> Headers { get; set; }
    }
}