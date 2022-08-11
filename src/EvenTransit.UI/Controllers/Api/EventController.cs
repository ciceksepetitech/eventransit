using EvenTransit.Domain.Constants;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;
using EvenTransit.UI.Filters;
using EvenTransit.UI.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EvenTransit.UI.Controllers.Api;

[ApiController]
[ValidateModel]
[Route("api/v1/event")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EventController(IEventService eventService,
        IHttpContextAccessor httpContextAccessor)
    {
        _eventService = eventService;
        _httpContextAccessor = httpContextAccessor;
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
    public IActionResult PostAsync([FromBody] EventRequest request)
    {
        var requestId = StringValues.Empty;
        
        _httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(HeaderConstants.RequestIdHeader,
            out requestId);

        _eventService.Publish(new EventRequestDto
        {
            EventName = request.EventName,
            Payload = request.Payload,
            Fields = request.Fields,
            CorrelationId = requestId
        });

        return Ok();
    }
}
