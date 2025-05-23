using System.Linq.Expressions;
using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions;

public interface IEventsRepository
{
    Task<List<Event>> GetEventsAsync();
    Task<Event> GetEventAsync(Expression<Func<Event, bool>> predicate);
    Event GetEvent(Expression<Func<Event, bool>> predicate);
    Task<Service> GetServiceByEventAsync(string eventName, string serviceName);
    Task AddServiceToEventAsync(Guid eventId, Service serviceData);
    Task UpdateServiceOnEventAsync(Guid eventId, Service serviceData);
    Task AddEvent(Event dataModel);
    Task DeleteEventAsync(Guid id);
    Task DeleteServiceAsync(Guid eventId, string serviceName);
}
