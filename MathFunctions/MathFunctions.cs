using System;
using System.Numerics;

namespace MathFunctions
{
    public static class MathFunctions
    {
        public static double Circle(double x, double y)
        {
            return Math.Pow(x, 2) + Math.Pow(y, 2);
        }

        public static double Product(double x, double y)
        {
            return x * y;
        }

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
    }
}
