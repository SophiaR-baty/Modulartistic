using System;
using System.Collections.Generic;
using System.Text;

namespace VectorImageFunctions
{
    public class CornerSegment : Segment
    {
        private Func<double, double> xFunc;
        private Func<double, double> yFunc;

        public CornerSegment(double p0_x, double p0_y, double p1_x, double p1_y, double p2_x, double p2_y)
        {
            xFunc = delegate (double t) { 
                if (t < 0.5) 
                {
                    return p0_x + 2 * t * (p1_x - p0_x);
                }
                else
                {
                    return p1_x + 2 * (t - 0.5) * (p2_x - p1_x);
                }
            };
            yFunc = delegate (double t) {
                if (t < 0.5)
                {
                    return p0_y + 2 * t * (p1_y - p0_y);
                }
                else
                {
                    return p1_y + 2 * (t - 0.5) * (p2_y - p1_y);
                }
            };
        }

        public double X(double t)
        {
            if (t < 0 || t > 1) { throw new ArgumentException("t must be between 0 and 1."); }
            return xFunc(t);
        }

        public double Y(double t)
        {
            if (t < 0 || t > 1) { throw new ArgumentException("t must be between 0 and 1."); }
            return yFunc(t);
        }
    }
}
