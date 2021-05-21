using System.Threading.Tasks;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Dto;

namespace EventTransit.Core.Domain.Common
{
    public class EventLog : IEventLog
    {
        private readonly ILogsMongoRepository _logsRepository;

        public EventLog(ILogsMongoRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public async Task LogAsync(EventLogDto details)
        {
            var data = new LogsDto
            {
                EventName = details.EventName,
                ServiceName = details.ServiceName,
                LogType = details.LogType,
                Details = details.Details
            };

            await _logsRepository.InsertLog(data);
        }
    }
}