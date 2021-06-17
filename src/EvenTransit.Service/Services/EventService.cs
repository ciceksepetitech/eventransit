using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Data.Abstractions;
using EvenTransit.Data.Entities;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto.Event;
using ServiceDto = EvenTransit.Service.Dto.Event.ServiceDto;

namespace EvenTransit.Service.Services
{
    public class EventService : IEventService
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;

        public EventService(IEventsRepository eventsRepository, IEventPublisher eventPublisher, IMapper mapper)
        {
            _eventsRepository = eventsRepository;
            _eventPublisher = eventPublisher;
            _mapper = mapper;
        }
        
        public Task<bool> PublishAsync(EventRequestDto requestDto)
        {
            _eventPublisher.Publish(requestDto.EventName, requestDto.Payload);
            return Task.FromResult(true);
        }

        public async Task<List<EventDto>> GetAllAsync()
        {
            var events = await _eventsRepository.GetEventsAsync();

            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<EventDto> GetEventDetailsAsync(string id)
        {
            var eventDetails = await _eventsRepository.GetEventAsync(x => x._id == id);
            return _mapper.Map<EventDto>(eventDetails);
        }

        public async Task SaveServiceAsync(SaveServiceDto model)
        {
            var eventDetails = await _eventsRepository.GetEventAsync(x => x._id == model.EventId);

            if (eventDetails == null) return;

            var serviceData = _mapper.Map<Data.Entities.Service>(model);
            var service = eventDetails.Services.FirstOrDefault(x => x.Name == model.ServiceName);

            if (service == null)
            {
                await _eventsRepository.AddServiceToEventAsync(model.EventId, serviceData);
            }
            else
            {
                await _eventsRepository.UpdateServiceOnEventAsync(model.EventId, serviceData);
            }
        }

        public async Task<ServiceDto> GetServiceDetailsAsync(string eventId, string serviceName)
        {
            var eventDetails = await _eventsRepository.GetEventAsync(x => x._id == eventId);
            var serviceDetails = eventDetails?.Services?.FirstOrDefault(x => x.Name == serviceName);
            var data = _mapper.Map<ServiceDto>(serviceDetails);

            return data;
        }

        public async Task<List<string>> GetServicesAsync(string eventName)
        {
            var queues = await _eventsRepository.GetEventAsync(x => x.Name == eventName);
            var queueNames = queues.Services.Select(x => x.Name).ToList();

            return queueNames;
        }

        public async Task<bool> SaveEventAsync(SaveEventDto data)
        {
            var dataModel = _mapper.Map<Event>(data);
            var @event = await _eventsRepository.GetEventAsync(x => x.Name == dataModel.Name);

            if (@event != null)
                return false;

            await _eventsRepository.AddEvent(dataModel);

            return true;
        }

        public async Task<bool> DeleteEventAsync(string id)
        {
            var @event = await _eventsRepository.GetEventAsync(x => x._id == id);

            if (@event == null)
                return false;

            await _eventsRepository.DeleteEventAsync(id);

            return true;
        }

        public async Task<bool> DeleteServiceAsync(string id, string name)
        {
            var @event = await _eventsRepository.GetEventAsync(x => x._id == id);
            var service = @event?.Services.FirstOrDefault(x => x.Name == name);
            if (service == null)
                return false;

            await _eventsRepository.DeleteServiceAsync(id, name);
            return true;
        }
    }
}