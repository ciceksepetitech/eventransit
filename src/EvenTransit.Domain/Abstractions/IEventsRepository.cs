using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions
{
    public interface IEventsRepository
    {
        Task<List<Event>> GetEventsAsync();
        Task<Event> GetEventAsync(Expression<Func<Event, bool>> predicate);
        Service GetServiceByEvent(string eventName, string serviceName);
        Task<Service> GetServiceByEventAsync(string eventName, string serviceName);
        Task AddServiceToEventAsync(Guid eventId, Service serviceData);
        Task UpdateServiceOnEventAsync(Guid eventId, Service serviceData);
        Task AddEvent(Event dataModel);
        Task DeleteEventAsync(Guid id);
        Task DeleteServiceAsync(Guid eventId, string serviceName);
    }
}