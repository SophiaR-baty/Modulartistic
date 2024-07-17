using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modulartistic.Drawing
{
    public struct Color
    {
        private SKColor _color;

        public byte R { get => _color.Red; }
        public byte G { get => _color.Green; }
        public byte B { get => _color.Blue; }
        public byte A { get => _color.Alpha; }

        #region Constructors
        public Color(byte red, byte green, byte blue, byte alpha)
        {
            _color = new SKColor(red, green, blue, alpha);
        }

        public Color(int red, int green, int blue, int alpha)
        {
            _color = new SKColor((byte)red, (byte)green, (byte)blue, (byte)alpha);
        }

        private Color(SKColor skc)
        {
            _color = skc;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a Color Object from HSV values
        /// </summary>
        /// <param name="hue">Hue of the Color</param>
        /// <param name="saturation">Saturation of the Color</param>
        /// <param name="value">Value of the Color</param>
        /// <returns>new Color Object</returns>
        public static Color FromHSV(float hue, float saturation, float value)
        {
            saturation = saturation % 360;
            float chroma = saturation * value;
            float Hi = hue / 60;
            float X = chroma * (1 - Math.Abs(Hi % 2 - 1));

            float[] rgb;

            if (0 <= Hi && Hi < 1)
                rgb = new float[3] { chroma, X, 0 };
            else if (1 <= Hi && Hi < 2)
                rgb = new float[3] { X, chroma, 0 };
            else if (2 <= Hi && Hi < 3)
                rgb = new float[3] { 0, chroma, X };
            else if (3 <= Hi && Hi < 4)
                rgb = new float[3] { 0, X, chroma };
            else if (4 <= Hi && Hi < 5)
                rgb = new float[3] { X, 0, chroma };
            else if (5 <= Hi && Hi < 6)
                rgb = new float[3] { chroma, 0, X };
            else
                rgb = new float[3] { 0, 0, 0 };
            byte r = (byte)(255 * (rgb[0] + (value - chroma)));
            byte g = (byte)(255 * (rgb[1] + (value - chroma)));
            byte b = (byte)(255 * (rgb[2] + (value - chroma)));

            return new Color(r, g, b, (byte)255);
        }

        /// <summary>
        /// Creates a Color Object from HSV values
        /// </summary>
        /// <param name="hue">Hue of the Color</param>
        /// <param name="saturation">Saturation of the Color</param>
        /// <param name="value">Value of the Color</param>
        /// <param name="a">Alpha channel of the Color</param>
        /// <returns>new Color Object</returns>
        public static Color FromHSV(float hue, float saturation, float value, float a)
        {
            saturation = saturation % 360;
            float chroma = saturation * value;
            float Hi = hue / 60;
            float X = chroma * (1 - Math.Abs(Hi % 2 - 1));

            float[] rgb;

            if (0 <= Hi && Hi < 1)
                rgb = new float[3] { chroma, X, 0 };
            else if (1 <= Hi && Hi < 2)
                rgb = new float[3] { X, chroma, 0 };
            else if (2 <= Hi && Hi < 3)
                rgb = new float[3] { 0, chroma, X };
            else if (3 <= Hi && Hi < 4)
                rgb = new float[3] { 0, X, chroma };
            else if (4 <= Hi && Hi < 5)
                rgb = new float[3] { X, 0, chroma };
            else if (5 <= Hi && Hi < 6)
                rgb = new float[3] { chroma, 0, X };
            else
                rgb = new float[3] { 0, 0, 0 };
            byte r = (byte)(255 * (rgb[0] + (value - chroma)));
            byte g = (byte)(255 * (rgb[1] + (value - chroma)));
            byte b = (byte)(255 * (rgb[2] + (value - chroma)));

            return new Color(r, g, b, (byte)(a*255));
        }

        /// <summary>
        /// Creates a Color Object from ARGB values
        /// </summary>
        /// <param name="a">Alpha value of the Color</param>
        /// <param name="r">Red value of the Color</param>
        /// <param name="g">Green value of the Color</param>
        /// <param name="b">Blue value of the Color</param>
        /// <returns></returns>
        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Gets the Hue of the Color
        /// </summary>
        /// <returns>The Hue value of the Color from 0-360</returns>
        public float GetHue()
        {
            return _color.Hue;
        }

        /// <summary>
        /// Gets the Brightness (Lightness) of the Color
        /// </summary>
        /// <returns>The Brightness value (0-1)</returns>
        public float GetBrightness()
        {
            _color.ToHsl(out _, out _, out float l);
            return l / 100;
        }

        /// <summary>
        /// Gets the Saturation of the Color
        /// </summary>
        /// <returns>The Saturation value (0-1)</returns>
        public float GetSaturation()
        {
            _color.ToHsl(out _, out float s, out _);
            return s / 100;
        }

        /// <summary>
        /// Gets the Value of the Color
        /// </summary>
        /// <returns>The Value value (0-1)</returns>
        public float GetValue()
        {
            _color.ToHsv(out _, out _, out float v);
            return v / 100;
        }
        #endregion

        // Implicit conversion operator from BitmapWrapper to SKBitmap
        public static implicit operator SKColor(Color wrapper)
        {
            return wrapper._color;
        }

        // Implicit conversion operator from BitmapWrapper to SKBitmap
        public static implicit operator Color(SKColor skcolor)
        {
            return new Color(skcolor);
        }
    }
}

