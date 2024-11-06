using System.Globalization;
using System.Numerics;
using Antlr4.Runtime.Misc;
using NCalc;
using Modulartistic.Common;

namespace Modulartistic.AddOns.Fractals
{
    [AddOn]
    public static class FractalFunctions
    {
        #region initialization

        private static string[] _addOns;
        private static AddOnInitializationArgs _initArgs;

        public static void Initialize(AddOnInitializationArgs args)
        {
            _addOns = args.AddOns;
            _initArgs = args;
        }

        #endregion

        #region Mandelbrot
        public static double Mandelbrot(double x, double y, int depth = 10, double escape = 2)
        {
            Complex z_0 = new Complex(x, y);
            Complex z = 0;

            int i = 0;
            while (i < depth)
            {
                z = Complex.Pow(z, 2) + z_0;
                double a = Complex.Abs(z);
                if (a > escape)
                    return double.NaN;
                i++;
            }
            return Complex.Abs(z);
        }

        public static double MandelbrotIdx(double x, double y, int depth = 10, double escape = 2)
        {
            Complex z_0 = new Complex(x, y);
            Complex z = 0;

            int i = 0;
            while (i < depth)
            {
                z = Complex.Pow(z, 2) + z_0;
                double a = Complex.Abs(z);
                if (a > escape)
                    return i;
                i++;
            }
            return i;
        }

        #endregion

        #region Juliaset
        public static double Juliaset(double z_x, double z_y, double c_x, double c_y, int depth = 10, double escape = 2)
        {
            Complex z_0 = new Complex(z_x, z_y);
            Complex c = new Complex(c_x, c_y);
            Complex z = z_0;

            int i = 0;
            while (i < depth)
            {
                z = Complex.Pow(z, 2) + c;
                double a = Complex.Abs(z);
                if (a > escape)
                    return Double.NaN;
                i++;
            }
            return Complex.Abs(z);
        }

        public static double JuliasetIdx(double z_x, double z_y, double c_x, double c_y, int depth = 10, double escape = 2)
        {
            Complex z_0 = new Complex(z_x, z_y);
            Complex c = new Complex(c_x, c_y);
            Complex z = z_0;

            int i = 0;
            while (i < depth)
            {
                z = Complex.Pow(z, 2) + c;
                double a = Complex.Abs(z);
                if (a > escape)
                    return i;
                i++;
            }
            return i;
        }
        #endregion

        #region BurningShip
        public static double BurningShip(double x, double y, int depth = 10, double escape = 4)
        {
            double zx = x;
            double zy = y;
            int i = 0;

            while (i < depth)
            {
                double xtemp = zx*zx - zy*zy + x;
                zy = Math.Abs(2*zx*zy) + y;
                zx = xtemp;

                if (zx*zx + zy*zy > escape)
                    return double.NaN;

                i++;
            }

            return zx * zx + zy * zy;
        }

        public static double BurningShipIdx(double x, double y, int depth = 10, double escape = 4)
        {
            double zx = x;
            double zy = y;
            int i = 0;

            while (i < depth)
            {
                double xtemp = zx * zx - zy * zy + x;
                zy = Math.Abs(2 * zx * zy) + y;
                zx = xtemp;

                if (zx * zx + zy * zy > escape)
                    return i;

                i++;
            }

            return i;
        }
        #endregion

        #region Newton
        public static double Newton(double x, double y, string f, string df, int depth = 10, double tolerance = 0.1)
        {
            ExtendedExpression _f = GetComplexExpression(f);
            ExtendedExpression _df = GetComplexExpression(df);
            Complex z = new Complex(x, y);

            int i = 0;
            while (i < depth)
            {
                _f.RegisterParameter("z", z);
                _df.RegisterParameter("z", z);

                Complex fz = ConvertToComplex(_f.Evaluate());
                Complex dfz = ConvertToComplex(_df.Evaluate());

                if (dfz == Complex.Zero) { break; }
                Complex zNext = z - fz / dfz;
                
                if ((zNext-z).Magnitude < tolerance) { return zNext.Magnitude; }
                z = zNext;
                i++;
            }
            return double.NaN;
        }

        public static double NewtonNoNaN(double x, double y, string f, string df, int depth = 10, double tolerance = 0.1)
        {
            ExtendedExpression _f = GetComplexExpression(f);
            ExtendedExpression _df = GetComplexExpression(df);
            Complex z = new Complex(x, y);

            int i = 0;
            while (i < depth)
            {
                _f.RegisterParameter("z", z);
                _df.RegisterParameter("z", z);

                Complex fz = (Complex)Convert.ChangeType(_f.Evaluate(), typeof(Complex));
                Complex dfz = (Complex)Convert.ChangeType(_df.Evaluate(), typeof(Complex));

                if (dfz == Complex.Zero) { break; }
                Complex zNext = z - fz / dfz;

                if ((zNext - z).Magnitude < tolerance) { return zNext.Magnitude; }
                z = zNext;
                i++;
            }
            return z.Magnitude;
        }

        public static double NewtonIdx(double x, double y, string f, string df, int depth = 10, double tolerance = 0.1)
        {
            ExtendedExpression _f = GetComplexExpression(f);
            ExtendedExpression _df = GetComplexExpression(df);
            Complex z = new Complex(x, y);

            int i = 0;
            while (i < depth)
            {
                _f.RegisterParameter("z", z);
                _df.RegisterParameter("z", z);

                Complex fz = ConvertToComplex(_f.Evaluate());
                Complex dfz = ConvertToComplex(_df.Evaluate());

                if (dfz == Complex.Zero) { break; }
                Complex zNext = z - fz / dfz;

                if ((zNext - z).Magnitude < tolerance) { return i; }
                z = zNext;
                i++;
            }
            return i;
        }
        #endregion

        #region escape time
        public static double EscapeTime(double x, double y, string func, int depth = 10, double escape = 2)
        {
            ExtendedExpression f = GetComplexExpression(func);
            Complex z = new Complex(x, y);

            int i = 0;
            while (i < depth)
            {
                f.RegisterParameter("z", z);
                z = ConvertToComplex(f.Evaluate());

                double a = Complex.Abs(z);
                if (a > escape)
                    return double.NaN;
                i++;
            }
            return Complex.Abs(z);
        }

        public static double EscapeTimeIdx(double x, double y, string func, int depth = 10, double escape = 2)
        {
            ExtendedExpression f = GetComplexExpression(func);
            Complex z = new Complex(x, y);

            int i = 0;
            while (i < depth)
            {
                f.RegisterParameter("z", z);
                z = ConvertToComplex(f.Evaluate());

                double a = Complex.Abs(z);
                if (a > escape)
                    return i;
                i++;
            }
            return i;
        }

        public static double EscapeTimeNoNaN(double x, double y, string func, int depth = 10, double escape = 2)
        {
            ExtendedExpression f = GetComplexExpression(func);
            Complex z = new Complex(x, y);

            int i = 0;
            while (i < depth)
            {
                f.RegisterParameter("z", z);
                z = ConvertToComplex(f.Evaluate());

                double a = Complex.Abs(z);
                if (a > escape)
                    return a;
                i++;
            }
            return Complex.Abs(z);
        }
        #endregion

        #region public helper functions
        public static string NumToString(object obj)
        {
            if (obj is decimal || obj is double || obj is float || obj is int || obj is long || obj is short || obj is uint || obj is ulong || obj is ushort)
            {
                return Convert.ToDecimal(obj).ToString("F", CultureInfo.InvariantCulture);
            }

            return obj.ToString();
        }
        #endregion



        #region private Helpers
        private static ExtendedExpression GetComplexExpression(string expr)
        {
            ExtendedExpression _expr = new ExtendedExpression(expr);
            _expr.LoadAddOns(_initArgs);
            return _expr;
        }

        private static Complex ConvertToComplex(object obj)
        {
            switch (obj)
            {
                case int i:
                    return new Complex(i, 0);
                case double d:
                    return new Complex(d, 0);
                case decimal dec:
                    return new Complex((double)dec, 0);
                case Complex c:
                    return c;
                case float f:
                    return new Complex(f, 0);
                case long l:
                    return new Complex(l, 0);
                default:
                    throw new ArgumentException("Unsupported type");
            }
        }
        #endregion
    }
}
