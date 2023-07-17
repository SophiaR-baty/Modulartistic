using ImageFunctions;
using MathNet.Numerics.Financial;
using NCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using VectorImageFunctions;

namespace Modulartistic
{
    public class Function
    {
        private Expression expression;

        private Dictionary<int, Curve> convertedImages;
        private string[] imageInFiles;

        public Function(Expression expression) 
        {
            this.expression = expression;
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;

            imageInFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Input").Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".bmp")).ToArray();

            convertedImages = new Dictionary<int, Curve>();

            // vector image stuff
            this.expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == "Image")
                {
                    int idx = (int)((double)args.Parameters[2].Evaluate());
                    Curve c;
                    if (convertedImages.ContainsKey(idx)) { c = convertedImages[idx]; }
                    else { c = Curve.FromImage(imageInFiles[idx], 150, 200); }
                    args.Result = c.distance((double)args.Parameters[0].Evaluate(), (double)args.Parameters[1].Evaluate(), 0.4);
                }
            };
        }

        public Function(string expression)
        {
            this.expression = new Expression(expression);
            this.expression.Options = EvaluateOptions.UseDoubleForAbsFunction;

            // LoadAddOn(AppDomain.CurrentDomain.BaseDirectory + "MathFunctions.dll");
            InitializeImageFunctions();

            /* temporarily disabled // vector image stuff
            
            imageInFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Input").Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".bmp")).OrderBy(s => s.Length).ThenBy(s => s).ToArray();
            convertedImages = new Dictionary<int, Curve>();

            this.expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == "Image")
                {
                    int idx = (int)(double)args.Parameters[2].Evaluate();
                    Curve c;
                    if (convertedImages.ContainsKey(idx)) { c = convertedImages[idx]; }
                    else
                    {
                        string s = imageInFiles[idx];
                        // Console.WriteLine(s);
                        c = Curve.FromImage(s, 150, 200); //, 150, 200
                        convertedImages[idx] = c;
                    }
                    args.Result = c.angle((double)args.Parameters[0].Evaluate(), (double)args.Parameters[1].Evaluate(), 0.1);
                }
            };
            */
        }

        public double Evaluate(double x, double y, List<double> paras, double mod)
        {
            
            
            expression.Parameters["x"] = x;
            expression.Parameters["y"] = y;
            expression.Parameters["Th"] = 180 * Math.Atan2(y, x) / Math.PI;
            expression.Parameters["r"] = Math.Sqrt(x * x + y * y);
            expression.Parameters["num"] = mod;

            expression.Parameters["i_0"] = paras[0];
            expression.Parameters["i"] = paras[0];
            expression.Parameters["i_1"] = paras[1];
            expression.Parameters["j"] = paras[1];
            expression.Parameters["i_2"] = paras[2];
            expression.Parameters["i_3"] = paras[3];
            expression.Parameters["i_4"] = paras[4];
            expression.Parameters["i_5"] = paras[5];
            expression.Parameters["i_6"] = paras[6];
            expression.Parameters["i_7"] = paras[7];
            expression.Parameters["i_8"] = paras[8];
            expression.Parameters["i_9"] = paras[9];

            return (double)expression.Evaluate();
        }

        public void LoadAddOn(string dll_path)
        {
            Assembly testDLL = Assembly.LoadFile(dll_path);
            foreach (Type type in testDLL.GetTypes())
            {
                // Console.WriteLine(type.FullName);
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                // foreach (MethodInfo methodInfo in methodInfos) { Console.WriteLine(methodInfo.Name); }
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    expression.EvaluateFunction += delegate (string name, FunctionArgs args)
                    {
                        if (name == methodInfo.Name)
                        {
                            object[] parameters = new object[args.Parameters.Length]; 
                            for (int i = 0; i < args.Parameters.Length; i++) { parameters[i] = args.Parameters[i].Evaluate(); }
                            args.Result = (double)methodInfo.Invoke(null, parameters); 
                        }
                    };
                }
            }
        }

        public void LoadAddOns(string[] dll_paths)
        {
            foreach (string dll in dll_paths)
            {
                LoadAddOn(dll);
            }
        }

        public void InitializeImageFunctions()
        {
            ImageFunctions.ImageFunctions.Initialize();
            this.expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == "ImageHUE")
                {
                    args.Result = ImageFunctions.ImageFunctions.ImageHUE(Convert.ToDouble(args.Parameters[0].Evaluate()), Convert.ToDouble(args.Parameters[1].Evaluate()), Convert.ToDouble(args.Parameters[2].Evaluate()));
                    return;
                }
                if (name == "ImageBrightness")
                {
                    args.Result = ImageFunctions.ImageFunctions.ImageBrightness(Convert.ToDouble(args.Parameters[0].Evaluate()), Convert.ToDouble(args.Parameters[1].Evaluate()), Convert.ToDouble(args.Parameters[2].Evaluate()));
                    return;
                }
                if (name == "ImageSaturation")
                {
                    args.Result = ImageFunctions.ImageFunctions.ImageSaturation(Convert.ToDouble(args.Parameters[0].Evaluate()), Convert.ToDouble(args.Parameters[1].Evaluate()), Convert.ToDouble(args.Parameters[2].Evaluate()));
                    return;
                }

            };
        }

        public bool IsValid()
        {
            expression.Parameters["x"] = 1;
            expression.Parameters["y"] = 1;
            expression.Parameters["Th"] = 1;
            expression.Parameters["r"] = 1;
            expression.Parameters["num"] = 1;

            expression.Parameters["i_0"] = 1;
            expression.Parameters["i"] = 1;
            expression.Parameters["i_1"] = 1;
            expression.Parameters["j"] = 1;
            expression.Parameters["i_2"] = 1;
            expression.Parameters["i_3"] = 1;
            expression.Parameters["i_4"] = 1;
            expression.Parameters["i_5"] = 1;
            expression.Parameters["i_6"] = 1;
            expression.Parameters["i_7"] = 1;
            expression.Parameters["i_8"] = 1;
            expression.Parameters["i_9"] = 1;
            expression.Options = EvaluateOptions.UseDoubleForAbsFunction;
            return !expression.HasErrors();
        }
    }
}
