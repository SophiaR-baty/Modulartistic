using Antlr4.Runtime.Tree;
using Modulartistic.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.Geometry
{
    [AddOn]
    public static class GeometryFunctions
    {
        public static double Test() { return 0; }
        
        
        
        public static Complex Point(double x, double y)
        {
            return new Complex(x, y);
        }

        public static Complex[] PointCloud(params Complex[] points)
        {
            return points;
        }

        // vertices of regular ngon with radius r
        public static Complex[] RegularNGonVertices(int n, double r, Complex center)
        {
            Complex[] vertices = new Complex[n];

            for (int i = 0; i < n; i++)
            {
                double theta = 2 * Math.PI * i / n;
                vertices[i] = new Complex(center.Real + r * Math.Cos(theta), center.Imaginary + r * Math.Sin(theta));
            }

            return vertices;
        }

        // Distance between two points
        public static double Distance(Complex p1, Complex p2)
        {
            return Complex.Abs(p1 - p2);
        }

        // Angle between two points in radians
        public static double AngleBetweenPoints(Complex p1, Complex p2)
        {
            return Math.Atan2(p2.Imaginary - p1.Imaginary, p2.Real - p1.Real);
        }

        // Rotate a point around another point by a specified angle (in radians)
        public static Complex RotatePoint(Complex point, Complex center, double angle)
        {
            Complex translatedPoint = point - center;
            Complex rotatedPoint = translatedPoint * Complex.FromPolarCoordinates(1, angle);
            return rotatedPoint + center;
        }

        // Area of a polygon given an array of vertices
        public static double PolygonArea(Complex[] vertices)
        {
            int n = vertices.Length;
            double area = 0.0;
            for (int i = 0; i < n; i++)
            {
                Complex current = vertices[i];
                Complex next = vertices[(i + 1) % n];
                area += (current.Real * next.Imaginary) - (next.Real * current.Imaginary);
            }
            return Math.Abs(area) / 2.0;
        }

        // Bounding box of a set of points
        public static (Complex min, Complex max) BoundingBox(Complex[] points)
        {
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (var point in points)
            {
                minX = Math.Min(minX, point.Real);
                minY = Math.Min(minY, point.Imaginary);
                maxX = Math.Max(maxX, point.Real);
                maxY = Math.Max(maxY, point.Imaginary);
            }

            return (new Complex(minX, minY), new Complex(maxX, maxY));
        }

        // Scale a set of points around a center by a given factor
        public static Complex[] ScaleShape(Complex[] points, Complex center, double scale)
        {
            Complex[] scaledPoints = new Complex[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                scaledPoints[i] = center + (points[i] - center) * scale;
            }
            return scaledPoints;
        }

        public static Complex ClosestPoint(Complex[] points, Complex target)
        {
            Complex closest = points[0];
            double minDistance = Distance(points[0], target);

            foreach (var point in points)
            {
                double distance = Distance(point, target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = point;
                }
            }

            return closest;
        }

        public static Complex ClosestPointTree(KDTree points, Complex target)
        {
            return points.FindNearest(target);
        }

        public static KDTree PointsToTree(Complex[] points)
        {
            return new KDTree(points);
        }
    }
}
