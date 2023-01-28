using Money2Word.Models;

namespace Money2Word.Services.Interfaces
{
    public interface IMoney2WordService
    {
        ResponseModel Convert(InputModel model);
    }
}
