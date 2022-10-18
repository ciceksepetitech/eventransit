using EvenTransit.Data.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories;

public class BaseMongoRepository<T>
{
    protected readonly IMongoCollection<T> Collection;

    public BaseMongoRepository(IOptions<MongoDbSettings> mongoDbSettings,
        MongoDbConnectionStringBuilder mongoDbConnectionStringBuilder)
    {
        var connectionString = mongoDbConnectionStringBuilder.ConnectionString;

        var pack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("EvenTransit Conventions", pack, t => true);

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(mongoDbSettings.Value.Database);
        Collection = database.GetCollection<T>(typeof(T).Name, new MongoCollectionSettings { GuidRepresentation = GuidRepresentation.Standard });
    }
}
