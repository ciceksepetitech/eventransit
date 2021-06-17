using System.Threading.Tasks;
using EvenTransit.Api.Models;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EvenTransit.Api.Controllers
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
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> PostAsync([FromBody] EventRequest request)
        {
            var response = await _eventService.PublishAsync(new EventRequestDto
            {
                EventName = request.EventName,
                Payload = request.Payload
            });
            return response ? Ok() : BadRequest();
        }
    }
}