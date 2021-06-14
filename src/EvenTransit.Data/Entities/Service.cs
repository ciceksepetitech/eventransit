using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace EvenTransit.Data.Entities
{
    public class Service
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Timeout { get; set; }
        
        [BsonElement]
        public Dictionary<string, string> Headers { get; set; }
    }
}