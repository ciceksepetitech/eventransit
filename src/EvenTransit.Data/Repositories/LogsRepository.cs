using System;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;

namespace EvenTransit.Data.Repositories
{
    public class LogsRepository : BaseMongoRepository<Logs>, ILogsRepository
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