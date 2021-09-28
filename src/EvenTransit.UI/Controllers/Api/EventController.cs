using System.Threading.Tasks;
using EvenTransit.Domain.Constants;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;
using EvenTransit.UI.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace EvenTransit.UI.Controllers.Api
{
    [ApiController]
    [Route("api/v1/event")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">Event information</param>
        /// <returns></returns>
        /// <response code="200">Event published to message broker.</response>
        /// <response code="400">Validation problems</response>
        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> PostAsync([FromBody] EventRequest request)
        {
            var @event = await _eventService.GetEventAsync(request.EventName);
            if (@event == null)
            {
                return NotFound();
            }
            
            _eventService.Publish(new EventRequestDto
            {
                EventName = request.EventName,
                Payload = request.Payload
            });
            return Ok();
        }
    }
}