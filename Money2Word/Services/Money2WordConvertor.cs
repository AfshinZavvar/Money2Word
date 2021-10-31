using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Money2Word.Services
{
    public class Money2WordConvertor : IMoney2WordConvertor
    {
        private Dictionary<int, string> Ones;
        private Dictionary<int, string> Tens;
        private Dictionary<int, string> Houndreds;
        private StringBuilder Text;
       
        public Money2WordConvertor()
        {
            Initialize();
        }

        private void Initialize()
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

        public (string Word, bool HasError) Money2Word(string money)
        {
            if (money == null)
            {
                return ("Money is not in correct format.Please provide value in {xxx.xx} format", true);

            }

            var regex = new Regex(@"^(\d{1}|\d{2}|\d{3})(\.(\d{1}|\d{2}))?$");
            Match match = regex.Match(money);

            if (!match.Success)
                return ("Money is not in correct format.Please provide value in {xxx.xx} format", true);

            var splittedNumber = money.Split('.');
            int cents = 0;

            if (!int.TryParse(splittedNumber[0], out int dollars)) dollars = 0;

            if (splittedNumber.Length > 1)
            {
                cents = AccurateCents(splittedNumber[1]);
            }

            Money2Word(dollars, cents);

            return (Text.ToString().ToUpper(), false);
        }

        private int AccurateCents(string cents)
        {
            return (int)(100 * Convert.ToDouble("0." + cents.PadRight(2, '0')));
        }

        private void Money2Word(int dollars, int cents)
        {
            Wordify(dollars);
            Text.Append(" Dollars and ");
            Wordify(cents);
            Text.Append(" Cents");
        }

        private void Wordify(int number)
        {
            int reminder = 0;
            int quotient = 0;

           if (number < 10)
            {
                Text.Append(Ones[number]);
            }

            else if (number < 100)
            {
                reminder = (number % 10);
                quotient = (int)(number / 10);

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
            }
            else
            {
                reminder = (number % 100);
                quotient = (int)(number / 100);

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
            }
        }
    }
}