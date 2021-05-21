using System.Collections.Generic;
using System.Threading.Tasks;
using EvenTransit.Core.Entities;

namespace EvenTransit.Core.Abstractions.Data
{
    public interface IEventsMongoRepository
    {
        Task<List<Events>> GetEvents();
        Task<Events> GetEvent(string eventName);
        Task<List<Entities.Service>> GetServicesByEvent(string eventName, string serviceName);
    }
}