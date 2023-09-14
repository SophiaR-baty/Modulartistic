using NCalc;
using System;
using System.IO;
using System.Reflection;

namespace Modulartistic.Core
{
    public class Function
    {
        private Expression expression;

        public Function(Expression expression)
        {
            this.expression = expression;
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
            expression.Options = 
        }

        public Function(string expression)
        {
            this.expression = new Expression(expression);
            this.expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
        }

        public double Evaluate(double x, double y, double[] paras, double mod)
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

            // Console.WriteLine((double)expression.Evaluate());
            return (double)expression.Evaluate();
        }

        public void LoadAddOn(string dll_path)
        {

            if (!File.Exists(dll_path)) { return; }

            Assembly testDLL = Assembly.LoadFile(dll_path);

            // enumerates all public classes/interfaces/enums/etc. 
            // -> Classes in a plugin should only be public if they should expose functions to the parser
            foreach (Type type in testDLL.GetTypes())
            {
                // gets all public static methods of the type
                // -> only methods that should be exposed to the parser should be public static
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                // iterates over all such methods
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    if (methodInfo.ReturnType == typeof(double))
                    {
                        expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                                // checks if the function args are of correct value, otherwise returns (error)
                                for (int i = 0; i < paraInf.Length; i++)
                                {
                                    if (i >= parameters.Length && paraInf[i].IsOptional) { break; }
                                    // this should have fixed the bug of ints not being able to be interpreted as doubles
                                    // was simply comparing the types before
                                    if (!paraInf[i].ParameterType.IsAssignableFrom(parameters[i].GetType())) { throw new ArgumentException("Wrong argument type for " + methodInfo.Name + " at argument " + i + ". Expected a " + paraInf[i].GetType().ToString() + " and got a " + parameters[i].GetType().ToString()); }
                                }

                                // Console.WriteLine((double)methodInfo.Invoke(null, parameters));
                                args.Result = (double)methodInfo.Invoke(null, parameters);
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(int))
                    {
                        expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }


                                // checks if the function args are of correct value, otherwise returns (error)
                                for (int i = 0; i < paraInf.Length; i++)
                                {
                                    if (i >= parameters.Length && paraInf[i].IsOptional) { break; }
                                    if (parameters[i].GetType() != paraInf[i].ParameterType) { throw new ArgumentException("Wrong argument type for " + methodInfo.Name + " at argument " + i + ". Expected a " + paraInf[i].GetType().ToString() + " and got a " + parameters[i].GetType().ToString()); }
                                }

                                args.Result = (int)methodInfo.Invoke(null, parameters);
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(string))
                    {
                        expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }


                                // checks if the function args are of correct value, otherwise returns (error)
                                for (int i = 0; i < paraInf.Length; i++)
                                {
                                    if (i >= parameters.Length && paraInf[i].IsOptional) { break; }
                                    if (parameters[i].GetType() != paraInf[i].ParameterType) { throw new ArgumentException("Wrong argument type for " + methodInfo.Name + " at argument " + i + ". Expected a " + paraInf[i].GetType().ToString() + " and got a " + parameters[i].GetType().ToString()); }
                                }

                                args.Result = (string)methodInfo.Invoke(null, parameters);
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(bool))
                    {
                        expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }


                                // checks if the function args are of correct value, otherwise returns (error)
                                for (int i = 0; i < paraInf.Length; i++)
                                {
                                    if (i >= parameters.Length && paraInf[i].IsOptional) { break; }
                                    if (parameters[i].GetType() != paraInf[i].ParameterType) { throw new ArgumentException("Wrong argument type for " + methodInfo.Name + " at argument " + i + ". Expected a " + paraInf[i].GetType().ToString() + " and got a " + parameters[i].GetType().ToString()); }
                                }

                                args.Result = (bool)methodInfo.Invoke(null, parameters);
                            }
                        };
                    }

                    else { continue; }
                }
            }
        }

        public void LoadAddOns(string[] dll_paths)
        {
            foreach (string dll in dll_paths)
            {
                LoadAddOn(Helper.GetAbsolutePath(dll));
            }
        }

        public bool IsValid()
        {
            expression.Parameters["x"] = 1;
            expression.Parameters["y"] = 1;
            expression.Parameters["Th"] = 1;
            expression.Parameters["r"] = 1;
            expression.Parameters["num"] = 1;

            expression.Parameters["i_0"] = 1;
            expression.Parameters["i"] = 1;
            expression.Parameters["i_1"] = 1;
            expression.Parameters["j"] = 1;
            expression.Parameters["i_2"] = 1;
            expression.Parameters["i_3"] = 1;
            expression.Parameters["i_4"] = 1;
            expression.Parameters["i_5"] = 1;
            expression.Parameters["i_6"] = 1;
            expression.Parameters["i_7"] = 1;
            expression.Parameters["i_8"] = 1;
            expression.Parameters["i_9"] = 1;
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
            return !expression.HasErrors();
        }
    }
}
