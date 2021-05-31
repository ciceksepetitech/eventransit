using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Constants;
using EvenTransit.Core.Entities;

namespace EvenTransit.Data.DataServices
{
    public class EventsDataService : IEventsDataService
    {
        private const int CacheExpireTime = 60;
        private readonly IEventsRepository _eventsRepository;
        private readonly ICacheService _cacheService;

        public EventsDataService(IEventsRepository eventsRepository, ICacheService cacheService)
        {
            _eventsRepository = eventsRepository;
            _cacheService = cacheService;
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

        public async Task AddServiceToEvent(string eventId, Service serviceData)
        {
            await _eventsRepository.AddServiceToEvent(eventId, serviceData);
        }
        
        public async Task UpdateServiceOnEvent(string eventId, Service serviceData)
        {
            await _eventsRepository.UpdateServiceOnEvent(eventId, serviceData);
        }

        public async Task<Service> GetServiceDetails(string eventId, string serviceName)
        {
            var eventDetails = await _eventsRepository.GetEventAsync(x => x._id == eventId);
            var serviceDetails = eventDetails?.Services?.FirstOrDefault(x => x.Name == serviceName);

            return serviceDetails;
        }
    }
}