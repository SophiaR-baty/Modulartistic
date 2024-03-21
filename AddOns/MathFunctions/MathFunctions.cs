using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using MathNet.Numerics.Distributions;

namespace MathFunctions
{
    public static class MathFunctions
    {
        private static readonly int primeInitializationLimit = 1000000;
        private static Lazy<List<int>> _primes = new Lazy<List<int>>(() =>
        {
            List<int> p = SieveOfEratosthenes(primeInitializationLimit);
            return p;
        });



        #region general functions
        /// <summary>
        /// returns x*y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Product(double x, double y)
        {
            return x * y;
        }

        /// <summary>
        /// returns x^2 + y^2
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Circle(double x, double y)
        {
            return x*x+y*y;
        }

        public static double Gamma(double a)
        {
            return MathNet.Numerics.SpecialFunctions.Gamma(a);
        }

        public static double GammaLn(double a)
        {
            return MathNet.Numerics.SpecialFunctions.GammaLn(a);
        }

        public static double Binomial(double a, double b)
        {
            return Gamma(a - 1) / Gamma(b - 1) / Gamma(a - b - 1);
        }
        #endregion

        #region Number Theory Functions
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

        public static double IsPrime(double n)
        {
            int i_n = (int)Math.Floor(n);
            
            if (_primes.Value.Contains(i_n)) { return 0; }
            return double.NaN;
        }

        public static int IsComposite(int n)
        {
            if (n < 2) return 0;
            if (n == 2 || n == 3) return 0;

            if (n % 2 == 0 || n % 3 == 0) return 1;

            int sqrtN = (int)Math.Sqrt(n);
            for (int i = 5; i <= sqrtN; i += 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                    return 1;
            }

            return 0;
        }

        public static int PrimeCount(double n) 
        {
            int i = 0;
            for (; _primes.Value[i] <= n; i++) { }

            return i;
        }

        public static int FactorCount(int n)
        {
            int result = 0;
            n = Math.Abs(n);
            for (int i = 1; i <= n; i++)
            {
                if (n % i == 0) { result++; } 
            }

            return result;
        }

        public static int PFactorCount(int n)
        {
            int result = 0;
            n = Math.Abs(n);
            for (int i = 0; _primes.Value[i] <= n; i++)
            {
                if (n % _primes.Value[i] == 0) { result++; }
            }

            return result;
        }

        public static int PFactorCount2(int n)
        {
            int result = 0;
            n = Math.Abs(n);
            for (int i = 0; _primes.Value[i] <= n; )
            {
                if (n % _primes.Value[i] == 0) 
                {
                    n /= _primes.Value[i];
                    result++; 
                }
                else { i++; }
                
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
