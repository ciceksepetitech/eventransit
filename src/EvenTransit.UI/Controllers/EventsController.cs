using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Common;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Dto.Service.Event;
using EvenTransit.UI.Models.Events;
using Microsoft.AspNetCore.Mvc;
using EventDto = EvenTransit.Core.Dto.UI.EventDto;

namespace EvenTransit.UI.Controllers
{
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
            var data = _mapper.Map<List<EventDto>>(events);

            return View(data);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var eventDetails = await _eventService.GetEventDetailsAsync(id);

            if (eventDetails == null) return NotFound();

            var data = _mapper.Map<EventDto>(eventDetails);

            return View(data);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var result = await _eventService.DeleteEventAsync(id);
            return Json(new {success = result});
        }
        
        [Route("Events/DeleteService/{eventId}/{serviceName}")]
        public async Task<IActionResult> DeleteService(string eventId, string serviceName)
        {
            var result = await _eventService.DeleteServiceAsync(eventId, serviceName);
            return Json(new {success = result});
        }

        [Route("Events/GetServiceDetails/{eventId}/{serviceName}")]
        public async Task<IActionResult> GetServiceDetails(string eventId, string serviceName)
        {
            var serviceData = await _eventService.GetServiceDetailsAsync(eventId, serviceName);
            var data = _mapper.Map<ServiceModel>(serviceData);

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEvent([FromBody] SaveEventModel model)
        {
            var data = _mapper.Map<SaveEventDto>(model);
            var result = await _eventService.SaveEventAsync(data);

            return Json(new {success = result});
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
            
            return Json(new {success = true});
        }
    }
}