using System.Linq.Expressions;
using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions;

public interface ILogsRepository
{
    Task InsertLogAsync(Logs model);
    Task<LogFilter> GetLogsAsync(Expression<Func<Logs, bool>> predicate, int page);
    Task<LogFilter> GetLogsAsync(Expression<Func<Logs, bool>> predicate, string requestBodyRegex, int page);
    Task<Logs> GetByIdAsync(Guid id);
}
