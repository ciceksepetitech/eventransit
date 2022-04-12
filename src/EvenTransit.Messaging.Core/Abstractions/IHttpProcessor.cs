using System.Threading.Tasks;
using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Abstractions;

public interface IHttpProcessor
{
    Task<bool> ProcessAsync(string eventName, ServiceDto service, EventPublishDto message);
}
