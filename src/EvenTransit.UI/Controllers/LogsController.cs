using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto.Service.Log;
using EvenTransit.UI.Models.Logs;
using Microsoft.AspNetCore.Mvc;

namespace EvenTransit.UI.Controllers
{
    public class LogsController : Controller
    {
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public LogsController(ILogService logService, IMapper mapper)
        {
            _logService = logService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index([FromQuery] LogFilterModel model)
        {
            if (model.Page <= 0) model.Page = 1;
            
            var request = _mapper.Map<LogSearchRequestDto>(model);
            var result = await _logService.SearchAsync(request);
            var response = _mapper.Map<List<LogSearchResultViewModel>>(result.Items);
            var responseModel = new LogsViewModel
            {
                LogList = new LogList
                {
                    Items = response,
                    TotalPages = result.TotalPages
                }
            };

            return View(responseModel);
        }
    }
}