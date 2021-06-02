using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto.Service;
using EvenTransit.UI.Models;
using Microsoft.AspNetCore.Mvc;
using EventDto = EvenTransit.Core.Dto.UI.EventDto;

namespace EvenTransit.UI.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public EventsController(IEventService eventService, IMapper mapper)
        {
            _eventService = eventService;
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

        public async Task<IActionResult> Delete()
        {
            return Json(new {success = true});
        }

        [Route("Events/GetServiceDetails/{eventId}/{serviceName}")]
        public async Task<IActionResult> GetServiceDetails(string eventId, string serviceName)
        {
            var serviceData = await _eventService.GetServiceDetails(eventId, serviceName);
            var data = _mapper.Map<ServiceModel>(serviceData);
            
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveService([FromBody]SaveServiceModel model)
        {
            var data = _mapper.Map<SaveServiceDto>(model);
            await _eventService.SaveService(data);
            
            return Json(new {success = true});
        }
    }
}