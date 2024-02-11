using System;
using System.Collections.Generic;
using System.Reflection;

namespace MiscFunctions
{
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
            while (a*b != 0)
            {
                double b_ = Math.Min(a, b);
                double a_ = Math.Max(a, b) - b_;

                a = a_;
                b = b_;
                
                result++;
            }

            return result;
        }
    }
}
