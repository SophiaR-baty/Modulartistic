using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenCvSharp;
using static OpenCvSharp.ML.DTrees;

namespace VectorImageFunctions
{
    public class Curve
    {
        public List<Segment> segments;
        public int s = 0;

        public Curve() 
        {
            segments = new List<Segment>();
        }

        public static Curve FromImage(string path_to_img, double threshold_low, double threshold_high)
        {
            
            Bitmap bm = BezierConverter.CannyExtern(path_to_img, threshold_low, threshold_high);
            return BezierConverter.TraceMat(bm);
        }

        public static Curve FromImage(string path_to_img)
        {

            Bitmap bm = new Bitmap(path_to_img);
            return BezierConverter.TraceMat(bm);
        }

        public double distance(double x, double y, double step)
        {
            if (segments.Count == 0) { return 0; }
            double minDistance = double.MaxValue;
            for (int i = 0; i < segments.Count; i++)
            {
                for (double t = 0; t < 1; t += step)
                {

                    double x_t = x - segments[i].X(t);
                    double y_t = y - segments[i].Y(t);
                    double distance = Math.Sqrt(x_t*x_t + y_t*y_t);

                    if (distance < 1.0/360) { return 0; }
                    if (distance < minDistance) 
                    {
                        minDistance = distance;
                    }
                }
            }
            // const double TOLERANCE = 0.5;
            // Func<double, double> bezierAngleFunc = (t) => Math.Atan2(yFunc(t), xFunc(t)); // from -180 to 180
            // Func<double, double> pointBezierAngleFunc = (t) => Math.Atan2(y-yFunc(t), x-xFunc(t)); // from -180 to 180
            // Func<double, double> distanceFunc = (t) => Math.Sqrt(Math.Pow(x - xFunc(t), 2) + Math.Pow(y - yFunc(t), 2));
            // Func<double, double> gradFunc = (t) => Math.Sqrt(Math.Pow(xFunc(t), 2) + Math.Pow(yFunc(t), 2));
            
            return minDistance;
        }

        public double distanceApprox(double x, double y, double step)
        {
            if (segments.Count == 0) { return 0; }
            double approx = double.MaxValue;
            double actualDistance = double.MaxValue;
            for (int i = 0; i < segments.Count; i++)
            {
                for (double t = 0; t < 1; t += step)
                {
                    double x_t = x - segments[i].X(t);
                    double y_t = y - segments[i].Y(t);
                    double distance = Math.Abs(x_t) + Math.Abs(y_t);
                    
                    if (distance < 1.0 / 360) { return 0; }
                    if (distance < approx)
                    {
                        approx = distance;
                        actualDistance = Math.Sqrt(x_t*x_t + y_t*y_t);
                    }
                }
            }
            
            return actualDistance;
        }

        public double angleApprox(double x, double y, double step)
        {
            if (segments.Count == 0) { return 0; }
            double approx = double.MaxValue;
            double min_x_t = 0;
            double min_y_t = 0;
            for (int i = 0; i < segments.Count; i++)
            {
                for (double t = 0; t < 1; t += step)
                {
                    double x_t = x - segments[i].X(t);
                    double y_t = y - segments[i].Y(t);
                    double distance = Math.Abs(x_t) + Math.Abs(y_t);

                    if (distance < approx)
                    {
                        approx = distance;
                        min_x_t = x_t;
                        min_y_t = y_t;
                    }
                }
            }

            return 180*Math.Atan2(min_y_t*min_y_t, min_x_t*min_x_t)/Math.PI;
        }

        public double angle(double x, double y, double step)
        {
            if (segments.Count == 0) { return 0; }
            double minDistance = double.MaxValue;
            double min_x_t = 0;
            double min_y_t = 0;
            for (int i = 0; i < segments.Count; i++)
            {
                for (double t = 0; t < 1; t += step)
                {

                    double x_t = x - segments[i].X(t);
                    double y_t = y - segments[i].Y(t);
                    double distance = Math.Sqrt(x_t * x_t + y_t * y_t);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        min_x_t = x_t;
                        min_y_t = y_t;
                    }
                }
            }
            // const double TOLERANCE = 0.5;
            // Func<double, double> bezierAngleFunc = (t) => Math.Atan2(yFunc(t), xFunc(t)); // from -180 to 180
            // Func<double, double> pointBezierAngleFunc = (t) => Math.Atan2(y-yFunc(t), x-xFunc(t)); // from -180 to 180
            // Func<double, double> distanceFunc = (t) => Math.Sqrt(Math.Pow(x - xFunc(t), 2) + Math.Pow(y - yFunc(t), 2));
            // Func<double, double> gradFunc = (t) => Math.Sqrt(Math.Pow(xFunc(t), 2) + Math.Pow(yFunc(t), 2));

            return 180*Math.Atan2(min_y_t * min_y_t, min_x_t * min_x_t)/Math.PI;
        }
    }
}
