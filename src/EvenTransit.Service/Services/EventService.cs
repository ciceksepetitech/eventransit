using AutoMapper;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Constants;
using EvenTransit.Domain.Entities;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto;
using EvenTransit.Service.Dto.Event;
using EvenTransit.Service.Extensions;
using Microsoft.Extensions.Logging;
using ServiceDto = EvenTransit.Service.Dto.Event.ServiceDto;

namespace EvenTransit.Service.Services;

public class EventService : IEventService
{
    private readonly IEventsRepository _eventsRepository;
    private readonly IEventLogStatisticRepository _eventLogStatisticRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly IEventConsumer _eventConsumer;
    private readonly ILogger<EventService> _logger;

    public EventService(
        IEventsRepository eventsRepository,
        IEventLogStatisticRepository eventLogStatisticRepository,
        IEventPublisher eventPublisher,
        IMapper mapper,
        IEventConsumer eventConsumer,
        ILogger<EventService> logger)
    {
        _eventsRepository = eventsRepository;
        _eventLogStatisticRepository = eventLogStatisticRepository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _eventConsumer = eventConsumer;
        _logger = logger;
    }

    public void Publish(EventRequestDto requestDto)
    {
        if (string.IsNullOrEmpty(requestDto.CorrelationId))
        {
            requestDto.CorrelationId = Guid.NewGuid().ToString();
        }
        
        _eventPublisher.Publish(requestDto);
    }

    public async Task<List<EventDto>> GetAllAsync()
    {
        var events = await _eventLogStatisticRepository.GetAllAsync();

        return _mapper.Map<List<EventDto>>(events);
    }

    public async Task<EventDto> GetEventDetailsAsync(Guid id)
    {
        var eventDetails = await _eventsRepository.GetEventAsync(x => x.Id == id);
        return _mapper.Map<EventDto>(eventDetails);
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

            var eventLogData = await _eventLogStatisticRepository.GetAsync(model.EventId);
            eventLogData.ServiceCount++;

            await _eventLogStatisticRepository.UpdateAsync(model.EventId, eventLogData);
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

        var eventLogModel = _mapper.Map<EventLogStatistic>(data);
        eventLogModel.EventId = dataModel.Id;

        await _eventLogStatisticRepository.InsertAsync(eventLogModel);

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

            foreach (var service in @event.Services) _eventConsumer.DeleteQueue(@event.Name, service.Name);

            await _eventsRepository.DeleteEventAsync(id);
            await _eventLogStatisticRepository.DeleteAsync(id);

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

            var eventLogData = await _eventLogStatisticRepository.GetAsync(id);
            var newServiceCount = eventLogData.ServiceCount - 1;
            if (newServiceCount < 0) newServiceCount = 0;
            eventLogData.ServiceCount = newServiceCount;

            await _eventLogStatisticRepository.UpdateAsync(id, eventLogData);

            return true;
        }
        catch (Exception e)
        {
            _logger.EventApiOperationFailed(e, MessageConstants.ServiceDeleteOperationFailed);
            return false;
        }
    }

    public async Task<EventDto> GetEventAsync(string eventName)
    {
        var @event = await _eventsRepository.GetEventAsync(x => x.Name == eventName);
        var eventDto = _mapper.Map<EventDto>(@event);

        return eventDto;
    }
}
