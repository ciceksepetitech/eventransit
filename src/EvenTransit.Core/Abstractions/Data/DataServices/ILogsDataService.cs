using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;

namespace EvenTransit.Core.Abstractions.Data.DataServices
{
    public interface ILogsDataService
    {
        Task<LogFilterDto> GetLogsAsync(Expression<Func<Logs, bool>> expression, int page);
        Task<LogsDto> GetByIdAsync(string id);
        Task<(long, long)> GetLogsCountByDay(DateTime day);
    }
}