using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;

namespace EvenTransit.Data.DataServices
{
    public class LogsDataService : ILogsDataService
    {
        private readonly ILogsRepository _logsRepository;

        public LogsDataService(ILogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public async Task<LogFilterDto> GetLogs(Expression<Func<Logs, bool>> expression, int page)
        {
            return await _logsRepository.GetLogs(expression, page);
        }

        public async Task<LogsDto> GetById(string id)
        {
            return await _logsRepository.GetById(id);
        }
    }
}