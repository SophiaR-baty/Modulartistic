using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
        #region Functions for basic n-dimensional types

        public static double[] Point(params double[] coordinates)
        {
            return coordinates.ToArray();
        }

        public static double[] Vector(params double[] coordinates)
        {
            return coordinates.ToArray();
        }

        #endregion

        #region Functions for Point Clouds

        public static double[][] PointCloud(params double[][] points)
        {
            double[][] copy = new double[points.Length][];

            for (int i = 0; i < points.Length; i++) { copy[i] = points[i].ToArray(); }

            return copy;
        }

        public static double[][] AddPoint(double[][] pointcloud, double[] point, int idx = -1)
        {
            double[][] newPointCloud = new double[pointcloud.Length + 1][];
            if (idx == -1)
            {
                Array.Copy(pointcloud, newPointCloud, idx);
                newPointCloud[idx] = point.ToArray();
                Array.Copy(pointcloud, idx, newPointCloud, idx + 1, pointcloud.Length - idx);

                return pointcloud; 
            }

            if (idx < 0 || idx >= pointcloud.Length) 
            { 
                return pointcloud;
            }

            Array.Copy(pointcloud, newPointCloud, pointcloud.Length);
            newPointCloud[pointcloud.Length] = point;
            return newPointCloud;
        }

        public static double[][] RemovePoint(double[][] pointcloud, int idx)
        {
            if (idx < 0 || idx >= pointcloud.Length)
                return pointcloud;

            double[][] newPointCloud = new double[pointcloud.Length - 1][];
            Array.Copy(pointcloud, newPointCloud, idx);
            Array.Copy(pointcloud, idx + 1, newPointCloud, idx, pointcloud.Length - idx - 1);
            return newPointCloud;
        }

        public static double[] GetPoint(double[][] pointcloud, int idx)
        {
            if (idx < 0 || idx >= pointcloud.Length)
                return new double[] { double.NaN };

            return pointcloud[idx].ToArray();
        }

        #endregion

        #region predefined shapes

        public static double[][] NCube(int n, double length, int depth)
        {
            double[][] vertices = NCubeVertices(n, length);

            List<double[]> edges = new List<double[]>(vertices);

            // Check all pairs of vertices for edges
            for (int i = 0; i < vertices.Length; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    Vector vertice1_v = new Vector(vertices[i]);
                    Vector vertice2_v = new Vector(vertices[j]);

                    bool is_edge = false;
                    int max_dim = Math.Max(vertice1_v.Dimension, vertice2_v.Dimension);
                    for (int k = 0; k < max_dim; k++)
                    {
                        if (vertice1_v[k] != vertice2_v[k])
                        {
                            if (!is_edge)
                            {
                                is_edge = true;
                            }
                            else
                            {
                                is_edge = false;
                                break;
                            }
                        }
                    }
                    
                    if (is_edge)
                    {
                        // Generate the edge with interpolated points
                        double[][] edge = new double[depth-1][]; // depth points - start and end point
                        for (int k = 1; k < depth; k++)
                        {
                            double t = (double)k / (depth);
                            edges.Add(InterpolatePoints(vertices[i], vertices[j], t));
                        }
                    }
                }
            }

            return edges.ToArray();
        }

        #endregion

        #region transformations

        public static double[] TranslatePoint(double[] point, double[] translationVector)
        {
            Vector trans_v = new Vector(translationVector);
            Vector v = new Vector(point) + trans_v;
            return v.Coordinates.ToArray();
        }

        public static double[][] TranslatePoints(double[][] points, double[] translationVector) 
        {
            List<double[]> result = new List<double[]>();
            for (int i = 0; i < points.Length; i++) 
            {
                result.Add(TranslatePoint(points[i], translationVector));
            }

            return result.ToArray();
        }

        public static double[] ScalePoint(double[] point, double scaleFactor)
        {
            return (new Vector(point) * scaleFactor).Coordinates.ToArray();
        }

        public static double[][] ScalePoints(double[][] points, double scaleFactor)
        {
            List<double[]> result = new List<double[]>();

            // Scale each point in the points array
            for (int i = 0; i < points.Length; i++)
            {
                result.Add(ScalePoint(points[i], scaleFactor));
            }

            return result.ToArray();
        }

        public static double[] RotatePoint(double[] point, double rotation, int axis1, int axis2)
        {
            Vector point_v = new Vector(point);
            double[] new_point = point.ToArray();
            if (axis1 >= new_point.Length || axis2 >= new_point.Length)
            {
                new_point = new double[Math.Max(axis1, axis2)];
                point.CopyTo(new_point, 0);
            }

            double rad = rotation / 360 * 2 * Math.PI;
            double cos_r = Math.Cos(rad);
            double sin_r = Math.Sin(rad);

            new_point[axis1] = point_v[axis1]*cos_r - point_v[axis2]*sin_r;
            new_point[axis2] = point_v[axis1]*sin_r + point_v[axis2]*cos_r;
            
            return new_point;
        }

        public static double[][] RotatePoints(double[][] points, double rotation, int axis1, int axis2) 
        {
            double rad = rotation / 360 * 2 * Math.PI;
            double cos_r = Math.Cos(rad);
            double sin_r = Math.Sin(rad);
            List<double[]> result = new List<double[]>();

            for (int i = 0; i < points.Length; i++) 
            {

                Vector point_v = new Vector(points[i]); 
                double[] new_point = points[i].ToArray();
                if (axis1 >= new_point.Length || axis2 >= new_point.Length)
                {
                    new_point = new double[Math.Max(axis1, axis2) + 1];
                    points[i].CopyTo(new_point, 0);
                }
                new_point[axis1] = point_v[axis1] * cos_r - point_v[axis2] * sin_r;
                new_point[axis2] = point_v[axis1] * sin_r + point_v[axis2] * cos_r;
                result.Add(new_point);
            }

            return result.ToArray();
        }

        #endregion

        public static double[] InterpolatePoints(double[] p1, double[] p2, double t)
        {
            int maxDim = Math.Max(p1.Length, p2.Length);
            double[] result = new double[maxDim];

            for (int i = 0; i < maxDim; i++)
            {
                double p1Value = i < p1.Length ? p1[i] : 0.0;
                double p2Value = i < p2.Length ? p2[i] : 0.0;

                result[i] = (1 - t) * p1Value + t * p2Value;
            }

            return result;
        }

        public static double[][] AddSides(double[][] vertices, int depth)
        {
            if (vertices == null || vertices.Length < 2)
                return vertices;
            if (depth < 0)
                return vertices;

            int totalPoints = vertices.Length + (vertices.Length * depth);
            double[][] result = new double[totalPoints][];
            int index = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                result[index++] = vertices[i];

                double[] nextVertex = vertices[(i + 1) % vertices.Length];

                for (int j = 1; j <= depth; j++)
                {
                    double t = (double)j / (depth + 1);
                    double[] interpolated = InterpolatePoints(vertices[i], nextVertex, t);
                    result[index++] = interpolated;
                }
            }

            return result;
        }

        public static double[][] ConnectPoints(double[][] vertices, int depth)
        {
            if (vertices == null || vertices.Length < 2)
                return vertices;
            if (depth < 0)
                return vertices;

            int totalPairs = vertices.Length * (vertices.Length - 1) / 2; // Combination nC2 = n * (n - 1) / 2
            int totalPoints = vertices.Length + totalPairs * depth;

            double[][] result = new double[totalPoints][];
            int index = 0;

            foreach (var vertex in vertices)
            {
                result[index++] = vertex;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    for (int k = 1; k <= depth; k++)
                    {
                        double t = (double)k / (depth + 1);
                        double[] interpolated = InterpolatePoints(vertices[i], vertices[j], t);
                        result[index++] = interpolated;
                    }
                }
            }

            return result;
        }


        #region Vertices

        public static double[][] NCubeVertices(int n, double length)
        {
            if (n <= 0)
                return new double[0][];
            if (length <= 0)
                length = Math.Abs(length);

            int numVertices = (int)Math.Pow(2, n); // 2^n vertices for an n-dimensional cube
            double[][] vertices = new double[numVertices][];

            // Generate all vertices
            for (int i = 0; i < numVertices; i++)
            {
                vertices[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    // Determine if the j-th coordinate of this vertex is on the "positive" or "negative" side
                    int mask = 1 << j; // Mask to extract the j-th bit
                    vertices[i][j] = ((i & mask) != 0 ? 1 : -1) * (length / 2.0);
                }
            }

            return vertices;
        }

        public static double[][] TriangleVertices(double[] A, double[] B, double[] C)
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

        #endregion Vertices

        #region "Output" Functions -> conversions to double

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
            double result = double.NaN;

            Vector pov_v = new Vector(pov);
            foreach (double[] point in points)
            {
                Vector point_v = new Vector(point);
                int min_dim = Math.Min(pov.Length, point.Length);
                int max_dim = Math.Max(pov.Length, point.Length);

                // check if pov->point might intersect the xy plane
                bool intersect = true;
                for (int i = 2; intersect && i < min_dim; i++)
                {
                    intersect = pov_v[i] * point_v[i] <= 0;
                }
                if (!intersect) { continue; }

                // find the t for which it intersects the xy plane
                double t = point_v[2] / (point_v[2] - pov_v[2]);
                for (int i = 3; i < max_dim; i++)
                {
                    double denom = point_v[i] - pov_v[i];
                    if (Math.Abs(denom) > 1E-15) // Avoid division by near-zero values
                    {
                        double new_t = pov_v[i] / denom;
                        if (Math.Abs(new_t - t) > 1E-15)
                        {
                            intersect = false;
                            break;
                        }
                        t = new_t;
                    }
                    else
                    {
                        intersect = false; 
                        break;
                    }
                }
                if (!intersect) { continue; }
                if (t < 0 || t > 1) { continue; }

                // calculate the point at which it intersects the xy plane
                Vector p = point_v + t * (pov_v - point_v);

                // Check if x,y is close to the intersection
                double _x = x - p[0], _y = y - p[1];

                if (Math.Abs(_x * _x + _y * _y) <= tolerance) 
                {
                    if (double.IsNaN(result)) { result = 0; }
                    result += (p - point_v).Norm;
                }
            }
            return result;
        }

        #endregion
    }
}
