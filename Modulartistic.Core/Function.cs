﻿using NCalc;
using System;
using System.IO;
using System.Reflection;
using Modulartistic.AddOns;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using FFMpegCore;
using System.Collections.Concurrent;
using Modulartistic.Common;

namespace Modulartistic.Core
{
    public class Function
    {
        private Expression? _expression;
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
        private string _function;

        public string FunctionString 
        {  
            get => _function;
        }


        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class by creating a new <see cref="Expression"/> object from the specified string.
        /// </summary>
        /// <param name="expression">A string representing the expression to be evaluated.</param>
        public Function(string expression) 
        {
            _function = expression;
            if (!string.IsNullOrEmpty(_function))
            {
                _expression = new Expression(_function);
                RegisterConversionFunctions();
                _expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
            }
            
        }

        public Function() : this("") { }

        #endregion

        public object Evaluate()
        {
            return _expression?.Evaluate() ?? 0;
        }

        public bool CanEvaluate()
        {
            return !_expression?.HasErrors() ?? true;
        }

        public void RegisterStateProperties(State s, StateOptions args)
        {
            if (_expression == null) { return; }
            Helper.ExprRegisterStateProperties(ref _expression, s);
            Helper.ExprRegisterStateOptions(ref _expression, args);
        }

        /// <summary>
        /// registers conversion methods like int() or Double() to the function
        /// </summary>
        private void RegisterConversionFunctions()
        {
            if (_expression == null) { return; }
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
        public void LoadAddOn(string dll_path, AddOnInitializationArgs initArgs)
        {
            if (_expression == null) { return; }
            if (!File.Exists(dll_path))
            {
                throw new FileNotFoundException($"Error loading AddOn: {dll_path} - file not found");
            }


            // Handles dependencies
            string addOnDependenciesPath = Path.GetFileNameWithoutExtension(dll_path);
            string addOnPath = Path.GetDirectoryName(dll_path);
            addOnDependenciesPath = Path.Combine(addOnPath, addOnDependenciesPath);
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var assemblyPath = Path.Combine(addOnDependenciesPath, new AssemblyName(args.Name).Name + ".dll");
                return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
            };

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
                    initMethod?.Invoke(null, [initArgs]);
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
            if (_expression == null) { return; }
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
            if (_expression == null) { return; }
            _expression.RegisterMethod(obj, mInfo);
        }

        /// <summary>
        /// Load AddOns from a Collection of strings containing paths to dll_files
        /// </summary>
        /// <param name="dll_paths"></param>
        public void LoadAddOns(IEnumerable<string> dll_paths, StateOptions sOpts, GenerationOptions gOpts)
        {
            if (_expression == null) { return; }
            IPathProvider pathProvider = gOpts.PathProvider;
            AddOnInitializationArgs initArgs = new AddOnInitializationArgs()
            {
                Framerate = sOpts.Framerate,
                Width = sOpts.Width,
                Height = sOpts.Height,
                Logger = gOpts.Logger,
                ProgressReporter = gOpts.ProgressReporter,
                Guid = gOpts.GenerationDataGuid,
            };
            foreach (string dll in dll_paths)
            {
                LoadAddOn(Helper.GetAddOnPath(dll, pathProvider), initArgs);
            }
        }
    }
}
