using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto.Log;
using EvenTransit.UI.Filters;
using EvenTransit.UI.Models.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvenTransit.UI.Controllers
{
    [ValidateModel]
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
            var responseModel = new LogsViewModel
            {
                Events = _mapper.Map<List<SelectListItem>>(events)
            };

            return View(responseModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Search([FromBody] LogFilterModel model)
        {
            if (model.Page <= 0) model.Page = 1;

            var request = _mapper.Map<LogSearchRequestDto>(model);
            var result = await _logService.SearchAsync(request);
            var response = _mapper.Map<List<LogSearchResultViewModel>>(result.Items);

            var responseModel = new LogList
            {
                Items = response,
                TotalPages = result.TotalPages
            };

            return Json(new {IsSuccess = true, Data = responseModel});
        }

        [Route("Logs/GetServices/{eventName}")]
        public async Task<IActionResult> GetServices(string eventName)
        {
            var services = await _eventService.GetServicesAsync(eventName);
            return Json(services);
        }

        public async Task<IActionResult> GetById(string id)
        {
            var data = await _logService.GetByIdAsync(id);
            var result = _mapper.Map<LogItemViewModel>(data);

            return Json(result);
        }
    }
}