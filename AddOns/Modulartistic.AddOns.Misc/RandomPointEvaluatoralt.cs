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

    public class RandomPointEvaluatorAlt
    {
        private List<Complex> points;
        private List<double> probabilities;


        public RandomPointEvaluatorAlt(List<Complex> points, List<double> probabilities)
        {
            this.points = points ?? throw new ArgumentNullException(nameof(points));
            this.probabilities = probabilities ?? throw new ArgumentNullException(nameof(probabilities));

            double total_prob = this.probabilities.Sum();
            this.probabilities = this.probabilities.Select(p => p/total_prob).ToList();
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
                double random_d = random.NextDouble();
                int i;
                for (i = 0; i < probabilities.Count; i++)
                {
                    double p = probabilities[i];
                    if (random_d < probabilities[i]) { break; }
                    random_d -= p;
                }

                Complex direction = points[i] - currentPoint;
                Complex moveVector = direction / direction.Magnitude * distance;

                currentPoint = currentPoint + moveVector;

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
                double random_d = random.NextDouble();
                int i;
                for (i = 0; i < probabilities.Count; i++)
                {
                    double p = probabilities[i];
                    if (random_d < probabilities[i]) { break; }
                    random_d -= p;
                }

                Complex direction = points[i] - currentPoint;
                Complex moveVector = direction / direction.Magnitude * distance;
                
                currentPoint = currentPoint + moveVector;

                steps++;
            }
        }

        public static Complex MoveTowards(Complex current, Complex target, double distance)
        {
            Complex direction = target - current;
            Complex moveVector = direction / direction.Magnitude * distance;
            Complex newPoint = current + moveVector;

            return newPoint;
        }
    }
}
