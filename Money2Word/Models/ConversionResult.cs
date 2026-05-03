using System.Diagnostics.CodeAnalysis;

namespace Money2Word.Models;

/// <summary>
/// Represents the outcome of a monetary conversion. Use <see cref="Success"/> or
/// <see cref="Failure"/> factory methods — never construct directly.
/// </summary>
public readonly record struct ConversionResult
{
    /// <summary>The English word representation. Non-null when <see cref="IsSuccess"/> is true.</summary>
    public string? Words { get; init; }

    /// <summary>Human-readable error description. Non-null when <see cref="IsSuccess"/> is false.</summary>
    public string? ErrorMessage { get; init; }

    [MemberNotNullWhen(true, nameof(Words))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; init; }

    private ConversionResult(bool isSuccess, string? words, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Words = words;
        ErrorMessage = errorMessage;
    }

    public static ConversionResult Success(string words) => new(true, words, null);
    public static ConversionResult Failure(string errorMessage) => new(false, null, errorMessage);
}
