using Microsoft.AspNetCore.Mvc;
using Money2Word.Models;
using System.Diagnostics;

namespace Money2Word.Controllers;

public class HomeController(ILogger<HomeController> logger, TimeProvider timeProvider) : Controller
{
    public IActionResult Index()
    {
        logger.LogInformation(
            "{Controller}|{Action}|Request received at {Time}",
            nameof(HomeController),
            nameof(Index),
            timeProvider.GetUtcNow()
        );
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        logger.LogError(
            "{Controller}|{Action}|Error encountered at {Time}",
            nameof(HomeController),
            nameof(Error),
            timeProvider.GetUtcNow()
        );

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
