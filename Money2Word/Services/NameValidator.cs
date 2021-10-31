using System.Text.RegularExpressions;

namespace Money2Word.Services
{
    public class NameValidator : INameValidator
    {

        public (bool HasError, string NameResult) Validate(string Name)
        {
            if (Name == null)
            {
                return (true, "Name length shuld be between 2 and 15 characters");
            }

            var regex = new Regex(@"^([a-zA-Z](([a-zA-Z ])?[a-zA-Z]*)){2,15}$");
            Match match = regex.Match(Name);

            if (!match.Success)
            {
                return (true, "Name length shuld be between 2 and 15 characters");
            }
            else
            {
                return (false, Name);
            }
        }
    }
}