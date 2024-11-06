using Modulartistic.Common;
using N = System.Numerics;

namespace Modulartistic.AddOns.Complex
{
    [AddOn]
    public static class ComplexFunctions
    {
        #region conversion functions

        public static N.Complex Complex(object obj)
        {
            switch (obj)
            {
                case null:
                    return N.Complex.NaN;

                case N.Complex c:
                    return c;

                case double d:
                    return new N.Complex(d, 0);
                case float f:
                    return new N.Complex(f, 0);

                case char c:
                    return new N.Complex(c, 0);
                case short s:
                    return new N.Complex(s, 0);
                case ushort us:
                    return new N.Complex(us, 0);

                case byte b:
                    return new N.Complex(b, 0);
                case sbyte sb:
                    return new N.Complex(sb, 0);

                case int i:
                    return new N.Complex(i, 0);
                case uint ui:
                    return new N.Complex(ui, 0);

                case long l:
                    return new N.Complex(l, 0);
                case ulong ul:
                    return new N.Complex(ul, 0);

                case decimal dec:
                    return new N.Complex((double)dec, 0);

                default:
                    return N.Complex.NaN;
            }
        }

        public static N.Complex Complex(double re, double im)
        {
            return new N.Complex(re, im);
        }

        public static N.Complex Complex(object re, object im)
        {
            N.Complex re_ = Complex(re);
            N.Complex im_ = Complex(im);

            return re_ + im_ * N.Complex.ImaginaryOne;
        }

        public static N.Complex ComplexFromPolar(double magnitude, double phase)
        {
            return N.Complex.FromPolarCoordinates(magnitude, phase);
        }

        #endregion

        #region Basic algebraic operations

        public static N.Complex cAdd(object a, object b)
        {
            return N.Complex.Add(Complex(a), Complex(b));
        }

        public static N.Complex cSubtract(object a, object b)
        {
            return N.Complex.Subtract(Complex(a), Complex(b));
        }

        public static N.Complex cMultiply(object a, object b)
        {
            return N.Complex.Multiply(Complex(a), Complex(b));
        }

        public static N.Complex cDivide(object a, object b)
        {
            return N.Complex.Divide(Complex(a), Complex(b));
        }

        #endregion

        public static double cReal(object a)
        {
            return Complex(a).Real;
        }

        public static double cImaginary(object a)
        {
            return Complex(a).Imaginary;
        }

        public static double cMagnitude(object a)
        {
            return Complex(a).Magnitude;
        }

        public static double cPhase(object a)
        {
            return Complex(a).Phase;
        }

        public static N.Complex cPow(object a, object b)
        {
            return N.Complex.Pow(Complex(a), Complex(b));
        }

        public static N.Complex cExp(object a)
        {
            return N.Complex.Exp(Complex(a));
        }

        public static N.Complex cConjugate(object a)
        {
            return N.Complex.Conjugate(Complex(a));
        }

        public static N.Complex cAbs(object a)
        {
            return N.Complex.Abs(Complex(a));
        }

        public static N.Complex cNegate(object a)
        {
            return N.Complex.Negate(Complex(a));
        }

        public static N.Complex cLog(object a)
        {
            return N.Complex.Log(Complex(a));
        }

        public static N.Complex cLog10(object a)
        {
            return N.Complex.Log10(Complex(a));
        }

        public static N.Complex cMaxMagnitude(object a, object b)
        {
            return N.Complex.MaxMagnitude(Complex(a), Complex(b));
        }

        public static N.Complex cMinMagnitude(object a, object b)
        {
            return N.Complex.MinMagnitude(Complex(a), Complex(b));
        }

        public static N.Complex cReciprocal(object a)
        {
            return N.Complex.Reciprocal(Complex(a));
        }

        public static N.Complex cSqrt(object a)
        {
            return N.Complex.Sqrt(Complex(a));
        }

        #region trigonometric functions

        public static N.Complex cSin(object a)
        {
            return N.Complex.Sin(Complex(a));
        }

        public static N.Complex cCos(object a)
        {
            return N.Complex.Cos(Complex(a));
        }

        public static N.Complex cTan(object a)
        {
            return N.Complex.Tan(Complex(a));
        }

        public static N.Complex cSinh(object a)
        {
            return N.Complex.Sinh(Complex(a));
        }

        public static N.Complex cCosh(object a)
        {
            return N.Complex.Cosh(Complex(a));
        }

        public static N.Complex cTanh(object a)
        {
            return N.Complex.Tanh(Complex(a));
        }

        public static N.Complex cAsin(object a)
        {
            return N.Complex.Asin(Complex(a));
        }

        public static N.Complex cAcos(object a)
        {
            return N.Complex.Acos(Complex(a));
        }

        public static N.Complex cAtan(object a)
        {
            return N.Complex.Atan(Complex(a));
        }

        #endregion


    }
}
