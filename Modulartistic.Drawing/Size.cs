using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Modulartistic.Drawing
{
    public struct Size
    {
        public int Width { readonly get; set; }
        public int Height { readonly get; set; }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
