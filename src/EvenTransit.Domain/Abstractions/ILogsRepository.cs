using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;

namespace EvenTransit.Domain.Abstractions
{
    public interface ILogsRepository
    {
        Task InsertLogAsync(Logs model);
        Task<LogFilter> GetLogsAsync(Expression<Func<Logs, bool>> predicate, int page);
        Task<Logs> GetByIdAsync(Guid id);
        Task<long> GetLogsCount(DateTime startDate, DateTime endDate, LogType type);
    }
}