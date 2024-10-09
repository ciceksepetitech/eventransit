using EvenTransit.Data.MongoDb.Abstractions;
using EvenTransit.Data.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories;

public class BaseMongoRepository<T>
{
    protected readonly IMongoCollection<T> Collection;

    public BaseMongoRepository(IOptions<MongoDbSettings> mongoDbSettings,
        IMongoClientProvider clientProvider)
    {
        var database = clientProvider.Client.GetDatabase(mongoDbSettings.Value.Database);
        Collection = database.GetCollection<T>(typeof(T).Name,
            new MongoCollectionSettings { GuidRepresentation = GuidRepresentation.Standard });
    }
}
