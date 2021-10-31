using Money2Word.Models;
using Money2Word.Services.Interfaces;

namespace Money2Word.Services
{
    public class Service : IService
    {
        private readonly IMoney2WordConvertor money2WordConvertor;
        private readonly IResponseModel responseModel;
        private readonly INameValidator nameValidator;

        public Service(
            IMoney2WordConvertor money2WordConvertor,
            INameValidator nameValidator,
            IResponseModel responseModel)
        {
            this.money2WordConvertor = money2WordConvertor;
            this.responseModel = responseModel;
            this.nameValidator = nameValidator;
        }

        public IResponseModel FillModel(IInputModel model)
        {
           var result= money2WordConvertor.Money2Word(model?.Amount);

            if (result.HasError)
            {
                responseModel.ErrorMessage = result.Word;
                return responseModel;
            }
            else
                responseModel.Amount = result.Word;

            var nameValidationResult=nameValidator.Validate(model?.Name);

            if (nameValidationResult.HasError )
            {
                responseModel.ErrorMessage = nameValidationResult.NameResult;
            }
            else
            {
                responseModel.Name = nameValidationResult.NameResult;
            }

            return responseModel;
        }
    }
}