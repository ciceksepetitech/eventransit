using EvenTransit.Data.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EvenTransit.Data.Repositories
{
    public class BaseMongoRepository<T>
    {
        protected readonly IMongoCollection<T> Collection;

        public BaseMongoRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.Host);
            var database = client.GetDatabase(mongoDbSettings.Value.Database);
            Collection = database.GetCollection<T>(typeof(T).Name);
        }
    }
}