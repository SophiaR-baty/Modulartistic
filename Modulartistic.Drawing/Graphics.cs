using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Modulartistic.Drawing
{
    public class Graphics
    {
        private SKCanvas _canvas;
        
        private Graphics(Bitmap bmp)
        {
            _canvas = new SKCanvas(bmp);
        }

        public static Graphics FromImage(Bitmap bmp)
        {
            return new Graphics(bmp);
        }

        public void DrawImage(Bitmap bmp, int x, int y, int width, int height)
        {
            SKRect destRect = new SKRect(x, 0, x+width, height);
            _canvas.DrawBitmap(bmp, destRect);
        }
    }
}
