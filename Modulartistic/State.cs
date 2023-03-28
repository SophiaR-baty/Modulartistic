using ScottPlot.Control.EventProcess.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;

namespace Modulartistic
{
    /// <summary>
    /// State Class to contain Data about Image States/Frames for Image Generation.
    /// </summary>
    public class State
    {
        #region Properties
        /// <summary>
        /// The Name of the state
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Modulus Number.
        /// </summary>
        public double Mod { get; set; }
        /// <summary>
        /// The Lower Limit of the Modulus number. Values below will be treated as invalid.
        /// </summary>
        public double ModLimLow { get; set; }
        /// <summary>
        /// The Upper Limit of the Modulus number. Values above will be treated as invalid.
        /// </summary>
        public double ModLimUp { get; set; }

        /// <summary>
        /// The x-Coordinate that will be in the middle of the screen.
        /// </summary>
        public double X0 { get; set; }
        /// <summary>
        /// The y-Coordinate that will be in the middle of the screen.
        /// </summary>
        public double Y0 { get; set; }
        /// <summary>
        /// The factor by which the x coordinates will be scaled.
        /// </summary>
        public double XZoom { get; set; }
        /// <summary>
        /// The factor by which the y coordinates will be scaled.
        /// </summary>
        public double YZoom { get; set; }
        /// <summary>
        /// The Amount of degrees the image will be rotated.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// The Minimum Color Hue.
        /// </summary>
        public double ColorMinimum { get; set; }
        /// <summary>
        /// The Saturation of th Colors.
        /// </summary>
        public double ColorSaturation { get; set; }
        /// <summary>
        /// The Value of the Colors.
        /// </summary>
        public double ColorValue { get; set; }
        /// <summary>
        /// The Alpha value of the Colors. (Doesn't work with animated GIFs)
        /// </summary>
        public double ColorAlpha { get; set; }
        /// <summary>
        /// The ARGB Color values of the Invalid Color.
        /// </summary>
        public double[] InvalidColor { get; set; }
        /// <summary>
        /// RGB Factors by which Color will be scaled
        /// </summary>
        public double[] ColorFactors { get; set; }

        /// <summary>
        /// Custom Parameters to use in your function.
        /// </summary>
        public List<double> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the indexed Property of the state.
        /// </summary>
        /// <param name="p">StateProperty indexer Enum</param>
        /// <returns></returns>
        public double this[StateProperty p]
        {
            get
            {
                switch (p)
                {
                    // default is parameters
                    case StateProperty.Mod: return Mod;
                    case StateProperty.ModLimLow: return ModLimLow; 
                    case StateProperty.ModLimUp: return ModLimUp;
                    case StateProperty.X0: return X0;
                    case StateProperty.Y0: return Y0;
                    case StateProperty.XZoom: return XZoom;
                    case StateProperty.YZoom: return YZoom;
                    case StateProperty.Rotation: return Rotation;
                    case StateProperty.ColorMinimum: return ColorMinimum;
                    case StateProperty.ColorSaturation: return ColorSaturation;
                    case StateProperty.ColorValue: return ColorValue;
                    case StateProperty.ColorAlpha: return ColorAlpha;
                    case StateProperty.InvalidColA: return InvalidColor[0];
                    case StateProperty.InvalidColR: return InvalidColor[1];
                    case StateProperty.InvalidColG: return InvalidColor[2];
                    case StateProperty.InvalidColB: return InvalidColor[3];
                    case StateProperty.FactorR: return ColorFactors[0];
                    case StateProperty.FactorG: return ColorFactors[1];
                    case StateProperty.FactorB: return ColorFactors[2];
                    default: return Parameters[(int)p-19];
                }
            }

            set
            {
                switch (p)
                {
                    // default is parameters
                    case StateProperty.Mod: Mod = value; break;
                    case StateProperty.ModLimLow: ModLimLow = value; break;
                    case StateProperty.ModLimUp: ModLimUp = value; break;
                    case StateProperty.X0: X0 = value; break;
                    case StateProperty.Y0: Y0 = value; break;
                    case StateProperty.XZoom: XZoom = value; break;
                    case StateProperty.YZoom: YZoom = value; break;
                    case StateProperty.Rotation: Rotation = value; break;
                    case StateProperty.ColorMinimum: ColorMinimum = value; break;
                    case StateProperty.ColorSaturation: ColorSaturation = value; break;
                    case StateProperty.ColorValue: ColorValue = value; break;
                    case StateProperty.ColorAlpha: ColorAlpha = value; break;
                    case StateProperty.InvalidColA: InvalidColor[0] = value; break;
                    case StateProperty.InvalidColR: InvalidColor[1] = value; break;
                    case StateProperty.InvalidColG: InvalidColor[2] = value; break;
                    case StateProperty.InvalidColB: InvalidColor[3] = value; break;
                    case StateProperty.FactorR: ColorFactors[0] = value; break;
                    case StateProperty.FactorG: ColorFactors[1] = value; break;
                    case StateProperty.FactorB: ColorFactors[2] = value; break;
                    default: Parameters[(int)p - 19] = value; break;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new State that has default values.
        /// </summary>
        public State()
        {
            Mod = 500;
            ModLimLow = 0;
            ModLimUp = 500;

            X0 = 0;
            Y0 = 0;
            XZoom = 1;
            YZoom = 1;
            Rotation = 0;

            ColorMinimum = 0;
            ColorSaturation = 1;
            ColorValue = 1;
            ColorAlpha = 1;
            InvalidColor = new double[] { 0, 0, 0, 0 };
            ColorFactors = new double[] { 1.0, 1.0, 1.0 };

            Parameters = new List<double>(new double[10]);

            Name = "State";
        }

        /// <summary>
        /// Constructs an "inbetween" State between two states, given the current progress and an Easing
        /// </summary>
        /// <param name="startState">The State at idx=0 (at the start)</param>
        /// <param name="endState">The State at idx=maxidx (at the end)</param>
        /// <param name="easing">An Easing Funtion</param>
        /// <param name="idx">The progress idx</param>
        /// <param name="maxidx">The maximum progress idx</param>
        public State(State startState, State endState, Easing easing, int idx, int maxidx) : this()
        {
            // Make the name. Pad With enough 0s to fit all possible idx
            Name = "Frame_" + idx.ToString().PadLeft(maxidx.ToString().Length, '0');

            // Get the eased "inbetween" value of each property
            for (StateProperty prop = StateProperty.Mod; prop <= StateProperty.i9; prop++)
            {
                this[prop] = easing.Ease(startState[prop], endState[prop], idx, maxidx);
            }
        }
        #endregion

        #region Methods for Generating Image (synchronous)
        /// <summary>
        /// Generates an Image of this State with a given a Size and a Function for the Color calculation.
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <param name="path_out">The Path to save the image at</param>
        public void GenerateImage(GenerationArgs args, string path_out = @"")
        {
            // Parsing GenerationArgs
            Size size = new Size(args.Size[0], args.Size[1]);
            Func<double, double, List<double>, double> func = (x, y, i) => Functions.Custom(new NCalc.Expression(args.Function), x, y, i);
            
            // Generate the image
            Bitmap image = GetBitmap(size, func);

            // Make path
            string path;
            if (path_out == "")
            {
                path = @"Output";
            }
            else 
            { 
                path = path_out; 
            }

            // Add State Name to Filename
            path += Path.DirectorySeparatorChar + Name;

            // Edit the filename so that it's unique
            path = Helper.ValidFileName(path);
            
            // Save the image
            image.Save(path + @".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        /// <summary>
        /// Generates the Bitmap object of the state, given a size and Function for Color calculation.
        /// </summary>
        /// <param name="size">The Size of the image</param>
        /// <param name="func">The Function for Color Calculation</param>
        /// <returns></returns>
        public Bitmap GetBitmap(Size size, Func<double, double, List<double>, double> func)
        {
            // Create instance of Bitmap for pixel data
            Bitmap image = new Bitmap(size.Width, size.Height);

            // Iterate over every pixel
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    // Calculate actual x,y values x_ & y_ (Implementing Scaling and rotation)
                    // Then pass them into the function
                    double x_ = X0 + Math.Cos(Rotation) * XZoom * (x - (double)size.Width / 2) - Math.Sin(Rotation) * YZoom * (y - size.Height / 2);
                    double y_ = Y0 + Math.Cos(Rotation) * YZoom * (y - (double)size.Height / 2) + Math.Sin(Rotation) * XZoom * (x - size.Width / 2);

                    // Validating Mod by making it double.Epsilon if it's less or equal to 0
                    if (Mod <= 0) { Mod = double.Epsilon; }

                    // Creating Instance of the pixel
                    double pixel;
                    try
                    {
                        double n = func(x_, y_, Parameters);
                        pixel = Functions.mod(n, Mod);
                    }
                    catch (Exception) { pixel = -1; }

                    // Only check for lower and upper bounds if... 
                    if (!(ModLimLow == 0 && ModLimUp == Mod))
                    {
                        // Setting Pixel to -1 if out of lower and upper bounds
                        double lowBound = Functions.mod(ModLimLow, Mod);
                        double upBound = Functions.mod(ModLimUp - 0.0001, Mod);

                        // If Bounds are equal, the pixel is automatically invalid
                        if (lowBound == upBound) { pixel = -1; }
                        
                        // if the lower bound is less than the upper bound,
                        // pixel is invalid if its value is not between the bound
                        if (lowBound < upBound)
                        {
                            if (!(pixel >= lowBound && pixel <= upBound)) { pixel = -1; }
                        }
                        // if the lower bound is greater than the upper bound,
                        // pixel is invalid if its value IS between the bounds
                        else if (lowBound > upBound)
                        {
                            if (pixel <= lowBound && pixel >= upBound) { pixel = -1; }
                        }
                    }
                    
                    // Setting col to inval col if pixel == -1
                    Color color;
                    if (pixel >= 0)
                    {
                        // Validate ColorAlpha, ColorSaturation, ColorValue (to range 0-1)
                        if (ColorAlpha > 1) { ColorAlpha = 1; } else if (ColorAlpha < 0) { ColorAlpha = 0; }
                        if (ColorSaturation > 1) { ColorSaturation = 1; } else if (ColorSaturation < 0) { ColorSaturation = 0; }
                        if (ColorValue > 1) { ColorValue = 1; } else if (ColorValue < 0) { ColorValue = 0; }

                        // Convert the value of Evaluation to hue and then to an RGB color
                        double hue = Functions.mod(ColorMinimum + 360 * pixel / Mod, 360);
                        color = Helper.ConvertHSV2RGB(hue, ColorSaturation, ColorValue);

                        // Apply the Color factors
                        int a, r, g, b;
                        a = (int)(255 * ColorAlpha);
                        r = (int)(color.R * ColorFactors[0]);
                        g = (int)(color.G * ColorFactors[1]);
                        b = (int)(color.B * ColorFactors[2]);

                        // Validate the Colors (range 0-1)
                        if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
                        if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
                        if (b > 255) { b = 255; } else if (b < 0) { b = 0; }

                        // Convert argb ints to a color Object
                        color = Color.FromArgb(a, r, g, b);
                    }
                    // If pixel was -1 set color to InvalidColor
                    else { color = Helper.ARGBFromArray(InvalidColor); }

                    // Append the pixel to the image bitmap
                    image.SetPixel(x, y, color);
                }
            }
            return image;
        }
        #endregion

        #region Serialize and Deserialize
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, options);
        }
        #endregion
    }

    /// <summary>
    /// Enum for indexing state properties
    /// </summary>
    public enum StateProperty
    {
        Mod,
        ModLimLow,
        ModLimUp,
        X0,
        Y0,
        XZoom,
        YZoom,
        Rotation,
        ColorMinimum,
        ColorSaturation,
        ColorValue,
        ColorAlpha,
        InvalidColA,
        InvalidColR,
        InvalidColG,
        InvalidColB,
        FactorR,
        FactorG,
        FactorB,
        i0, i1, i2, i3, i4, i5, i6, i7, i8, i9
    }
}
