using System.Threading.Tasks;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Abstractions.Data
{
    public interface ILogsRepository
    {
        Task InsertLog(LogsDto model);
    }
}