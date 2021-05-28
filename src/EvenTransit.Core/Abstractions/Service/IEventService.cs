using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Core.Dto.Service;

namespace EvenTransit.Core.Abstractions.Service
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllAsync();
        Task<EventDto> GetEventDetailsAsync(string id);
        Task SaveService(SaveServiceDto model);
    }
}