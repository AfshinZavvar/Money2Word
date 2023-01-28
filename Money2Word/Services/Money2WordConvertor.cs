using System.Text;

namespace Money2Word.Services
{
    public class Money2WordConvertor : IMoney2WordConvertor
    {
        private Dictionary<int, string> Ones { get; init; }
        private Dictionary<int, string> Tens { get; init; }
        private Dictionary<int, string> Houndreds { get; init; }
        private StringBuilder Text { get; set; }

        public Money2WordConvertor()
        {
            Text = new StringBuilder("");

            Ones = new Dictionary<int, string>
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

            Tens = new Dictionary<int, string>
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
                [40] = "Fourty",
                [50] = "Fifty",
                [60] = "Sixty",
                [70] = "Seventy",
                [80] = "Eighty",
                [90] = "Ninety"
            };

            Houndreds = new Dictionary<int, string>()
            {
                [100] = "Hundred",
            };

        }

        public (string Word, bool HasError) Money2Word(decimal money)
        {
            money = Math.Abs(money);

            if (!decimal.TryParse(money.ToString()?.Split('.')?[1] ?? "0", out var cents))
            {
                cents = 0;
            };
            var dollars = Math.Truncate(money);

            Money2Word(dollars, cents);

            return (Text.ToString().ToUpper(), false);
        }

        private void Money2Word(decimal dollars, decimal cents)
        {
            Wordify((int)dollars);
            Text.Append(" Dollars and ");
            Wordify((int)cents);
            Text.Append(" Cents");
        }

        private void Wordify(int number)
        {
            int reminder;
            int quotient;
            switch (number)
            {
                case < 10:
                    Text.Append(Ones[number]);
                    break;
                case < 100:
                    reminder = number % 10;
                    quotient = number / 10;

                    if (reminder == 0 || quotient == 1)
                    {
                        Text.Append(Tens[number]);
                    }
                    else
                    {
                        Text.Append(Tens[quotient * 10]);
                        Text.Append("-");
                        Wordify(reminder);
                    }
                    break;
                default:
                    reminder = number % 100;
                    quotient = number / 100;

                    if (reminder == 0)
                    {
                        Wordify(quotient);
                        Text.Append($" {Houndreds[100]}");
                    }
                    else
                    {
                        Wordify(quotient);
                        Text.Append($" {Houndreds[100]}");
                        Text.Append(" AND ");
                        Wordify(reminder);
                    }
                    break;
            }
        }
    }
}