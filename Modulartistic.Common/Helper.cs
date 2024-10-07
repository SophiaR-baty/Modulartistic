using NCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Modulartistic.Common
{
    public static class Helper
    {
        /// <summary>
        /// Register a custom method info to this expressions EvaluateFunction.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="obj"></param>
        /// <param name="mInfo"></param>
        /// <param name="custom_name"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static void RegisterMethod(this Expression expr, object? obj, MethodInfo mInfo, string custom_name = "")
        {
            Type? type = mInfo.DeclaringType;
            if (type == null) { return; }

            // register method
            expr.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (args.HasResult) { return; }
                if ((custom_name == "" && (name == mInfo.Name || name == $"{type.Name}_{mInfo.Name}")) || (custom_name != "" && name == custom_name))
                {
                    int parameter_count = args.Parameters.Length;

                    // create parameters
                    ParameterInfo[] paraInf = mInfo.GetParameters();
                    int mInfoPCount = paraInf.Length;
                    object[] parameters = new object[mInfoPCount];


                    if (parameter_count > paraInf.Length && paraInf[mInfoPCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length == 0)
                    {
                        args.HasResult = false;
                        return;
                    }

                    // fill parameters with passed parameters
                    for (int i = 0; i < mInfoPCount; i++)
                    {
                        if (paraInf[i].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
                        {
                            int params_count = parameter_count - mInfoPCount;
                            // object[] params_parameter = new object[params_count+1];

                            // Get the type of the elements in the params array
                            Type paramsElementType = paraInf[i].ParameterType.GetElementType() ?? typeof(object);

                            // Create an array of the correct type and size
                            Array params_parameter = Array.CreateInstance(paramsElementType, params_count + 1);

                            for (int j = i; j < parameter_count; j++)
                            {
                                // params_parameter[j - i] = args.Parameters[j].Evaluate();
                                params_parameter.SetValue(args.Parameters[j].Evaluate(), j - i);
                            }

                            parameters[i] = params_parameter;
                        }
                        else if (i >= parameter_count)
                        {
                            if (paraInf[i].HasDefaultValue) { parameters[i] = paraInf[i].DefaultValue; }
                            else
                            {
                                args.HasResult = false;
                                return;
                            }
                        }
                        else
                        {
                            parameters[i] = args.Parameters[i].Evaluate();
                        }
                    }
                    // OLD
                    // for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }

                    object? result = mInfo.Invoke(obj, parameters) ?? throw new Exception("Result of Functions may not be null");
                    args.Result = Convert.ChangeType(result, mInfo.ReturnType);
                }
            };
        }

        public static string GetPrimitiveName(Type type)
        {
            // Dictionary to map .NET type names to C# primitive names
            var typeToPrimitiveName = new Dictionary<string, string>
            {
                { "System.Int32", "int" },
                { "System.Int64", "long" },
                { "System.Int16", "short" },
                { "System.Byte", "byte" },
                { "System.SByte", "sbyte" },
                { "System.UInt32", "uint" },
                { "System.UInt64", "ulong" },
                { "System.UInt16", "ushort" },
                { "System.Single", "float" },
                { "System.Double", "double" },
                { "System.Decimal", "decimal" },
                { "System.Char", "char" },
                { "System.Boolean", "bool" }
            };

            // Check if the dictionary contains the type's full name and return the corresponding primitive name
            return typeToPrimitiveName.TryGetValue(type.FullName, out string primitiveName)
                ? primitiveName
                : type.Name; // Return the .NET type name if not found
        }

        /// <summary>
        /// Load AddOns from <see cref="StateOptions"/>
        /// </summary>
        /// <param name="expr">The <see cref="ExtendedExpression"/> to load AddOns for</param>
        public static void LoadAddOns(this ExtendedExpression expr, AddOnInitializationArgs args)
        {
            IEnumerable<string> dll_paths = args.AddOns;
            AddOnInitializationArgs initArgs = new AddOnInitializationArgs()
            {
                Framerate = args.Framerate,
                Width = args.Width,
                Height = args.Height,
                Logger = args.Logger,
                ProgressReporter = args.ProgressReporter,
                Guid = args.Guid,
                AddOns = args.AddOns,
                AddOnDir = args.AddOnDir,
            };
            foreach (string dll in dll_paths)
            {
                string full_path = Path.IsPathRooted(dll) ? dll : Path.Combine(args.AddOnDir, dll);
                expr.LoadAddOn(full_path, initArgs);
            }
        }
    }
}
