namespace Money2Word.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceFirstOccurrence(this string source, string find, string replace) =>
            ReplaceOccurrence(source, find, replace, first: true);

        public static string ReplaceLastOccurrence(this string source, string find, string replace) =>
            ReplaceOccurrence(source, find, replace, first: false);

        private static string ReplaceOccurrence(string source, string find, string replace, bool first)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(find))
                return source ?? string.Empty;

            var comparison = StringComparison.OrdinalIgnoreCase;
            var index = first
                ? source.IndexOf(find, comparison)
                : source.LastIndexOf(find, comparison);

            if (index < 0)
                return source;

            return string.Concat(
                source.AsSpan(0, index),
                replace,
                source.AsSpan(index + find.Length)
            );
        }
    }
}