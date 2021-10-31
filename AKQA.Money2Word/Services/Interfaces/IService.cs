using Money2Word.Models;

namespace Money2Word.Services.Interfaces
{
    public interface IService
    {
        IResponseModel FillModel(IInputModel model);
    }
}
