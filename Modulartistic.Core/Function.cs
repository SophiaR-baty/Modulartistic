using NCalc;
using System;
using System.IO;
using System.Reflection;
using Modulartistic.AddOns;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using FFMpegCore;

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

            
            
            
            
            object res = _expression.Evaluate();

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
            Type[] typeInfos = testDLL.GetTypes().Where(type => type.GetCustomAttribute(typeof(AddOnAttribute)) is not null).ToArray();


            foreach (Type type in typeInfos)
            {
                // gets all public static methods of the type
                // -> only methods that should be exposed to the parser should be public static
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                
                // create a set for method names, to skip repeated ones
                HashSet<string> registeredMethods = new HashSet<string>();

                // iterates over all such methods
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    // if method with name has already been registered, skip iteration
                    if (registeredMethods.Contains(methodInfo.Name)) { continue; }
                    
                    MethodInfo[] overloads = type.GetMethods()
                        .Where(m => m.Name == methodInfo.Name)
                        .ToArray();

                    // register method
                    _expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                    {
                        if (name == methodInfo.Name || name == $"{type.Name}.{methodInfo.Name}")
                        {
                            int parameter_count = args.Parameters.Length;

                            // try to select overload
                            MethodInfo? invokedMethod = null;
                            foreach (MethodInfo overloadInfo in overloads)
                            {
                                ParameterInfo[] overloadParaInfo = overloadInfo.GetParameters();
                                if (parameter_count > overloadParaInfo.Length) { continue; }
                                else
                                {
                                    // check if parameter types match
                                    bool parameterTypesMismatch = false;
                                    for (int i = 0; i < parameter_count; i++)
                                    {
                                        if (!overloadParaInfo[i].ParameterType.IsInstanceOfType(args.Parameters[i].Evaluate()))
                                        {
                                            parameterTypesMismatch = true;
                                            break;
                                        }
                                    }
                                    // fill remaining parameters with default arameters
                                    for (int i = args.Parameters.Length; i < overloadParaInfo.Length; i++)
                                    {
                                        if (!overloadParaInfo[i].HasDefaultValue) 
                                        {
                                            parameterTypesMismatch = true;
                                            break;
                                        }
                                    }
                                    if (parameterTypesMismatch) { continue; }

                                    // set invoked method and break out of loop
                                    invokedMethod = overloadInfo;
                                    break;
                                }
                            }
                            // throw exception if no overload fits
                            if (invokedMethod is null) { throw new ArgumentException($"No overload matches arguments"); }

                            // create parameters
                            ParameterInfo[] paraInf = invokedMethod.GetParameters();
                            object[] parameters = new object[paraInf.Length];

                            // fill parameters with passed parameters
                            for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                            // fill remaining parameters with default arameters
                            for (int i = args.Parameters.Length; i < paraInf.Length; i++)
                            {
                                if (paraInf[i].HasDefaultValue) { parameters[i] = paraInf[i].DefaultValue; }
                                else { throw new Exception($"Parameter {paraInf[i].Name} of method {methodInfo.Name} has no default value, you must specify this parameter."); }
                            }

                            object? result = invokedMethod.Invoke(null, parameters) ?? throw new Exception("Result of Functions may not be null"); ;
                            args.Result = Convert.ChangeType(result, invokedMethod.ReturnType);
                        }
                    };

                    // add method to registered
                    registeredMethods.Add(methodInfo.Name);
                }
            }
        }

        /// <summary>
        /// Load AddOns from a Collection of strings containing paths to dll_files
        /// </summary>
        /// <param name="dll_paths"></param>
        public void LoadAddOns(IEnumerable<string> dll_paths, IPathProvider pathProvider)
        {
            foreach (string dll in dll_paths)
            {
                LoadAddOn(Helper.GetAddOnPath(dll, pathProvider));
            }
        }
    }
}
