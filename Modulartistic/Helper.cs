using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Modulartistic
{
    public class Helper
    {
        public static Color ConvertHSV2RGB(double hue, double saturation, double value)
        {
            double chroma = saturation * value;
            double Hi = hue / 60;
            double X = chroma * (1 - Math.Abs(Hi % 2 - 1));

            Color color;
            double[] rgb;

            if (0 <= Hi && Hi < 1)
                rgb = new double[3] { chroma, X, 0 };
            else if (1 <= Hi && Hi < 2)
                rgb = new double[3] { X, chroma, 0 };
            else if (2 <= Hi && Hi < 3)
                rgb = new double[3] { 0, chroma, X };
            else if (3 <= Hi && Hi < 4)
                rgb = new double[3] { 0, X, chroma };
            else if (4 <= Hi && Hi < 5)
                rgb = new double[3] { X, 0, chroma };
            else if (5 <= Hi && Hi < 6)
                rgb = new double[3] { chroma, 0, X };
            else
                rgb = new double[3] { 0, 0, 0 };
            int r = (int)(255 * (rgb[0] + (value - chroma)));
            int g = (int)(255 * (rgb[1] + (value - chroma)));
            int b = (int)(255 * (rgb[2] + (value - chroma)));
            color = Color.FromArgb(255, r, g, b);
            return color;
        }

        public static Color ARGBFromArray(double[] array)
        {
            double a = array[0];
            double r = array[1];
            double g = array[2];
            double b = array[3];

            if (a > 1) { a = 1; }
            if (r > 1) { r = 1; }
            if (g > 1) { g = 1; }
            if (b > 1) { b = 1; }

            return Color.FromArgb((int)(255 * a), (int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        public static string ValidFileName(string filename)
        {
            if (File.Exists(filename + @".png") || File.Exists(filename + @".gif") || File.Exists(filename + @".txt") || File.Exists(filename + @".json") || Directory.Exists(filename))
            {
                int i = 1;
                while (File.Exists(filename + "_" + i + @".png") || File.Exists(filename + "_" + i + @".gif") || File.Exists(filename + "_" + i + @".txt") || Directory.Exists(filename + "_" + i))
                {
                    i++;
                }
                filename = filename + "_" + i;
            }

            return filename;
        }
    }
}
