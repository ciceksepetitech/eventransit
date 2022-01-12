using System;
using System.Threading.Tasks;
using EvenTransit.Domain.Constants;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;
using EvenTransit.UI.Filters;
using EvenTransit.UI.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EvenTransit.UI.Controllers.Api
{
    [ApiController]
    [ValidateModel]
    [Route("api/v1/event")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Publish an event to EvenTransit
        /// </summary>
        /// <param name="request">Event information</param>
        /// <returns></returns>
        /// <response code="200">Event published to message broker.</response>
        /// <response code="400">Validation problems</response>
        /// <response code="404">Not found</response>
        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> PostAsync([FromBody] EventRequest request)
        {
            var @event = await _eventService.GetEventAsync(request.EventName);
            if (@event == null)
            {
                return NotFound();
            }

            var requestId = HttpContext.Request.Headers[HeaderConstants.RequestIdHeader];
            var outboxEventIdHeader = HttpContext.Request.Headers[HeaderConstants.OutboxEventIdHeader];
            var correlationId = !StringValues.IsNullOrEmpty(requestId) ? requestId.ToString() : Guid.NewGuid().ToString();
            string outboxEventId = null;

            if (!StringValues.IsNullOrEmpty(outboxEventIdHeader))
                outboxEventId = outboxEventIdHeader.ToString();
            
            _eventService.Publish(new EventRequestDto
            {
                EventName = request.EventName,
                Payload = request.Payload,
                Fields = request.Fields,
                CorrelationId = correlationId,
                OutboxEventId = outboxEventId
            });
            
            return Ok();
        }
    }
}