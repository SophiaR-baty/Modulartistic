using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.IFS
{
    internal class SierpinskiTriangle : IIteratedFunctionSystem
    {
        public List<Func<Complex, Complex>> Functions { get; set; }
        public List<Complex> Points { get; set; }

        private Random _random;

        public SierpinskiTriangle(Complex p1, Complex p2, Complex p3)
        {
            Functions = new List<Func<Complex, Complex>>()
            {
                c => new Complex(c.Real + p1.Real, c.Imaginary + p1.Imaginary) / 2,
                c => new Complex(c.Real + p2.Real, c.Imaginary + p2.Imaginary) / 2,
                c => new Complex(c.Real + p3.Real, c.Imaginary + p3.Imaginary) / 2,
            };

            Points = new List<Complex>() { new Complex(p1.Real, p1.Imaginary) };
            _random = new Random();
        }

        public void GeneratePoints(int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                Complex point = Points.Last();
                int idx = (int) Math.Floor(_random.NextDouble() * 3);
                Func<Complex, Complex> f = Functions[idx];

                Points.Add(f(point));
            }
        }
    }
}
