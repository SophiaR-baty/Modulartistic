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

        /// <summary>
        /// Creates a new Graphics Object from a Bitmap
        /// </summary>
        /// <param name="bmp">The Bitmap</param>
        /// <returns>a new Graphics Object</returns>
        public static Graphics FromImage(Bitmap bmp)
        {
            return new Graphics(bmp);
        }

        /// <summary>
        /// Draws a Bitmap on the Graphic
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="x">Left</param>
        /// <param name="y">Top</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void DrawImage(Bitmap bmp, int x, int y, int width, int height)
        {
            SKRect destRect = new SKRect(x, y, x + width, y + height);
            _canvas.DrawBitmap(bmp, destRect);
        }
    }
}
