using NCalc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GUI
{
    internal class FunctionDictionary
    {
        private Dictionary<string, List<string>> mNames;
        private static string[] mNative = new string[]
            {
                "x",
                "y",
                "Th",
                "r",
                "num",
                "i",
                "j",
                "i_0",
                "i_1",
                "i_2",
                "i_3",
                "i_4",
                "i_5",
                "i_6",
                "i_7",
                "i_8",
                "i_9",

                "Abs()",
                "Acos()",
                "Asin()",
                "Atan()",
                "Ceiling()",
                "Cos()",
                "Exp()",
                "Floor()",
                "Log()",
                "Log10()",
                "Max()",
                "Min()",
                "Pow()",
                "Round()",
                "Sign()",
                "Sin()",
                "Sqrt()",
                "Tan()",
                "Truncate()"
            };


        public static string[] NativeNames
        {
            get => mNative;
        }
        
        public FunctionDictionary()
        {
            mNames = new Dictionary<string, List<string>>();
        }

        public void LoadAddOn(string dll_path)
        {
            // if dll has been Loaded already, throws exception
            if (mNames.ContainsKey(dll_path))
            {
                throw new Exception("AddOn " + dll_path + " has already be loaded. ");
            }
            else { mNames[dll_path]  = new List<string>(); }
            
            // Load DLL
            Assembly testDLL = Assembly.LoadFile(dll_path);
            foreach (Type type in testDLL.GetTypes())
            {
                // Get all the Types
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    // add the Function name
                    mNames[dll_path].Add(methodInfo.Name + "()");
                }
            }
        }

        public void RemoveAddOn(string dll_path) 
        {
            
        }

        public List<string> GetDictionary()
        {
            List<string> result = new List<string>();
            
            foreach (string word in NativeNames)
            {
                result.Add(word);
            }

            foreach (KeyValuePair<string, List<string>> kvp in mNames)
            {
                foreach (string word in kvp.Value)
                {
                    result.Add(word);
                }
            }

            return result;
        }
    }
}
