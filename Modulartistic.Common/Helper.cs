using NCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
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
        /// <param name="expression"></param>
        /// <param name="obj"></param>
        /// <param name="method_info"></param>
        /// <param name="custom_name"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static void RegisterMethod(this Expression expression, object? obj, MethodInfo method_info, string custom_name = "")
        {
            Type? type = method_info.DeclaringType;
            if (type == null) { return; }

            // register method
            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (args.HasResult) { return; }
                if ((custom_name == "" && (name == method_info.Name || name == $"{type.Name}_{method_info.Name}")) || (custom_name != "" && name == custom_name))
                {
                    int passed_parameters_count = args.Parameters.Length;

                    // create parameters
                    ParameterInfo[] expected_parameters = method_info.GetParameters();
                    int expected_parameters_count = expected_parameters.Length;
                    object[] parameters = new object[expected_parameters_count];

                    // check if the passed parameter count exceeds the expected parameters
                    bool allow_additional_parameters = expected_parameters[expected_parameters_count - 1].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length == 0;
                    if (passed_parameters_count > expected_parameters_count && allow_additional_parameters)
                    {
                        args.HasResult = false;
                        return;
                    }

                    // fill parameters with passed parameters
                    for (int i = 0; i < expected_parameters_count; i++)
                    {
                        // case: array parameter
                        if (expected_parameters[i].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
                        {
                            int additional_parameters_count = passed_parameters_count - expected_parameters_count;
                            // object[] params_parameter = new object[params_count+1];

                            // Get the type of the elements in the params array
                            Type additional_parameter_type = expected_parameters[i].ParameterType.GetElementType() ?? typeof(object);

                            // Create an array of the correct type and size
                            Array additional_parameters = Array.CreateInstance(additional_parameter_type, additional_parameters_count + 1);

                            for (int j = i; j < passed_parameters_count; j++)
                            {
                                object additional_parameter = args.Parameters[j].Evaluate();

                                Type expected_type = additional_parameter_type;
                                Type passed_type = additional_parameter.GetType();

                                // if no implicit conversion possible, cancel evaluation
                                if (!CanCast(passed_type, expected_type))
                                {
                                    args.HasResult = false;
                                    return;
                                }

                                additional_parameters.SetValue(additional_parameter, j - i);
                            }

                            parameters[i] = additional_parameters;
                        }
                        // case: ran out of passed parameters -> check for default values
                        else if (i >= passed_parameters_count)
                        {
                            if (expected_parameters[i].HasDefaultValue) 
                            { 
                                parameters[i] = expected_parameters[i].DefaultValue; 
                            }
                            else
                            {
                                args.HasResult = false;
                                return;
                            }
                        }
                        // case: regular parameter
                        else
                        {
                            parameters[i] = args.Parameters[i].Evaluate();

                            Type expected_type = expected_parameters[i].ParameterType;
                            Type passed_type = parameters[i].GetType();

                            // if no implicit conversion possible, cancel evaluation
                            if (!CanCast(passed_type, expected_type))
                            {
                                args.HasResult = false;
                                return;
                            }
                        }
                    }

                    object? result = method_info.Invoke(obj, parameters) ?? throw new Exception("Result of Functions may not be null");
                    args.Result = Convert.ChangeType(result, method_info.ReturnType);
                }
            };
        }

        static bool CanCast(Type sourceType, Type targetType)
        {
            if (targetType.IsAssignableFrom(sourceType))
            {
                return true;
            }

            if (sourceType.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(m => m.Name == "op_Implicit" && m.ReturnType == targetType))
            {
                return true;
            }
            
            // Define valid implicit numeric conversions
            if (sourceType == typeof(sbyte) &&
               (targetType == typeof(short) || targetType == typeof(int) || targetType == typeof(long) || targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            if (sourceType == typeof(byte) &&
               (targetType == typeof(short) || targetType == typeof(ushort) || targetType == typeof(int) || targetType == typeof(uint) || targetType == typeof(long) || targetType == typeof(ulong) || targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            if (sourceType == typeof(short) &&
               (targetType == typeof(int) || targetType == typeof(long) || targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            if (sourceType == typeof(ushort) &&
               (targetType == typeof(int) || targetType == typeof(uint) || targetType == typeof(long) || targetType == typeof(ulong) || targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            if (sourceType == typeof(int) &&
               (targetType == typeof(long) || targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            if (sourceType == typeof(uint) &&
               (targetType == typeof(long) || targetType == typeof(ulong) || targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            if (sourceType == typeof(long) &&
               (targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            if (sourceType == typeof(char) &&
               (targetType == typeof(ushort) || targetType == typeof(int) || targetType == typeof(uint) || targetType == typeof(long) || targetType == typeof(ulong) || targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            if (sourceType == typeof(float) &&
               (targetType == typeof(double)))
                return true;

            if (sourceType == typeof(ulong) &&
               (targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
                return true;

            // No match found
            return false;
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
