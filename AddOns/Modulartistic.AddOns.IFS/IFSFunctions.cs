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

        public static double BarnsleyFern(double x, double y, int depth, double tolerance)
        {

            IIteratedFunctionSystem barnsleyFern;
            Complex xy = new Complex(x, y);
            LoadBarnsleyFern(depth, out barnsleyFern);

            for (int i = 0; i < barnsleyFern.Points.Count; i++)
            {
                foreach (Complex c in barnsleyFern.Points[i])
                {
                    if (Complex.Abs(c - xy) <= tolerance)
                    {
                        return i;
                    }
                }
            }

            return double.NaN;
        }

        public static double BarnsleyFern(double x, double y, int depth)
        {

            IIteratedFunctionSystem sierpinski;
            Complex xy = new Complex(x, y);
            LoadBarnsleyFern(depth, out sierpinski);

            double minDistance = double.PositiveInfinity;

            foreach (var clist in sierpinski.Points)
            {
                foreach (var c in clist)
                {
                    double dist = Complex.Abs(c - xy);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                    }
                }
            }

            return minDistance;
        }



        public static double SierpinskiTriangle(double x, double y, int depth, double tolerance)
        {

            IIteratedFunctionSystem sierpinski;
            Complex xy = new Complex(x, y);
            LoadSierpinskiTriangle(depth, out sierpinski);

            for (int i = 0; i < sierpinski.Points.Count; i++)
            {
                foreach (Complex c in sierpinski.Points[i])
                {
                    if (Complex.Abs(c - xy) <= tolerance)
                    {
                        return i;
                    }
                }
            }

            return double.NaN;
        }

        public static double SierpinskiTriangle(double x, double y, int depth)
        {

            IIteratedFunctionSystem sierpinski;
            Complex xy = new Complex(x, y);
            LoadSierpinskiTriangle(depth, out sierpinski);

            double minDistance = double.PositiveInfinity;

            foreach (var clist in sierpinski.Points)
            {
                foreach (var c in clist)
                {
                    double dist = Complex.Abs(c - xy);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                    }
                }
            }

            return minDistance;
        }




        private static Func<Complex, Complex> Transform(double a, double b, double c, double d, double e, double f)
        {
            return z => new Complex(a * z.Real + b * z.Imaginary + e, c * z.Real + d * z.Imaginary + f);
        }

        private static WeightedListItem<Func<Complex, Complex>> GetWeightedTransform(double a, double b, double c, double d, double e, double f, float p)
        {
            return new WeightedListItem<Func<Complex, Complex>>(Transform(a, b, c, d, e, f), p);
        }

        private static void LoadBarnsleyFern(int depth, out IIteratedFunctionSystem ifs)
        {
            string key = "barnsley_fern";
            if (ifs_cache.ContainsKey(key)) { ifs = ifs_cache[key]; }
            else
            {
                ifs = new BarnsleyFern(new Complex(0, 0));

                ifs.GeneratePoints(depth);
                ifs_cache.TryAdd(key, ifs);
            }
        }

        private static void LoadSierpinskiTriangle(int depth, out IIteratedFunctionSystem ifs)
        {
            string key = "sierpinski_triangle";
            if (ifs_cache.ContainsKey(key)) { ifs = ifs_cache[key]; }
            else
            {
                // ifs = new SierpinskiTriangle(new Complex(0, 0), new Complex(1, 0), new Complex(0, 1));
                ifs = new SierpinskiTriangle(new Complex(0, 2*Math.Sqrt(3)/3), new Complex(-1/2, -Math.Sqrt(3)/3), new Complex(1/2, -Math.Sqrt(3)/3));

                ifs.GeneratePoints(depth);
                ifs_cache.TryAdd(key, ifs);
            }
        }
    }
}
