namespace Money2Word.Services
{
    public interface IMoney2WordConvertor
    {
        (string Word, bool HasError) Money2Word(string input);
    }
}