using System.Threading.Tasks;
using EventTransit.Core.Dto;

namespace EventTransit.Core.Abstractions.Data
{
    public interface ILogsMongoRepository
    {
        Task InsertLog(LogsDto model);
    }
}