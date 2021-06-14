using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Core.Enums;
using EvenTransit.Data.Entities;

namespace EvenTransit.Data.Abstractions
{
    public interface ILogsRepository
    {
        Task InsertLogAsync(Logs model);
        Task<LogFilter> GetLogsAsync(Expression<Func<Logs, bool>> predicate, int page);
        Task<Logs> GetByIdAsync(string id);
        Task<long> GetLogsCount(DateTime startDate, DateTime endDate, LogType type);
    }
}