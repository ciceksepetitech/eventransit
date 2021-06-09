using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;

namespace EvenTransit.Core.Abstractions.Data
{
    public interface ILogsRepository
    {
        Task InsertLogAsync(LogsDto model);
        Task<LogFilterDto> GetLogsAsync(Expression<Func<Logs, bool>> predicate, int page);
        Task<LogsDto> GetByIdAsync(string id);
    }
}