using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Constants;
using EvenTransit.Core.Dto.Service.Event;
using EvenTransit.Core.Entities;

namespace EvenTransit.Data.DataServices
{
    public class EventsDataService : IEventsDataService
    {
        private const int CacheExpireTime = 60;
        private readonly IEventsRepository _eventsRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public EventsDataService(IEventsRepository eventsRepository, ICacheService cacheService, IMapper mapper)
        {
            _eventsRepository = eventsRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<List<string>> GetQueueNamesByEventAsync(string eventName)
        {
            var key = string.Format(CacheConstants.QueuesByEventKey, eventName);
            var queueNames = await _cacheService.GetAsync(key, CacheExpireTime, async () =>
            {
                var queues = await _eventsRepository.GetEventAsync(x => x.Name == eventName);
                var data = queues.Services.Select(x => x.Name).ToList();

                return data;
            });

            return queueNames;
        }

        public async Task<List<Event>> GetEventsAsync()
        {
            var key = CacheConstants.EventsKey;
            var events = await _cacheService.GetAsync(key, CacheExpireTime,
                async () => await _eventsRepository.GetEventsAsync());

            return events;
        }

        public async Task<Event> GetEventAsync(string id)
        {
            return await _eventsRepository.GetEventAsync(x => x._id == id);
        }

        public async Task AddServiceToEventAsync(string eventId, Service serviceData)
        {
            await _eventsRepository.AddServiceToEventAsync(eventId, serviceData);
        }
        
        public async Task UpdateServiceOnEventAsync(string eventId, Service serviceData)
        {
            await _eventsRepository.UpdateServiceOnEventAsync(eventId, serviceData);
        }

        public async Task<Service> GetServiceDetailsAsync(string eventId, string serviceName)
        {
            var eventDetails = await _eventsRepository.GetEventAsync(x => x._id == eventId);
            var serviceDetails = eventDetails?.Services?.FirstOrDefault(x => x.Name == serviceName);

            return serviceDetails;
        }

        public async Task<bool> AddEventAsync(SaveEventDto data)
        {
            var dataModel = _mapper.Map<Event>(data);
            var @event = await _eventsRepository.GetEventAsync(x => x.Name == dataModel.Name);

            if (@event != null)
                return false;
            
            await _eventsRepository.AddEvent(dataModel);
            await _cacheService.DeleteAsync(CacheConstants.EventsKey);

            return true;
        }

        public async Task<bool> DeleteEventAsync(string id)
        {
            var @event = await _eventsRepository.GetEventAsync(x => x._id == id);

            if (@event == null)
                return false;

            await _eventsRepository.DeleteEventAsync(id);
            await _cacheService.DeleteAsync(CacheConstants.EventsKey);
            
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