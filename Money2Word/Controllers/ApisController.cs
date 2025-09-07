using Microsoft.AspNetCore.Mvc;
using Money2Word.Models;
using Money2Word.Services.Interfaces;

namespace Money2Word.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ApiController(ILogger<ApiController> logger, IMoney2WordService service) : ControllerBase
    {
        private readonly ILogger<ApiController> _logger = logger;
        private readonly IMoney2WordService _service = service;

        [HttpPost("show")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        public IActionResult Show([FromBody] InputModel model)
        {
            _logger.LogInformation("Show endpoint called with amount: {Amount}", model.Amount);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Show endpoint");
                return BadRequest(new { message = "Enter required fields" });
            }

            var response = _service.Convert(model);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                _logger.LogError("Conversion failed: {ErrorMessage}", response.ErrorMessage);
                return BadRequest(new { error = response.ErrorMessage });
            }

            _logger.LogInformation("Conversion succeeded for amount: {Amount}", model.Amount);
            return Ok(response);
        }
    }
}
