using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto.Service;

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
    }
}