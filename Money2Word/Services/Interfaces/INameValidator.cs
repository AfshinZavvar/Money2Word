namespace Money2Word.Services
{
    public interface INameValidator
    {
        (bool HasError, string NameResult) Validate(string Name);
    }
}