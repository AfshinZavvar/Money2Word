using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Money2Word.Models;
using Money2Word.Services.Interfaces;
using System.Collections.Frozen;
using System.Text;

namespace Money2Word.Services;

public class Money2WordService(ILogger<Money2WordService> logger, TelemetryClient telemetryClient) : IMoney2WordService
{
    private const decimal MaxSupportedAmount = 999_999_999_999_999.99m;

    // FrozenDictionary is read-optimised: no lock overhead, perfect-hash lookup
    private static readonly FrozenDictionary<int, string> Ones = new Dictionary<int, string>
    {
        [0] = "Zero", [1] = "One",   [2] = "Two",   [3] = "Three", [4] = "Four",
        [5] = "Five",  [6] = "Six",   [7] = "Seven", [8] = "Eight", [9] = "Nine"
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<int, string> Tens = new Dictionary<int, string>
    {
        [10] = "Ten",     [11] = "Eleven",    [12] = "Twelve",    [13] = "Thirteen",
        [14] = "Fourteen",[15] = "Fifteen",   [16] = "Sixteen",   [17] = "Seventeen",
        [18] = "Eighteen",[19] = "Nineteen",  [20] = "Twenty",    [30] = "Thirty",
        [40] = "Forty",   [50] = "Fifty",     [60] = "Sixty",     [70] = "Seventy",
        [80] = "Eighty",  [90] = "Ninety"
    }.ToFrozenDictionary();

    // Indexed 0–4: units, thousands, millions, billions, trillions
    private static readonly string[] Scales = ["", " Thousand", " Million", " Billion", " Trillion"];

        public ConversionResult Convert(decimal amount)
        {
            logger.LogInformation("Convert called with Amount: {Amount}", amount);

            if (amount < 0)
                amount = Math.Abs(amount);

            if (amount > MaxSupportedAmount)
            {
                logger.LogWarning("Amount {Amount} exceeds maximum supported value", amount);
                return ConversionResult.Failure(
                    $"Amount exceeds the maximum supported value of {MaxSupportedAmount:N2}");
            }

            try
            {
                var dollars = (long)Math.Truncate(amount);
                // Stay in decimal arithmetic to avoid floating-point precision loss
                var cents = (int)Math.Round((amount - dollars) * 100m, MidpointRounding.AwayFromZero);

                logger.LogDebug("Parsed dollars: {Dollars}, cents: {Cents}", dollars, cents);

                var sb = new StringBuilder();
                if (dollars == 0)
                    sb.Append(Ones[0]);
                else
                    WordifyLarge(dollars, sb);

                sb.Append(dollars == 1 ? " Dollar" : " Dollars");

                if (cents > 0)
                {
                    sb.Append(" and ");
                    Wordify(cents, sb);
                    sb.Append(cents == 1 ? " Cent" : " Cents");
                }

                var result = sb.ToString().ToUpperInvariant();

                logger.LogInformation("Conversion successful: {Result}", result);

                telemetryClient.TrackEvent("AmountConverted", new Dictionary<string, string>
                {
                    ["DollarAmount"] = dollars.ToString(),
                    ["HasCents"]     = (cents > 0).ToString()
                });

                return ConversionResult.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during conversion");
                return ConversionResult.Failure("An unexpected error occurred during conversion.");
            }
        }

        private static void WordifyLarge(long number, StringBuilder sb)
        {
            if (number == 0) return;

            // Stack naturally reverses insertion order (LIFO), avoiding O(n²) List.Insert(0,...)
            var parts = new Stack<string>();
            int scaleIndex = 0;

            while (number > 0)
            {
                int chunk = (int)(number % 1000);
                if (chunk > 0)
                {
                    var chunkBuilder = new StringBuilder();
                    Wordify(chunk, chunkBuilder);
                    chunkBuilder.Append(Scales[scaleIndex]);
                    parts.Push(chunkBuilder.ToString());
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
                    sb.Append('-');
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
