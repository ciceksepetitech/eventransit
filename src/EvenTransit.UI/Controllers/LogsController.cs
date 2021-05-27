using Microsoft.AspNetCore.Mvc;

namespace EvenTransit.UI.Controllers
{
    public class LogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}