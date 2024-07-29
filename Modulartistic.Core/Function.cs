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
        private HashSet<string> _registeredSymbols;
        private readonly HashSet<Type> _allowedResultTypes = new HashSet<Type>()
        {
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
        };

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class using an existing <see cref="Expression"/> object.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> object to be used for this function.</param>
        public Function(Expression expression)
        {
            _expression = expression;
            _registeredSymbols = new HashSet<string>();
            RegisterConversionFunctions();
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class by creating a new <see cref="Expression"/> object from the specified string.
        /// </summary>
        /// <param name="expression">A string representing the expression to be evaluated.</param>
        public Function(string expression) : this(new Expression(expression)) { }

        #endregion


        public double Evaluate(double x, double y)
        {
            _expression.Parameters["x"] = x;
            _expression.Parameters["y"] = y;
            _expression.Parameters["Th"] = 180 * Math.Atan2(y, x) / Math.PI;
            _expression.Parameters["r"] = Math.Sqrt(x * x + y * y);

            _registeredSymbols.Add("x");
            _registeredSymbols.Add("y");
            _registeredSymbols.Add("Th");
            _registeredSymbols.Add("r");

            object res = _expression.Evaluate();

            if (_allowedResultTypes.Contains(res.GetType()))
            {
                return Convert.ToDouble(res);
            }
            else
            {
                throw new InvalidOperationException($"The result is not a supported type: {res.GetType().Name}");
            }
        }

        public void RegisterStateProperties(State s, StateOptions args)
        {
            Helper.ExprRegisterStateProperties(ref _expression, s);
            Helper.ExprRegisterStateOptions(ref _expression, args);
        }

        /// <summary>
        /// registers conversion methods like int() or Double() to the function
        /// </summary>
        private void RegisterConversionFunctions()
        {
            foreach (Type type in _allowedResultTypes)
            {
                _expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                {
                    if (name == type.Name || name == Helper.GetPrimitiveName(type))
                    {
                        args.Result = Convert.ChangeType(args.Parameters[0].Evaluate(), type);
                    }
                };
                _registeredSymbols.Add(type.Name);
                _registeredSymbols.Add(Helper.GetPrimitiveName(type));
            }
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

                // iterates over all such methods
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    // if method with name has already been registered, skip iteration
                    if (_registeredSymbols.Contains(methodInfo.Name)) { continue; }
                    
                    MethodInfo[] overloads = type.GetMethods()
                        .Where(m => m.Name == methodInfo.Name)
                        .ToArray();

                    // register method
                    _expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                    {
                        if (name == methodInfo.Name || name == $"{type.Name}_{methodInfo.Name}")
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
                            if (invokedMethod is null) 
                            {
                                string specified = $"{name}(";
                                specified += string.Join(", ", args.Parameters.Select(p => $"{p.Evaluate().GetType().Name}"));
                                specified.Trim(' ', ',');
                                specified += ")";

                                string[] foundOverloads = new string[overloads.Length];
                                for (int i = 0; i < overloads.Length; i++)
                                {
                                    string methodName = overloads[i].Name;
                                    string parametersString = string.Join(", ", overloads[i].GetParameters().Select(p => $"{p.ParameterType.Name}"));

                                    foundOverloads[i] = $"{methodName}({parametersString})";
                                }
                                string message = "No overload matches arguments\n";
                                message += $"Specified prototype: {specified} \n";
                                message += "Found prototypes: \n";
                                foreach (string prot in foundOverloads) { message += prot + "\n"; }
                                
                                throw new ArgumentException(message); 
                            }

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
                    _registeredSymbols.Add(methodInfo.Name);
                }
            }
        }

        /// <summary>
        /// Registers a single Parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public void RegisterParameter(string name, object value)
        {
            _expression.Parameters[name] = value;
            _registeredSymbols.Add(name);
        }

        /// <summary>
        /// Registers a method via methodInfo and object to invoke on
        /// </summary>
        /// <param name="mInfo">the method info of the method</param>
        /// <param name="obj">the object to invoke the method for</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public void RegisterFunction(MethodInfo mInfo, object obj)
        {
            // if method with name has already been registered, skip iteration
            if (_registeredSymbols.Contains(mInfo.Name)) { return; }

            Type? type = mInfo.DeclaringType;
            if (type == null) { return; }


            MethodInfo[] overloads = type.GetMethods()
                .Where(m => m.Name == mInfo.Name)
                .ToArray();

            // register method
            _expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == mInfo.Name || name == $"{type.Name}_{mInfo.Name}")
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
                    if (invokedMethod is null)
                    {
                        string specified = $"{name}(";
                        specified += string.Join(", ", args.Parameters.Select(p => $"{p.Evaluate().GetType().Name}"));
                        specified.Trim(' ', ',');
                        specified += ")";

                        string[] foundOverloads = new string[overloads.Length];
                        for (int i = 0; i < overloads.Length; i++)
                        {
                            string methodName = overloads[i].Name;
                            string parametersString = string.Join(", ", overloads[i].GetParameters().Select(p => $"{p.ParameterType.Name}"));

                            foundOverloads[i] = $"{methodName}({parametersString})";
                        }
                        string message = "No overload matches arguments\n";
                        message += $"Specified prototype: {specified} \n";
                        message += "Found prototypes: \n";
                        foreach (string prot in foundOverloads) { message += prot + "\n"; }

                        throw new ArgumentException(message);
                    }

                    // create parameters
                    ParameterInfo[] paraInf = invokedMethod.GetParameters();
                    object[] parameters = new object[paraInf.Length];

                    // fill parameters with passed parameters
                    for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                    // fill remaining parameters with default arameters
                    for (int i = args.Parameters.Length; i < paraInf.Length; i++)
                    {
                        if (paraInf[i].HasDefaultValue) { parameters[i] = paraInf[i].DefaultValue; }
                        else { throw new Exception($"Parameter {paraInf[i].Name} of method {mInfo.Name} has no default value, you must specify this parameter."); }
                    }

                    object? result = invokedMethod.Invoke(obj, parameters) ?? throw new Exception("Result of Functions may not be null"); ;
                    args.Result = Convert.ChangeType(result, invokedMethod.ReturnType);
                }
            };

            // add method to registered
            _registeredSymbols.Add(mInfo.Name);
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
