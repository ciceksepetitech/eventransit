using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Domain.Common
{
    public class EventLog : IEventLog
    {
        private readonly ILogsRepository _logsRepository;

        public EventLog(ILogsRepository logsRepository)
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