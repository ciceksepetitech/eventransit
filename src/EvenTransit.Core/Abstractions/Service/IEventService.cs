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
        Task SaveService(SaveServiceDto model);
        Task<ServiceDto> GetServiceDetails(string eventId, string serviceName);
    }
}