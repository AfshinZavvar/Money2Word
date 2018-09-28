namespace AKQA.Money2Word.Models
{
    public interface IResponseModel
    {
        string Amount { get; set; }
        string Name { get; set; }
        string ErrorMessage { get; set; }
    }
}