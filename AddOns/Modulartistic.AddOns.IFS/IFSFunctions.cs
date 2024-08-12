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
        private static ConcurrentDictionary<string, IteratedFunctionSystem> ifs_cache;

        public static void Initialize(AddOnInitializationArgs args)
        {
            ifs_cache = new ConcurrentDictionary<string, IteratedFunctionSystem>();
        }

        public static double BarnsleyFern(double x, double y, int depth, double tolerance)
        {

            IteratedFunctionSystem barnsleyFern;
            Complex xy = new Complex(x, y);
            LoadBarnsleyFern(depth, out barnsleyFern);

            for (int i = 0; i < barnsleyFern.Points.Count; i++)
            {
                Complex z = barnsleyFern.Points[i];
                if (Complex.Abs(z - xy) <= tolerance)
                {
                    return i;
                }
            }

            return double.NaN;
        }

        private static Func<Complex, Complex> Transform(double a, double b, double c, double d, double e, double f)
        {
            return z => new Complex(a * z.Real + b * z.Imaginary + e, c * z.Real + d * z.Imaginary + f);
        }

        private static WeightedListItem<Func<Complex, Complex>> GetWeightedTransform(double a, double b, double c, double d, double e, double f, float p)
        {
            return new WeightedListItem<Func<Complex, Complex>>(Transform(a, b, c, d, e, f), p);
        }

        private static void LoadBarnsleyFern(int depth, out IteratedFunctionSystem ifs)
        {
            string key = "barnsley_fern";
            if (ifs_cache.ContainsKey(key)) { ifs = ifs_cache[key]; }
            else
            {
                ifs = new IteratedFunctionSystem();

                ifs.Functions.Add(GetWeightedTransform(0, 0, 0, 0.16, 0, 0, 0.01f));
                ifs.Functions.Add(GetWeightedTransform(0.85, 0.04, -0.04, 0.85, 0, 1.6, 0.85f));
                ifs.Functions.Add(GetWeightedTransform(0.20, -0.26, 0.23, 0.22, 0, 1.6, 0.07f));
                ifs.Functions.Add(GetWeightedTransform(-0.15, 0.28, 0.26, 0.24, 0, 0.44, 0.07f));

                ifs.GeneratePoints(depth);

                if (ifs_cache.Count > 3) { ifs_cache.Clear(); }
                ifs_cache.TryAdd(key, ifs);
            }
        }
    }
}
