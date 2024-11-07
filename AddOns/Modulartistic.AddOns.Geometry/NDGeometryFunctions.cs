using System;
using System.Collections.Generic;
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
            return coordinates;
        }

        public static double[][] PointCloud(params double[][] points)
        {
            return points;
        }

        public static double[][] Triangle(double[] A, double[] B, double[] C)
        { 
            return [A, B, C];
        }

        public static double[][] RegularNGonVertices(int n, double r, double[] center, double[]? rx = null, double[]? ry = null)
        {
            // check and adjust inputs
            center = GuaranteeDimension(center, 2);
            if (rx == null && ry == null)
            {
                // Check if the plane is valid
                rx = [center[0] + 1, center[1]];
                ry = [center[0], center[1] + 1];
            }
            else if (rx == null || ry == null)
            {
                throw new Exception("Either two or both axes must be defined.");
            }
            else
            {
                rx = GuaranteeDimension(rx, 2);
                ry = GuaranteeDimension(ry, 2);
            }



            // Generate vertices on the given plane
            double[][] vertices = new double[n][];
            double[] planeNormal = ComputePlaneNormal(plane);

            for (int i = 0; i < n; i++)
            {
                double theta = 2 * Math.PI * i / n;
                double[] pointOnPlane = new double[center.Length];
                double[] direction1 = SubtractVectors(plane[1], plane[0]);
                double[] direction2 = SubtractVectors(plane[2], plane[0]);

                // Calculate position in the plane using the direction vectors
                for (int j = 0; j < center.Length; j++)
                {
                    pointOnPlane[j] = center[j] + r * (Math.Cos(theta) * direction1[j] + Math.Sin(theta) * direction2[j]);
                }

                vertices[i] = ProjectPointOntoPlane(pointOnPlane, plane[0], planeNormal);
            }

            return vertices;
        }

        private static double[] GuaranteeDimension(double[] point, int dim) 
        {
            double[] result = point;
            if (point.Length < dim)
            {
                result = new double[dim];
                for (int i = 0; i  < dim; i++)
                {
                    result[i] = point[i];
                }
            }

            return result;
        }

        private static bool IsPointOnPlane(double[] point, double[][] plane)
        {
            double[] normal = ComputePlaneNormal(plane);
            double[] vectorToPoint = SubtractVectors(point, plane[0]);
            return Math.Abs(DotProduct(vectorToPoint, normal)) < 1e-6; // tolerance for floating-point precision
        }

        private static double[] ComputePlaneNormal(double[][] plane)
        {
            double[] v1 = SubtractVectors(plane[1], plane[0]);
            double[] v2 = SubtractVectors(plane[2], plane[0]);
            return CrossProduct(v1, v2);
        }

        private static double[] ProjectPointOntoPlane(double[] point, double[] planePoint, double[] planeNormal)
        {
            double[] vectorToPoint = SubtractVectors(point, planePoint);
            double distanceToPlane = DotProduct(vectorToPoint, planeNormal) / DotProduct(planeNormal, planeNormal);
            double[] projection = new double[point.Length];

            for (int i = 0; i < point.Length; i++)
            {
                projection[i] = point[i] - distanceToPlane * planeNormal[i];
            }

            return projection;
        }

        private static double[] SubtractVectors(double[] a, double[] b)
        {
            return a.Zip(b, (x, y) => x - y).ToArray();
        }

        private static double DotProduct(double[] a, double[] b)
        {
            // implements the Kahan summation algorithm
            // https://en.wikipedia.org/wiki/Kahan_summation_algorithm

            int min_dim = Math.Min(a.Length, b.Length);
            
            double sum = 0.0;
            double c = 0.0;
            for (int i = 0; i < min_dim; i++)
            {
                double y = a[i] * b[i] - c;
                double t = sum + y;
                c = (t - sum) - y;
                sum = t;
            }

            return sum;
        }

        private static double[] CrossProduct(double[] a, double[] b)
        {
            if (a.Length != 3 || b.Length != 3)
            {
                throw new ArgumentException("Cross product is only defined in 3 dimensions.");
            }

            return new double[]
            {
            a[1] * b[2] - a[2] * b[1],
            a[2] * b[0] - a[0] * b[2],
            a[0] * b[1] - a[1] * b[0]
            };
        }
    }
}
