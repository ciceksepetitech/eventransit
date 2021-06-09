using System.Threading.Tasks;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Dto.Service.Log;

namespace EvenTransit.Core.Abstractions.Service
{
    public interface ILogService
    {
        Task<LogSearchResultDto> SearchAsync(LogSearchRequestDto request);
        Task<LogsDto> GetByIdAsync(string id);
    }
}