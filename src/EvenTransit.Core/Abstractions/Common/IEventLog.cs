using System.Threading.Tasks;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Abstractions.Common
{
    public interface IEventLog
    {
        Task LogAsync(EventLogDto details);
    }
}