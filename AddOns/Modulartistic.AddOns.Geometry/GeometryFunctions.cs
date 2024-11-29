using Modulartistic.Common;

namespace Modulartistic.AddOns.Geometry
{
    [AddOn]
    public static class GeometryFunctions
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

        public static double[][] HyperBall(double[] origin, double radius, int n, int depth)
        {
            List<double[]> points = new List<double[]>();

            // Recursive function to generate points
            void Generate(int dim, double[] currentPoint, double remainingRadius)
            {
                if (dim == n)
                {
                    // Base case: add the point to the list
                    points.Add((double[])currentPoint.Clone());
                    return;
                }

                for (int i = 0; i < depth; i++)
                {
                    // Calculate angle for this dimension
                    double angle = (2 * Math.PI / depth) * i;

                    // Compute the coordinate in the current dimension
                    double coord = Math.Cos(angle) * remainingRadius;

                    // Update the point with the new coordinate
                    currentPoint[dim] = dim < origin.Length ? origin[dim] + coord : coord;

                    // Recur to the next dimension
                    double newRemainingRadius = remainingRadius * Math.Sin(angle);
                    Generate(dim + 1, currentPoint, newRemainingRadius);
                }
            }

            Generate(0, new double[n], radius);
            return points.ToArray();
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

        public static double[] ProjectTo2D(double[] point, double scale = 1)
        {
            if (point.Length < 2)
            {
                return new double[0];
            }
            
            double[] projection = new double[2];
            projection[0] = point[0];
            projection[1] = point[1];

            for (int i = 2; i < point.Length; i++)
            {
                double distance = DistanceToXYPlane(point) / scale;
                if (distance != 0)
                {
                    projection[0] /= distance;
                    projection[1] /= distance;
                }
            }

            return projection;
        }

        public static double[][] ProjectTo2D(double[][] points, double scale = 1)
        {
            double[][] projections = new double[points.Length][];

            for (int i = 0; i < points.Length; i++)
            {
                projections[i] = ProjectTo2D(points[i]);
            }

            return projections;
        }


        public static double DistanceToXYPlane(double[] point)
        {
            Vector v = new Vector(point.Skip(2).ToArray());
            return v.Norm;
        }

        public static double AverageDistance(double x, double y, double[][] points, double tolerance = double.PositiveInfinity)
        {
            double result = 0;
            int added_points = 0;
            foreach (double[] point in points)
            {
                Vector v = new Vector(point) - new Vector(x, y);

                double norm = v.Norm;

                if (norm < tolerance)
                {
                    result += v.Norm;
                    added_points++;
                }
            }
            result /= added_points;
            return result;
        }

        public static double[][] ProjectToVectorSpace(double[][] points, double[] origin, double[][] basis)
        {
            List<Vector> b = new List<Vector>();
            for (int i = 0; i < basis.Length; i++)
            {
                b.Add(new Vector(basis[i]));
            }

            VectorSpace vs = new VectorSpace(new Vector(origin), b.ToArray());

            double[][] result = new double[points.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = vs.ToUnitSpace(new Vector(points[i])).Coordinates;
            }

            return result;
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

        // Distance between two points
        public static double Distance(double[] point1, double[] point2)
        {
            return (new Vector(point1) - new Vector(point2)).Norm;
        }

        // Angle between two points in radians
        public static double AngleBetweenPoints(double[] point1, double[] point2)
        {
            Vector v1, v2;
            v1 = new Vector(point1);
            v2 = new Vector(point2);
            double cos_th = v1 * v2 / (v1.Norm * v2.Norm);

            return Math.Acos(cos_th);
        }

        public static double[] ClosestPoint(double[][] points, double[] target)
        {
            double[] closest = points[0];
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
    }
}
