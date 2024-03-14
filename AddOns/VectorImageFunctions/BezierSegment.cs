using System;
using System.Collections.Generic;
using System.Text;

namespace VectorImageFunctions
{
    public class BezierSegment : Segment
    {
        double p0_x, p0_y;
        double p1_x, p1_y;
        double p2_x, p2_y;
        double p3_x, p3_y;

        public BezierSegment(double p0_x, double p0_y, double p1_x, double p1_y, double p2_x, double p2_y, double p3_x, double p3_y)
        {
            this.p0_x = p0_x;
            this.p0_y = p0_y;
            this.p1_x = p1_x;
            this.p1_y = p1_y;
            this.p2_x = p2_x;
            this.p2_y = p2_y;
            this.p3_x = p3_x;
            this.p3_y = p3_y;
        }

        public double X(double t)
        {
            if (t < 0 || t > 1) { throw new ArgumentException("t must be between 0 and 1."); }

            double t_1 = t;
            double t_2 = t_1 * t;
            double t_3 = t_2 * t;

            double subt_1 = 1 - t;
            double subt_2 = subt_1 * subt_1;
            double subt_3 = subt_2 * subt_1;

            return subt_3 * p0_x + 3 * (subt_2 * t_1 * p1_x + subt_1 * t_2 * p2_x) + t_3 * p3_x;
        }

        public double Y(double t)
        {
            if (t < 0 || t > 1) { throw new ArgumentException("t must be between 0 and 1."); }

            double t_1 = t;
            double t_2 = t_1 * t;
            double t_3 = t_2 * t;

            double subt_1 = 1 - t;
            double subt_2 = subt_1 * subt_1;
            double subt_3 = subt_2 * subt_1;

            return subt_3 * p0_y + 3 * (subt_2 * t_1 * p1_y + subt_1 * t_2 * p2_y) + t_3 * p3_y;
        }
    }
}
