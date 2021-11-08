using EvenTransit.Data.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories
{
    public class BaseMongoRepository<T>
    {
        protected readonly IMongoCollection<T> Collection;
        private readonly MongoDbConnectionStringBuilder _mongoDbConnectionStringBuilder;
        
        public BaseMongoRepository(IOptions<MongoDbSettings> mongoDbSettings, MongoDbConnectionStringBuilder mongoDbConnectionStringBuilder)
        {
            var connectionString = mongoDbConnectionStringBuilder.ConnectionString;
            var collectionSettings = new MongoCollectionSettings
            {
                GuidRepresentation = GuidRepresentation.Standard
            };

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(mongoDbSettings.Value.Database);
            Collection = database.GetCollection<T>(typeof(T).Name, collectionSettings);
        }
    }
}