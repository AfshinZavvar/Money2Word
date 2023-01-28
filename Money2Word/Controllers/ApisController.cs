using Microsoft.AspNetCore.Mvc;
using Money2Word.Models;
using Money2Word.Services.Interfaces;

namespace Money2Word.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMoney2WordService service;

        public ApiController(ILogger<HomeController> logger, IMoney2WordService service)
        {
            _logger = logger;
            this.service = service;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Route("/api/show")]
        [IgnoreAntiforgeryToken]
        public IActionResult Show(InputModel model)
        {
            _logger.LogInformation($"{nameof(Show)}| Running");

            if (!ModelState.IsValid)
            {
                return BadRequest("Enter required fields");
            }
            return Ok(service.Convert(model));
        }
    }
}
