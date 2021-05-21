using System.Threading.Tasks;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Abstractions.Data
{
    public interface ILogsMongoRepository
    {
        Task InsertLog(LogsDto model);
    }
}