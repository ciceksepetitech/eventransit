using System.Linq.Expressions;
using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions;

public interface ILogsRepository
{
    Task InsertLogAsync(Logs model);
    Task<LogFilter> GetLogsAsync(Expression<Func<Logs, bool>> predicate, string requestBodyRegex, int page);
    Task<Logs> GetByIdAsync(Guid id);
    (long, long) GetLogsCountByEvent(string eventName, DateTime startDate);
    (long, long) GetLogsCountByEvent(string eventName, string serviceName, DateTime startDate);
}
