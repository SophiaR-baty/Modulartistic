using System;
using System.Collections.Generic;
using System.Text;
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


        public void SetPixel(int x, int y, Color color)
        {
            _bm.SetPixel(x, y, color.SK);
        }
    }
}
