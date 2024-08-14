using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.IFS
{
    internal interface IIteratedFunctionSystem
    {
        public List<Func<Complex, Complex>> Functions { get; set; }
        public List<Complex> Points { get; set; }

        public void GeneratePoints(int depth);
    }
}
