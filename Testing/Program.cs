using System;
using VectorImageFunctions;


namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Curve c = Curve.FromImage("C:\\Users\\Maxim\\Downloads\\cat.jpg", 130, 200);
            Console.WriteLine(c.distance(3, 2, 0.1));
            // BezierConverter.CannyEdgeDetector("C:\\Users\\Maxim\\Downloads\\cat.jpg");
            // BezierConverter.CannyExtern("C:\\Users\\Maxim\\Downloads\\cat.jpg");
        }
    }
}
