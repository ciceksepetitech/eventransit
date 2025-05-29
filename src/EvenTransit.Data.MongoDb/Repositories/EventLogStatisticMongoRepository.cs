using EvenTransit.Data.MongoDb.Abstractions;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories;

public class EventLogStatisticMongoRepository : BaseMongoRepository<EventLogStatistic>, IEventLogStatisticRepository
{
    public EventLogStatisticMongoRepository(IOptions<MongoDbSettings> mongoDbSettings,
        IMongoClientProvider clientProvider) : base(mongoDbSettings, clientProvider)
    {
    }

    public async Task<List<EventLogStatistic>> ListAsync()
    {
        var data = await Collection.FindAsync(_ => true);
        return await data.ToListAsync();
    }

    public async Task<List<EventLogStatistic>> ListAsync(string eventName)
    {
        return await Collection.Find(x => x.EventName == eventName).ToListAsync();
    }

    public async Task<EventLogStatistic> GetAsync(string eventName, string serviceName, DateTime date)
    {
        return await Collection.Find(x => x.EventName == eventName && x.ServiceName == serviceName && x.CreatedOn == date).FirstOrDefaultAsync();
    }

    public async Task InsertAsync(EventLogStatistic data)
    {
        data.Id = Guid.NewGuid();
        await Collection.InsertOneAsync(data);
    }

    public async Task UpdateAsync(Guid id, long successCount, long failCount)
    {
        var update = Builders<EventLogStatistic>.Update
            .Inc(s => s.FailCount, failCount)
            .Inc(s => s.SuccessCount, successCount);

        await Collection.UpdateOneAsync(x => x.Id == id, update);
    }
}
