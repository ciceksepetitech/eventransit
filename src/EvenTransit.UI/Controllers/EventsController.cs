using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Domain.Constants;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto.Event;
using EvenTransit.UI.Filters;
using EvenTransit.UI.Models.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvenTransit.UI.Controllers
{
    [ValidateModel]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;

        public EventsController(IEventService eventService, IEventPublisher eventPublisher, IMapper mapper)
        {
            _eventService = eventService;
            _eventPublisher = eventPublisher;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllAsync();
            var data = _mapper.Map<List<EventListViewModel>>(events);

            return View(data);
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var eventDetails = await _eventService.GetEventDetailsAsync(id);

            if (eventDetails == null) return NotFound();

            var data = _mapper.Map<EventViewModel>(eventDetails);

            return View(data);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isSuccess = await _eventService.DeleteEventAsync(id);
            var statusCode = isSuccess ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;

            return StatusCode(statusCode);
        }
        
        [HttpGet]
        [Route("Events/DeleteService/{eventId}/{serviceName}")]
        public async Task<IActionResult> DeleteService(Guid eventId, string serviceName)
        {
            var isSuccess = await _eventService.DeleteServiceAsync(eventId, serviceName);
            var message = isSuccess ? MessageConstants.ServiceDeleted : MessageConstants.ServiceDeleteOperationFailed;
            var response = new {isSuccess = isSuccess, Message =  message};
            var statusCode = isSuccess ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;

            return StatusCode(statusCode, response);
        }

        [HttpGet]
        [Route("Events/GetServiceDetails/{eventId}/{serviceName}")]
        public async Task<IActionResult> GetServiceDetails(Guid eventId, string serviceName)
        {
            var serviceData = await _eventService.GetServiceDetailsAsync(eventId, serviceName);
            var data = _mapper.Map<ServiceModel>(serviceData);

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEvent([FromBody] SaveEventModel model)
        {
            //TODO: Add Event - Service Name regex 
            var data = _mapper.Map<SaveEventDto>(model);
            var result = await _eventService.SaveEventAsync(data);
            
            if (!result)
            {
                return Json(new {IsSuccess = false, Errors = new[]{"This service already created before"}});
            }

            return Json(new {IsSuccess = true});
        }

        [HttpPost]
        public async Task<IActionResult> SaveService([FromBody] SaveServiceModel model)
        {
            var data = _mapper.Map<SaveServiceDto>(model);
            await _eventService.SaveServiceAsync(data);

            var eventDetails = await _eventService.GetEventDetailsAsync(model.EventId);
            _eventPublisher.RegisterNewService(new NewServiceDto
            {
                EventName = eventDetails.Name,
                ServiceName = model.ServiceName
            });
            
            return Json(new {IsSuccess = true});
        }
    }
}