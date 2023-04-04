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

        public static double Juliaset(double z_x, double z_y, double c_x, double c_y, int depth)
        {
            Complex z_0 = new Complex(z_x, z_y);
            Complex c = new Complex(c_x, c_y);
            Complex z = z_0;

            int i = 0;
            while (i < depth)
            {
                z = Complex.Pow(z, 2) + c;
                double a = Complex.Abs(z);
                if (a > 2)
                    return Double.NaN;
                i++;
            }
            return Complex.Abs(z);
        }

        public static double Mod(double d1, double d2)
        {
            if (d2 <= 0)
                throw new DivideByZeroException();
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }
    }
}
