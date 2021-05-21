using System.Threading.Tasks;
using EventTransit.Core.Dto;

namespace EventTransit.Core.Abstractions.Common
{
    public interface IEventLog
    {
        Task LogAsync(EventLogDto details);
    }
}