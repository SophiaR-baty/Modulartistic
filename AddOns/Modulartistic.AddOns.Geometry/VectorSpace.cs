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

            Vector result = new Vector(Origin);
            for (int i = 0; i < Dimension; i++) 
            {
                result += point[i] * Basis[i];
            }

            return result;
        }
    }

    internal class Matrix
    {
        private double[,] _data;

        public int Rows => _data.GetLength(0);
        public int Columns => _data.GetLength(1);

        public Matrix(int rows, int columns)
        {
            _data = new double[rows, columns];
        }

        public double this[int row, int column]
        {
            get => row >= Rows || column >= Columns ? 0 : _data[row, column];
            set => _data[row, column] = value;
        }

        public void SetRow(int row, params double[] values)
        {
            for (int i = 0; i < Columns; i++)
            {
                this[row, i] = values[i];
            }
        }

        public void SetColumn(int column, params double[] values)
        {
            for (int i = 0; i < Columns; i++)
            {
                this[i, column] = values[i];
            }
        }

        public Matrix Transpose()
        {
            Matrix transposed = new Matrix(Columns, Rows);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    transposed[j, i] = this[i, j];
                }
            }
            return transposed;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            int max_rows = Math.Max(m1.Rows, m2.Rows);
            int max_cols = Math.Max(m1.Columns, m2.Columns);
            
            Matrix result = new(max_rows, max_cols);
            for (int i = 0; i < max_rows; i++)
            {
                for (int j = 0; j < max_cols; j++)
                {
                    for (int k = 0; k < max_cols; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }

            return result;
        }

        public static Vector operator *(Matrix m, Vector v)
        {
            Vector result = new Vector(m.Rows);
            for (int i = 0; i < result.Dimension; i++)
            {
                double sum = 0;
                for (int j = 0; j < m.Columns; j++)
                {
                    sum += m[i, j] * v[j];
                }
                result[i] = sum;
            }

            return result;
        }
    }

    internal class Vector
    {
        #region properties

        public double[] Coordinates { get; private set; }
        public int Dimension { get => Coordinates.Length; }
        public double this[int i] 
        { 
            get 
            { 
                if (i >=  Dimension) { return 0; }
                return Coordinates[i]; 
            } 
            set => Coordinates[i] = value;
        }

        public double Norm
        {
            get
            {
                // implements the Kahan summation algorithm
                // https://en.wikipedia.org/wiki/Kahan_summation_algorithm

                double sum = 0.0;
                double c = 0.0;
                for (int i = 0; i < Dimension; i++)
                {
                    double y = Coordinates[i] * Coordinates[i] - c;
                    double t = sum + y;
                    c = (t - sum) - y;
                    sum = t;
                }
                // Console.WriteLine(sum);
                return Math.Sqrt(sum);
            }
        }

        #endregion

        #region constructors

        public Vector(int length)
        {
            Coordinates = new double[length];
        }

        public Vector(params double[] coordinates)
        {
            Coordinates = coordinates.ToArray();
        }

        public Vector(Vector vec)
        {
            Coordinates = vec.Coordinates.ToArray();
        }

        #endregion

        #region operators

        public static Vector operator +(Vector a, Vector b)
        {
            int max_dim = Math.Max(a.Dimension, b.Dimension);
            double[] v = new double[max_dim];

            for (int i = 0; i < max_dim; i++)
            {
                v[i] = a[i] + b[i];
            }

            return new Vector(v);
        }

        public static Vector operator -(Vector a)
        {
            double[] x = a.Coordinates.ToArray();

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
            double[] x = v.Coordinates.ToArray();

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
            double[] x = v.Coordinates.ToArray();

            for (int i = 0; i < v.Dimension; i++)
            {
                x[i] /= a;
            }

            return new Vector(x);
        }

        #endregion

        public Vector Normalize()
        {
            return this / Norm; 
        }

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
