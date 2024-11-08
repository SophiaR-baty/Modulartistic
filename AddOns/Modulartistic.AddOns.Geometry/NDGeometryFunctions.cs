using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Modulartistic.Common;
using static System.Net.Mime.MediaTypeNames;

namespace Modulartistic.AddOns.Geometry
{
    [AddOn]
    public static class NDGeometryFunctions
    {
        public static double[] Point(params double[] coordinates)
        {
            return coordinates.ToArray();
        }

        public static double[] Vector(params double[] coordinates)
        {
            return coordinates.ToArray();
        }

        public static double[][] PointCloud(params double[][] points)
        {
            double[][] copy = new double[points.Length][];

            for (int i = 0; i < points.Length; i++) { copy[i] = points[i].ToArray(); }

            return copy;
        }

        public static double[][] Triangle(double[] A, double[] B, double[] C)
        { 
            return [A.ToArray(), B.ToArray(), C.ToArray()];
        }

        public static double[][] RegularNGonVertices(int n, double r, double[] center, double[]? rx = null, double[]? ry = null)
        {
            if (rx is null ^ ry is null)
            {
                throw new Exception("Either two or no axes must be defined.");
            }
            if (rx is null) { rx = [1, 0]; }
            if (ry is null) { rx = [0, 1]; }

            double[][] vertices = new double[n][];
            VectorSpace VS = new VectorSpace(new Vector(center), [new Vector(rx), new Vector(ry)]);
            for (int i = 0; i < n; i++)
            {
                double theta = 2 * Math.PI * i / n;
                vertices[i] = VS.ToUnitSpace(new Vector(r * Math.Cos(theta), r * Math.Sin(theta))).Coordinates;
            }

            return vertices;
        }

        public static double DistanceToXYPlane(double[] point)
        {
            Vector v = new Vector(point.Skip(2).ToArray());
            return v.Norm;
        }

        public static double ProjectPoint(double x, double y, double[][] points, double tolerance = 1)
        {
            double result = 0;
            foreach (double[] point in points)
            {
                double _x = x - point[0], _y = y - point[1];
                if (Math.Abs(_x*_x + _y*_y) <= tolerance) { result += DistanceToXYPlane(point); }
            }
            return result;
        }

        public static double ProjectPointPOV(double x, double y, double[][] points, double[] pov, double tolerance = 1)
        {
            double result = 0;

            Vector pov_v = new Vector(pov);
            foreach (double[] point in points)
            {
                Vector point_v = new Vector(point);
                int min_dim = Math.Min(pov.Length, point.Length);
                int max_dim = Math.Max(pov.Length, point.Length);
                bool intersect = true;
                for (int i = 2; intersect && i < min_dim; i++)
                {
                    intersect = pov_v[i] * point_v[i] <= 0;
                }
                if (!intersect) { continue; }

                double t = point_v[2] / (point_v[2] - pov_v[2]); ;
                for (int i = 3; i < max_dim; i++)
                {
                    double new_t = point_v[i] / (point_v[i] - pov_v[i]);
                    if (Math.Abs(new_t - t) > 1E-15) { intersect = false; break; }
                    
                    t = new_t;
                }
                if (!intersect) { continue; }

                Vector xy_v = new Vector(x, y);
                Vector p = point_v + t * (pov_v - point_v);
                Console.WriteLine($"({x}, {y}) -> {p}");
                double _x = x - p[0], _y = y - p[1];
                if (Math.Abs(_x * _x + _y * _y) <= tolerance) { result += (p - point_v).Norm; }
            }
            return result;
        }

    }
}
