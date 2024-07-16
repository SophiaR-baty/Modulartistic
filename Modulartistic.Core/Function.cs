using NCalc;
using System;
using System.IO;
using System.Reflection;
using Modulartistic.AddOns;

namespace Modulartistic.Core
{
    public class Function
    {
        private Expression _expression;

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class using an existing <see cref="Expression"/> object.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> object to be used for this function.</param>
        public Function(Expression expression)
        {
            _expression = expression;
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class by creating a new <see cref="Expression"/> object from the specified string.
        /// </summary>
        /// <param name="expression">A string representing the expression to be evaluated.</param>
        public Function(string expression)
        {
            _expression = new Expression(expression);
            _expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
        }
        #endregion


        public double Evaluate(double x, double y)
        {

            _expression.Parameters["x"] = x;
            _expression.Parameters["y"] = y;
            _expression.Parameters["Th"] = 180 * Math.Atan2(y, x) / Math.PI;
            _expression.Parameters["r"] = Math.Sqrt(x * x + y * y);

<<<<<<< HEAD
            object res = _expression.Evaluate();
=======
            // Console.WriteLine((double)m_expression.Evaluate());
            object res = m_expression.Evaluate();
>>>>>>> 7b7e6fbe81380a6166298393c317ff4ed76eef5c

            if (res is int)
            {
                return Convert.ToDouble((int)res);
            }
            else if (res is double)
            {
                return (double)res;
            }
            else
            {
                throw new InvalidOperationException("The result is neither an int nor a double.");
            }
        }

        public void RegisterStateProperties(State s, StateOptions args)
        {
            Helper.ExprRegisterStateProperties(ref _expression, s);
            Helper.ExprRegisterStateOptions(ref _expression, args);
        }

        /// <summary>
        /// Load AddOns from a dll file. The dll should contain a type marked with the AddOn Attribute from Modulartistic.AddOns, All public static of these types are exposed to the parser
        /// </summary>
        /// <param name="dll_path"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void LoadAddOn(string dll_path)
        {
            if (!File.Exists(dll_path)) 
            { 
                throw new FileNotFoundException($"Error loading AddOn: {dll_path} - file not found"); 
            }

            Assembly testDLL = Assembly.LoadFile(dll_path);

            // enumerates all public classes/interfaces/enums/etc. 
            // -> Classes in a plugin should only be public if they should expose functions to the parser
            foreach (Type type in testDLL.GetTypes().Where(type => type.GetCustomAttribute(typeof(AddOnAttribute)) is not null))
            {
                // gets all public static methods of the type
                // -> only methods that should be exposed to the parser should be public static
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                // iterates over all such methods
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    if (methodInfo.ReturnType == typeof(double))
                    {
                        _expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                                args.Result = methodInfo.Invoke(null, parameters) as double?;
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(int))
                    {
                        _expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                                args.Result = methodInfo.Invoke(null, parameters) as int?;
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(string))
                    {
                        _expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                        {
                            if (name == methodInfo.Name)
                            {
                                ParameterInfo[] paraInf = methodInfo.GetParameters();
                                if (args.Parameters.Length > paraInf.Length) { throw new ArgumentException("Too many Arguments for function " + methodInfo.Name); }

                                object[] parameters = new object[args.Parameters.Length];
                                for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                                args.Result = methodInfo.Invoke(null, parameters) as string;
                            }
                        };
                    }

                    else if (methodInfo.ReturnType == typeof(bool))
                    {
                        _expression.EvaluateFunction += delegate (string name, FunctionArgs args)
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

        /// <summary>
        /// Load AddOns from a Collection of strings containing paths to dll_files
        /// </summary>
        /// <param name="dll_paths"></param>
        public void LoadAddOns(IEnumerable<string> dll_paths)
        {
            foreach (string dll in dll_paths)
            {
                LoadAddOn(Helper.GetAbsolutePath(dll));
            }
        }
    }
}
