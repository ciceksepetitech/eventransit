using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Entities;
using MongoDB.Driver;

namespace EvenTransit.Data.Repositories
{
    public class EventsMongoRepository : BaseMongoRepository<Event>, IEventsRepository
    {
        public async Task<List<Event>> GetEventsAsync()
        {
            var result = await Collection.FindAsync(_ => true);
            return await result.ToListAsync();
        }

        public async Task<Event> GetEventAsync(Expression<Func<Event, bool>> predicate)
        {
            var result = await Collection.FindAsync(predicate);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<List<Service>> GetServicesByEventAsync(string eventName, string serviceName)
        {
            var result = await Collection.FindAsync(x => x.Name == eventName);
            var @event = await result.FirstOrDefaultAsync();
            
            return @event.Services.Where(x => x.Name == serviceName).ToList();
        }
    }
}