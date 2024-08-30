using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.Misc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;

    public class RandomPointEvaluator
    {
        private List<Complex> points;
        private double probability;


        public RandomPointEvaluator(List<Complex> points, double probability)
        {
            this.points = points ?? throw new ArgumentNullException(nameof(points));
            this.probability = probability;
        }

        private double Distance(Complex p1, Complex p2)
        {
            return Math.Sqrt(Math.Pow(p1.Real - p2.Real, 2) + Math.Pow(p1.Imaginary - p2.Imaginary, 2));
        }

        public Complex Evaluate(Complex initialPoint, out int steps)
        {
            steps = 0;
            Complex currentPoint = initialPoint;

            while (true)
            {
                // Find the closest point in the list to the current point
                Complex closestPoint = points.OrderBy(p => Distance(currentPoint, p)).First();
                double distance = Distance(currentPoint, closestPoint);

                // If the current point is close enough to one of the points, stop the procedure
                if (distance < 1e-6)  // This assumes a very small threshold for floating-point precision
                {
                    return closestPoint;
                }

                // Randomly decide whether to move towards or away from the closest point
                Random random = new Random();
                double factor = random.NextDouble() < probability ? 1 : -1;

                // Calculate the new point
                double newX = currentPoint.Real + factor * (closestPoint.Real - currentPoint.Real);
                double newY = currentPoint.Imaginary + factor * (closestPoint.Imaginary - currentPoint.Imaginary);
                currentPoint = new Complex(newX, newY);

                steps++;
            }
        }

        public int EvaluateIdx(Complex initialPoint, out int steps)
        {
            steps = 0;
            Complex currentPoint = initialPoint;

            while (true)
            {
                // Find the closest point in the list to the current point
                Complex closestPoint = points.OrderBy(p => Distance(currentPoint, p)).First();
                double distance = Distance(currentPoint, closestPoint);

                int result = points.IndexOf(currentPoint);
                if (result > -1) { return result; }

                // Randomly decide whether to move towards or away from the closest point
                Random random = new Random();
                double factor = random.NextDouble() < probability ? 1 : -1;

                // Calculate the new point
                double newX = currentPoint.Real + factor * (closestPoint.Real - currentPoint.Real);
                double newY = currentPoint.Imaginary + factor * (closestPoint.Imaginary - currentPoint.Imaginary);
                currentPoint = new Complex(newX, newY);

                steps++;
            }
        }
    }
}
