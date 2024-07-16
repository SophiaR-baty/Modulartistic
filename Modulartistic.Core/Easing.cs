using NCalc;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Schema;

namespace Modulartistic.Core
{
    /// <summary>
    /// Class implementing an Easing effect for a variable, given a start value, end value, index and maxIndex <br/>
    /// Based on https://easings.net/
    /// </summary>
    public class Easing
    {
        #region Fields
        /// <summary>
        /// the actual function used for easing
        /// </summary>
        private Func<double, double, double, double> _easingFunction;

        /// <summary>
        /// the easing type
        /// </summary>
        private EasingType _type;
        #endregion

        #region Properties
        /// <summary>
        /// Get the EasingType of this Easing
        /// </summary>
        public EasingType Type { get => _type; }

        #endregion

        #region Constructors
        /// <summary>
        /// Crreates a new Easing object using the specified easing function
        /// </summary>
        /// <param name="f">the function to use for easing</param>
        /// <param name="type">the EasingType - can only be set here</param>
        private Easing(Func<double, double, double, double> f, EasingType type)
        {
            _easingFunction = f;
            _type = type;
        }
        #endregion

        #region static Getters for specific Easing Types
        /// <summary>
        /// Get an Easing object of a specific Easing type. <br/>
        /// For more informations about easing types refer to https://easings.net/ all easing types from that website are fully implemented
        /// </summary>
        /// <param name="type">EasingType</param>
        /// <returns>new Easing object</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Easing FromType(EasingType type)
        {
            switch (type)
            {
                // simple
                case EasingType.Linear: return Linear();
                case EasingType.Multiplicative: return Multiplicative();

                // sine
                case EasingType.SineIn: return SineIn();
                case EasingType.SineOut: return SineOut();
                case EasingType.SineInOut: return SineInOut();

                // quadratic
                case EasingType.QuadIn: return QuadIn();
                case EasingType.QuadOut: return QuadOut();
                case EasingType.QuadInOut: return QuadInOut();

                // cubic
                case EasingType.CubicIn: return CubicIn();
                case EasingType.CubicOut: return CubicOut();
                case EasingType.CubicInOut: return CubicInOut();

                // quartic
                case EasingType.QuartIn: return QuartIn();
                case EasingType.QuartOut: return QuartOut();
                case EasingType.QuartInOut: return QuartInOut();

                // quintic
                case EasingType.QuintIn: return QuintIn();
                case EasingType.QuintOut: return QuintOut();
                case EasingType.QuintInOut: return QuintInOut();

                // exponential
                case EasingType.ExpoIn: return ExpoIn();
                case EasingType.ExpoOut: return ExpoOut();
                case EasingType.ExpoInOut: return ExpoInOut();

                // circle
                case EasingType.CircIn: return CircIn();
                case EasingType.CircOut: return CircOut();
                case EasingType.CircInOut: return CircInOut();

                // back
                case EasingType.BackIn: return BackIn();
                case EasingType.BackOut: return BackOut();
                case EasingType.BackInOut: return BackInOut();

                // elastic
                case EasingType.ElasticIn: return ElasticIn();
                case EasingType.ElasticOut: return ElasticOut();
                case EasingType.ElasticInOut: return ElasticInOut();
                
                // bounce
                case EasingType.BounceIn: return BounceIn();
                case EasingType.BounceOut: return BounceOut();
                case EasingType.BounceInOut: return BounceInOut();

                default: throw new NotImplementedException();
            }
        }

        #region basic easings
        /// <summary>
        /// Get an Easing object with linear easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing Linear()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * x;
            return new Easing(f, EasingType.Linear);
        }

        /// <summary>
        /// Get an Easing object with multiplicative easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing Multiplicative()
        {
            Func<double, double, double, double> f = (start, end, x) =>
            {
                if (start == 0) { return 0; }
                return start * Math.Pow(end / start, x);
            };
            return new Easing(f, EasingType.Multiplicative);
        }
        #endregion

        #region sine easings
        /// <summary>
        /// Get an Easing object with sineIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing SineIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (1 - Math.Cos(Math.PI * x / 2));
            return new Easing(f, EasingType.SineIn);
        }

        /// <summary>
        /// Get an Easing object with sineOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing SineOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * Math.Sin(Math.PI * x / 2);
            return new Easing(f, EasingType.SineOut);
        }

        /// <summary>
        /// Get an Easing object with sineInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing SineInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (-(Math.Cos(Math.PI * x) - 1) / 2);
            return new Easing(f, EasingType.SineInOut);
        }
        #endregion

        #region elastic easings
        /// <summary>
        /// Get an Easing object with elasticIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing ElasticIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (-Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * (2 * Math.PI) / 3));
            return new Easing(f, EasingType.ElasticIn);
        }

        
        /// <summary>
        /// Get an Easing object with elasticOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing ElasticOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * (2 * Math.PI) / 3) + 1);
            return new Easing(f, EasingType.ElasticOut);
        }

        /// <summary>
        /// Get an Easing object with elasticInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing ElasticInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * (2 * Math.PI) / 3) + 1);
            return new Easing(f, EasingType.ElasticInOut);
        }
        #endregion

        #region bounce easings
        /// <summary>
        /// Get an Easing object with bounceIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing BounceIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * BounceOut_Function(1 - x);
            return new Easing(f, EasingType.BounceIn);
        }

        /// <summary>
        /// Get an Easing object with bounceOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        public static Easing BounceOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * BounceOut_Function(x);
            return new Easing(f, EasingType.BounceOut);
        }

        /// <summary>
        /// Get an Easing object with bounceInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing BounceInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x < 0.5
                ? (1 - BounceOut_Function(1 - 2 * x)) / 2
                : (1 + BounceOut_Function(2 * x - 1)) / 2);
            return new Easing(f, EasingType.BounceInOut);
        }
        #endregion

        #region back easings
        /// <summary>
        /// Get an Easing object with backIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing BackIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (2.70158 * x * x * x - 1.70158 * x * x);
            return new Easing(f, EasingType.BackIn);
        }

        /// <summary>
        /// Get an Easing object with backOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing BackOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (1 + 2.70158 * Math.Pow(x - 1, 3) + 1.70158 * Math.Pow(x - 1, 2));
            return new Easing(f, EasingType.BackOut);
        }

        /// <summary>
        /// Get an Easing object with backInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing BackInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * BackInOut_Function(x);
            return new Easing(f, EasingType.BackInOut);
        }
        #endregion

        #region circ easings
        /// <summary>
        /// Get an Easing object with circIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing CircIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (1 - Math.Sqrt(1 - Math.Pow(x, 2)));
            return new Easing(f, EasingType.CircIn);
        }

        /// <summary>
        /// Get an Easing object with circOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing CircOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * Math.Sqrt(1 - Math.Pow(x - 1, 2));
            return new Easing(f, EasingType.CircOut);
        }

        /// <summary>
        /// Get an Easing object with circInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing CircInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x < 0.5
                ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2
                : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2);
            return new Easing(f, EasingType.CircInOut);
        }
        #endregion

        #region exponential easings
        /// <summary>
        /// Get an Easing object with ExpoIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing ExpoIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x == 0 ? 0 : Math.Pow(2, 10 * x - 10));
            return new Easing(f, EasingType.ExpoIn);
        }

        /// <summary>
        /// Get an Easing object with ExpoOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing ExpoOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x == 1 ? 1 : 1 - Math.Pow(2, -10 * x));
            return new Easing(f, EasingType.ExpoOut);
        }

        /// <summary>
        /// Get an Easing object with ExpoInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing ExpoInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * ExpoInOut_Function(x);
            return new Easing(f, EasingType.ExpoInOut);
        }
        #endregion

        #region polynomial easings

        #region quadratic
        /// <summary>
        /// Get an Easing object with quadIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuadIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * x * x;
            return new Easing(f, EasingType.QuadIn);
        }

        /// <summary>
        /// Get an Easing object with quadOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuadOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (1 - (1 - x) * (1 - x));
            return new Easing(f, EasingType.QuadOut);
        }

        /// <summary>
        /// Get an Easing object with quadInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuadInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2);
            return new Easing(f, EasingType.QuadInOut);
        }
        #endregion

        #region cubic
        /// <summary>
        /// Get an Easing object with cubicIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing CubicIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x * x * x);
            return new Easing(f, EasingType.CubicIn);
        }

        /// <summary>
        /// Get an Easing object with cubicOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing CubicOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (1 - Math.Pow(1 - x, 3));
            return new Easing(f, EasingType.CubicOut);
        }

        /// <summary>
        /// Get an Easing object with cubicInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing CubicInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2);
            return new Easing(f, EasingType.CubicInOut);
        }
        #endregion

        #region quartic
        /// <summary>
        /// Get an Easing object with quartIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuartIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x * x * x * x);
            return new Easing(f, EasingType.QuartIn);
        }

        /// <summary>
        /// Get an Easing object with quartOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuartOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (1 - Math.Pow(1 - x, 4));
            return new Easing(f, EasingType.QuartOut);
        }

        /// <summary>
        /// Get an Easing object with quartInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuartInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2);
            return new Easing(f, EasingType.QuartInOut);
        }
        #endregion

        #region quintic
        /// <summary>
        /// Get an Easing object with quintIn easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuintIn()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x * x * x * x * x);
            return new Easing(f, EasingType.QuintIn);
        }
        /// <summary>
        /// Get an Easing object with quintOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuintOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (1 - Math.Pow(1 - x, 5));
            return new Easing(f, EasingType.QuintOut);
        }
        /// <summary>
        /// Get an Easing object with quintInOut easing
        /// </summary>
        /// <returns>new Easing object</returns>
        private static Easing QuintInOut()
        {
            Func<double, double, double, double> f = (start, end, x) => start + (end - start) * (x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
            return new Easing(f, EasingType.QuintInOut);
        }
        #endregion

        #endregion

        #endregion

        #region private helper methods for specific easings
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

        private static double ExpoInOut_Function(double x)
        {
            return x == 0
                ? 0
                : x == 1
                ? 1
                : x < 0.5 
                ? Math.Pow(2, 20 * x - 10) / 2
                : (2 - Math.Pow(2, -20 * x + 10)) / 2;
        }

        private static double BackInOut_Function(double x)
        {
            double c1 = 1.70158;
            double c2 = c1 * 1.525;

            return x < 0.5
                ? (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
                : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        }
        #endregion

        /// <summary>
        /// Get a value between startValue and endValue. The exact value depends on the type of easing function and the ratio of idx and max_idx
        /// </summary>
        /// <param name="startValue">The start value for the easing function</param>
        /// <param name="endValue">The end value for the easing function</param>
        /// <param name="x">ratio between idx and maxIdx - between 0 and 1 - the progress of the easing</param>
        /// <returns></returns>
        public double Ease(double startValue, double endValue, double x)
        {
            return _easingFunction(startValue, endValue, x);
        }
    }
}