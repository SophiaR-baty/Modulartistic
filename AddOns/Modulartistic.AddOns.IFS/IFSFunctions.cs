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

        public static double BarnsleyFern(double x, double y, Complex initial, int depth, double tolerance)
        {

            IIteratedFunctionSystem fern;
            Complex xy = new Complex(x, y);
            LoadBarnsleyFern(depth, initial, out fern);

            for (int i = 0; i < fern.Points.Count; i++)
            {
                Complex c = fern.Points[i];
                if (Complex.Abs(c - xy) <= tolerance)
                {
                    return i;
                }
            }

            return double.NaN;
        }

        public static double BarnsleyFern(double x, double y, Complex initial, int depth)
        {

            IIteratedFunctionSystem fern;
            Complex xy = new Complex(x, y);
            LoadBarnsleyFern(depth, initial, out fern);

            double minDistance = double.PositiveInfinity;

            foreach (var c in fern.Points)
            {
                double dist = Complex.Abs(c - xy);
                if (dist < minDistance)
                {
                    minDistance = dist;
                }
            }

            return minDistance;
        }



        public static double SierpinskiTriangle(double x, double y, Complex A, Complex B, Complex C, int depth, double tolerance)
        {

            IIteratedFunctionSystem sierpinski;
            Complex xy = new Complex(x, y);
            LoadSierpinskiTriangle(depth, A, B, C, out sierpinski);

            for (int i = 0; i < sierpinski.Points.Count; i++)
            {
                Complex c = sierpinski.Points[i];
                if (Complex.Abs(c - xy) <= tolerance)
                {
                    return i;
                }
            }

            return double.NaN;
        }

        public static double SierpinskiTriangle(double x, double y, Complex A, Complex B, Complex C, int depth)
        {

            IIteratedFunctionSystem sierpinski;
            Complex xy = new Complex(x, y);
            LoadSierpinskiTriangle(depth, A, B, C, out sierpinski);

            double minDistance = double.PositiveInfinity;

            foreach (var c in sierpinski.Points)
            {

                double dist = Complex.Abs(c - xy);
                if (dist < minDistance)
                {
                    minDistance = dist;
                }
            }

            return minDistance;
        }

        public static double InvertedSierpinskiTriangle(double x, double y, Complex A, Complex B, Complex C, int depth)
        {

            IIteratedFunctionSystem sierpinski;
            Complex xy = new Complex(x, y);
            LoadSierpinskiTriangle(depth, A, B, C, out sierpinski);

            double maxDistance = 0;

            foreach (var c in sierpinski.Points)
            {

                double dist = Complex.Abs(c - xy);
                if (dist > maxDistance)
                {
                    maxDistance = dist;
                }
            }

            return maxDistance;
        }

        public static Complex Point(double x, double y)
        {
            return new Complex(x, y);
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
