using System;
using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Dto;
using EventTransit.Core.Entities;

namespace EventTransit.Data.Repositories
{
    public class LogsMongoRepository : BaseMongoRepository<Logs>, ILogsMongoRepository
    {
        public async Task InsertLog(LogsDto model)
        {
            var data = new Logs
            {
                EventName = model.EventName,
                ServiceName = model.ServiceName,
                LogType = model.LogType,
                Details = model.Details,
                CreatedOn = DateTime.UtcNow
            };

            await Collection.InsertOneAsync(data);
        }
    }
}