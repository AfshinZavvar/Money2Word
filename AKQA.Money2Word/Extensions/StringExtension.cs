
using System;

namespace AKQA.Money2Word.Extensions
{
    public static class StringExtension
    {
        public static string ReplaceFirstOccurrence(this string Source, string Find, string Replace)
        {
            if (string.IsNullOrEmpty(Source))
                return string.Empty;

            int Place = Source.IndexOf(Find, StringComparison.InvariantCultureIgnoreCase);

            if (Place < 0)
                return Source;

            return Source.Remove(Place, Find.Length).Insert(Place, Replace);          
        }

        public static string ReplaceLastOccurrence(this string Source, string Find, string Replace)
        {
            if (string.IsNullOrEmpty(Source))
                return string.Empty;

            int Place = Source.LastIndexOf(Find, StringComparison.InvariantCultureIgnoreCase);

            if (Place < 0)
                return Source;

            return Source.Remove(Place, Find.Length).Insert(Place, Replace);
        }
    }
}