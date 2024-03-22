using System;
using System.IO;
using NCalc;

namespace Modulartistic.Core
{
    public class Helper
    {
        public static string ValidFileName(string filename)
        {
            if (File.Exists(filename + @".mp4") || File.Exists(filename + @".png") || File.Exists(filename + @".gif") || File.Exists(filename + @".txt") || File.Exists(filename + @".json") || Directory.Exists(filename))
            {
                int i = 1;
                while (File.Exists(filename + "_" + i + @".mp4") || File.Exists(filename + "_" + i + @".png") || File.Exists(filename + "_" + i + @".gif") || File.Exists(filename + "_" + i + @".txt") || File.Exists(filename + "_" + i + @".json") || Directory.Exists(filename + "_" + i))
                {
                    i++;
                }
                filename = filename + "_" + i;
            }

            return filename;
        }

        
        public static double mod(double d1, double d2)
        {
            if (d2 <= 0)
                throw new DivideByZeroException();
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }

        public static double inclusiveMod(double d1, double d2)
        {
            double result = mod(d1, d2);
            return d1 != 0 && result == 0 ? d2 : result;
        }

        public static double circ(double d1, double d2)
        {
            double result = mod(d1, 2*d2);
            return result < d2 ? result : 2*d2-result;
        }

        public static string GetAbsolutePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                // Path is already absolute
                return path;
            }
            else
            {
                // Convert to absolute using the current working directory
                string addonDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "addons");
                return Path.Combine(addonDir, path);
            }
        }

        public static void ExprRegisterStateProperties(ref Expression expr, State s)
        {
            expr.Parameters["x_0"] = s.X0;
            expr.Parameters["y_0"] = s.Y0;
            expr.Parameters["x_fact"] = s.XFactor;
            expr.Parameters["y_fact"] = s.YFactor;
            expr.Parameters["x_rotcent"] = s.XRotationCenter;
            expr.Parameters["y_rotcent"] = s.YRotationCenter;
            expr.Parameters["rotation"] = s.Rotation;

            expr.Parameters["modnum"] = s.Mod;
            expr.Parameters["num"] = s.Mod;
            expr.Parameters["modlimlow"] = s.ModLimLow;
            expr.Parameters["modlimup"] = s.ModLimUpp;

            expr.Parameters["col_rh"] = s.ColorRH;
            expr.Parameters["col_gs"] = s.ColorGS;
            expr.Parameters["col_bv"] = s.ColorBV;
            expr.Parameters["col_alp"] = s.ColorAlp;

            expr.Parameters["inv_col_rh"] = s.InvColorRH;
            expr.Parameters["inv_col_gs"] = s.InvColorGS;
            expr.Parameters["inv_col_bv"] = s.InvColorBV;
            expr.Parameters["inv_col_alp"] = s.InvColorAlp;

            expr.Parameters["col_fact_rh"] = s.ColorFactorRH;
            expr.Parameters["col_fact_gs"] = s.ColorFactorGS;
            expr.Parameters["col_fact_bv"] = s.ColorFactorBV;

            expr.Parameters["i_0"] = s.Parameters[0];
            expr.Parameters["i"] = s.Parameters[0];
            expr.Parameters["i_1"] = s.Parameters[1];
            expr.Parameters["j"] = s.Parameters[1];
            expr.Parameters["i_2"] = s.Parameters[2];
            expr.Parameters["i_3"] = s.Parameters[3];
            expr.Parameters["i_4"] = s.Parameters[4];
            expr.Parameters["i_5"] = s.Parameters[5];
            expr.Parameters["i_6"] = s.Parameters[6];
            expr.Parameters["i_7"] = s.Parameters[7];
            expr.Parameters["i_8"] = s.Parameters[8];
            expr.Parameters["i_9"] = s.Parameters[9];
        }

        public static void ExprRegisterGenArgs(ref Expression expr, GenerationOptions args)
        {
            expr.Parameters["img_width"] = (double)args.Width;
            expr.Parameters["img_height"] = (double)args.Height;
            expr.Parameters["ani_framerate"] = (double)args.Framerate;
        }
    }
}
