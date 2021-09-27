using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Dto;
using EvenTransit.Service.Dto.Event;
using ServiceDto = EvenTransit.Service.Dto.Event.ServiceDto;

namespace EvenTransit.Service.Abstractions
{
    public interface IEventService
    {
        Task<bool> PublishAsync(EventRequestDto requestDto);
        Task<List<EventDto>> GetAllAsync();
        Task<EventDto> GetEventDetailsAsync(Guid id);
        Task<BaseResponseDto> SaveServiceAsync(SaveServiceDto model);
        Task<ServiceDto> GetServiceDetailsAsync(Guid eventId, string serviceName);
        Task<List<string>> GetServicesAsync(string eventName);
        Task<bool> SaveEventAsync(SaveEventDto data);
        Task<bool> DeleteEventAsync(Guid id);
        Task<bool> DeleteServiceAsync(Guid id, string name);
    }
}