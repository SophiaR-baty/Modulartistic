using System;
using MiscFunctions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace AddOnTesting
{
    internal class Program
    {
        static void Main()
        {
            
            for (int i = -250; i <= 250; i++)
            {
                for (int j = -250; j <= 250; j++)
                {
                    Console.Write(i);
                    Console.Write(" ");
                    Console.Write(j);
                    Console.Write(": ");
                    Console.WriteLine(MiscFunctions.MiscFunctions.DigSum(555, i*j/500.0));
                }
            }
        }
    }
}