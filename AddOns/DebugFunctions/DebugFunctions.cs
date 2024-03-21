using System;

namespace DebugFunctions
{
    public static class DebugFunctions
    {
        public static double Debug(double x, double y, double th, double n)
        {
            if (x < th / 2 && x > -th / 2) { return y < 0 ? 0 : 0.5; }
            if (y < th / 2 && y > -th / 2) { return x < 0 ? 0.25 : 0.75; }
            if (Math.Abs(x - Math.Round(x / n) * n) < th && Math.Abs(y - Math.Round(y / n) * n) < th) { return x*y; }

            return double.NaN;
        }

        public static double DebugCircle(double x, double y, double x_0, double y_0, double r, double th)
        {
            double circ_calc = (x - x_0) * (x - x_0) + (y - y_0) * (y - y_0);
            if (circ_calc > (r - th / 2) * (r - th / 2) && circ_calc < (r + th / 2) * (r + th / 2)) { return 0; }
            if (x > x_0 - th/2 && x < x_0 + th / 2 && y > y_0 - th / 2 && y < y_0 + th / 2) { return 0; }
            return double.NaN;
        }

        public static int IntProd1(int i1, int i2)
        {
            return i1*i2;
        }

        public static double IntProd2(int i1, int i2)
        {
            return i1 * i2;
        }

        public static int IntProd3(double i1, double i2)
        {
            return (int)(i1 * i2);
        }

        public static int ToInt(double d) { return (int)d; }
        public static double ToDouble(int i) { return i; }
    }
}