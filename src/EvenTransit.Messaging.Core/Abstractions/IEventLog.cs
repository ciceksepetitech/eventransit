using System.Threading.Tasks;
using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Abstractions
{
    public interface IEventLog
    {
        Task LogAsync(EventLogDto details);
    }
}