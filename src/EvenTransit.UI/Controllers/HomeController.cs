using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using EvenTransit.Service.Abstractions;
using Microsoft.AspNetCore.Mvc;
using EvenTransit.UI.Models;
using EvenTransit.UI.Models.Home;

namespace EvenTransit.UI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly ILogService _logService;

        public HomeController(ILogService logService)
        {
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            var logInfo = await _logService.GetDashboardStatistics();
            var responseData = new DashboardViewModel
            {
                Dates = JsonSerializer.Serialize(logInfo.Dates),
                SuccessCount = JsonSerializer.Serialize(logInfo.SuccessCount),
                FailCount = JsonSerializer.Serialize(logInfo.FailCount)
            };
            
            return View(responseData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}