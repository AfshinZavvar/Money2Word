using System.ComponentModel.DataAnnotations;

namespace Money2Word.Models;

public record InputModel
{
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999_999_999_999_999.99,
        ErrorMessage = "Amount must be between 0.01 and 999,999,999,999,999.99")]
    public decimal Amount { get; init; }
}
