using Money2Word.Models;

namespace Money2Word.Services.Interfaces;

/// <summary>Converts a monetary decimal amount to its English word representation.</summary>
public interface IMoney2WordService
{
    /// <summary>
    /// Converts <paramref name="amount"/> to English words.
    /// Negative values are treated as their absolute equivalent.
    /// </summary>
    ConversionResult Convert(decimal amount);
}
