using System;
using Modulartistic.Core;

namespace Debug
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Point p = new Point(5, 5);
            VectorSpace vs = new VectorSpace()
                .ScaleBy(1, -1)
                .ShiftPointToPoint(new Point(250, 250), new Point(-100, -100))
                .RotateAroundPoint(new Point(50, -50), 90)
                ;
            Console.WriteLine(vs.GetProjected(new Point(400, 200)));
        }
    }
}
