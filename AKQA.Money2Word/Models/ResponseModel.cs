namespace AKQA.Money2Word.Models
{
    public class ResponseModel : IResponseModel
    {
        public string ErrorMessage { get; set; }
        public string Amount { get; set; }
        public string Name { get; set; }
    }
}