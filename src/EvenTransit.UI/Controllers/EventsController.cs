using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Service;
using Microsoft.AspNetCore.Mvc;

namespace EvenTransit.UI.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllAsync();
            
            // TODO Map to dto
            
            return View(events);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var eventDetails = await _eventService.GetEventDetailsAsync(id);

            if (eventDetails == null) return NotFound();
            
            // TODO Map to dto
            
            return View(eventDetails);
        }
    }
}