using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using MathNet.Numerics.Random;

namespace MathFunctions
{
    public static class MathFunctions
    {
        public static double Tanh(double x)
        {
            return Math.Tanh(x);
        }
        
        public static double Circle(double x, double y)
        {
            return x*x+y*y;
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

        public static double Ngon(double x, double y, double n)
        {
            double result = Math.Sqrt(Circle(x, y));

            double numerator = Math.Sin((n - 2) / (2 * n) * Math.PI);
            double denominator = Math.Sin(
                Math.PI - (
                2 * Math.PI / n * Math.Atan(
                Math.Tan(
                n / (2 * Math.PI) * ((
                Math.Pow(Math.Sqrt(x * x) / x, 2) * Math.Atan(y / x) +
                (1 - (Math.Sqrt(x * x) / x)) / 2 *
                (1 + Math.Sqrt(y * y) / y - Math.Pow(Math.Sqrt(y * y) / y, 2)) * Math.PI) 
                + Math.PI / n) * Math.PI))/Math.PI + Math.PI/n) - Math.PI*(n-2)/(2*n));
            result -= numerator/denominator; 
            return result;
        }

        public static double Gamma(double a)
        {
            return MathNet.Numerics.SpecialFunctions.Gamma(a);
        }

        public static double GammaLn(double a)
        {
            return MathNet.Numerics.SpecialFunctions.GammaLn(a);
        }

        public static double Binomial(double a, double b)
        {
            return Gamma(a-1) / Gamma(b-1) / Gamma(a-b-1);
        }

        public static double Harmonic(double a)
        {
            return MathNet.Numerics.SpecialFunctions.Harmonic((int)a);
        }

        public static double GeneralHarmonic(double a, double b)
        {
            return MathNet.Numerics.SpecialFunctions.GeneralHarmonic((int)a, (int)b);
        }

        public static double BesselI0(double a)
        {
            return MathNet.Numerics.SpecialFunctions.BesselI0(a);
        }

        public static double ModIFS1(double x, double y, int depth)
        {
            double new_x = x;
            double new_y = y;
            
            for (int i = 0; i < depth; i++)
            {
                new_x += Mod(new_x, Math.Abs(new_y));
                new_y += Mod(new_y, Math.Abs(new_x));
            }
            return Math.Sqrt(new_x*new_x+new_y*new_y);
        }

        public static double ModIFS2(double x, double y, int depth)
        {
            double new_x = x;
            double new_y = y;

            for (int i = 0; i < depth; i++)
            {
                new_x += Mod(new_x, Math.Abs(new_y));
                new_y += Mod(new_y, Math.Abs(new_x));
            }
            return 180 * Math.Atan2(new_y, new_x) / Math.PI;
        }

        public static double RandomNumber(double min, double max)
        {
            Random r = new Random();
            return min + r.NextDouble()*(max-min);
        }

        public static double Bezier(double x, double y, double p0_x, double p0_y, double p1_x, double p1_y, double p2_x, double p2_y, double p3_x, double p3_y)
        {
            const double STEP = 0.001;
            // const double TOLERANCE = 0.5;
            
            Func<double, double> xFunc = (t) => Math.Pow(1 - t, 3) * p0_x + 3 * (1 - t) * (1 - t) * t * p1_x + 3 * (1 - t) * t * t * p2_x + Math.Pow(t, 3) * p3_x;
            Func<double, double> yFunc = (t) => Math.Pow(1 - t, 3) * p0_y + 3 * (1 - t) * (1 - t) * t * p1_y + 3 * (1 - t) * t * t * p2_y + Math.Pow(t, 3) * p3_y;
            // Func<double, double> bezierAngleFunc = (t) => Math.Atan2(yFunc(t), xFunc(t)); // from -180 to 180
            // Func<double, double> pointBezierAngleFunc = (t) => Math.Atan2(y-yFunc(t), x-xFunc(t)); // from -180 to 180
            Func<double, double> distanceFunc = (t) => Math.Sqrt(Math.Pow(x - xFunc(t), 2) + Math.Pow(y - yFunc(t), 2));
            // Func<double, double> gradFunc = (t) => Math.Sqrt(Math.Pow(xFunc(t), 2) + Math.Pow(yFunc(t), 2));

            double minDistance = double.MaxValue;
            for (double t = 0; t < 1; t += STEP)
            {
                double distance = distanceFunc(t);
                if (distance < minDistance) { minDistance = distance; }
            }

            return minDistance;
        }



        public static double Mod(double d1, double d2)
        {
            if (d2 <= 0)
                throw new DivideByZeroException();
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }

        public static double A004738(double x)
        {
            double z = Math.Floor(Math.Sqrt(x) + 1.0 / 2);
            return z + 1 - Math.Abs(x - 1 - z * z);
        }
    }
}
