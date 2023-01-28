using Money2Word.Models;
using Money2Word.Services.Interfaces;

namespace Money2Word.Services
{
    public class Money2WordService : IMoney2WordService
    {
        private readonly IMoney2WordConvertor money2WordConvertor;

        public Money2WordService(
            IMoney2WordConvertor money2WordConvertor)
        {
            this.money2WordConvertor = money2WordConvertor;
        }

        public ResponseModel Convert(InputModel model)
        {
            var (word, hasError) = money2WordConvertor.Money2Word(model.Amount);
            var response = new ResponseModel();
            if (hasError)
            {
                response.ErrorMessage = word;
                return response;
            }
            else
                response.Amount = word;
            return response;
        }
    }
}