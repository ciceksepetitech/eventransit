using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Constants;
using MongoDB.Driver;

namespace EventTransit.Data.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable(MongoConstants.Host));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable(MongoConstants.Database));
            _collection = database.GetCollection<T>(typeof(T).Name);
        }

        public async Task<List<T>> GetEvents()
        {
            var result = await _collection.FindAsync(_ => true);
            return await result.ToListAsync();
        }
    }
}