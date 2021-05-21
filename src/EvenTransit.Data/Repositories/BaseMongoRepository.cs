using System;
using EvenTransit.Core.Constants;
using MongoDB.Driver;

namespace EvenTransit.Data.Repositories
{
    public class BaseMongoRepository<T>
    {
        protected readonly IMongoCollection<T> Collection;

        public BaseMongoRepository()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable(MongoConstants.Host));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable(MongoConstants.Database));
            Collection = database.GetCollection<T>(typeof(T).Name);
        }
    }
}