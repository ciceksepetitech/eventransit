using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;

namespace EvenTransit.Core.Abstractions.Data
{
    public interface ILogsRepository
    {
        Task InsertLog(LogsDto model);
        Task<LogFilterDto> GetLogs(Expression<Func<Logs, bool>> predicate, int page);
        Task<LogsDto> GetById(string id);
    }
}