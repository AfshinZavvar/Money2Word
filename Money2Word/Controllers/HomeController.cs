using Microsoft.AspNetCore.Mvc;
using Money2Word.Models;
using System.Diagnostics;

namespace Money2Word.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index()
        {
            _logger.LogInformation(
                "{Controller}|{Action}|Request received at {Time}",
                nameof(HomeController),
                nameof(Index),
                DateTime.UtcNow
            );
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError(
                "{Controller}|{Action}|Error encountered at {Time}",
                nameof(HomeController),
                nameof(Error),
                DateTime.UtcNow
            );

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}