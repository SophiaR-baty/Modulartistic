using Antlr4.Runtime.Misc;
using FFMpegCore;
using Modulartistic.AddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    internal class ParameterFunction
    {
        public string Name { get; set; }

        public string Expression { get; set; }

        public bool IsStatic { get; set; }

        public bool IsStaticAuto { get; set; }

        public Function Function { get; set; }

        private object? _value;
        

        public ParameterFunction(string name, string expression)
        {
            Name = name;
            Expression = expression;
            IsStaticAuto = true;
            IsStatic = false;

            Function = new Function(expression);
        }

        public ParameterFunction(StateOptionsParameter param, string[] addons, StateOptions args, GenerationOptions options)
        {
            Name = param.Name;
            Expression = param.Expression;
            IsStaticAuto = param.Static.ToLower() == "auto";
            IsStatic = param.Static.ToLower() == "true";

            Function = new Function(Expression);

            if (IsStaticAuto)
            {
                IsStatic = CanEvaluate();
            }
            if (IsStatic)
            {
                lock (param.ValueLock)
                {
                    object? paramval = param.Value;
                    if (paramval is null)
                    {
                        Function.LoadAddOns(args.AddOns.ToArray(), args, options);
                        paramval = Function.Evaluate();
                        param.Value = paramval;
                    }

                    _value = paramval;
                }
            }
            else
            {
                Function.LoadAddOns(args.AddOns.ToArray(), args, options);
            }
        }

        public object? Evaluate()
        {
            if (!IsStatic)
            {
                _value = Function.Evaluate();
            }

            return _value;
        }

        public bool CanEvaluate()
        {
            return Function.CanEvaluate();
        }

        public void RegisterParameter(string name, object value)
        {
            Function.RegisterParameter(name, value);
        }

        public void RegisterMethod(MethodInfo mInf, object instance)
        {
            Function.RegisterFunction(mInf, instance);
        }
    }
}
