using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories;

public class LogStatisticsMongoRepository : BaseMongoRepository<LogStatistic>, ILogStatisticsRepository
{
    public LogStatisticsMongoRepository(IOptions<MongoDbSettings> mongoDbSettings,
        MongoDbConnectionStringBuilder connectionStringBuilder) : base(mongoDbSettings, connectionStringBuilder)
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
        var update = Builders<LogStatistic>.Update.Set("FailCount", failCount);
        update.Set("SuccessCount", successCount);
        Collection.UpdateOne(x => x.Id == id, update);
    }

    public async Task<List<LogStatistic>> GetStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        return await Collection.Find(x => x.Date >= startDate
                                          && x.Date <= endDate).ToListAsync();
    }
}
