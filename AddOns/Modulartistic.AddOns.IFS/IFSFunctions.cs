using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.IFS
{
    [AddOn]
    public static class IFSFunctions
    {
        private static ConcurrentDictionary<string, IIteratedFunctionSystem> ifs_cache;

        public static void Initialize(AddOnInitializationArgs args)
        {
            ifs_cache = new ConcurrentDictionary<string, IIteratedFunctionSystem>();
        }

        public static double Distance(double x, double y, IIteratedFunctionSystem ifs)
        {
            Complex xy = new Complex(x, y);
            double minDistance = double.PositiveInfinity;

            foreach (var c in ifs.Points)
            {
                double dist = Complex.Abs(c - xy);
                if (dist < minDistance)
                {
                    minDistance = dist;
                }
            }

            return minDistance;
        }

        public static double InvertedDistance(double x, double y, IIteratedFunctionSystem ifs)
        {
            Complex xy = new Complex(x, y);
            double maxDistance = 0;

            foreach (var c in ifs.Points)
            {

                double dist = Complex.Abs(c - xy);
                if (dist > maxDistance)
                {
                    maxDistance = dist;
                }
            }

            return maxDistance;
        }

        public static double Match(double x, double y, IIteratedFunctionSystem ifs, double tolerance)
        {
            Complex xy = new Complex(x, y);

            for (int i = 0; i < ifs.Points.Count; i++)
            {
                Complex c = ifs.Points[i];
                if (Complex.Abs(c - xy) <= tolerance)
                {
                    return i;
                }
            }

            return double.NaN;
        }


        public static Complex Point(double x, double y)
        {
            return new Complex(x, y);
        }

        public static IIteratedFunctionSystem BarnsleyFern(Complex initial, int depth)
        {
            IIteratedFunctionSystem ifs = new BarnsleyFern(initial);
            ifs.GeneratePoints(depth);

            return ifs;
        }

        public static IIteratedFunctionSystem SierpinskiTriangle(Complex A, Complex B, Complex C, int depth)
        {
            IIteratedFunctionSystem ifs = new SierpinskiTriangle(A, B, C);
            ifs.GeneratePoints(depth);

            return ifs;
        }




        private static void LoadBarnsleyFern(int depth, Complex A, out IIteratedFunctionSystem ifs)
        {
            string key = $"barnsley_fern_{A.Real}_{A.Imaginary}";
            if (ifs_cache.ContainsKey(key)) { ifs = ifs_cache[key]; }
            else
            {
                ifs = new BarnsleyFern(A);

                ifs.GeneratePoints(depth);
                ifs_cache.TryAdd(key, ifs);
            }
        }

        private static void LoadSierpinskiTriangle(int depth, Complex A, Complex B, Complex C, out IIteratedFunctionSystem ifs)
        {
            string key = $"sierpinski_triangle_{A.Real}_{A.Imaginary}_{B.Real}_{B.Imaginary}_{C.Real}_{C.Imaginary}";
            if (ifs_cache.ContainsKey(key)) { ifs = ifs_cache[key]; }
            else
            {
                // ifs = new SierpinskiTriangle(new Complex(0, 0), new Complex(1, 0), new Complex(0, 1));
                // ifs = new SierpinskiTriangle(new Complex(0, 2*Math.Sqrt(3)/3), new Complex(-1/2, -Math.Sqrt(3)/3), new Complex(1/2, -Math.Sqrt(3)/3));
                ifs = new SierpinskiTriangle(A, B, C);

                ifs.GeneratePoints(depth);
                ifs_cache.TryAdd(key, ifs);
            }
        }
    }
}
