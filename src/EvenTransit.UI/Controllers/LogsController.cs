using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto.Service.Log;
using EvenTransit.UI.Models.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvenTransit.UI.Controllers
{
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

        public async Task<IActionResult> Index([FromQuery] LogFilterModel model)
        {
            if (model.Page <= 0) model.Page = 1;
            
            var request = _mapper.Map<LogSearchRequestDto>(model);
            var result = await _logService.SearchAsync(request);
            var response = _mapper.Map<List<LogSearchResultViewModel>>(result.Items);
            var events = await _eventService.GetAllAsync();
            
            var responseModel = new LogsViewModel
            {
                LogList = new LogList
                {
                    Items = response,
                    TotalPages = result.TotalPages
                },
                Events = _mapper.Map<List<SelectListItem>>(events)
            };

            return View(responseModel);
        }

        [Route("Logs/GetServices/{eventName}")]
        public async Task<IActionResult> GetServices(string eventName)
        {
            var services = await _eventService.GetServices(eventName);
            return Json(services);
        }
    }
}