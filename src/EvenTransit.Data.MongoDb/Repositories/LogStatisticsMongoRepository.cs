using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories
{
    public class LogStatisticsMongoRepository : BaseMongoRepository<LogStatistic>, ILogStatisticsRepository
    {
        public LogStatisticsMongoRepository(IOptions<MongoDbSettings> mongoDbSettings) : base(mongoDbSettings)
        {
        }

        public LogStatistic GetStatistic(DateTime date)
        {
            return Collection.Find(x => x.Date == date).FirstOrDefault();
        }

        public void AddStatistic(LogStatistic logStatistic)
        {
            Collection.InsertOne(logStatistic);
        }

        public void UpdateStatistic(Guid id, long successCount, long failCount)
        {
            var data = new LogStatistic
            {
                FailCount = failCount,
                SuccessCount = successCount
            };
            Collection.ReplaceOne(x => x.Id == id, data);
        }

        public async Task<List<LogStatistic>> GetStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            return await Collection.Find(x => x.Date >= startDate
                                              && x.Date <= endDate).ToListAsync();
        }
    }
}