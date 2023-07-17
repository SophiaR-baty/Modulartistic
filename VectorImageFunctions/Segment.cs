using System;
using System.Collections.Generic;
using System.Text;

namespace VectorImageFunctions
{
    public interface Segment
    {
        public double X(double t);
        public double Y(double t);
    }
}
