using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.Misc
{
    [AddOn]
    public static class MiscFunctions
    {
        private static List<int> GetNumInBase(int num, int num_base)
        {
            if (num < 0) { num *= -1; }
            if (num_base < 0) { num_base *= -1; }
            if (Math.Abs(num_base) <= 1) { new List<int>(); }

            List<int> digitList = new List<int>();
            int idx = 0;
            while (num > 0)
            {
                // Console.WriteLine(inum);
                digitList.Add(num % num_base);
                num /= num_base;
                idx++;
            }

            return digitList;
        }

        public static double Reverse(double num, double num_base)
        {
            int inum = (int)Math.Floor(num);
            int inum_base = (int)Math.Floor(num_base);

            if (inum < 0) { inum *= -1; }
            if (inum_base < 0) { inum_base *= -1; }
            if (Math.Abs(inum_base) <= 1) { return double.NaN; }

            List<int> digitList = GetNumInBase(inum, inum_base);

            // Convert the reversed number back to base 10
            int reversedNumBase10 = 0;
            for (int i = 0; i < digitList.Count; i++)
            {
                reversedNumBase10 = reversedNumBase10 * inum_base + digitList[i];
            }

            return reversedNumBase10;
        }

        public static double DigSum(double num, double num_base)
        {
            int inum = (int)Math.Floor(num);
            int inum_base = (int)Math.Floor(num_base);

            if (inum < 0) { inum *= -1; }
            if (inum_base < 0) { inum_base *= -1; }
            if (Math.Abs(inum_base) <= 1) { return double.NaN; }

            List<int> digitList = GetNumInBase(inum, inum_base);

            // Convert the reversed number back to base 10
            int digit_sum = 0;
            for (int i = 0; i < digitList.Count; i++)
            {
                digit_sum += digitList[i];
            }

            return digit_sum;
        }

        public static double RecDigSum(double num, double num_base)
        {
            int inum = (int)Math.Floor(num);
            int inum_base = (int)Math.Floor(num_base);

            if (inum < 0) { inum *= -1; }
            if (inum_base < 0) { inum_base *= -1; }
            if (Math.Abs(inum_base) <= 1) { return double.NaN; }

            int result = inum;
            for (; ; )
            {
                int digsum = (int)DigSum(result, inum_base);
                if (digsum == result) { break; }
                result = digsum;
            }

            return result;
        }

        public static double LeastSquares(double a, double b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            int result = 0;
            while (a * b != 0)
            {
                double b_ = Math.Min(a, b);
                double a_ = Math.Max(a, b) - b_;

                a = a_;
                b = b_;

                result++;
            }

            return result;
        }

        public static double GetNumberedSquare(double x, double y, double width, double height)
        {
            width = Math.Abs(width);
            height = Math.Abs(height);

            if (height * width == 0) { return double.NaN; }
            return Mod(y, height) * width + Mod(x, width);
        }

        public static double CoordToSqrSpiral(double x, double y)
        {
            double s = Math.Abs(x) >= Math.Abs(y) ? x : y;
            double d = x == s ? -1 : 1;

            if (s >= 0) { return 4 * s * s - x + y; }
            else { return 4 * s * s + d * (2 * s + x + y); }
        }

        public static double CoordToSqrSpiral2(double x, double y)
        {
            int ix = (int)x;
            int iy = (int)y;

            int s = Math.Abs(ix) >= Math.Abs(iy) ? ix : iy;
            int side_len = 2 * s + 1;
            int max_ring_num = side_len * side_len - 1;

            int result;

            if (x == -s)
            {
                result = max_ring_num - (iy + s);
            }
            else if (y == s)
            {
                result = max_ring_num - 2 * s - (ix + s);
            }
            else if (x == s)
            {
                result = max_ring_num - 4 * s - (-iy + s);
            }
            else
            {
                result = max_ring_num - 6 * s - (-ix + s);
            }

            return result;
        }

        public static double RecursiveTest(double x, double y, double r, double min_r)
        {
            if (r < min_r) { return x * y; }
            return RecursiveTest(x, y, r - 2, min_r) + RecursiveTest(x, y, r - 1, min_r) - r;
        }

        private static double Mod(double d1, double d2)
        {
            if (d2 <= 0)
                throw new DivideByZeroException();
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }
    }
}
