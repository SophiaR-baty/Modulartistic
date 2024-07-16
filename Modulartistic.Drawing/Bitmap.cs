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
        private IntPtr _pixelsPtr;

        public int Width { get => _bm.Width; }
        public int Height { get => _bm.Height; }

        public Bitmap(int width, int height)
        {
            _bm = new SKBitmap(width, height);
            _pixelsPtr = _bm.GetPixels();
        }

        public Bitmap(string file)
        {
            _bm = SKBitmap.Decode(file);
            _pixelsPtr = _bm.GetPixels();
        }


        public void SetPixel(int x, int y, Color color)
        {
            // _bm.SetPixel(x, y, color);

            // Ensure the bitmap is not null
            if (_bm == null)
            {
                throw new ArgumentNullException(nameof(_bm));
            }

            // Ensure the coordinates are within bounds
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
            }

            // Calculate the offset to the pixel at (x, y)
            int offset = (y * _bm.RowBytes) + (x * 4); // Assuming 32-bit (4 bytes per pixel) RGBA

            // Use unsafe code to manipulate the pixel data
            unsafe
            {
                byte* pixelPtr = (byte*)_pixelsPtr.ToPointer() + offset;

                // Set the pixel color
                pixelPtr[0] = color.B;    // Blue component
                pixelPtr[1] = color.G;   // Green component
                pixelPtr[2] = color.R;     // Red component
                pixelPtr[3] = color.A;   // Alpha component
            }
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
                success = _bm.Encode(fs, SKEncodedImageFormat.Png, 100);
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
