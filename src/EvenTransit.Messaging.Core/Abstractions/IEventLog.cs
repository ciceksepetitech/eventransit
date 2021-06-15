using System.Threading.Tasks;
using EvenTransit.Data.Entities;

namespace EvenTransit.Messaging.Core.Abstractions
{
    public interface IEventLog
    {
        Task LogAsync(Logs details);
    }
}