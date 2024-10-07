using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulartistic.Common;

namespace Modulartistic.AddOns.Debug
{
    [AddOn]
    public static class DebugFunctions
    {
        public static double Debug(double x, double y, double th, double n)
        {
            if (x < th / 2 && x > -th / 2) { return y < 0 ? 0 : 0.5; }
            if (y < th / 2 && y > -th / 2) { return x < 0 ? 0.25 : 0.75; }
            if (Math.Abs(x - Math.Round(x / n) * n) < th && Math.Abs(y - Math.Round(y / n) * n) < th) { return x * y; }

            return double.NaN;
        }

        public static double DebugCircle(double x, double y, double x_0, double y_0, double r, double th)
        {
            double circ_calc = (x - x_0) * (x - x_0) + (y - y_0) * (y - y_0);
            if (circ_calc > (r - th / 2) * (r - th / 2) && circ_calc < (r + th / 2) * (r + th / 2)) { return 0; }
            if (x > x_0 - th / 2 && x < x_0 + th / 2 && y > y_0 - th / 2 && y < y_0 + th / 2) { return 0; }
            return double.NaN;
        }
    }
}
