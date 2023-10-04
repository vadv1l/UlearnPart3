using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.RationalNumbers
{
     public class Rational
    {
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        public bool IsNan => Denominator == 0; 
        public Rational(int numerator,int denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
            ZeroNumerator();
            Minus();
            ReduceRational();
        }
        public Rational(int numerator)
        {
            Numerator = numerator;
            Denominator = 1;
        }
        public void ZeroNumerator()
        {
            if (Numerator == 0)
            {
                Denominator = 1;
            }
        }
        public void Minus()
        {
            if (Denominator < 0)
            {
                Denominator = -Denominator;
                Numerator = -Numerator;
            }
        }
        public void ReduceRational()
        {
            for (int i = Math.Abs(Numerator); i>0; i--)
            {
                if (Numerator % i == 0 && Denominator % i == 0)
                {
                    Numerator /= i;
                    Denominator /= i;
                }
            }
        }
        public static Rational operator +(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
            {
                return new Rational(1, 0);
            }
            return new Rational(r1.Numerator * r2.Denominator + r2.Numerator * r1.Denominator, r1.Denominator * r2.Denominator);
        }
        public static Rational operator -(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
            {
                return new Rational(1, 0);
            }
            return new Rational(r1.Numerator * r2.Denominator - r2.Numerator * r1.Denominator, r1.Denominator * r2.Denominator);
        }
        public static Rational operator *(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
            {
                return new Rational(1, 0);
            }
            return new Rational(r1.Numerator*r2.Numerator,r1.Denominator*r2.Denominator);
        }
        public static Rational operator /(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
            {
                return new Rational(1, 0);
            }
            return new Rational(r1.Numerator * r2.Denominator, r1.Denominator * r2.Numerator);
        }
        public static implicit operator double(Rational r1)
        {
            if (r1.IsNan||r1.Numerator==0)
            {
                return double.NaN;
            }
            return r1.Numerator/(double)r1.Denominator;
        }
        public static explicit operator int(Rational r1)
        {
            if (r1.Numerator % r1.Denominator != 0)
            {
                throw new Exception();
            }
            else
            {
                int numer = r1.Numerator / r1.Denominator;
                return numer;
            }
        }
        public static implicit operator Rational(int number)
        {
            return new Rational(number);
        }
    }
}
