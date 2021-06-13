using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;
using EvenTransit.Core.Enums;

namespace EvenTransit.Data.DataServices
{
    public class LogsDataService : ILogsDataService
    {
        private readonly ILogsRepository _logsRepository;

        public LogsDataService(ILogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public async Task<LogFilterDto> GetLogsAsync(Expression<Func<Logs, bool>> expression, int page)
        {
            return await _logsRepository.GetLogsAsync(expression, page);
        }

        public async Task<LogsDto> GetByIdAsync(string id)
        {
            return await _logsRepository.GetByIdAsync(id);
        }

        public async Task<(long, long)> GetLogsCountByDay(DateTime day)
        {
            var startDate = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
            var endDate = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);

            var successLogsCount = await _logsRepository.GetLogsCount(startDate, endDate, LogType.Success);
            var failLogsCount = await _logsRepository.GetLogsCount(startDate, endDate, LogType.Fail);

            return (successLogsCount, failLogsCount);
        }
    }
}