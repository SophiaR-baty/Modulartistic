using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.IFS
{
    internal class BarnsleyFern : IIteratedFunctionSystem
    {
        public List<Func<Complex, Complex>> Functions { get; set; }
        public List<Complex> Points { get; set; }

        private Random _random;

        public BarnsleyFern(Complex initial) 
        {
            Functions = new List<Func<Complex, Complex>>()
            {
                GetTransform(0, 0, 0, 0.16, 0, 0),
                GetTransform(0.85, 0.04, -0.04, 0.85, 0, 1.6),
                GetTransform(0.20, -0.26, 0.23, 0.22, 0, 1.6),
                GetTransform(-0.15, 0.28, 0.26, 0.24, 0, 0.44)
            };
            Points = new List<Complex>() { initial };
            _random = new Random();
        }

        public void GeneratePoints(int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                Complex point = Points.Last();
                double randomValue = _random.NextDouble();

                Func<Complex, Complex>? f = null;
                if (randomValue <= 0.01)
                {
                    f = Functions[0];
                }
                else if (randomValue <= 0.86)
                {
                    f = Functions[1];
                }
                else if (randomValue <= 0.93)
                {
                    f = Functions[2];
                }
                else
                {
                    f = Functions[3];
                }

                Points.Add(f(point));
            }
        }

        private static Func<Complex, Complex> GetTransform(double a, double b, double c, double d, double e, double f)
        {
            return z => new Complex(a * z.Real + b * z.Imaginary + e, c * z.Real + d * z.Imaginary + f);
        }

    }
}
