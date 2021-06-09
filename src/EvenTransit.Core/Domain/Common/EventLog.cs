using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Domain.Common
{
    public class EventLog : IEventLog
    {
        private readonly ILogsRepository _logsRepository;
        private readonly IMapper _mapper;

        public EventLog(ILogsRepository logsRepository, IMapper mapper)
        {
            _logsRepository = logsRepository;
            _mapper = mapper;
        }

        public async Task LogAsync(EventLogDto details)
        {
            var data = _mapper.Map<LogsDto>(details);

            await _logsRepository.InsertLogAsync(data);
        }
    }
}