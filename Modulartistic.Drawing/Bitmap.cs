using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SkiaSharp;

namespace Modulartistic.Drawing
{
    public class Bitmap
    {
        private SKBitmap _bm;

        public int Width { get => _bm.Width; }
        public int Height { get => _bm.Height; }

        public Bitmap(int width, int height)
        {
            _bm = new SKBitmap(width, height);
        }

        public Bitmap(string file)
        {
            _bm = SKBitmap.Decode(file);
        }


        public void SetPixel(int x, int y, Color color)
        {
            _bm.SetPixel(x, y, color);
        }

        public bool Save(string destination)
        {
            // Delete the file if it exists.
            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            bool success = false;
            //Create the file.
            using (FileStream fs = File.Create(destination))
            {
                success = _bm.Encode(fs, SKEncodedImageFormat.Png, 100) ;
            }

            return success;
        }

        public Color GetPixel(int x, int y)
        {
            return _bm.GetPixel(x, y);
        }

        // Implicit conversion operator from BitmapWrapper to SKBitmap
        public static implicit operator SKBitmap(Bitmap wrapper)
        {
            return wrapper._bm;
        }
    }
}
