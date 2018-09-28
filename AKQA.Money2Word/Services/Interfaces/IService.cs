using AKQA.Money2Word.Models;

namespace AKQA.Money2Word.Services.Interfaces
{
    public interface IService
    {
        IResponseModel FillModel(IInputModel model);
    }
}
