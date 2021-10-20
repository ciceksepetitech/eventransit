using EvenTransit.Data.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories
{
    public class BaseMongoRepository<T>
    {
        protected readonly IMongoCollection<T> Collection;

        public BaseMongoRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var collectionSettings = new MongoCollectionSettings
            {
                GuidRepresentation = GuidRepresentation.Standard
            };

            var mongoClientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(mongoDbSettings.Value.Host)
            };

            if (!string.IsNullOrEmpty(mongoDbSettings.Value.UserName) &&
                !string.IsNullOrEmpty(mongoDbSettings.Value.Password))
            {
                mongoClientSettings.Credential = MongoCredential.CreateCredential(mongoDbSettings.Value.Database, mongoDbSettings.Value.UserName, mongoDbSettings.Value.Password);
            }
            
            var client = new MongoClient(mongoClientSettings);
            var database = client.GetDatabase(mongoDbSettings.Value.Database);
            Collection = database.GetCollection<T>(typeof(T).Name, collectionSettings);
        }
    }
}