using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Data.Abstractions;
using EvenTransit.Data.Entities;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Domain
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
            var data = _mapper.Map<Logs>(details);
            await _logsRepository.InsertLogAsync(data);
        }
    }
}