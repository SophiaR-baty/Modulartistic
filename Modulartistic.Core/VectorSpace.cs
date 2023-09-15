using System;
using System.Collections.Generic;
using System.Text;

namespace Modulartistic.Core
{
    class VectorSpace
    {
        private List<ITransformation> transformations;

        public VectorSpace()
        {
            transformations = new List<ITransformation>();
        }

        /// <summary>
        /// Shifts the Point to the current origin
        /// </summary>
        /// <param name="p">The point to shift to</param>
        /// <returns></returns>
        public VectorSpace ShiftPointToOrigin(Point p)
        {
            Point pr = GetProjected(p);
            transformations.Add(new Shift(-pr.X, -pr.Y));
            return this;
        }

        /// <summary>
        /// Shifts the original origin to the specified Point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public VectorSpace ShiftOriginToPoint(Point p)
        {
            Point pr = GetProjected(Point.Origin);
            transformations.Add(new Shift(-pr.X - p.X, -pr.Y - p.Y));
            return this;
        }

        /// <summary>
        /// Shifts the original origin to the current origin. 
        /// </summary>
        /// <returns></returns>
        public VectorSpace ResetOrigin()
        {
            return ShiftPointToOrigin(Point.Origin);
        }

        /// <summary>
        /// Shifts everything by x and y
        /// </summary>
        /// <param name="x">the x value to shift by</param>
        /// <param name="y">the y value to shift by</param>
        /// <returns></returns>
        public VectorSpace ShiftBy(double x, double y)
        {
            transformations.Add(new Shift(x, y));
            return this;
        }

        /// <summary>
        /// Scales x and y coordinates be specified value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public VectorSpace ScaleBy(double x, double y)
        {
            transformations.Add(new Scale(x, y));
            return this;
        }

        /// <summary>
        /// Rotates the Vectorspace by specified amount of degrees
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public VectorSpace RotateBy(double degrees)
        {
            transformations.Add(new Rotate(degrees));
            return this;
        }

        public VectorSpace RotateAroundPoint(Point p, double degrees)
        {
            Point old_origin = GetProjected(Point.Origin);
            ShiftPointToOrigin(p);
            RotateBy(degrees);
            ShiftPointToOrigin(old_origin);
            return this;
        }




        public Point GetProjected(double x, double y)
        {
            return GetProjected(new Point(x, y));
        }

        public Point GetProjected(Point p)
        {
            if (transformations.Count == 0) { return p; }
            
            Point current = p;
            for (int i = 0; i < transformations.Count; i++) { transformations[i].Apply(ref current); }
            return current;
        }

        #region transformations
        interface ITransformation
        {
            public void Apply(ref Point p);
        }
        
        class Shift : ITransformation
        {
            private double _x;
            private double _y;
            public Shift(double x, double y)
            {
                _x = x;
                _y = y;
            }

            public void Apply(ref Point p)
            {
                p.X += _x;
                p.Y += _y;
            }
        }
        
        class Scale : ITransformation
        {
            private double _x;
            private double _y;
            public Scale(double x, double y)
            {
                _x = x;
                _y = y;
            }

            public void Apply(ref Point p)
            {
                p.X *= _x;
                p.Y *= _y;
            }
        }

        class Rotate : ITransformation
        {
            private double _deg;
            private double _sindeg;
            private double _cosdeg;

            public Rotate(double degrees)
            {
                _deg = 2 * Math.PI * degrees / 360;
                _sindeg = Math.Sin(_deg);
                _cosdeg = Math.Cos(_deg);
            }

            public void Apply(ref Point p)
            {
                double _x = p.X * _cosdeg - p.Y * _sindeg;
                p.Y = p.X * _sindeg + p.Y * _cosdeg;
                p.X = _x;
            }
        }
        #endregion
    }

    class Point
    {
        private double _x;
        private double _y;

        public static Point Origin = new Point(0, 0);
        
        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }

        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }
    }
}
