using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Modulartistic.Common;

namespace Modulartistic.AddOns.Misc
{
    [AddOn]
    public static class MiscFunctions
    {

        public static void Initialize(AddOnInitializationArgs args)
        {
            collatz_cache = new Dictionary<(int, int), int>();
        }

        #region Memory fields
        private static Dictionary<(int, int), int> collatz_cache;
        private static object collatz_lock = new object();
        #endregion


        /// <summary>
        /// calculates the number that would result in reversing the representation of num in base num_base
        /// </summary>
        /// <param name="num">The number to reverse</param>
        /// <param name="num_base">The base to reverse the number in</param>
        /// <returns></returns>
        public static double DigitReverse(double num, double num_base)
        {
            int inum = (int)Math.Floor(num);
            int inum_base = (int)Math.Floor(num_base);

            if (inum < 0) { inum *= -1; }
            if (inum_base < 0) { inum_base *= -1; }
            if (Math.Abs(inum_base) <= 1) { return double.NaN; }

            List<int> digitList = GetNumInBase(inum, inum_base);

            // Convert the reversed number back to base 10
            int reversedNumBase10 = 0;
            for (int i = 0; i < digitList.Count; i++)
            {
                reversedNumBase10 = reversedNumBase10 * inum_base + digitList[i];
            }

            return reversedNumBase10;
        }
        
        /// <summary>
        /// calculates the digit sum of num in base num_base
        /// </summary>
        /// <param name="num">the number to calculate the digit sum of</param>
        /// <param name="num_base">the base to calculate the digit sum in</param>
        /// <returns></returns>
        public static double DigitSum(double num, double num_base)
        {
            int inum = (int)Math.Floor(num);
            int inum_base = (int)Math.Floor(num_base);

            if (inum < 0) { inum *= -1; }
            if (inum_base < 0) { inum_base *= -1; }
            if (Math.Abs(inum_base) <= 1) { return double.NaN; }

            List<int> digitList = GetNumInBase(inum, inum_base);

            // Convert the reversed number back to base 10
            int digit_sum = 0;
            for (int i = 0; i < digitList.Count; i++)
            {
                digit_sum += digitList[i];
            }

            return digit_sum;
        }

        /// <summary>
        /// calculates the digital root of num in base num_base, which is the repeated digital_sum until a one digit number is returned
        /// </summary>
        /// <param name="num">the number to calculate the digital root of</param>
        /// <param name="num_base">the base to calculate the digital root in</param>
        /// <returns></returns>
        public static double DigitalRoot(double num, double num_base)
        {
            int inum = (int)Math.Floor(num);
            int inum_base = (int)Math.Floor(num_base);

            if (inum < 0) { inum *= -1; }
            if (inum_base < 0) { inum_base *= -1; }
            if (Math.Abs(inum_base) <= 1) { return double.NaN; }

            int result = inum;
            for (; ; )
            {
                int digsum = (int)DigitSum(result, inum_base);
                if (digsum == result) { break; }
                result = digsum;
            }

            return result;
        }

        /// <summary>
        /// converts (x, y) coordinates to indexes from top-left to bottom-right such that (0, 0) -> 0 and (width, height)->width*height
        /// </summary>
        /// <param name="x">input x coordinate</param>
        /// <param name="y">input y coordinate</param>
        /// <param name="width">max width, tiles</param>
        /// <param name="height">max height, tiles</param>
        /// <returns></returns>
        public static double GetNumberedSquare(double x, double y, double width, double height)
        {
            width = Math.Abs(width);
            height = Math.Abs(height);

            if (height * width == 0) { return double.NaN; }
            return Mod(y, height) * width + Mod(x, width);
        }

        /// <summary>
        /// converts (x, y) coordinates to indexes from 0 at (0, 0) spiraling outwards in a square/pixel manner, it starts going up and goes clockwise, (0, 0)->0 (0, -1)->1 (1, -1)->2 (1, 0)->3 ...
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double CoordToSqrSpiral(double x, double y)
        {
            double s = Math.Abs(x) >= Math.Abs(y) ? x : y;
            double d = x == s ? -1 : 1;

            if (s >= 0) { return 4 * s * s - x + y; }
            else { return 4 * s * s + d * (2 * s + x + y); }
        }

        private static int CollatzStep(int n)
        {
            if (n % 2 == 0) { n /= 2; }
            else { n *= 3; n++; }
            return n;
        }

        public static double Collatz(int n, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                if (collatz_cache.ContainsKey((n, depth - i)))
                {
                    int previous = collatz_cache[(n, depth - i)];

                    Dictionary<(int, int), int> tmp_collatz2 = new Dictionary<(int, int), int>();
                    while (i > 0)
                    {
                        i--;
                        previous = CollatzStep(previous);
                        tmp_collatz2.Add((n, depth - i), previous);
                    }

                    lock (collatz_lock)
                    {
                        foreach (KeyValuePair<(int, int), int> kvp in tmp_collatz2)
                        {
                            collatz_cache.Add(kvp.Key, kvp.Value);
                        }
                    }

                    return previous;
                }
            }


            int result = n;
            Dictionary<(int, int), int> tmp_collatz = new Dictionary<(int, int), int>();

            for (int i = 0; i < depth; i++)
            {
                tmp_collatz.Add((n, i), result);
                result = CollatzStep(result);
            }

            lock (collatz_lock)
            {
                foreach (KeyValuePair<(int, int), int> kvp in tmp_collatz)
                {
                    collatz_cache.Add(kvp.Key, kvp.Value);
                }
                collatz_cache.TryAdd((n, depth), result);
            }
            
            return result;
        }


        public static double CollatzNaive(int n, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                n = CollatzStep(n);
            }
            return n;
        }




        public static Dictionary<Tuple<int, int>, double> CalculateAverageDistance(RandomPointEvaluator evaluator, int minX, int maxX, int minY, int maxY, int depth)
        {
            Dictionary<Tuple<int, int>, double> averages = new Dictionary<Tuple<int, int>, double>();

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Complex startPoint = new Complex(x, y);
                    int average = 0;
                    for (int i = 0; i < depth; i++)
                    {
                        int steps = 0;
                        int idx = evaluator.EvaluateIdx(startPoint, out steps);
                        average += idx;
                    }
                    averages[new Tuple<int, int>(x, y)] = ((double)average) / depth;
                }
            }

            return averages;
        }

        public static double FractalNGon(double x, double y, Complex[] vertices, int depth)
        {
            RandomPointEvaluator rpe = new RandomPointEvaluator(vertices.ToList(), 0.5);
            Complex startPoint = new Complex(x, y);
            int average = 0;
            for (int i = 0; i < depth; i++)
            {
                int steps = 0;
                int idx = rpe.EvaluateIdx(startPoint, out steps);
                average += idx;
            }

            return ((double)average) / depth;
        }

        public static double FractalNGon(double x, double y, Complex[] vertices, double[] probabilities, int depth)
        {
            RandomPointEvaluatorAlt rpe = new RandomPointEvaluatorAlt(vertices.ToList(), probabilities.ToList());
            Complex startPoint = new Complex(x, y);
            int average = 0;
            for (int i = 0; i < depth; i++)
            {
                int steps = 0;
                int idx = rpe.EvaluateIdx(startPoint, out steps);
                average += idx;
            }

            return ((double)average) / depth;
        }

        public static Complex[] NGonVertices(int n, double r, Complex center)
        {
            Complex[] vertices = new Complex[n];
            
            for (int i = 0; i < n; i++) 
            {
                double theta = 2*Math.PI*i/n;
                vertices[i] = new Complex(center.Real + r*Math.Cos(theta), center.Imaginary + r*Math.Sin(theta));
            }

            return vertices;
        }

        public static double[] Probabilities(params double[] probs) 
        { 
            return probs; 
        }

        public static RandomPointEvaluator RPE(double probability, params Complex[] points)
        {
            return new RandomPointEvaluator(points.ToList(), probability);
        }

        public static Complex Point(double x, double y)
        {
            return new Complex(x, y);
        }

        public static double Average(double x, double y, Dictionary<Tuple<int, int>, double> grid)
        {
            Tuple<int, int> key = new Tuple<int, int>((int)Math.Floor(x), (int)Math.Floor(y));
            
            if (!grid.TryGetValue(key, out double result))
            {
                return double.NaN;
            }

            // Console.WriteLine(result);
            return result;
            
        }

        #region private helperfunctions

        /// <summary>
        /// gets a number in a specified base, the result is a list where each element represents a digit and is in range 0-num_base
        /// </summary>
        /// <param name="num">The number to convert (as a regular int), only works with non-negative numbers</param>
        /// <param name="num_base">the number base to convert to, if this is 0 or 1 returns an empty list</param>
        /// <returns>List of integers representing digits in a specified base</returns>
        private static List<int> GetNumInBase(int num, int num_base)
        {
            if (num < 0) { num *= -1; }
            if (num_base < 0) { num_base *= -1; }
            if (Math.Abs(num_base) <= 1) { return new List<int>(); }

            List<int> digitList = new List<int>();
            int idx = 0;
            while (num > 0)
            {
                // Console.WriteLine(inum);
                digitList.Add(num % num_base);
                num /= num_base;
                idx++;
            }

            return digitList;
        }


        private static double Mod(double d1, double d2)
        {
            if (d2 <= 0)
                throw new DivideByZeroException();
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }


        #endregion

    }
}
