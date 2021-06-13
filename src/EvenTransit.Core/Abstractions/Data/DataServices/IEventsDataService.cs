using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Core.Dto.Service.Event;
using EvenTransit.Core.Entities;

namespace EvenTransit.Core.Abstractions.Data.DataServices
{
    public interface IEventsDataService
    {
        Task<List<string>> GetQueueNamesByEventAsync(string eventName);
        Task<List<Event>> GetEventsAsync();
        Task<Event> GetEventAsync(string id);
        Task AddServiceToEventAsync(string eventId, Entities.Service serviceData);
        Task UpdateServiceOnEventAsync(string eventId, Entities.Service serviceData);
        Task<Entities.Service> GetServiceDetailsAsync(string eventId, string serviceName);
        Task AddEvent(SaveEventDto data);
    }
}