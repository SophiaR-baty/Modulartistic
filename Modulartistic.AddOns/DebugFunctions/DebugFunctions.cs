using System;

namespace DebugFunctions
{
    public static class DebugFunctions
    {
        public static double Debug(double x, double y, double th, double n)
        {
            if (x < th / 2 && x > -th / 2) { return y < 0 ? 0 : 0.5; }
            if (y < th / 2 && y > -th / 2) { return y < 0 ? 0.25 : 0.75; }
            if (Math.Abs(x - Math.Round(x / n) * n) < th && Math.Abs(y - Math.Round(y / n) * n) < th) { return x*y; }

            return double.NaN;
        }
    }
}