using NCalc;
using System;
using System.Linq;
using System.Reflection.Metadata;

namespace Modulartistic
{
    public class Easing
    {
        // based on https://easings.net/

        #region Fields
        private Func<double, double, double, double, double> easingFunction;
        private Expression easingExpression;
        private string name;
        
        private static string[] types = new string[]
        {
            "Linear",

            "SineIn",
            "SineOut",
            "SineInOut",

            "ElasticIn",
            "ElasticOut",
            "ElasticInOut",

            "BounceOut"
        };
        #endregion

        #region Properties
        public string Type { get => name; }

        public static string[] ImplementedEasingTypes { get => types; }
        #endregion

        #region Constructors
        private Easing(Func<double, double, double, double, double> f, string name)
        {
            easingFunction = f;
            this.name = name;
        }
        #endregion

        public static Easing FromString(string type)
        {
            if (!ImplementedEasingTypes.Contains(type)) { throw new NotImplementedException(); }
            
            if (type == "Linear") { return Linear(); }

            if (type == "SineIn") { return SineIn(); }
            if (type == "SineOut") { return SineOut(); }
            if (type == "SineInOut") { return SineInOut(); }

            if (type == "ElasticIn") { return ElasticIn(); }
            if (type == "ElasticOut") { return ElasticOut(); }
            if (type == "ElasticInOut") { return ElasticInOut(); }

            if (type == "BounceOut") { return BounceOut(); }

            throw new NotImplementedException();
        }

        public double Ease(double start, double end, int idx, int maxIdx)
        {
            return easingFunction(start, end, idx, maxIdx);
        }

        public static Easing Linear()
        {
            Func<double, double, double, double, double> f = (start, end, idx, maxIdx) => start + (end - start) * idx / maxIdx;
            return new Easing(f, "Linear");
        }

        public static Easing SineIn()
        {
            Func<double, double, double, double, double> f = (start, end, idx, maxIdx) => start + (end - start) * (1 - Math.Cos(Math.PI * idx / maxIdx / 2));
            return new Easing(f, "SineIn");
        }

        public static Easing SineOut()
        {
            Func<double, double, double, double, double> f = (start, end, idx, maxIdx) => start + (end - start) * (Math.Sin(Math.PI * idx / maxIdx / 2));
            return new Easing(f, "SineOut");
        }

        public static Easing SineInOut()
        {
            Func<double, double, double, double, double> f = (start, end, idx, maxIdx) => start + (end - start) * (-(Math.Cos(Math.PI * idx / maxIdx) - 1) / 2);
            return new Easing(f, "SineInOut");
        }

        public static Easing ElasticIn()
        {
            Func<double, double, double, double, double> f = (start, end, idx, maxIdx) => start + (end - start) * (-Math.Pow(2, 10 * idx / maxIdx - 10) * Math.Sin((idx / maxIdx * 10 - 10.75) * (2 * Math.PI) / 3));
            return new Easing(f, "ElasticIn");
        }

        public static Easing ElasticOut()
        {
            Func<double, double, double, double, double> f = (start, end, idx, maxIdx) => start + (end - start) * (Math.Pow(2, -10 * idx / maxIdx) * Math.Sin((idx / maxIdx * 10 - 0.75) * (2 * Math.PI) / 3) + 1);
            return new Easing(f, "ElasticOut");
        }

        public static Easing ElasticInOut()
        {
            Func<double, double, double, double, double> f = (start, end, idx, maxIdx) => start + (end - start) * (Math.Pow(2, -10 * idx / maxIdx) * Math.Sin((idx / maxIdx * 10 - 0.75) * (2 * Math.PI) / 3) + 1);
            return new Easing(f, "ElasticInOut");
        }

        public static Easing BounceOut()
        {
            Func<double, double, double, double, double> f = (start, end, idx, maxIdx) => start + (end - start) * BounceOut_Function(idx/maxIdx);
            return new Easing(f, "BounceOut");
        }

        private static double BounceOut_Function(double x)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x -= 1.5 / d1) * x + 0.75;
            }
            else if (x < 2.5 / d1)
            {
                return n1 * (x -= 2.25 / d1) * x + 0.9375;
            }
            else
            {
                return n1 * (x -= 2.625 / d1) * x + 0.984375;
            }
        }
    }
}