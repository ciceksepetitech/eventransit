using System.Collections.Generic;
using System.Threading.Tasks;
using EventTransit.Core.Entities;

namespace EventTransit.Core.Abstractions.Data
{
    public interface IEventsMongoRepository
    {
        Task<List<Events>> GetEvents();
        Task<Events> GetEvent(string eventName);
        Task<List<Entities.Service>> GetServicesByEvent(string eventName, string serviceName);
    }
}