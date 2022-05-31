using EvenTransit.Data.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories;

public class BaseMongoRepository<T>
{
    protected readonly IMongoCollection<T> Collection;
    private readonly MongoDbConnectionStringBuilder _mongoDbConnectionStringBuilder;

    public BaseMongoRepository(IOptions<MongoDbSettings> mongoDbSettings,
        MongoDbConnectionStringBuilder mongoDbConnectionStringBuilder)
    {
        var connectionString = mongoDbConnectionStringBuilder.ConnectionString;
        var collectionSettings = new MongoCollectionSettings { GuidRepresentation = GuidRepresentation.Standard };

        var pack = new ConventionPack();
        pack.Add(new IgnoreExtraElementsConvention(true));
        ConventionRegistry.Register("EvenTransit Conventions", pack, t => true);
        
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(mongoDbSettings.Value.Database);
        Collection = database.GetCollection<T>(typeof(T).Name, collectionSettings);
    }
}
