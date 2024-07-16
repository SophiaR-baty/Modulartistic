using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using MathNet.Numerics.Distributions;
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
        /// <summary>
        /// Determines if a given number is a prime number.
        /// </summary>
        /// <param name="n">The number to check for primality.</param>
        /// <returns>Returns 0 if the number is prime; otherwise, returns <see cref="Double.NaN"/>.</returns>
        /// <remarks>
        /// The method converts the input number to an integer by taking the floor of the input value.
        /// It then checks if the integer is in a predefined list of prime numbers.
        /// </remarks>
        public static double IsPrime(double n)
        {
            int i_n = (int)Math.Floor(n);

            if (_primes.Value.Contains(i_n)) { return 0; }
            return double.NaN;
        }

        /// <summary>
        /// Determines if a given number is a composite number.
        /// </summary>
        /// <param name="n">The number to check for compositeness.</param>
        /// <returns>
        /// Returns 0 if the number is composite; returns <see cref="Double.NaN"/> if the number is not composite; 
        /// otherwise, returns <see cref="Double.NaN"/>.
        /// </returns>
        /// <remarks>
        /// The method first checks if the input number is less than or equal to a predefined prime initialization limit.
        /// If so, it calls <see cref="IsPrime(double)"/> to check if the number is prime. If the number is not prime, it is composite.
        /// Otherwise, the method converts the input number to an integer by taking the floor of the input value and performs the following checks:
        /// - If the integer is less than 2, it is not composite.
        /// - If the integer is exactly 2 or 3, it is not composite.
        /// - If the integer is divisible by 2 or 3, it is composite.
        /// - If the integer is divisible by any number up to the square root of the integer (using a 6k ± 1 optimization), it is composite.
        /// If none of these conditions are met, the number is not composite.
        /// </remarks>
        public static double IsComposite(double n)
        {
            if (n <= primeInitializationLimit) 
            {
                return double.IsNaN(IsPrime(n)) ? 0 : double.NaN;
            }

            int i_n = (int)Math.Floor(n);

            if (i_n < 2) return 0;
            if (i_n == 2 || i_n == 3) return double.NaN;

            if (i_n % 2 == 0 || i_n % 3 == 0) return 0;

            int sqrtN = (int)Math.Sqrt(i_n);
            for (int i = 5; i <= sqrtN; i += 6)
            {
                if (i_n % i == 0 || i_n % (i + 2) == 0)
                    return 0;
            }

            return double.NaN;
        }

        /// <summary>
        /// Counts the number of prime numbers less than or equal to the given value.
        /// </summary>
        /// <param name="n">The value up to which to count the prime numbers.</param>
        /// <returns>The number of prime numbers less than or equal to <paramref name="n"/>.</returns>
        /// <remarks>
        /// The method iterates through a predefined list of prime numbers and counts how many of them are less than or equal to the input value.
        /// </remarks>
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

        /// <summary>
        /// Counts the number of prime factors of a given integer.
        /// </summary>
        /// <param name="d_n">The number for which to count the prime factors.</param>
        /// <returns>The number of prime factors of <paramref name="d_n"/>.</returns>
        /// <remarks>
        /// Factors are counted up to the square root of <paramref name="d_n"/> for efficiency.
        /// The method converts <paramref name="d_n"/> to an integer by taking the floor of its value.
        /// It then iterates through a list of precomputed prime numbers (_primes.Value) and counts how many of them are factors of <paramref name="d_n"/>.
        /// If <paramref name="d_n"/> has any prime factors greater than its square root after the loop, they are counted as well.
        /// </remarks>
        public static int PFactorCount(double d_n)
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

        /// <summary>
        /// Computes the harmonic number of an integer.
        /// </summary>
        /// <param name="a">The integer for which to compute the harmonic number.</param>
        /// <returns>The harmonic number of <paramref name="a"/>.</returns>
        /// <remarks>
        /// The harmonic number H(n) is defined as the sum of the reciprocals of the first n natural numbers:
        /// H(n) = 1 + 1/2 + 1/3 + ... + 1/n
        /// </remarks>
        public static double Harmonic(double a)
        {
            return MathNet.Numerics.SpecialFunctions.Harmonic((int)a);
        }

        /// <summary>
        /// Computes the generalized harmonic number.
        /// </summary>
        /// <param name="a">The integer for which to compute the generalized harmonic number.</param>
        /// <param name="b">The real number representing the power of the denominator.</param>
        /// <returns>The generalized harmonic number H(a, b).</returns>
        /// <remarks>
        /// The generalized harmonic number H(a, b) is defined as the sum of the reciprocals of the first n natural numbers raised to the power b:
        /// H(a, b) = 1^b + 2^b + 3^b + ... + a^b
        /// </remarks>
        public static double GeneralHarmonic(double a, double b)
        {
            return MathNet.Numerics.SpecialFunctions.GeneralHarmonic((int)a, b);
        }

        /// <summary>
        /// Computes the modulus operation of two numbers.
        /// </summary>
        /// <param name="d1">The dividend.</param>
        /// <param name="d2">The divisor, which must be positive.</param>
        /// <returns>The remainder after dividing <paramref name="d1"/> by <paramref name="d2"/>.</returns>
        /// <remarks>
        /// The modulus operation computes the remainder when dividing <paramref name="d1"/> by <paramref name="d2"/>.
        /// If <paramref name="d2"/> is non-positive, <see cref="Double.NaN"/> is returned.
        /// </remarks>
        public static double Mod(double d1, double d2)
        {
            if (d2 <= 0)
                return double.NaN;
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }
        #endregion

        #region Fractal Functions
        public static double Mandelbrot(double x, double y, int depth)
        {
            Complex z_0 = new Complex(x, y);
            Complex z = 0;

            int i = 0;
            while (i < depth)
            {
                z = Complex.Pow(z, 2) + z_0;
                double a = Complex.Abs(z);
                if (a > 2)
                    return double.NaN;
                i++;
            }
            return Complex.Abs(z);
        }

        public static double Juliaset(double z_x, double z_y, double c_x, double c_y, int depth)
        {
            Complex z_0 = new Complex(z_x, z_y);
            Complex c = new Complex(c_x, c_y);
            Complex z = z_0;

            int i = 0;
            while (i < depth)
            {
                z = Complex.Pow(z, 2) + c;
                double a = Complex.Abs(z);
                if (a > 2)
                    return Double.NaN;
                i++;
            }
            return Complex.Abs(z);
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

        #region other Functions
        public static double PascalsTriangleLn(double x, double y, double skew)
        {
            return MathNet.Numerics.SpecialFunctions.BinomialLn((int)Math.Floor(-y), (int)Math.Floor((x - skew - y / 2)));
        }

        public static double PascalsTriangle(double x, double y, double skew)
        {
            return MathNet.Numerics.SpecialFunctions.Binomial((int)Math.Floor(-y), (int)Math.Floor(x - skew - y / 2));
        }

        public static double PascalsTriangleGamma(double x, double y, double skew)
        {
            double a = -y;
            double b = x - skew - y / 2;

            return Gamma(a - 1) / Gamma(b - 1) / Gamma(a - b - 1);
        }

        public static double PascalsTriangleGammaLn(double x, double y, double skew)
        {
            double a = -y;
            double b = x - skew - y / 2;

            return GammaLn(a - 1) / GammaLn(b - 1) / GammaLn(a - b - 1);
        }
        #endregion















        /*
        
        public static double Bezier(double x, double y, double p0_x, double p0_y, double p1_x, double p1_y, double p2_x, double p2_y, double p3_x, double p3_y)
        {
            const double STEP = 0.001;
            // const double TOLERANCE = 0.5;
            
            Func<double, double> xFunc = (t) => Math.Pow(1 - t, 3) * p0_x + 3 * (1 - t) * (1 - t) * t * p1_x + 3 * (1 - t) * t * t * p2_x + Math.Pow(t, 3) * p3_x;
            Func<double, double> yFunc = (t) => Math.Pow(1 - t, 3) * p0_y + 3 * (1 - t) * (1 - t) * t * p1_y + 3 * (1 - t) * t * t * p2_y + Math.Pow(t, 3) * p3_y;
            // Func<double, double> bezierAngleFunc = (t) => Math.Atan2(yFunc(t), xFunc(t)); // from -180 to 180
            // Func<double, double> pointBezierAngleFunc = (t) => Math.Atan2(y-yFunc(t), x-xFunc(t)); // from -180 to 180
            Func<double, double> distanceFunc = (t) => Math.Sqrt(Math.Pow(x - xFunc(t), 2) + Math.Pow(y - yFunc(t), 2));
            // Func<double, double> gradFunc = (t) => Math.Sqrt(Math.Pow(xFunc(t), 2) + Math.Pow(yFunc(t), 2));

            double minDistance = double.MaxValue;
            for (double t = 0; t < 1; t += STEP)
            {
                double distance = distanceFunc(t);
                if (distance < minDistance) { minDistance = distance; }
            }

            return minDistance;
        }

        public static double A004738(double x)
        {
            double z = Math.Floor(Math.Sqrt(x) + 1.0 / 2);
            return z + 1 - Math.Abs(x - 1 - z * z);
        }
    
        public static double Collatz(double start, double steps)
        {
            int x = (int)Math.Round(start);
            int stp = (int)Math.Round(steps);

            for (int i = 0; i < stp; i++)
            {
                x = x % 2 == 0 ? x / 2 : 3 * x + 1;
            }

            return x;
        }
        */


    }
}
