using System;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;

namespace EvenTransit.Data.Repositories
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