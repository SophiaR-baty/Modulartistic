using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public enum ParameterEvaluationStrategy
    {
        Auto = 0,
        Global = 1,
        PerGeneration = 2,
        PerState = 3,
        PerPixel = 4,
    }
}
