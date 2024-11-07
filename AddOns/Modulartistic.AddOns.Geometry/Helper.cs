using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.Geometry
{
    internal static class Helper
    {
        public static double KahanSum(double[] values)
        {
            // implements the Kahan summation algorithm
            // https://en.wikipedia.org/wiki/Kahan_summation_algorithm

            double sum = 0.0;
            double c = 0.0;
            for (int i = 0; i < values.Length; i++)
            {
                double y = values[i] - c;
                double t = sum + y;
                c = (t - sum) - y;
                sum = t;
            }

            return sum;
        }
    }
}
