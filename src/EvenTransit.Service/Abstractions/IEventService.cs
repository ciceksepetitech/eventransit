using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Service.Dto.Event;

namespace EvenTransit.Service.Abstractions
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllAsync();
        Task<EventDto> GetEventDetailsAsync(string id);
        Task SaveServiceAsync(SaveServiceDto model);
        Task<ServiceDto> GetServiceDetailsAsync(string eventId, string serviceName);
        Task<List<string>> GetServicesAsync(string eventName);
        Task<bool> SaveEventAsync(SaveEventDto data);
        Task<bool> DeleteEventAsync(string id);
        Task<bool> DeleteServiceAsync(string id, string name);
    }
}