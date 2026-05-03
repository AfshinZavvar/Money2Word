using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Money2Word.Models;
using Money2Word.Services.Interfaces;
using System.Diagnostics;

namespace Money2Word.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ApiController(ILogger<ApiController> logger, IMoney2WordService service, TelemetryClient telemetryClient) : ControllerBase
{
    /// <summary>Converts a monetary amount to its English word representation.</summary>
    /// <param name="model">The input containing the amount to convert.</param>
    /// <returns>200 with <c>{ Words }</c>, or 400 with RFC 7807 problem details.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/show
    ///     { "Amount": 1234.56 }
    ///
    /// Sample response:
    ///
    ///     { "Words": "ONE THOUSAND TWO HUNDRED AND THIRTY-FOUR DOLLARS AND FIFTY-SIX CENTS" }
    /// </remarks>
    [HttpPost("show")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Show([FromBody] InputModel model)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for Show endpoint");
            return ValidationProblem(ModelState);
        }

        logger.LogInformation("Show called with Amount: {Amount}", model.Amount);

        var stopwatch = Stopwatch.StartNew();
        var result = service.Convert(model.Amount);
        stopwatch.Stop();
        telemetryClient.GetMetric("ConversionDurationMs").TrackValue(stopwatch.Elapsed.TotalMilliseconds);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Conversion failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ProblemDetails
            {
                Title = "Conversion failed",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            });
        }

        logger.LogInformation("Conversion succeeded for Amount: {Amount}", model.Amount);
        return Ok(new { Words = result.Words });
    }
}