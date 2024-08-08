using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Modulartistic.AddOns;
using NCalc;

namespace Modulartistic.Core
{
    public class FunctionParameter
    {
        public string Name { get; set; }
        
        public string Expression { get; set; }

        public bool IsStatic { get; set; }

        private object _value;
        private Function _func;

        public FunctionParameter(string name, string expression)
        {
            Name = name;
            Expression = expression;
            IsStatic = false;
            _func = new Function(expression);
        }

        public object Evaluate()
        {
            if (!IsStatic) 
            {
                _value = _func.Evaluate(); 
            }
            
            return _value;
        }

        public bool CanEvaluate()
        {
            return _func.CanEvaluate();
        }

        public void RegisterParameter(string name, object value)
        {
            _func.RegisterParameter(name, value);
        }

        public void RegisterMethod(MethodInfo mInf, object instance)
        {
            _func.RegisterFunction(mInf, instance);
        }

        public void LoadAddOn(string dll_path, AddOnInitializationArgs args)
        {
            _func.LoadAddOn(dll_path, args);
        }
    }
}
