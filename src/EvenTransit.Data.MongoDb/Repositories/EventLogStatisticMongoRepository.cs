﻿using EvenTransit.Data.MongoDb.Abstractions;
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

    public async Task<List<EventLogStatistic>> GetAllAsync()
    {
        var data = await Collection.FindAsync(_ => true);
        return await data.ToListAsync();
    }

    public List<EventLogStatistic> GetAll()
    {
        return Collection.Find(_ => true).ToList();
    }

    public async Task<EventLogStatistic> GetAsync(Guid eventId)
    {
        return await Collection.Find(x => x.EventId == eventId).FirstOrDefaultAsync();
    }

    public async Task<EventLogStatistic> GetAsync(string name)
    {
        return await Collection.Find(x => x.EventName == name).FirstOrDefaultAsync();
    }

    public async Task InsertAsync(EventLogStatistic data)
    {
        data.Id = Guid.NewGuid();

        await Collection.InsertOneAsync(data);
    }

    public void Update(Guid id, EventLogStatistic data)
    {
        Collection.ReplaceOne(x => x.EventId == id, data);
    }

    public async Task UpdateAsync(Guid id, EventLogStatistic data)
    {
        await Collection.ReplaceOneAsync(x => x.EventId == id, data);
    }

    public async Task DeleteAsync(Guid id)
    {
        await Collection.DeleteOneAsync(x => x.EventId == id);
    }

    public async Task UpdateStatisticAsync(Guid id, long successCount, long failCount)
    {
        var update = Builders<EventLogStatistic>.Update
            .Inc(s => s.FailCount, failCount)
            .Inc(s => s.SuccessCount, successCount);

        await Collection.UpdateOneAsync(x => x.Id == id, update);
    }
}
