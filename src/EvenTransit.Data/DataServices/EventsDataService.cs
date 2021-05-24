using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Constants;

namespace EvenTransit.Data.DataServices
{
    public class EventsDataService : IEventsDataService
    {
        private readonly IEventsMongoRepository _eventsRepository;
        private readonly ICacheService _cacheService;

        public EventsDataService(IEventsMongoRepository eventsRepository, ICacheService cacheService)
        {
            _eventsRepository = eventsRepository;
            _cacheService = cacheService;
        }

        public async Task<List<string>> GetQueueNamesByEventAsync(string eventName)
        {
            var key = string.Format(CacheConstants.QueuesByEventKey, eventName);
            var queueNames =
                await _cacheService.GetAsync<List<string>>(key);

            if (queueNames == null)
            {
                var queues = await _eventsRepository.GetEvent(eventName);
                queueNames = queues.Services.Select(x => x.Name).ToList();

                await _cacheService.SetAsync(key, queueNames);
            }

            return queueNames;
        }
    }
}