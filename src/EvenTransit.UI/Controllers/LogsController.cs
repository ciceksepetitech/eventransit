using AutoMapper;
using EvenTransit.Domain.Extensions;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto.Log;
using EvenTransit.UI.Filters;
using EvenTransit.UI.Models.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace EvenTransit.UI.Controllers;

[ValidateModel]
[ApiExplorerSettings(IgnoreApi = true)]
public class LogsController : Controller
{
    private readonly ILogService _logService;
    private readonly IMapper _mapper;
    private readonly IEventService _eventService;

    public LogsController(ILogService logService, IMapper mapper, IEventService eventService)
    {
        _logService = logService;
        _mapper = mapper;
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _eventService.GetAllAsync();
        var responseModel = new LogsViewModel { Events = _mapper.Map<List<SelectListItem>>(events.OrderBy(w => w.Name)) };

        return View(responseModel);
    }

    [HttpGet]
    public async Task<IActionResult> Search(LogFilterModel model)
    {
        if (model.Page <= 0) model.Page = 1;

        var request = new LogSearchRequestDto
        {
            EventName = model.EventName,
            ServiceName = model.ServiceName,
            LogType = model.LogType,
            Page = model.Page,
            LogDateFrom = model.LogDateFrom.ConvertToDate(),
            LogDateTo = model.LogDateTo.ConvertToDate(),
            RequestBodyRegex = model.Query
        };

        var result = await _logService.SearchAsync(request);
        if (!result.Items.Any())
            return Ok(new
            {
                IsSuccess = false,
                Message = "Record not found"
            });
        var response = _mapper.Map<List<LogSearchResultViewModel>>(result.Items);

        var responseModel = new LogList
        {
            Items = response,
            TotalPages = result.TotalPages
        };

        return StatusCode(StatusCodes.Status200OK, new
        {
            IsSuccess = true,
            Data = responseModel
        });
    }

    [HttpGet]
    [Route("Logs/SearchByCorrelationId/{correlationId}")]
    public async Task<IActionResult> SearchByCorrelationId(string correlationId)
    {
        var result = await _logService.SearchAsync(correlationId);
        var response = _mapper.Map<List<LogSearchResultViewModel>>(result.Items);
        var responseModel = new LogList
        {
            Items = response,
            TotalPages = result.TotalPages
        };

        return StatusCode(StatusCodes.Status200OK, new
        {
            IsSuccess = true,
            Data = responseModel
        });
    }

    [HttpGet]
    [Route("Logs/GetServices/{eventName}")]
    public async Task<IActionResult> GetServices(string eventName)
    {
        var services = await _eventService.GetServicesAsync(eventName);
        return StatusCode(StatusCodes.Status200OK, services);
    }

    public async Task<IActionResult> GetById(Guid id)
    {
        var data = await _logService.GetByIdAsync(id);
        var result = _mapper.Map<LogItemViewModel>(data);

        var statusCode = data == null ? StatusCodes.Status404NotFound : StatusCodes.Status200OK;
        return StatusCode(statusCode, result);
    }
}
