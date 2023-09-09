using System;
using System.IO;

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
    }
}
