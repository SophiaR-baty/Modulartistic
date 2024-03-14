using NCalc;
using System;
using System.IO;
using System.Reflection;

namespace Modulartistic.Core
{
    public class Function
    {
        private Expression m_expression;

        public Function(Expression expression)
        {
            m_expression = expression;
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
        }

        public Function(string expression)
        {
            m_expression = new Expression(expression);
            m_expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
        }

        public double Evaluate(double x, double y)
        {

            m_expression.Parameters["x"] = x;
            m_expression.Parameters["y"] = y;
            m_expression.Parameters["Th"] = 180 * Math.Atan2(y, x) / Math.PI;
            m_expression.Parameters["r"] = Math.Sqrt(x * x + y * y);

            // Console.WriteLine((double)m_expression.Evaluate());
            return (double)m_expression.Evaluate();
        }

        public void RegisterStateProperties(State s, GenerationArgs args)
        {
            Helper.ExprRegisterStateProperties(ref m_expression, s);
            Helper.ExprRegisterGenArgs(ref m_expression, args);
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
                        m_expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                                args.Result = (double)methodInfo.Invoke(null, parameters);
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(int))
                    {
                        m_expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                                args.Result = (int)methodInfo.Invoke(null, parameters);
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(string))
                    {
                        m_expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                                args.Result = (string)methodInfo.Invoke(null, parameters);
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(bool))
                    {
                        m_expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

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
            m_expression.Parameters["x"] = 1;
            m_expression.Parameters["y"] = 1;
            m_expression.Parameters["Th"] = 1;
            m_expression.Parameters["r"] = 1;
            m_expression.Parameters["num"] = 1;

            m_expression.Parameters["i_0"] = 1;
            m_expression.Parameters["i"] = 1;
            m_expression.Parameters["i_1"] = 1;
            m_expression.Parameters["j"] = 1;
            m_expression.Parameters["i_2"] = 1;
            m_expression.Parameters["i_3"] = 1;
            m_expression.Parameters["i_4"] = 1;
            m_expression.Parameters["i_5"] = 1;
            m_expression.Parameters["i_6"] = 1;
            m_expression.Parameters["i_7"] = 1;
            m_expression.Parameters["i_8"] = 1;
            m_expression.Parameters["i_9"] = 1;
            m_expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
            return !m_expression.HasErrors();
        }
    }
}
