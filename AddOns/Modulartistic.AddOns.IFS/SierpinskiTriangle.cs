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
        public List<List<Complex>> Points { get; set; }

        public SierpinskiTriangle(Complex p1, Complex p2, Complex p3)
        {
            Functions = new List<Func<Complex, Complex>>()
            {
                c => new Complex(c.Real + p1.Real, c.Imaginary + p1.Imaginary) / 2,
                c => new Complex(c.Real + p2.Real, c.Imaginary + p2.Imaginary) / 2,
                c => new Complex(c.Real + p3.Real, c.Imaginary + p3.Imaginary) / 2,
            };

            Points = [new List<Complex>() 
            { 
                new Complex(p1.Real, p1.Imaginary),
                new Complex(p2.Real, p2.Imaginary),
                new Complex(p3.Real, p3.Imaginary),
            }];
        }

        public void GeneratePoints(int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                List<Complex> points = new List<Complex>();
                foreach (var point in Points.Last())
                {
                    foreach (var f in Functions)
                    {
                        points.Add(f(point));
                    }
                }
                Points.Add(points);
            }
        }
    }
}
