using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/*
THIS DOES NOT CURRENTLY WORK THE WAY I WANT IT.
in theory it's fine but what I didn't consider is that all the array assignments are by reference and not by value
so in the operators the operands get changed, also in other places reference assignments are made that shouldn't be made
to fix this we should be able to simply change assignments to copy to stuff, I already started that, but need to do at more places

ALSO CHECK IN NDGeometryFunctions.cs if there are similar mistakes/issues there!!!
 */






namespace Modulartistic.AddOns.Geometry
{
    internal class VectorSpace
    {
        public Vector Origin { get; private set; }
        public Vector[] Basis { get; private set; }

        public int Dimension { get => Basis.Length; }
        
        public VectorSpace(Vector origin, Vector[] basis)
        {
            Origin = origin;
            Basis = basis;
        }

        public Vector ToUnitSpace(Vector point)
        {
            if (point.Dimension > Dimension) { throw new Exception(); }



            Console.WriteLine($"{Basis[0]} {Basis[1]}");
            Console.WriteLine($"{point} to unit space");
            Vector result = new Vector(Origin);
            for (int i = 0; i < Dimension; i++) 
            {
                Console.WriteLine(result.ToString());
                result += point[i] * Basis[i];
            }
            Console.WriteLine(result.ToString());

            return result;
        }
    }

    internal class Vector
    {
        private double? _norm = null;

        public double[] Coordinates { get; private set; }
        public int Dimension { get => Coordinates.Length; }
        public double this[int i] 
        { 
            get 
            { 
                if (i >=  Dimension) { return 0; }
                return Coordinates[i]; 
            } 
            private set => Coordinates[i] = value;
        }

        public double Norm
        {
            get
            {
                if (_norm is null)
                {
                    // implements the Kahan summation algorithm
                    // https://en.wikipedia.org/wiki/Kahan_summation_algorithm

                    double sum = 0.0;
                    double c = 0.0;
                    for (int i = 0; i < Dimension; i++)
                    {
                        double y = Coordinates[i]* Coordinates[i] - c;
                        double t = sum + y;
                        c = (t - sum) - y;
                        sum = t;
                    }

                    return Math.Sqrt(sum);
                }
                return _norm.Value;
            }
        }

        public Vector(params double[] coordinates)
        {
            Coordinates = new double[coordinates.Length];
            coordinates.CopyTo(Coordinates, 0);
        }

        public Vector(Vector vec)
        {
            Coordinates = new double[vec.Dimension];
            vec.Coordinates.CopyTo(Coordinates, 0);
        }

        #region operators

        public static Vector operator +(Vector a, Vector b)
        {
            double[] x = a.Dimension > b.Dimension ? a.Coordinates : b.Coordinates;
            double[] y = a.Dimension < b.Dimension ? a.Coordinates : b.Coordinates;

            for (int i = 0; i < y.Length; i++)
            {
                x[i] += y[i];
            }

            return new Vector(x);
        }

        public static Vector operator -(Vector a)
        {
            double[] x = a.Coordinates;

            for (int i = 0; i < a.Dimension; i++)
            {
                x[i] = -x[i];
            }

            return new Vector(x);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return a + (-b);
        }

        public static Vector operator *(Vector v, double a)
        {
            double[] x = v.Coordinates;

            for (int i = 0; i < v.Dimension; i++)
            {
                x[i] *= a;
            }

            return new Vector(x);
        }

        public static Vector operator *(double a, Vector v)
        {
            return v * a;
        }



        public static double operator *(Vector v1, Vector v2)
        {
            // Dot product
            
            // implements the Kahan summation algorithm
            // https://en.wikipedia.org/wiki/Kahan_summation_algorithm


            int min_dim = Math.Min(v1.Dimension, v1.Dimension);

            double sum = 0.0;
            double c = 0.0;
            for (int i = 0; i < min_dim; i++)
            {
                double y = v1[i] * v2[i] - c;
                double t = sum + y;
                c = (t - sum) - y;
                sum = t;
            }

            return sum;
        }

        public static Vector operator /(Vector v, double a)
        {
            double[] x = v.Coordinates;

            for (int i = 0; i < v.Dimension; i++)
            {
                x[i] /= a;
            }

            return new Vector(x);
        }

        #endregion

        public override string ToString()
        {
            string s = "(";
            for (int i = 0; i < Dimension; i++)
            {
                double coord = Coordinates[i];
                s += $"{coord}";
                s += i < Dimension - 1 ? ", " : ")";
            }
            
            return s;
        }
    }
}
