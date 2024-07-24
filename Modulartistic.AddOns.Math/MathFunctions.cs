using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using Modulartistic.AddOns;

namespace MathFunctions
{
    [AddOn]
    public static class MathFunctions
    {
        #region private fields and methods for prime initialization
        private static readonly int primeInitializationLimit = 1000000;
        private static Lazy<List<int>> _primes = new Lazy<List<int>>(() =>
        {
            List<int> p = SieveOfEratosthenes(primeInitializationLimit);
            return p;
        });

        private static List<int> SieveOfEratosthenes(int limit)
        {
            List<int> primes = new List<int>();

            if (limit < 2)
                return primes;

            bool[] isComposite = new bool[limit];
            for (int i = 2; i * i < limit; i++)
            {
                if (!isComposite[i])
                {
                    for (int j = i * i; j < limit; j += i)
                    {
                        isComposite[j] = true;
                    }
                }
            }

            for (int i = 2; i < limit; i++)
            {
                if (!isComposite[i])
                {
                    primes.Add(i);
                }
            }

            return primes;
        }
        #endregion

        #region general functions
        /// <summary>
        /// Computes the Gamma function for a given value.
        /// </summary>
        /// <param name="a">The value for which to compute the Gamma function.</param>
        /// <returns>The Gamma function evaluated at <paramref name="a"/>.</returns>
        public static double Gamma(double a)
        {
            return MathNet.Numerics.SpecialFunctions.Gamma(a);
        }

        /// <summary>
        /// Computes the natural logarithm of the Gamma function for a given value.
        /// </summary>
        /// <param name="a">The value for which to compute the natural logarithm of the Gamma function.</param>
        /// <returns>The natural logarithm of the Gamma function evaluated at <paramref name="a"/>.</returns>
        public static double GammaLn(double a)
        {
            return MathNet.Numerics.SpecialFunctions.GammaLn(a);
        }

        /// <summary>
        /// Computes the binomial coefficient for the given values using the Gamma function.
        /// </summary>
        /// <param name="a">The first value for the binomial coefficient calculation.</param>
        /// <param name="b">The second value for the binomial coefficient calculation.</param>
        /// <returns>The binomial coefficient for <paramref name="a"/> and <paramref name="b"/>.</returns>
        /// <remarks>
        /// This method uses the relationship between the Gamma function and binomial coefficients.
        /// Specifically, the binomial coefficient is calculated as:
        /// <code>Gamma(a - 1) / (Gamma(b - 1) * Gamma(a - b - 1))</code>
        /// </remarks>
        public static double Binomial(double a, double b)
        {
            return Gamma(a - 1) / Gamma(b - 1) / Gamma(a - b - 1);
        }

        /// <summary>
        /// Computes the factorial of a non-negative integer using the Gamma function.
        /// </summary>
        /// <param name="n">The number for which to compute the factorial.</param>
        /// <returns>The factorial of <paramref name="n"/>.</returns>
        /// <remarks>
        /// The factorial of a non-negative integer n is defined as:
        /// <code>n! = Gamma(n + 1)</code>
        /// This relationship leverages the fact that Gamma(n) = (n-1)! for positive integers.
        /// </remarks>
        public static double Factorial(int n)
        {
            return MathNet.Numerics.SpecialFunctions.Gamma(n + 1);
        }

        #endregion

        #region Number Theory Functions

        public static bool IsPrime(double n)
        {
            int i_n = (int)Math.Floor(n);

            return _primes.Value.Contains(i_n);
        }

        public static bool IsComposite(double n)
        {
            if (n <= primeInitializationLimit) 
            {
                return !IsPrime(n);
            }

            int i_n = (int)Math.Floor(n);

            if (i_n < 2) return true;
            if (i_n == 2 || i_n == 3) return false;

            if (i_n % 2 == 0 || i_n % 3 == 0) return true;

            int sqrtN = (int)Math.Sqrt(i_n);
            for (int i = 5; i <= sqrtN; i += 6)
            {
                if (i_n % i == 0 || i_n % (i + 2) == 0)
                    return true;
            }

            return false;
        }

        public static int PrimeCount(double n)
        {
            if (n > primeInitializationLimit) { return _primes.Value.Count; }
            int i = 0;
            for (; _primes.Value[i] <= n; i++) { }

            return i;
        }

        public static int FactorCount(double n)
        {
            int i_n = (int)Math.Floor(n);
            int result = 0;
            i_n = Math.Abs(i_n);
            int sqrtN = (int)Math.Sqrt(i_n);

            for (int i = 1; i <= sqrtN; i++)
            {
                if (i_n % i == 0) { result += 2; }
            }

            if (sqrtN * sqrtN == n)
            {
                result--;
            }

            return result;
        }

        public static int PrimeFactorCount(double d_n)
        {
            int n = (int)Math.Floor(d_n);
            int result = 0;
            n = Math.Abs(n);
            int sqrtN = (int)Math.Sqrt(n);
            for (int i = 0; _primes.Value[i] <= sqrtN;)
            {
                if (n % _primes.Value[i] == 0)
                {
                    n /= _primes.Value[i];
                    result++;
                }
                else { i++; }
            }

            // If there is any prime factor greater than sqrt(n) left
            if (n > 1)
            {
                result++;
            }

            return result;
        }

        public static double Harmonic(double a)
        {
            return MathNet.Numerics.SpecialFunctions.Harmonic((int)a);
        }

        public static double GeneralHarmonic(double a, double b)
        {
            return MathNet.Numerics.SpecialFunctions.GeneralHarmonic((int)a, b);
        }

        public static double Mod(double d1, double d2)
        {
            if (d2 <= 0)
                return double.NaN;
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }

        #endregion

        #region Random Functions

        public static double RandomNumber(double min, double max)
        {
            Random r = new Random();
            return min + r.NextDouble() * (max - min);
        }

        public static double NormalNumber(double mu, double sigma)
        {
            Normal n = new Normal(mu, Math.Abs(sigma));
            return n.Sample();
        }

        #endregion
    
        public static double Tanh(double x)
        {
            return Math.Tanh(x);
        }

        public static double Sinh(double x)
        {
            return Math.Sinh(x);
        }

        public static double Cosh(double x)
        {
            return Math.Cosh(x);
        }
    }
}
