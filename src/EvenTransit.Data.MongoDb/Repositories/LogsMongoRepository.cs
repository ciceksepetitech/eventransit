using EvenTransit.Data.MongoDb.Abstractions;
using System.Linq.Expressions;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace EvenTransit.Data.MongoDb.Repositories;

public class LogsMongoRepository : BaseMongoRepository<Logs>, ILogsRepository
{
    public LogsMongoRepository(IOptions<MongoDbSettings> mongoDbSettings,
        IMongoClientProvider clientProvider) : base(mongoDbSettings, clientProvider)
    {
    }

    public async Task InsertLogAsync(Logs model)
    {
        model.Id = Guid.NewGuid();
        model.CreatedOn = DateTime.UtcNow;

        await Collection.InsertOneAsync(model);
    }

    public async Task<LogFilter> GetLogsAsync(Expression<Func<Logs, bool>> predicate, string requestBodyRegex, int page)
    {
        const int perPage = 100;
        var definition = new FilterDefinitionBuilder<Logs>();
        var contains = FilterDefinition<Logs>.Empty;
        if (!string.IsNullOrWhiteSpace(requestBodyRegex))
        {
            var regex = new BsonRegularExpression(Regex.Escape(requestBodyRegex), "i");
            contains = Builders<Logs>.Filter.Regex(w => w.Details.Request.Body, regex);
        }

        var filter = definition.And(predicate, contains);

        var count = await Collection.Find(filter).CountDocumentsAsync();
        var totalPages = (int)Math.Ceiling((double)count / perPage);
        var result = await Collection.Find(filter)
            .Skip((page - 1) * perPage)
            .Limit(perPage)
            .ToListAsync();

        return new LogFilter
        {
            Items = result,
            TotalPages = totalPages
        };
    }

    public async Task<Logs> GetByIdAsync(Guid id)
    {
        return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
    
    public (long, long) GetLogsCountByEvent(string eventName, string serviceName)
    {
        var successLogCount = Collection.CountDocuments(x => x.EventName == eventName && x.ServiceName == serviceName && x.LogType == LogType.Success);
        var failLogCount = Collection.CountDocuments(x => x.EventName == eventName && x.ServiceName == serviceName && x.LogType == LogType.Fail);
        return (successLogCount, failLogCount);
    }
}
