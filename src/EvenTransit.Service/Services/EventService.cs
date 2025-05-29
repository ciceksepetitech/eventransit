using AutoMapper;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Constants;
using EvenTransit.Domain.Entities;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto;
using EvenTransit.Service.Dto.Event;
using EvenTransit.Service.Extensions;
using Microsoft.Extensions.Logging;

namespace EvenTransit.Service.Services;

public class EventService : IEventService
{
    private readonly IEventsRepository _eventsRepository;
    private readonly IEventLogStatisticRepository _eventLogStatisticRepository;
    private readonly IMapper _mapper;
    private readonly IEventConsumer _eventConsumer;
    private readonly ILogger<EventService> _logger;

    public EventService(
        IEventsRepository eventsRepository,
        IEventLogStatisticRepository eventLogStatisticRepository,
        IMapper mapper,
        IEventConsumer eventConsumer,
        ILogger<EventService> logger)
    {
        _eventsRepository = eventsRepository;
        _eventLogStatisticRepository = eventLogStatisticRepository;
        _mapper = mapper;
        _eventConsumer = eventConsumer;
        _logger = logger;
    }

    public async Task<List<EventDto>> GetAllAsync()
    {
        var list = new List<EventDto>();
        var events = await _eventsRepository.GetEventsAsync();
        var eventLogStatistics = await _eventLogStatisticRepository.ListAsync();

        foreach (var @event in events)
        {
            var dto = new EventDto { Services = new List<ServiceDto>() };
            var eventStatistics = eventLogStatistics.Where(p => p.EventName == @event.Name).ToList();

            dto.Id = @event.Id.ToString();
            dto.ServiceCount = @event.ServiceCount;
            dto.FailCount = eventStatistics.Sum(s => s.FailCount);
            dto.SuccessCount = eventStatistics.Sum(s => s.SuccessCount);
            dto.Name = @event.Name;

            foreach (var service in @event.Services)
            {
                dto.Services.Add(new ServiceDto
                {
                    Url = service.Url,
                    Name = service.Name,
                    Method = service.Method,
                    Timeout = service.Timeout,
                    DelaySeconds = service.DelaySeconds
                });
            }

            list.Add(dto);
        }

        return list;
    }

    public async Task<EventDto> GetEventDetailsAsync(Guid id)
    {
        var eventDetails = await _eventsRepository.GetEventAsync(x => x.Id == id);
        var eventDetailDto = _mapper.Map<EventDto>(eventDetails);

        var eventLogStatistics = await _eventLogStatisticRepository.ListAsync(eventDetailDto.Name);

        foreach (var service in eventDetailDto.Services)
        {
            var eventStatistics = eventLogStatistics.Where(p => p.ServiceName == service.Name).ToList();

            service.SuccessCount = eventStatistics.Sum(s => s.SuccessCount);
            service.FailCount = eventStatistics.Sum(s => s.FailCount);
        }

        return eventDetailDto;
    }

    public async Task<BaseResponseDto> SaveServiceAsync(SaveServiceDto model)
    {
        var eventDetails = await _eventsRepository.GetEventAsync(x => x.Id == model.EventId);

        if (eventDetails == null)
            return new BaseResponseDto { IsSuccess = false, Message = MessageConstants.EventNotFound };

        if (!string.IsNullOrWhiteSpace(model.HiddenServiceName) && model.HiddenServiceName != model.ServiceName)
            return new BaseResponseDto { IsSuccess = false, Message = MessageConstants.CannotUpdateServiceName };

        var serviceData = _mapper.Map<Domain.Entities.Service>(model);
        var service = eventDetails.Services.FirstOrDefault(x => x.Name == model.ServiceName);

        if (service == null)
        {
            await _eventsRepository.AddServiceToEventAsync(model.EventId, serviceData);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(model.HiddenServiceName))
                return new BaseResponseDto { IsSuccess = false, Message = MessageConstants.ServiceNameAlreadyExist };

            await _eventsRepository.UpdateServiceOnEventAsync(model.EventId, serviceData);
        }

        return new BaseResponseDto { IsSuccess = true, Message = MessageConstants.ServiceRequestProceeded };
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
        var queueNames = queues.Services.Select(x => x.Name).OrderBy(x => x).ToList();

        return queueNames;
    }

    public async Task<bool> SaveEventAsync(SaveEventDto data)
    {
        var dataModel = _mapper.Map<Event>(data);
        var @event = await _eventsRepository.GetEventAsync(x => x.Name == dataModel.Name);

        if (@event != null)
            return false;

        await _eventsRepository.AddEvent(dataModel);
        return true;
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        try
        {
            var @event = await _eventsRepository.GetEventAsync(x => x.Id == id);

            if (@event == null)
                return false;

            _eventConsumer.DeleteExchange(@event.Name);

            foreach (var service in @event.Services)
                _eventConsumer.DeleteQueue(@event.Name, service.Name);

            await _eventsRepository.DeleteEventAsync(id);

            return true;
        }
        catch (Exception e)
        {
            _logger.EventApiOperationFailed(e, MessageConstants.EventNotDeleted);
            return false;
        }
    }

    public async Task<bool> DeleteServiceAsync(Guid id, string name)
    {
        var @event = await _eventsRepository.GetEventAsync(x => x.Id == id);
        var service = @event?.Services.FirstOrDefault(x => x.Name == name);
        if (service == null)
            return false;

        try
        {
            _eventConsumer.DeleteQueue(@event.Name, service.Name);

            await _eventsRepository.DeleteServiceAsync(id, name);

            return true;
        }
        catch (Exception e)
        {
            _logger.EventApiOperationFailed(e, MessageConstants.ServiceDeleteOperationFailed);
            return false;
        }
    }
}
