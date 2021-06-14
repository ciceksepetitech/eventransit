using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Enums;
using EvenTransit.Data.Abstractions;
using EvenTransit.Data.Entities;
using EvenTransit.Data.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EvenTransit.Data.Repositories
{
    public class LogsMongoRepository : BaseMongoRepository<Logs>, ILogsRepository
    {
        private readonly IMapper _mapper;

        public LogsMongoRepository(IOptions<MongoDbSettings> mongoDbSettings, IMapper mapper) : base(mongoDbSettings)
        {
            _mapper = mapper;
        }

        public async Task InsertLogAsync(Logs model)
        {
            model.CreatedOn = DateTime.UtcNow;

            await Collection.InsertOneAsync(model);
        }

        public async Task<LogFilter> GetLogsAsync(Expression<Func<Logs, bool>> predicate, int page)
        {
            const int perPage = 100;

            var count = await Collection.Find(predicate).CountDocumentsAsync();
            var totalPages = (int) Math.Ceiling((double) count / perPage);
            var result = await Collection.Find(predicate)
                .Sort(Builders<Logs>.Sort.Ascending(x => x.CreatedOn))
                .Skip((page - 1) * perPage)
                .Limit(perPage)
                .ToListAsync();

            return new LogFilter
            {
                Items = result,
                TotalPages = totalPages
            };
        }

        public async Task<Logs> GetByIdAsync(string id)
        {
            return await Collection.Find(x => x._id == id).FirstOrDefaultAsync();
        }

        public async Task<long> GetLogsCount(DateTime startDate, DateTime endDate, LogType type)
        {
            return await Collection.CountDocumentsAsync(x =>
                x.CreatedOn >= startDate && x.CreatedOn <= endDate && x.LogType == type);
        }
    }
}