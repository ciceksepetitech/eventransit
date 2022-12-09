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

    public async  Task<LogStatistic> GetStatisticAsync(DateTime date)
    {
        return (await Collection.FindAsync(x => x.Date == date)).FirstOrDefault();
    }

    public async Task AddStatisticAsync(LogStatistic logStatistic)
    {
        await Collection.InsertOneAsync(logStatistic);
    }

    public async Task UpdateStatisticAsync(Guid id, long successCount, long failCount)
    {
        var update = Builders<LogStatistic>.Update
            .Inc(s => s.FailCount, failCount)
            .Inc(s => s.SuccessCount, successCount);

        await Collection.UpdateOneAsync(x => x.Id == id, update);
    }

    public async Task<List<LogStatistic>> GetStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        return await Collection.Find(x => x.Date >= startDate && x.Date <= endDate).ToListAsync();
    }
}
