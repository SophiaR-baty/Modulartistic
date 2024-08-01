using NCalc;
using System;
using System.IO;
using System.Reflection;
using Modulartistic.AddOns;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using FFMpegCore;
using System.Collections.Concurrent;

namespace Modulartistic.Core
{
    public class Function
    {
        private Expression _expression;
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

        private static ConcurrentDictionary<string, Type> _addOnCache = new ConcurrentDictionary<string, Type>();

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class using an existing <see cref="Expression"/> object.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> object to be used for this function.</param>
        public Function(Expression expression)
        {
            _expression = expression;
            RegisterConversionFunctions();
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class by creating a new <see cref="Expression"/> object from the specified string.
        /// </summary>
        /// <param name="expression">A string representing the expression to be evaluated.</param>
        public Function(string expression) : this(new Expression(expression)) { }

        #endregion


        public double Evaluate()
        {
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
            }
        }

        /// <summary>
        /// Load AddOns from a dll file. The dll should contain a type marked with the AddOn Attribute from Modulartistic.AddOns, All public static of these types are exposed to the parser
        /// </summary>
        /// <param name="dll_path"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void LoadAddOn(string dll_path, State s, StateOptions sOpts, GenerationOptions gOpts)
        {
            if (!File.Exists(dll_path))
            {
                throw new FileNotFoundException($"Error loading AddOn: {dll_path} - file not found");
            }

            Assembly testDLL = Assembly.LoadFile(dll_path);

            // enumerates all public classes/interfaces/enums/etc. 
            // -> Classes in a plugin should only be public if they should expose functions to the parser
            Type[] typeInfos = testDLL.GetTypes().Where(type => type.GetCustomAttribute(typeof(AddOnAttribute)) is not null).ToArray();


            for (int i = 0; i < typeInfos.Length; i++)
            {
                Type type = typeInfos[i];
                MethodInfo[] methodInfos;
                if (!_addOnCache.ContainsKey(type.FullName)) 
                {
                    // gets all public static methods of the type
                    // -> only methods that should be exposed to the parser should be public static
                    _addOnCache.TryAdd(type.FullName, type);
                    methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                    // initialize if exists
                    MethodInfo? initMethod = type.GetMethod("Initialize");
                    initMethod?.Invoke(null, [s, sOpts, gOpts]);
                }
                type = _addOnCache[type.FullName];
                methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                // iterates over all such methods
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    _expression.RegisterMethod(null, methodInfo);
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
            _expression.RegisterMethod(obj, mInfo);
        }

        /// <summary>
        /// Load AddOns from a Collection of strings containing paths to dll_files
        /// </summary>
        /// <param name="dll_paths"></param>
        public void LoadAddOns(IEnumerable<string> dll_paths, State s, StateOptions sOpts, GenerationOptions gOpts)
        {
            IPathProvider pathProvider = gOpts.PathProvider;
            foreach (string dll in dll_paths)
            {
                LoadAddOn(Helper.GetAddOnPath(dll, pathProvider), s, sOpts, gOpts);
            }
        }
    }
}
