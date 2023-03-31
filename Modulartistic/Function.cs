using NCalc;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Modulartistic
{
    public class Function
    {
        private Expression expression;

        public Function(Expression expression) 
        {
            this.expression = expression;
        }

        public Function(string expression)
        {
            this.expression = new Expression(expression);
        }

        public double Evaluate(double x, double y, List<double> paras, double mod)
        {
            expression.Parameters["x"] = x;
            expression.Parameters["y"] = y;
            expression.Parameters["Th"] = 180 * Math.Atan2(y, x) / Math.PI;
            expression.Parameters["r"] = Math.Sqrt(x * x + y * y);
            expression.Parameters["num"] = mod;

            expression.Parameters["i_0"] = paras[0];
            expression.Parameters["i"] = paras[0];
            expression.Parameters["i_1"] = paras[1];
            expression.Parameters["j"] = paras[1];
            expression.Parameters["i_2"] = paras[2];
            expression.Parameters["i_3"] = paras[3];
            expression.Parameters["i_4"] = paras[4];
            expression.Parameters["i_5"] = paras[5];
            expression.Parameters["i_6"] = paras[6];
            expression.Parameters["i_7"] = paras[7];
            expression.Parameters["i_8"] = paras[8];
            expression.Parameters["i_9"] = paras[9];
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
            return (double)expression.Evaluate();
        }

        public void LoadAddOn(string dll_path)
        {
            
            Assembly testDLL = Assembly.LoadFile(dll_path);
            foreach (Type type in testDLL.GetTypes())
            {
                // Console.WriteLine(type.FullName);
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                // foreach (MethodInfo methodInfo in methodInfos) { Console.WriteLine(methodInfo.Name); }
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                    {
                        if (name == methodInfo.Name)
                        {
                            object[] parameters = new object[args.Parameters.Length]; 
                            for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }
                            args.Result = (double)methodInfo.Invoke(null, parameters); 
                        }
                    };
                }
            }
        }

        public void LoadAddOns(string[] dll_paths)
        {
            foreach (string dll in dll_paths)
            {
                LoadAddOn(dll);
            }
        }
    }
}
