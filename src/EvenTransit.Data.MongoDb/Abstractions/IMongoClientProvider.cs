using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Abstractions;

public interface IMongoClientProvider
{
    MongoClient Client { get; }
}
