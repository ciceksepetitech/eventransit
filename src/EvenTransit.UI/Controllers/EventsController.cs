using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto.UI;
using Microsoft.AspNetCore.Mvc;

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
    }
}