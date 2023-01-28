using System.ComponentModel.DataAnnotations;

namespace Money2Word.Models
{
    public record InputModel
    {
        [Required(ErrorMessage ="Amount is not valid")]
        public decimal Amount { get; set; }
    }
}