using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

/* FULLY AI GENERATED */
/* ChECK IF THIS IS GOOD CODE ETC */

namespace Modulartistic.AddOns.Geometry
{
    public class KDTree
    {
        private KDTreeNode Root;

        public KDTree(IEnumerable<Complex> points)
        {
            Root = BuildTree(points.ToList(), depth: 0);
        }

        private KDTreeNode BuildTree(List<Complex> points, int depth)
        {
            if (points == null || points.Count == 0) return null;

            int axis = depth % 2; // Alternate between X and Y axis (0 = X, 1 = Y)
            points.Sort((a, b) => axis == 0
                                  ? a.Real.CompareTo(b.Real)
                                  : a.Imaginary.CompareTo(b.Imaginary));

            int medianIndex = points.Count / 2;
            Complex medianPoint = points[medianIndex];

            var node = new KDTreeNode(medianPoint, axis);
            node.Left = BuildTree(points.GetRange(0, medianIndex), depth + 1);
            node.Right = BuildTree(points.GetRange(medianIndex + 1, points.Count - medianIndex - 1), depth + 1);

            return node;
        }

        public Complex FindNearest(Complex targetPoint)
        {
            return FindNearest(Root, targetPoint, Root.Point, double.MaxValue);
        }

        private Complex FindNearest(KDTreeNode node, Complex targetPoint, Complex bestPoint, double bestDistance)
        {
            if (node == null) return bestPoint;

            double distance = DistanceSquared(node.Point, targetPoint);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPoint = node.Point;
            }

            int axis = node.SplitDimension;
            bool goLeft = (axis == 0)
                            ? targetPoint.Real < node.Point.Real
                            : targetPoint.Imaginary < node.Point.Imaginary;

            KDTreeNode first = goLeft ? node.Left : node.Right;
            KDTreeNode second = goLeft ? node.Right : node.Left;

            bestPoint = FindNearest(first, targetPoint, bestPoint, bestDistance);
            bestDistance = DistanceSquared(bestPoint, targetPoint);

            // Check the other side if necessary
            double splitDistance = axis == 0
                                   ? Math.Pow(targetPoint.Real - node.Point.Real, 2)
                                   : Math.Pow(targetPoint.Imaginary - node.Point.Imaginary, 2);

            if (splitDistance < bestDistance)
            {
                bestPoint = FindNearest(second, targetPoint, bestPoint, bestDistance);
            }

            return bestPoint;
        }

        private double DistanceSquared(Complex p1, Complex p2)
        {
            return Math.Pow(p1.Real - p2.Real, 2) + Math.Pow(p1.Imaginary - p2.Imaginary, 2);
        }
    }

    internal class KDTreeNode
    {
        public Complex Point { get; set; }
        public KDTreeNode Left { get; set; }
        public KDTreeNode Right { get; set; }
        public int SplitDimension { get; set; } // 0 for X-axis, 1 for Y-axis

        public KDTreeNode(Complex point, int splitDimension)
        {
            Point = point;
            SplitDimension = splitDimension;
            Left = null;
            Right = null;
        }
    }
}
