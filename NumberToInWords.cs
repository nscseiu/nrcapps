using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Web.SessionState;
using System.Linq;

namespace NRCAPPS
{
    class NumberToInWords
    { 
        public NumberToInWords()
        {
        }

        public static string DecimalToWordsUSD(decimal d)
        {
            //Grab a string form of your decimal value ("12.34")
            var formatted = d.ToString();

            if (formatted.Contains("."))
            {
                //If it contains a decimal point, split it into both sides of the decimal
                string[] sides = formatted.Split('.');

                if (sides[1].Count() == 1) { sides[1] += "0"; }

                //Process each side and append them with "and", "dot" or "point" etc.
                return " us dollars " + NumberToWords(Int32.Parse(sides[0])) + " and cents " + NumberToWords(Int32.Parse(sides[1])) + " only";
            }
            else
            {
                //Else process as normal
                return " us dollars " + NumberToWords(Convert.ToInt32(d)) + " only";
            }
        }

        public static string DecimalToWordsSR(decimal d)
        {
            //Grab a string form of your decimal value ("12.34")
            var formatted = d.ToString();

            if (formatted.Contains("."))
            {
                //If it contains a decimal point, split it into both sides of the decimal
                string[] sides = formatted.Split('.');

                if (sides[1].Count() == 1) { sides[1] += "0"; }

                //Process each side and append them with "and", "dot" or "point" etc.
                return " saudi riyals " + NumberToWords(Int32.Parse(sides[0])) + " and halalas " + NumberToWords(Int32.Parse(sides[1])) + " only";
            }
            else
            {
                //Else process as normal
                return " saudi riyals " + NumberToWords(Convert.ToInt32(d)) + " only";
            }
        }

        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number / 1000000000 > 0)
            {
                words += NumberToWords(number / 1000000000) + " billion ";
                number %= 1000000000;
            }

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            words = SmallNumberToWord(number, words);

            return words;
        }

        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
            return words;
        }


        public static string NumberToWordsInteger(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

         

    }

}