using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Money2Word.Controllers;
using Money2Word.Models;
using Money2Word.Services.Interfaces;
using NSubstitute;

namespace Money2Word.Tests.Controllers;

public class ApiControllerTests
{
    private static readonly TelemetryClient _telemetryClient = new(TelemetryConfiguration.CreateDefault());
    private readonly IMoney2WordService _service = Substitute.For<IMoney2WordService>();
    private readonly ApiController _sut;

    public ApiControllerTests()
    {
        _sut = new ApiController(NullLogger<ApiController>.Instance, _service, _telemetryClient)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public void Show_ValidModel_Returns200WithWords()
    {
        var model = new InputModel { Amount = 1.00m };
        _service.Convert(1.00m).Returns(ConversionResult.Success("ONE DOLLAR"));

        var actionResult = _sut.Show(model);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(new { Words = "ONE DOLLAR" });
    }

    [Fact]
    public void Show_InvalidModelState_Returns400ValidationProblem()
    {
        _sut.ModelState.AddModelError("Amount", "Amount is required");
        var model = new InputModel { Amount = 0m };

        var actionResult = _sut.Show(model);

        // ValidationProblem() returns an ObjectResult carrying a ValidationProblemDetails payload
        actionResult.Should().BeAssignableTo<ObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>();
        _service.DidNotReceive().Convert(Arg.Any<decimal>());
    }

    [Fact]
    public void Show_ServiceReturnsFailure_Returns400ProblemDetails()
    {
        var model = new InputModel { Amount = 1_000_000_000_000_000m };
        _service.Convert(Arg.Any<decimal>()).Returns(
            ConversionResult.Failure("Amount exceeds the maximum supported value"));

        var actionResult = _sut.Show(model);

        var badRequestResult = actionResult.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
        var problem = badRequestResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problem.Title.Should().Be("Conversion failed");
        problem.Detail.Should().Contain("maximum supported value");
    }

    [Fact]
    public void Show_ServiceReturnsSuccess_DoesNotCallConvertMoreThanOnce()
    {
        var model = new InputModel { Amount = 42.00m };
        _service.Convert(42.00m).Returns(ConversionResult.Success("FORTY-TWO DOLLARS"));

        _sut.Show(model);

        _service.Received(1).Convert(42.00m);
    }
}
