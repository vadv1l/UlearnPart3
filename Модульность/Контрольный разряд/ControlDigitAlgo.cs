using System;
using System.Collections.Generic;
using System.Linq;

namespace SRP.ControlDigit
{
    public static class Extensions
    {
        public static List<int> ReverseNumb(this long number)
        {
            List<int> reverseNumber = new List<int>();
            while (number >= 1)
            {
                reverseNumber.Add((int)number % 10);
                number /= 10;
            }
            return reverseNumber;
        }
    }

    public static class ControlDigitAlgo
    {
        public static int Upc(long number)
        {
            int sum = 0;
            int factor = 3;
            do
            {
                int digit = (int)(number % 10);
                sum += factor * digit;
                factor = 4 - factor;
                number /= 10;

            }
            while (number > 0);

            int result = sum % 10;
            if (result != 0)
                result = 10 - result;
            return result;
        }

        public static char Isbn10(long number)
        {
            number *= 10;
            List<int> reverseNum = number.ReverseNumb();
            int sum = 0;
            for (int i = 0; i < reverseNum.Count; i++)
            {
                sum += (i + 1) * reverseNum[i];
            }
            int result = 11 - sum % 11;

            if (result%11 == 10)
            {
                return 'X';
            }
            else
            {
                return (result%11).ToString().First();
            }
        }

        public static int Luhn(long number)
        {
            List<int> numberr = number.ReverseNumb();
            if (numberr.Count % 2 == 1)
            {
                for (int i = 0; i < numberr.Count; i+=2)
                {
                    numberr[i] *= 2;
                    if (numberr[i] > 9)
                    {
                        numberr[i] = numberr[i] % 10 + numberr[i] / 10;
                    }
                }
            }
            else
            {
                for (int i = 1; i < numberr.Count; i+=2)
                {

                }
            }

        }
    }
}
