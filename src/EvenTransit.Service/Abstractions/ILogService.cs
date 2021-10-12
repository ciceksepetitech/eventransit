using System;
using System.Threading.Tasks;
using EvenTransit.Service.Dto.Log;

namespace EvenTransit.Service.Abstractions
{
    public interface ILogService
    {
        Task<LogSearchResultDto> SearchAsync(LogSearchRequestDto request);
        Task<LogSearchResultDto> SearchAsync(string correlationId);
        Task<LogItemDto> GetByIdAsync(Guid id);
        Task<LogStatisticsDto> GetDashboardStatistics();
    }
}