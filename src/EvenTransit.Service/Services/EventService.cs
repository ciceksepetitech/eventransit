using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
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
        private readonly IEventLogStatisticRepository _eventLogStatisticRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;

        public EventService(
            IEventsRepository eventsRepository,
            IEventLogStatisticRepository eventLogStatisticRepository,
            IEventPublisher eventPublisher, 
            IMapper mapper)
        {
            _eventsRepository = eventsRepository;
            _eventLogStatisticRepository = eventLogStatisticRepository;
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
            var events = await _eventLogStatisticRepository.GetAllAsync();

            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<EventDto> GetEventDetailsAsync(Guid id)
        {
            var eventDetails = await _eventsRepository.GetEventAsync(x => x.Id == id);
            return _mapper.Map<EventDto>(eventDetails);
        }

        public async Task SaveServiceAsync(SaveServiceDto model)
        {
            var eventDetails = await _eventsRepository.GetEventAsync(x => x.Id == model.EventId);

            if (eventDetails == null) return;

            var serviceData = _mapper.Map<Domain.Entities.Service>(model);
            var service = eventDetails.Services.FirstOrDefault(x => x.Name == model.ServiceName);

            if (service == null)
            {
                await _eventsRepository.AddServiceToEventAsync(model.EventId, serviceData);

                var eventLogData = await _eventLogStatisticRepository.GetAsync(model.EventId);
                eventLogData.ServiceCount++;

                await _eventLogStatisticRepository.UpdateAsync(model.EventId, eventLogData);
            }
            else
            {
                await _eventsRepository.UpdateServiceOnEventAsync(model.EventId, serviceData);
            }
        }

        public async Task<ServiceDto> GetServiceDetailsAsync(Guid eventId, string serviceName)
        {
            var eventDetails = await _eventsRepository.GetEventAsync(x => x.Id == eventId);
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
            
            var eventLogModel = _mapper.Map<EventLogStatistic>(data);
            eventLogModel.EventId = dataModel.Id;
            
            await _eventLogStatisticRepository.InsertAsync(eventLogModel);

            return true;
        }

        public async Task<bool> DeleteEventAsync(Guid id)
        {
            var @event = await _eventsRepository.GetEventAsync(x => x.Id == id);

            if (@event == null)
                return false;

            await _eventsRepository.DeleteEventAsync(id);
            await _eventLogStatisticRepository.DeleteAsync(id);

            return true;
        }

        public async Task<bool> DeleteServiceAsync(Guid id, string name)
        {
            var @event = await _eventsRepository.GetEventAsync(x => x.Id == id);
            var service = @event?.Services.FirstOrDefault(x => x.Name == name);
            if (service == null)
                return false;

            await _eventsRepository.DeleteServiceAsync(id, name);

            var eventLogData = await _eventLogStatisticRepository.GetAsync(id);
            var newServiceCount = eventLogData.ServiceCount - 1;
            if (newServiceCount < 0) newServiceCount = 0;
            eventLogData.ServiceCount = newServiceCount;

            await _eventLogStatisticRepository.UpdateAsync(id, eventLogData);
            
            return true;
        }
    }
}