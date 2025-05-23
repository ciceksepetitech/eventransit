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
    private readonly IEventPublisherService _eventPublisherService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogService _logService;

    public EventController(IEventPublisherService eventPublisherService,
        IHttpContextAccessor httpContextAccessor,
        ILogService logService)
    {
        _eventPublisherService = eventPublisherService;
        _httpContextAccessor = httpContextAccessor;
        _logService = logService;
    }

    /// <summary>
    /// Publish an event to EvenTransit
    /// </summary>
    /// <param name="request">Event information</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult PostAsync([FromBody] EventRequest request)
    {
        var requestId = StringValues.Empty;

        _httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(HeaderConstants.RequestIdHeader,
            out requestId);

        _eventPublisherService.Publish(new EventRequestDto
        {
            EventName = request.EventName,
            Payload = request.Payload,
            Fields = request.Fields,
            CorrelationId = requestId
        });

        return Ok();
    }

    /// <summary>
    /// Refresh the event log statistic
    /// </summary>
    /// <returns></returns>
    [HttpPost("statistics/refresh")]
    public async Task<IActionResult> RefreshEventLogStatistic()
    {
        await _logService.RefreshEventLogStatistic();
        return StatusCode(StatusCodes.Status204NoContent, new { });
    }
}
