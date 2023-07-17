using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ImageFunctions
{
    public static class ImageFunctions
    {
        private static bool initialized = false;
        private static List<string> files;
        private static Dictionary<int, Bitmap> img_cache;

        #region Functions for Class Usage
        public static void Initialize(string path = "")
        {
            files = new List<string>();
            if (path == "") { files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Input").Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".bmp")).OrderBy(s => s.Length).ThenBy(s => s).ToArray().ToList(); }
            else if (Directory.Exists(path)) { files = Directory.GetFiles(path).Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".bmp")).OrderBy(s => s.Length).ThenBy(s => s).ToArray().ToList(); }
            else { return; }

            img_cache = new Dictionary<int, Bitmap>();

            initialized = true;
        }

        #endregion

        public static double ImageHUE(double i, double x, double y)
        {
            int idx = (int)i;
            // Console.WriteLine("{0} {1} {2} {3}", (int)x, (int)y, i, idx);
            if (!initialized) { Initialize(); }
            if (idx >= files.Count || idx < 0) { throw new Exception("index out of range"); }


            Bitmap bmp;
            if (img_cache.ContainsKey(idx)) { bmp = img_cache[idx]; }
            else 
            {
                bmp = new Bitmap(files[idx]);
                if (img_cache.Count > 3) { img_cache.Clear(); } 
                img_cache.Add(idx, bmp);
            }
            
            return bmp.GetPixel((int)x, (int)y).GetHue()/360;
        }

        public static double ImageBrightness(double i, double x, double y)
        {
            int idx = (int)i;
            if (!initialized) { Initialize(); }
            if (idx >= files.Count || idx < 0) { throw new Exception("index out of range"); }


            Bitmap bmp;
            if (img_cache.ContainsKey(idx)) { bmp = img_cache[idx]; }
            else
            {
                bmp = new Bitmap(files[idx]);
                if (img_cache.Count > 3) { img_cache.Clear(); }
                img_cache.Add(idx, bmp);
            }

            return bmp.GetPixel((int)x, (int)y).GetBrightness();
        }

        public static double ImageSaturation(double i, double x, double y)
        {
            int idx = (int)i;
            if (!initialized) { Initialize(); }
            if (idx >= files.Count || idx < 0) { throw new Exception("index out of range"); }


            Bitmap bmp;
            if (img_cache.ContainsKey(idx)) { bmp = img_cache[idx]; }
            else
            {
                bmp = new Bitmap(files[idx]);
                if (img_cache.Count > 3) { img_cache.Clear(); }
                img_cache.Add(idx, bmp);
            }

            return bmp.GetPixel((int)x, (int)y).GetSaturation();
        }

        #region private helper functions
        /*
        
        private static double[,] GaussianBlur(int length, double weight)
        {
            double[,] kernel = new double[length, length];
            double kernelSum = 0;
            int foff = (length - 1) / 2;
            double distance = 0;
            double constant = 1d / (2 * Math.PI * weight * weight);
            for (int y = -foff; y <= foff; y++)
            {
                for (int x = -foff; x <= foff; x++)
                {
                    distance = ((y * y) + (x * x)) / (2 * weight * weight);
                    kernel[y + foff, x + foff] = constant * Math.Exp(-distance);
                    kernelSum += kernel[y + foff, x + foff];
                }
            }
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    kernel[y, x] = kernel[y, x] * 1d / kernelSum;
                }
            }
            return kernel;
        }

        public static double[,] Convolve(double[,] srcImage, double[,] kernel)
        {
            // setting width and height
            int width = srcImage.GetLength(0);
            int height = srcImage.GetLength(1);

            int kernel_size = kernel.GetLength(0);

            int[,] result = new int[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < kernel_size; k++)
                    {
                        result[i, j] +=
                    }
                }
            }

            return result;
        }

        public static double ImageGradient(string file, int x, int y)
        {
            // load the bitmap
            Bitmap img = new Bitmap(file);

            // make it greyscale
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    Color oldcol = img.GetPixel(j, i);
                    int colval = (oldcol.R + oldcol.G + oldcol.B) / 3;
                    img.SetPixel(j, i, Color.FromArgb(colval, colval, colval));
                }
            }

            // Make the Gaussian Filter
            double[,] gauss = GaussianBlur(5, 1);
            // Apply the Gaussian Filter
            img = Convolve(img, gauss);

            // apply Sobel kernels
            int[,] sobel_x = {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1 }
            };
            int[,] sobel_y = {
                { 1, 2, 1 },
                { 0, 0, 0 },
                { -1, -2, -1 }
            };


        }
        */
        #endregion
    }
}
