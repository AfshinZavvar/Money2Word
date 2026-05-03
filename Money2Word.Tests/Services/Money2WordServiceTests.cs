using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging.Abstractions;
using Money2Word.Services;

namespace Money2Word.Tests.Services;

public class Money2WordServiceTests
{
    private static readonly TelemetryClient _telemetryClient = new(TelemetryConfiguration.CreateDefault());
    private readonly Money2WordService _sut = new(NullLogger<Money2WordService>.Instance, _telemetryClient);

    [Theory]
    [InlineData(1.00, "ONE DOLLAR")]
    [InlineData(2.00, "TWO DOLLARS")]
    [InlineData(0.01, "ZERO DOLLARS AND ONE CENT")]
    [InlineData(0.10, "ZERO DOLLARS AND TEN CENTS")]
    [InlineData(1.01, "ONE DOLLAR AND ONE CENT")]
    [InlineData(2.50, "TWO DOLLARS AND FIFTY CENTS")]
    [InlineData(1000.00, "ONE THOUSAND DOLLARS")]
    [InlineData(1_000_000.00, "ONE MILLION DOLLARS")]
    [InlineData(1_000_000_000.00, "ONE BILLION DOLLARS")]
    [InlineData(1_000_000_000_000.00, "ONE TRILLION DOLLARS")]
    [InlineData(21.00, "TWENTY-ONE DOLLARS")]
    [InlineData(100.00, "ONE HUNDRED DOLLARS")]
    [InlineData(110.00, "ONE HUNDRED AND TEN DOLLARS")]
    [InlineData(999.99, "NINE HUNDRED AND NINETY-NINE DOLLARS AND NINETY-NINE CENTS")]
    // Teen numbers (separate TryGetValue branch)
    [InlineData(11.00, "ELEVEN DOLLARS")]
    [InlineData(19.00, "NINETEEN DOLLARS")]
    // Zero dollars (cents-only path)
    [InlineData(0.00, "ZERO DOLLARS")]
    // Multi-scale with a zero middle chunk — tests the chunk > 0 guard in WordifyLarge
    [InlineData(1_000_001.00, "ONE MILLION ONE DOLLARS")]
    // Comprehensive multi-scale with all chunk types and cents
    [InlineData(1_234_567.89, "ONE MILLION TWO HUNDRED AND THIRTY-FOUR THOUSAND FIVE HUNDRED AND SIXTY-SEVEN DOLLARS AND EIGHTY-NINE CENTS")]
    public void Convert_ValidAmount_ReturnsExpectedWords(double amount, string expected)
    {
        var result = _sut.Convert((decimal)amount);

        result.IsSuccess.Should().BeTrue();
        result.Words.Should().Be(expected);
    }

    // Regression tests for the decimal precision bug (floating-point subtraction trap)
    [Theory]
    [InlineData(1.15, "ONE DOLLAR AND FIFTEEN CENTS")]
    [InlineData(1.10, "ONE DOLLAR AND TEN CENTS")]
    [InlineData(10.10, "TEN DOLLARS AND TEN CENTS")]
    [InlineData(0.99, "ZERO DOLLARS AND NINETY-NINE CENTS")]
    public void Convert_DecimalCents_PrecisionIsCorrect(double amount, string expected)
    {
        var result = _sut.Convert((decimal)amount);

        result.IsSuccess.Should().BeTrue();
        result.Words.Should().Be(expected);
    }

    [Fact]
    public void Convert_NegativeAmount_TreatedAsAbsoluteValue()
    {
        var positive = _sut.Convert(5.00m);
        var negative = _sut.Convert(-5.00m);

        negative.IsSuccess.Should().BeTrue();
        negative.Words.Should().Be(positive.Words);
    }

    [Fact]
    public void Convert_MaxSupportedAmount_Succeeds()
    {
        var result = _sut.Convert(999_999_999_999_999.99m);

        result.IsSuccess.Should().BeTrue();
        result.Words.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Convert_AboveMaxAmount_ReturnsFailure()
    {
        var result = _sut.Convert(1_000_000_000_000_000m);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        result.Words.Should().BeNull();
    }

    [Fact]
    public void Convert_Success_ReturnsWordsAndNoErrorMessage()
    {
        var result = _sut.Convert(1.00m);

        result.IsSuccess.Should().BeTrue();
        result.Words.Should().NotBeNull();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Convert_Failure_ReturnsErrorMessageAndNoWords()
    {
        var result = _sut.Convert(1_000_000_000_000_000m);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
        result.Words.Should().BeNull();
    }
}
