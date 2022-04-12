using System.Threading.Tasks;
using EvenTransit.Domain.Entities;

namespace EvenTransit.Messaging.Core.Abstractions;

public interface IEventLog
{
    Task LogAsync(Logs details);
}
