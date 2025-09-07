using Money2Word.Models;
using Money2Word.Services.Interfaces;
using System.Text;

namespace Money2Word.Services
{
    public class Money2WordService(ILogger<Money2WordService> logger) : IMoney2WordService
    {
        private readonly ILogger<Money2WordService> _logger = logger;

        private static readonly Dictionary<int, string> Ones = new()
        {
            [0] = "Zero",
            [1] = "One",
            [2] = "Two",
            [3] = "Three",
            [4] = "Four",
            [5] = "Five",
            [6] = "Six",
            [7] = "Seven",
            [8] = "Eight",
            [9] = "Nine"
        };

        private static readonly Dictionary<int, string> Tens = new()
        {
            [10] = "Ten",
            [11] = "Eleven",
            [12] = "Twelve",
            [13] = "Thirteen",
            [14] = "Fourteen",
            [15] = "Fifteen",
            [16] = "Sixteen",
            [17] = "Seventeen",
            [18] = "Eighteen",
            [19] = "Nineteen",
            [20] = "Twenty",
            [30] = "Thirty",
            [40] = "Forty",
            [50] = "Fifty",
            [60] = "Sixty",
            [70] = "Seventy",
            [80] = "Eighty",
            [90] = "Ninety"
        };

        private static readonly string[] Scales = ["", " Thousand", " Million", " Billion", " Trillion"];

        public ResponseModel Convert(InputModel model)
        {
            _logger.LogInformation("Convert method called with Amount: {Amount}", model.Amount);

            try
            {
                var amount = Math.Abs(model.Amount);
                var dollars = (long)Math.Truncate(amount);
                var cents = (int)((amount - dollars) * 100);

                _logger.LogDebug("Parsed dollars: {Dollars}, cents: {Cents}", dollars, cents);

                var sb = new StringBuilder();
                if (dollars == 0)
                {
                    sb.Append(Ones[0]);
                }
                else
                {
                    WordifyLarge(dollars, sb);
                }
                sb.Append(dollars == 1 ? " Dollar" : " Dollars");

                if (cents > 0)
                {
                    sb.Append(" and ");
                    Wordify(cents, sb);
                    sb.Append(cents == 1 ? " Cent" : " Cents");
                }

                var result = sb.ToString().ToUpperInvariant();

                _logger.LogInformation("Conversion successful: {Result}", result);

                return new ResponseModel { Amount = result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during money-to-word conversion");
                return new ResponseModel { ErrorMessage = ex.Message };
            }
        }

        private static void WordifyLarge(long number, StringBuilder sb)
        {
            if (number == 0) return;

            var parts = new List<string>();
            int scaleIndex = 0;

            while (number > 0)
            {
                int chunk = (int)(number % 1000);
                if (chunk > 0)
                {
                    var chunkBuilder = new StringBuilder();
                    Wordify(chunk, chunkBuilder);
                    chunkBuilder.Append(Scales[scaleIndex]);
                    parts.Insert(0, chunkBuilder.ToString());
                }
                number /= 1000;
                scaleIndex++;
            }

            sb.Append(string.Join(" ", parts));
        }

        private static void Wordify(int number, StringBuilder sb)
        {
            if (number < 10)
            {
                sb.Append(Ones[number]);
            }
            else if (number < 100)
            {
                if (Tens.TryGetValue(number, out var word))
                {
                    sb.Append(word);
                }
                else
                {
                    sb.Append(Tens[number / 10 * 10]);
                    sb.Append("-");
                    sb.Append(Ones[number % 10]);
                }
            }
            else
            {
                sb.Append(Ones[number / 100]);
                sb.Append(" Hundred");
                if (number % 100 > 0)
                {
                    sb.Append(" and ");
                    Wordify(number % 100, sb);
                }
            }
        }
    }
}