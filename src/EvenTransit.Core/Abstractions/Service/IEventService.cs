using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Dto.Service.Event;

namespace EvenTransit.Core.Abstractions.Service
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