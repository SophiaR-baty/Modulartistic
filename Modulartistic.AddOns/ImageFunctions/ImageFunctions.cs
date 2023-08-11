using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ImageFunctions
{
    public static class ImageFunctions
    {
        #region Memory fields
        private static bool initialized = false;
        private static ConcurrentDictionary<string, Bitmap> img_cache;
        private static ConcurrentDictionary<string, string[]> dir_cache;

        private static ConcurrentDictionary<string, double[,]> img_grad_x_cache;
        private static ConcurrentDictionary<string, double[,]> img_grad_y_cache;
        #endregion

        #region Function for usage in Function parser
        public static double ImageHue(double x, double y, string abs_path_to_file)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }
            
            LoadBitmap(abs_path_to_file, out Bitmap bmp);

            if (x < 0 || x >= bmp.Width || y < 0 || y >= bmp.Height) { return double.NaN; }

            return bmp.GetPixel((int)x, (int)y).GetHue() / 360;
        }

        public static double ImageHue(double x, double y, string abs_path_to_dir, double i)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_dir) || !Directory.Exists(abs_path_to_dir)) { return double.NaN; }

            GetDirectoryContents(abs_path_to_dir, out string[] files);
            
            int idx = (int)i;
            if (idx >= files.Length || idx < 0) { return double.NaN; }

            string file = files[idx];
            return ImageHue(x, y, file);
        }

        public static double ImageBrightness(double x, double y, string abs_path_to_file)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadBitmap(abs_path_to_file, out Bitmap bmp);

            if (x < 0 || x >= bmp.Width || y < 0 || y >= bmp.Height) { return double.NaN; }

            return bmp.GetPixel((int)x, (int)y).GetBrightness();
        }

        public static double ImageBrightness(double x, double y, string abs_path_to_dir, double i)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_dir) || !Directory.Exists(abs_path_to_dir)) { return double.NaN; }

            GetDirectoryContents(abs_path_to_dir, out string[] files);

            int idx = (int)i;
            if (idx >= files.Length || idx < 0) { return double.NaN; }

            string file = files[idx];
            return ImageBrightness(x, y, file);
        }

        public static double ImageSaturation(double x, double y, string abs_path_to_file)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadBitmap(abs_path_to_file, out Bitmap bmp);

            if (x < 0 || x >= bmp.Width || y < 0 || y >= bmp.Height) { return double.NaN; }

            return bmp.GetPixel((int)x, (int)y).GetSaturation();
        }

        public static double ImageSaturation(double x, double y, string abs_path_to_dir, double i)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_dir) || !Directory.Exists(abs_path_to_dir)) { return double.NaN; }

            GetDirectoryContents(abs_path_to_dir, out string[] files);

            int idx = (int)i;
            if (idx >= files.Length || idx < 0) { return double.NaN; }


            string file = files[idx];
            return ImageSaturation(x, y, file);
        }

        public static double ImageXDiff(double x, double y, string abs_path_to_file)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadImgGradX(abs_path_to_file, out double[,] img_grad);

            if (x < 0 || x >= img_grad.GetLength(1) || y < 0 || y >= img_grad.GetLength(0)) { return double.NaN; }

            return img_grad[(int)y, (int)x];
        }

        public static double ImageXDiff(double x, double y, string abs_path_to_dir, double i)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_dir) || !Directory.Exists(abs_path_to_dir)) { return double.NaN; }

            GetDirectoryContents(abs_path_to_dir, out string[] files);

            int idx = (int)i;
            if (idx >= files.Length || idx < 0) { return double.NaN; }

            string file = files[idx];
            return ImageXDiff(x, y, file);
        }

        public static double ImageYDiff(double x, double y, string abs_path_to_file)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadImgGradY(abs_path_to_file, out double[,] img_grad);

            if (x < 0 || x >= img_grad.GetLength(1) || y < 0 || y >= img_grad.GetLength(0)) { return double.NaN; }

            return img_grad[(int)y, (int)x];
        }

        public static double ImageYDiff(double x, double y, string abs_path_to_dir, double i)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_dir) || !Directory.Exists(abs_path_to_dir)) { return double.NaN; }

            GetDirectoryContents(abs_path_to_dir, out string[] files);

            int idx = (int)i;
            if (idx >= files.Length || idx < 0) { return double.NaN; }

            string file = files[idx];
            return ImageYDiff(x, y, file);
        }

        public static double ImageDirection(double x, double y, string abs_path_to_file)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadImgGradX(abs_path_to_file, out double[,] img_grad_x);
            LoadImgGradY(abs_path_to_file, out double[,] img_grad_y);

            if (x < 0 || x >= img_grad_x.GetLength(1) || y < 0 || y >= img_grad_x.GetLength(0)) { return double.NaN; }
            if (x < 0 || x >= img_grad_y.GetLength(1) || y < 0 || y >= img_grad_y.GetLength(0)) { return double.NaN; }

            return Math.Atan2(img_grad_y[(int)y, (int)x], img_grad_x[(int)y, (int)x]);
        }

        public static double ImageDirection(double x, double y, string abs_path_to_dir, double i)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_dir) || !Directory.Exists(abs_path_to_dir)) { return double.NaN; }

            GetDirectoryContents(abs_path_to_dir, out string[] files);

            int idx = (int)i;
            if (idx >= files.Length || idx < 0) { return double.NaN; }

            string file = files[idx];
            return ImageDirection(x, y, file);
        }

        public static double ImageGradient(double x, double y, string abs_path_to_file)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadImgGradX(abs_path_to_file, out double[,] img_grad_x);
            LoadImgGradY(abs_path_to_file, out double[,] img_grad_y);

            if (x < 0 || x >= img_grad_x.GetLength(1) || y < 0 || y >= img_grad_x.GetLength(0)) { return double.NaN; }
            if (x < 0 || x >= img_grad_y.GetLength(1) || y < 0 || y >= img_grad_y.GetLength(0)) { return double.NaN; }

            return Math.Sqrt(img_grad_x[(int)y, (int)x] * img_grad_x[(int)y, (int)x] + img_grad_y[(int)y, (int)x] * img_grad_y[(int)y, (int)x]);
        }

        public static double ImageGradient(double x, double y, string abs_path_to_dir, double i)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_dir) || !Directory.Exists(abs_path_to_dir)) { return double.NaN; }

            GetDirectoryContents(abs_path_to_dir, out string[] files);

            int idx = (int)i;
            if (idx >= files.Length || idx < 0) { return double.NaN; }

            string file = files[idx];
            return ImageGradient(x, y, file);
        }
        #endregion

        #region private helper functions
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

        private static double[,] Convolute(double[,] srcImage, double[,] kernel)
        {
            // setting width and height
            int width = srcImage.GetLength(1);
            int height = srcImage.GetLength(0);
            // setting kernel size and checking if width == height
            int kernel_size = kernel.GetLength(0);
            if (kernel_size != kernel.GetLength(1)) { throw new Exception("Filter Kernels must be square. "); }

            // iterating all pixels of srcImage
            double[,] result = new double[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // iterating all kernel entries and adding to result
                    result[y, x] = 0;
                    for (int ky = 0; ky < kernel_size; ky++)
                    {
                        for (int kx = 0; kx < kernel_size; kx++)
                        {
                            int ix = x + kx;
                            int iy = y + ky;
                            if (iy < 0) { iy = 0; }
                            else if (iy >= height) { iy = height-1; }
                            if (ix < 0) { ix = 0; }
                            else if (ix >= width) { ix = width - 1; }

                            // adding weighte dpixel to result
                            result[y, x] += kernel[ky, kx]*srcImage[iy, ix];
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the img with gauss and sobel opertator applied. 
        /// </summary>
        /// <param name="file">The image file</param>
        /// <param name="mode">either 'x' or 'y' to specify which derivative to return</param>
        /// <returns></returns>
        private static double[,] ImageGradient(string file, char mode)
        {
            // load the bitmap
            LoadBitmap(file, out Bitmap img_bm);

            // make it greyscale and double[,]
            double[,] img = new double[img_bm.Height, img_bm.Width];
            for (int iy = 0; iy < img_bm.Height; iy++)
            {
                for (int ix = 0; ix < img_bm.Width; ix++)
                {
                    Color oldcol = img_bm.GetPixel(ix, iy);
                    double colval = (oldcol.R + oldcol.G + oldcol.B) / 3;
                    img[iy, ix] = colval;
                }
            }


            // Make the Gaussian Filter
            double[,] gauss = GaussianBlur(5, 1);
            // Apply the Gaussian Filter
            img = Convolute(img, gauss);

            // apply Sobel kernels
            double[,] sobel_x = {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1 }
            };
            double[,] sobel_y = {
                { 1, 2, 1 },
                { 0, 0, 0 },
                { -1, -2, -1 }
            };

            double[,] img_grad_x = Convolute(img, sobel_x);
            double[,] img_grad_y = Convolute(img, sobel_y);
            


            switch (mode)
            {
                case 'x': return img_grad_x;
                case 'y': return img_grad_y;
                default: return img_grad_x;
            }
        }

        private static void LoadBitmap(string key, out Bitmap bmp)
        {
            if (img_cache.ContainsKey(key)) { bmp = img_cache[key]; }
            else
            {
                bmp = new Bitmap(key);
                if (img_cache.Count > 3) { img_cache.Clear(); }
                img_cache.TryAdd(key, bmp);
            }
        }

        private static void LoadImgGradX(string key, out double[,] img_grad_x)
        {
            if (img_grad_x_cache.ContainsKey(key)) { img_grad_x = img_grad_x_cache[key]; }
            else
            {
                img_grad_x = ImageGradient(key, 'x');
                if (img_grad_x_cache.Count > 3) { img_grad_x_cache.Clear(); }
                img_grad_x_cache.TryAdd(key, img_grad_x);
            }
        }

        private static void LoadImgGradY(string key, out double[,] img_grad_y)
        {
            if (img_grad_y_cache.ContainsKey(key)) { img_grad_y = img_grad_y_cache[key]; }
            else
            {
                img_grad_y = ImageGradient(key, 'x');
                if (img_grad_y_cache.Count > 3) { img_grad_y_cache.Clear(); }
                img_grad_y_cache.TryAdd(key, img_grad_y);
            }
        }

        private static void GetDirectoryContents(string key, out string[] files)
        {
            if (dir_cache.ContainsKey(key)) { files = dir_cache[key]; }
            else
            {
                files = Directory.GetFiles(key).Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".bmp")).OrderBy(s => s.Length).ThenBy(s => s).ToArray();
                if (dir_cache.Count > 3) { dir_cache.Clear(); }
                dir_cache.TryAdd(key, files);
            }
        }

        private static void Initialize()
        {
            img_cache = new ConcurrentDictionary<string, Bitmap>();
            dir_cache = new ConcurrentDictionary<string, string[]>();
            img_grad_x_cache = new ConcurrentDictionary<string, double[,]>();
            img_grad_y_cache = new ConcurrentDictionary<string, double[,]>();

            initialized = true;
        }
        #endregion
    }
}
