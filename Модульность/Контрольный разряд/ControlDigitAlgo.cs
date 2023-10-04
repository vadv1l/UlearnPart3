using System;
using System.Collections.Generic;
using System.Linq;

namespace SRP.ControlDigit
{
    public static class Extensions
    {
        // Вспомогательные методы-расширения поместите в этот класс.
        // Они должны быть понятны и потенциально полезны вне контекста задачи расчета контрольных разрядов.
        public static List<int> ReverseNumber(this long numb)
        {
            List<int> numbers = new List<int>();
            while (numb > 0)
            {
                numbers.Add(Convert.ToInt32(numb % 10));
                numb /= 10;
            }
            return numbers;
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
        private static int SumIsbn10(List<int> numbers)
        {
            int sum = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                sum += (i + 1) * numbers[i];
            }
            return sum;
        }
        public static char Isbn10(long number)
        {
            number *= 10;
            List<int> reverseNumb = number.ReverseNumber();
            int sumIsbn = SumIsbn10(reverseNumb);

            int result = 11 - sumIsbn%11;
            if (result % 11 == 10)
            {
                return ('X');
            }
            else
            {
                return (result%11).ToString().First();
            }
        }

        private static int CalculateNumbLuhn(int numb)
        {
            numb *= 2;
            if (numb > 9)
            {
                return numb % 10 + numb / 10;
            }
            else
            {
                return numb;
            }
        }
        private static List<int> NewListLuhn(long number)
        {
            List<int> numbers = number.ReverseNumber();
            for (int i = 0; i < numbers.Count; i++)
            {
                if (numbers.Count%2==0)
                {
                    if ((i+1) % 2 == 1)
                    {
                        numbers[i] *= 2;
                        if (numbers[i] > 9)
                        {
                            numbers[i] -= 9;
                        }
                    }
                }
                else
                {
                    if ((i+1) % 2 == 1)
                    {
                        numbers[i] = CalculateNumbLuhn(numbers[i]);
                    }
                }
            }
            return numbers;
        }
        private static int SumLuhn(List<int>numbers)
        {
            int sumNumbers = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                sumNumbers += numbers[i];
            }
            return sumNumbers;
        }
        public static int Luhn(long number)
        {
            List<int> numbers = NewListLuhn(number);
            int sumNumbers = SumLuhn(numbers);
            if (sumNumbers%10 == 0)
            {
                return 0;
            }
            else
            {
                return 10 - sumNumbers % 10;
            }
        }
    }
}
