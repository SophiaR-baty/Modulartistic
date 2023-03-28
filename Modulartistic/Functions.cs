using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using MathFunctions;

namespace Modulartistic
{
    public class Functions
    {
        private static List<MethodInfo> AddOnMethods = new List<MethodInfo>();
        
        public static double mod(double d1, double d2)
        {
            if (d2 <= 0)
                throw new DivideByZeroException();
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }

        public static void LoadAddOn(string dll_path)
        {
            
            Assembly testDLL = Assembly.LoadFile(dll_path);
            foreach (Type type in testDLL.GetTypes())
            {
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                AddOnMethods.AddRange(methodInfos);
            }
        }

        public static double Custom(Expression expression, double x, double y, List<double> paras)
        {
            Expression func = expression;
            func.Parameters["x"] = x;
            func.Parameters["y"] = y;
            func.Parameters["Th"] = 180*Math.Atan2(y, x)/Math.PI;
            func.Parameters["r"] = Math.Sqrt(x*x+y*y);

            /*
            func.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                foreach (MethodInfo mi in AddOnMethods) 
                {
                    if (mi.ReturnType == Type.GetType("Double") && name == mi.Name) { args.Result = mi.Invoke(null, args.Parameters); break; }
                }
            };
            */
            func.Parameters["i_0"] = paras[0];
            func.Parameters["i"] = paras[0];
            func.Parameters["i_1"] = paras[1];
            func.Parameters["j"] = paras[1];
            func.Parameters["i_2"] = paras[2];
            func.Parameters["i_3"] = paras[3];
            func.Parameters["i_4"] = paras[4];
            func.Parameters["i_5"] = paras[5];
            func.Parameters["i_6"] = paras[6];
            func.Parameters["i_7"] = paras[7];
            func.Parameters["i_8"] = paras[8];
            func.Parameters["i_9"] = paras[9];
            func.Options = EvaluateOptions.UseDoubleForAbsFunction;
            Console.WriteLine(func.Evaluate());
            return (double)func.Evaluate();
        }

        private static List<MethodInfo> LoadAddonFunctions(string assembly_path)
        {
            List<MethodInfo> functions = new List<MethodInfo>();
            List<Type> types = new List<Type>();

            Assembly testDLL = Assembly.LoadFile(assembly_path);
            types.AddRange(testDLL.GetTypes());
            Console.WriteLine(types.Count);
            foreach (Type type in types)
            {
                Console.WriteLine(type.Name);
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                functions.AddRange(methodInfos);
            }
            return functions;
        }
    }
}
