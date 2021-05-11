using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EventTransit.Core.Entities
{
    public class Events
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Name { get; set; }
        public List<Service> Services { get; set; }
    }
}