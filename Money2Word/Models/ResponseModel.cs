namespace Money2Word.Models
{
    public record ResponseModel
    {
        public string? ErrorMessage { get; set; }
        public string? Amount { get; set; }
    }
}