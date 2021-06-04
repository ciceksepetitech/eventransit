using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Dto.Service.Event;

namespace EvenTransit.Service.Services
{
    public class EventService : IEventService
    {
        private readonly IEventsDataService _eventDataService;
        private readonly IMapper _mapper;

        public EventService(IEventsDataService eventDataService, IMapper mapper)
        {
            _eventDataService = eventDataService;
            _mapper = mapper;
        }

        public async Task<List<EventDto>> GetAllAsync()
        {
            var events = await _eventDataService.GetEventsAsync();
            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<EventDto> GetEventDetailsAsync(string id)
        {
            var eventDetails = await _eventDataService.GetEventAsync(id);
            return _mapper.Map<EventDto>(eventDetails);
        }

        public async Task SaveService(SaveServiceDto model)
        {
            var eventDetails = await _eventDataService.GetEventAsync(model.EventId);

            if (eventDetails == null) return;

            var serviceData = _mapper.Map<Core.Entities.Service>(model);
            var service = eventDetails.Services.FirstOrDefault(x => x.Name == model.ServiceName);

            if (service == null)
            {
                await _eventDataService.AddServiceToEvent(model.EventId, serviceData);
            }
            else
            {
                await _eventDataService.UpdateServiceOnEvent(model.EventId, serviceData);
            }
        }

        public async Task<ServiceDto> GetServiceDetails(string eventId, string serviceName)
        {
            var serviceDetails = await _eventDataService.GetServiceDetails(eventId, serviceName);
            var data = _mapper.Map<ServiceDto>(serviceDetails);

            return data;
        }
    }
}