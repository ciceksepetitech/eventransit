using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Core.Entities;

namespace EvenTransit.Core.Abstractions.Data.DataServices
{
    public interface IEventsDataService
    {
        Task<List<string>> GetQueueNamesByEventAsync(string eventName);
        Task<List<Event>> GetEventsAsync();
        Task<Event> GetEventAsync(string id);
        Task AddServiceToEvent(string eventId, Entities.Service serviceData);
        Task UpdateServiceOnEvent(string eventId, Entities.Service serviceData);
    }
}