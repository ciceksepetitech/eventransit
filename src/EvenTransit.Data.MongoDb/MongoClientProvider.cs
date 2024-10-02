using EvenTransit.Data.MongoDb.Abstractions;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb;

public class MongoClientProvider : IMongoClientProvider
{
    public MongoClient Client
    {
        get => _lazyClient.Value;
    }

    private readonly Lazy<MongoClient> _lazyClient;
    
    private readonly MongoDbConnectionStringBuilder _mongoDbConnectionStringBuilder;

    public MongoClientProvider(MongoDbConnectionStringBuilder mongoDbConnectionStringBuilder)
    {
        _mongoDbConnectionStringBuilder = mongoDbConnectionStringBuilder;
        
        _lazyClient = new Lazy<MongoClient>(CreateClient);
    }
    
    private MongoClient CreateClient()
    {
        var connectionString = _mongoDbConnectionStringBuilder.ConnectionString;

        var pack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("EvenTransit Conventions", pack, t => true);

        return new MongoClient(connectionString);
    }
}
