using EvenTransit.Service.Dto.Event;
using System;
using System.Threading.Tasks;
using EvenTransit.Service.Dto.Log;

namespace EvenTransit.Service.Abstractions;

public interface ILogService
{
    Task<LogSearchResultDto> SearchAsync(LogSearchRequestDto request);
    Task<LogSearchResultDto> SearchAsync(string correlationId);
    Task<LogItemDto> GetByIdAsync(Guid id);
    Task<LogStatisticsDto> GetDashboardStatistics();
    Task<bool> ResendRequest(LogItemDto data, ServiceDto eventService);
}
