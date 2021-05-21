using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Entities;
using MongoDB.Driver;

namespace EvenTransit.Data.Repositories
{
    public class EventsMongoRepository : BaseMongoRepository<Events>, IEventsMongoRepository
    {
        public async Task<List<Events>> GetEvents()
        {
            var result = await Collection.FindAsync(_ => true);
            return await result.ToListAsync();
        }

        public async Task<Events> GetEvent(string eventName)
        {
            var result = await Collection.FindAsync(x => x.Name == eventName);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<List<Service>> GetServicesByEvent(string eventName, string serviceName)
        {
            var result = await Collection.FindAsync(x => x.Name == eventName);
            var @event = await result.FirstOrDefaultAsync();
            
            return @event.Services.Where(x => x.Name == serviceName).ToList();
        }
    }
}