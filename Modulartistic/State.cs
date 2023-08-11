using System;
using System.Drawing;
using System.Threading;
using System.IO;
using MathNet.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;

#nullable enable

namespace Modulartistic
{
    /// <summary>
    /// State Class to contain Data about Image States/Frames for Image Generation.
    /// </summary>
    public class State
    {
        #region Static Constants
        

        private static Dictionary<string, StateProperty> PropertyStrings = new Dictionary<string, StateProperty>();
        private static bool PropStringFilled = false;
        #endregion

        #region Fields
        private string? m_name;              // the name of this state

        private double? m_x0;                // the x-coordinate in the middle of the image
        private double? m_y0;                // the y-coordinate in the middle of the image
        private double? m_x_rot_center;      // the x-coordinate of the point around which is rotated
        private double? m_y_rot_center;      // the y-coordinate of the point around which is rotated
        private double? m_x_factor;          // value by which x-coordinates are multiplied
        private double? m_y_factor;          // value by which y-coordinates are multiplied
        private double? m_rotation;          // rotation in degrees
        
        private double? m_num;               // num which all functions are taken modulo by
        private double? m_lim_low;           // if a function mod num is less than this, assign the inval value
        private double? m_lim_high;          // if a function mod num is greater than this, assign the inval value
                                            // if lim_low > lim_high -> effects are reversed

        private double? m_hue;               // the hue offset or constant value
        private double? m_saturation;        // the saturation offset or constant value
        private double? m_value;             // the value offset or constant value

        private double? m_inv_hue;           // the hue invalid value
        private double? m_inv_saturation;    // the saturation invalid value
        private double? m_inv_value;         // the value invalid value

        private double? m_red;               // the red offset or constant value
        private double? m_green;             // the green offset or constant value
        private double? m_blue;              // the blue offset or constant value

        private double? m_inv_red;           // the red invalid value
        private double? m_inv_green;         // the green invalid value
        private double? m_inv_blue;          // the blue invalid value

        private double? m_factor_r;          // the red color factor
        private double? m_factor_g;          // the green color factor
        private double? m_factor_b;          // the blue color factor

        private double? m_alpha;             // the alpha offset or constant value
        private double? m_inv_alpha;         // the alpha invalid value

        private double[] m_parameters;
        #endregion

        #region Properties
        /// <summary>
        /// The Name of the state
        /// </summary>
        public string? Name { get => m_name; set => m_name = value; }

        /// <summary>
        /// The x-Coordinate that will be in the middle of the screen. 
        /// </summary>
        public double? X0 { get => m_x0; set => m_x0 = value; }
        /// <summary>
        /// The y-Coordinate that will be in the middle of the screen. 
        /// </summary>
        public double? Y0 { get => m_y0; set => m_y0 =value; }
        /// <summary>
        /// The x-Coordinate around which is rotated. 
        /// </summary>
        public double? XRotationCenter { get => m_x_rot_center; set => m_x_rot_center = value; }
        /// <summary>
        /// The y-Coordinate around which is rotated. 
        /// </summary>
        public double? YRotationCenter { get => m_y_rot_center; set => m_y_rot_center = value; }
        /// <summary>
        /// The factor by which the x-Coordinates will be scaled.
        /// </summary>
        public double? XFactor { get => m_x_factor; set => m_x_factor = value; }
        /// <summary>
        /// The factor by which the y-coordinates will be scaled.
        /// </summary>
        public double? YFactor { get => m_y_factor; set => m_y_factor = value; }
        /// <summary>
        /// The Amount of degrees the image will be rotated.
        /// </summary>
        public double? Rotation { get => m_rotation; set => m_rotation = value; }


        /// <summary>
        /// The Modulus Number by which all functions are taken modulo.
        /// </summary>
        public double? Mod { get => m_num; set => m_num = value; }
        /// <summary>
        /// The Lower Limit of the Modulus number. Values below will be treated as invalid.
        /// </summary>
        public double? ModLimLow { get => m_lim_low; set => m_lim_low = value; }
        /// <summary>
        /// The Upper Limit of the Modulus number. Values above will be treated as invalid.
        /// </summary>
        public double? ModLimUp { get => m_lim_high; set => m_lim_high = value; }

        
        /// <summary>
        /// The Hue Offset or Constant.
        /// </summary>
        public double? ColorHue { get => m_hue; set => m_hue = value; }
        /// <summary>
        /// The Saturation Offset or Constant. Has to be from 0-1.
        /// </summary>
        public double? ColorSaturation { get => m_saturation; set => m_saturation = value; }
        /// <summary>
        /// The Value Offset or Constant. Has to be from 0-1.
        /// </summary>
        public double? ColorValue { get => m_value; set => m_value = value; }


        /// <summary>
        /// The Hue Value for invalid results.
        /// </summary>
        public double? InvalidColorHue { get => m_inv_hue; set => m_inv_hue = value; }
        /// <summary>
        /// The Saturation Value for invalid results. Has to be from 0-1.
        /// </summary>
        public double? InvalidColorSaturation { get => m_inv_saturation; set => m_inv_saturation = value; }
        /// <summary>
        /// The Value Value for invalid results. Has to be from 0-1.
        /// </summary>
        public double? InvalidColorValue { get => m_inv_value; set => m_inv_value = value; }


        /// <summary>
        /// The Red Offset or Constant. Has to be from 0-1.
        /// </summary>
        public double? ColorRed { get => m_red; set => m_red = value; }
        /// <summary>
        /// The Green Offset or Constant. Has to be from 0-1.
        /// </summary>
        public double? ColorGreen { get => m_green; set => m_green = value; }
        /// <summary>
        /// The Blue Offset or Constant. Has to be from 0-1.
        /// </summary>
        public double? ColorBlue { get => m_blue; set => m_blue = value; }


        /// <summary>
        /// The Red Value for invalid results. Has to be from 0-1.
        /// </summary>
        public double? InvalidColorRed { get => m_inv_red; set => m_inv_red = value; }
        /// <summary>
        /// The Green Value for invalid results. Has to be from 0-1.
        /// </summary>
        public double? InvalidColorGreen { get => m_inv_green; set => m_inv_green = value; }
        /// <summary>
        /// The Blue Value for invalid results. Has to be from 0-1.
        /// </summary>
        public double? InvalidColorBlue { get => m_inv_blue; set => m_inv_blue = value; }


        /// <summary>
        /// Factor by which red is scaled at the end.
        /// </summary>
        public double? ColorFactorR { get => m_factor_r; set => m_factor_r = value; }
        /// <summary>
        /// Factor by which green is scaled at the end.
        /// </summary>
        public double? ColorFactorG { get => m_factor_g; set => m_factor_g = value; }
        /// <summary>
        /// Factor by which blue is scaled at the end.
        /// </summary>
        public double? ColorFactorB { get => m_factor_b; set => m_factor_b = value; }

        
        /// <summary>
        /// The Alpha Offset or Constant.
        /// </summary>
        public double? ColorAlpha { get => m_alpha; set => m_alpha = value; }
        /// <summary>
        /// The Alpha Value for invalid results.
        /// </summary>
        public double? InvalidColorAlpha { get => m_inv_alpha; set=> m_inv_alpha = value; }


        /// <summary>
        /// Custom Parameters to use in your function.
        /// </summary>
        public double[] Parameters { get => m_parameters; set => m_parameters = value; }


        /// <summary>
        /// Gets or sets the indexed Property of the state.
        /// </summary>
        /// <param name="p">StateProperty indexer Enum</param>
        /// <returns></returns>
        [JsonIgnore]
        public double? this[StateProperty p]
        {
            get
            {
                switch (p)
                {
                    // default is parameters
                    case StateProperty.X0: return X0;
                    case StateProperty.Y0: return Y0;
                    case StateProperty.XRotationCenter: return XRotationCenter;
                    case StateProperty.YRotationCenter: return YRotationCenter;
                    case StateProperty.XFactor: return XFactor;
                    case StateProperty.YFactor: return YFactor;
                    case StateProperty.Rotation: return Rotation;

                    case StateProperty.Mod: return Mod;
                    case StateProperty.ModLimLow: return ModLimLow; 
                    case StateProperty.ModLimHigh: return ModLimUp;
                    
                    case StateProperty.ColorHue: return ColorHue;
                    case StateProperty.ColorSaturation: return ColorSaturation;
                    case StateProperty.ColorValue: return ColorValue;
                    case StateProperty.InvalidColorHue: return InvalidColorHue;
                    case StateProperty.InvalidColorSaturation: return InvalidColorSaturation;
                    case StateProperty.InvalidColorValue: return InvalidColorValue;

                    case StateProperty.ColorRed: return ColorRed;
                    case StateProperty.ColorGreen: return ColorGreen;
                    case StateProperty.ColorBlue: return ColorBlue;
                    case StateProperty.InvalidColorRed: return InvalidColorRed;
                    case StateProperty.InvalidColorGreen: return InvalidColorGreen;
                    case StateProperty.InvalidColorBlue: return InvalidColorBlue;

                    case StateProperty.ColorFactorR: return ColorFactorR;
                    case StateProperty.ColorFactorG: return ColorFactorG;
                    case StateProperty.ColorFactorB: return ColorFactorB;

                    case StateProperty.ColorAlpha: return ColorAlpha;
                    case StateProperty.InvalidColorAlpha: return InvalidColorAlpha;

                    default: return Parameters[(int)p - (int)StateProperty.i0];
                }
            }

            set
            {
                switch (p)
                {
                    // default is parameters
                    case StateProperty.X0: X0 = value; break;
                    case StateProperty.Y0: Y0 = value; break;
                    case StateProperty.XRotationCenter: XRotationCenter = value; break;
                    case StateProperty.YRotationCenter: YRotationCenter = value; break;
                    case StateProperty.XFactor: XFactor = value; break;
                    case StateProperty.YFactor: YFactor = value; break;
                    case StateProperty.Rotation: Rotation = value; break;

                    case StateProperty.Mod: Mod = value; break;
                    case StateProperty.ModLimLow: ModLimLow = value; break;
                    case StateProperty.ModLimHigh: ModLimUp = value; break;

                    case StateProperty.ColorHue: ColorHue = value; break;
                    case StateProperty.ColorSaturation: ColorSaturation = value; break;
                    case StateProperty.ColorValue: ColorValue = value; break;
                    case StateProperty.InvalidColorHue: InvalidColorHue = value; break;
                    case StateProperty.InvalidColorSaturation: InvalidColorSaturation = value; break;
                    case StateProperty.InvalidColorValue: InvalidColorValue = value; break;

                    case StateProperty.ColorRed: ColorRed = value; break;
                    case StateProperty.ColorGreen: ColorGreen = value; break;
                    case StateProperty.ColorBlue: ColorBlue = value; break;
                    case StateProperty.InvalidColorRed: InvalidColorRed = value; break;
                    case StateProperty.InvalidColorGreen: InvalidColorGreen = value; break;
                    case StateProperty.InvalidColorBlue: InvalidColorBlue = value; break;

                    case StateProperty.ColorFactorR: ColorFactorR = value; break;
                    case StateProperty.ColorFactorG: ColorFactorG = value; break;
                    case StateProperty.ColorFactorB: ColorFactorB = value; break;

                    case StateProperty.ColorAlpha: ColorAlpha = value; break;
                    case StateProperty.InvalidColorAlpha: InvalidColorAlpha = value; break;

                    default: Parameters[(int)p - (int)StateProperty.i0] = value.GetValueOrDefault(0); break;
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
            if (!PropStringFilled) { FillPropertyStringDict(); }

            Name = null;

            X0 = null;
            Y0 = null;
            XRotationCenter = null;
            YRotationCenter = null;
            XFactor = null;
            YFactor = null;
            Rotation = null;

            Mod = null;
            ModLimLow = null;
            ModLimUp = null;

            ColorHue = null;
            ColorSaturation = null;
            ColorValue = null;

            InvalidColorHue = null;
            InvalidColorSaturation = null;
            InvalidColorValue = null;

            ColorRed = null;
            ColorBlue = null;
            ColorGreen = null;

            InvalidColorRed = null;
            InvalidColorBlue = null;
            InvalidColorGreen = null;

            ColorAlpha = null;
            InvalidColorAlpha = null;
            
            ColorFactorR = null;
            ColorFactorG = null;
            ColorFactorB = null;

            m_parameters = new double[10];
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
            if (!PropStringFilled) { FillPropertyStringDict(); }

            // Make the name. Pad With enough 0s to fit all possible idx
            Name = "Frame_" + idx.ToString().PadLeft(maxidx.ToString().Length, '0');

            // Get the eased "inbetween" value of each property
            for (StateProperty prop = StateProperty.Mod; prop <= StateProperty.i9; prop++)
            {
                this[prop] = easing.Ease(
                    startState[prop].GetValueOrDefault(GetDefaultValue(prop)), 
                    endState[prop].GetValueOrDefault(GetDefaultValue(prop)), 
                    idx, 
                    maxidx);
            }
        }
        #endregion

        #region Methods for Generating Image (synchronous)
        /// <summary>
        /// Generates an Image of this State with a given a Size and a Function for the Color calculation.
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <param name="path_out">The Path to save the image at</param>
        /// <returns>the filepath to the generated image</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown if path_out does not exist</exception>
        public string GenerateImage(GenerationArgs args, string path_out = @"")
        {
            // Generate the image
            Bitmap image = GetBitmap(args);

            // Creating filename and path, checking if directory exists
            string path = path_out == "" ? AppDomain.CurrentDomain.BaseDirectory + "Output" : path_out;
            if (!Directory.Exists(path)) { throw new DirectoryNotFoundException("The Directory " + path + " was not found."); }
            path += Path.DirectorySeparatorChar + (string.IsNullOrEmpty(Name) ? Constants.STATENAME_DEFAULT : Name);

            // Edit the filename so that it's unique
            path = Helper.ValidFileName(path);
            
            // Save the image
            image.Save(path + @".png", System.Drawing.Imaging.ImageFormat.Png);

            // return the filepath
            return path + @".png";
        }

        /// <summary>
        /// Generates the Bitmap object of the state, given a size and Function for Color calculation.
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <returns>The Generated Bitmap</returns>
        public Bitmap GetBitmap(GenerationArgs args)
        {
            // Parsing GenerationArgs
            Size size = new Size(args.Size[0], args.Size[1]);

            bool invalGlobal = args.InvalidColorGlobal.GetValueOrDefault(Constants.INVALIDCOLORGLOBAL_DEFAULT);
            bool circ = args.Circular.GetValueOrDefault(Constants.CIRCULAR_DEFAULT);
            bool useRGB = args.UseRGB.GetValueOrDefault(Constants.USERGB_DEFAULT);

            
            Function? HueFunc = null;
            Function? SatFunc = null;
            Function? ValFunc = null;

            Function? RedFunc = null;
            Function? GreFunc = null;
            Function? BluFunc = null;

            Function? AlpFunc = null;

            if (useRGB)
            {
                if (!string.IsNullOrEmpty(args.RedFunction))
                {
                    RedFunc = new Function(args.RedFunction);
                    if (args.AddOns != null) RedFunc.LoadAddOns(args.AddOns.ToArray());
                }
                if (!string.IsNullOrEmpty(args.GreenFunction))
                {
                    GreFunc = new Function(args.GreenFunction);
                    if (args.AddOns != null) GreFunc.LoadAddOns(args.AddOns.ToArray());
                }
                if (!string.IsNullOrEmpty(args.BlueFunction))
                {
                    BluFunc = new Function(args.BlueFunction);
                    if (args.AddOns != null) BluFunc.LoadAddOns(args.AddOns.ToArray());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(args.HueFunction))
                {
                    HueFunc = new Function(args.HueFunction);
                    if (args.AddOns != null) HueFunc.LoadAddOns(args.AddOns.ToArray());
                }

                if (!string.IsNullOrEmpty(args.SaturationFunction))
                {
                    SatFunc = new Function(args.SaturationFunction);
                    if (args.AddOns != null) SatFunc.LoadAddOns(args.AddOns.ToArray());
                }

                if (!string.IsNullOrEmpty(args.ValueFunction))
                {
                    ValFunc = new Function(args.ValueFunction);
                    if (args.AddOns != null) ValFunc.LoadAddOns(args.AddOns.ToArray());
                }
            }

            if (!string.IsNullOrEmpty(args.AlphaFunction))
            {
                AlpFunc = new Function(args.AlphaFunction);
                if (args.AddOns != null) AlpFunc.LoadAddOns(args.AddOns.ToArray());
            }

            // Create instance of Bitmap for pixel data
            Bitmap image = new Bitmap(size.Width, size.Height);

            // Iterate over every pixel
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    // Calculate actual x,y values x_ & y_ (Implementing Scaling and rotation)
                    // shift the Rotation Center to Origin
                    double x_1 = XFactor.GetValueOrDefault(Constants.XYFACTOR_DEFAULT) * x + XRotationCenter.GetValueOrDefault(Constants.XYROTCENTER_DEFAULT);
                    double y_1 = YFactor.GetValueOrDefault(Constants.XYFACTOR_DEFAULT) * y + YRotationCenter.GetValueOrDefault(Constants.XYROTCENTER_DEFAULT);
                    // calculate the rotation in radiants
                    double rotrad = 2 * Math.PI * Rotation.GetValueOrDefault(Constants.ROTATION_DEFAULT) / 360;
                    double x_ = x_1 * Math.Cos(rotrad) - y_1 * Math.Sin(rotrad)  // Apply rotation
                        + X0.GetValueOrDefault(Constants.XY0_DEFAULT)                      // Shift X0 to Origin
                        - size.Width / 2;                                        // Shift Origin to middle of screen
                    
                    double y_ = x_1 * Math.Sin(rotrad) + y_1 * Math.Cos(rotrad)  // Apply rotation
                        + Y0.GetValueOrDefault(Constants.XY0_DEFAULT)                      // Shift Y0 to Origin
                        - size.Height / 2;                                       // Shift Origin to middle of Screen

                    // Validating Mod by making it double.Epsilon if it's less or equal to 0
                    if (Mod <= 0) { Mod = double.Epsilon; }

                    // Creating Instance of the pixel
                    double pixel_r_h = 0, // red or hue
                        pixel_g_s = 0,    // green or saturation
                        pixel_b_v = 0,    // blue or value
                        pixel_alp = 0;    // alpha

                    // this whole section feels like terrible bad practice
                    bool all_inval = invalGlobal;
                    do
                    {
                        // just a container variable
                        double n;
                        
                        // the try catches possible Errors and sets the value
                        
                        // Red or Hue
                        try
                        {
                            if ((useRGB ? RedFunc : HueFunc) == null) { pixel_r_h = 0; }
                            else
                            {
                                n = (useRGB ? RedFunc : HueFunc).Evaluate(x_, y_, Parameters, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                                pixel_r_h = n.IsFinite() ? Helper.mod(n, Mod.GetValueOrDefault(Constants.NUM_DEFAULT)) : -1;
                            }
                        }
                        catch (Exception) { pixel_r_h = -1; }
                        if (all_inval && (pixel_r_h == -1)) { break; }

                        // Green or Saturation
                        try
                        {
                            if ((useRGB ? GreFunc : SatFunc) == null) { pixel_g_s = 0; }
                            else
                            {
                                n = (useRGB ? GreFunc : SatFunc).Evaluate(x_, y_, Parameters, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                                pixel_g_s = n.IsFinite() ? Helper.mod(n, Mod.GetValueOrDefault(Constants.NUM_DEFAULT)) : -1;
                            }
                        }
                        catch (Exception) { pixel_g_s = -1; }
                        if (all_inval && (pixel_g_s == -1)) { break; }

                        // Blue or Value
                        try
                        {
                            if ((useRGB ? BluFunc : ValFunc) == null) { pixel_b_v = 0; }
                            else
                            {
                                n = (useRGB ? BluFunc : ValFunc).Evaluate(x_, y_, Parameters, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                                pixel_b_v = n.IsFinite() ? Helper.mod(n, Mod.GetValueOrDefault(Constants.NUM_DEFAULT)) : -1;
                            }
                        }
                        catch (Exception) { pixel_b_v = -1; }
                        if (all_inval && (pixel_b_v == -1)) { break; }

                        // Alpha
                        try
                        {
                            if (AlpFunc == null) { pixel_alp = 0; }
                            else
                            {
                                n = AlpFunc.Evaluate(x_, y_, Parameters, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                                pixel_alp = n.IsFinite() ? Helper.mod(n, Mod.GetValueOrDefault(Constants.NUM_DEFAULT)) : -1;
                            }
                            
                        }
                        catch (Exception) { pixel_alp = -1; }
                        if (all_inval &= (pixel_alp == -1)) { break; }
                    } while (false);

                    // if all inval is true set all values to -1
                    if (all_inval)
                    {
                        pixel_r_h = -1;
                        pixel_g_s = -1;
                        pixel_b_v = -1;
                        pixel_alp = -1;
                    }
                    
                    // Only check for lower and upper bounds if... 
                    if (!all_inval && !(ModLimLow == 0 && ModLimUp == Mod))
                    {
                        // Setting Pixel to -1 if out of lower and upper bounds
                        double lowBound = Helper.inclusiveMod(ModLimLow.GetValueOrDefault(Constants.LIMLOW_DEFAULT), Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                        double upBound = Helper.inclusiveMod(ModLimUp.GetValueOrDefault(Constants.LIMHIGH_DEFAULT), Mod.GetValueOrDefault(Constants.NUM_DEFAULT));

                        // If Bounds are equal, the pixel is automatically invalid
                        if (lowBound == upBound)
                        {
                            pixel_r_h = -1;
                            pixel_g_s = -1;
                            pixel_b_v = -1;
                            pixel_alp = -1;
                        }

                        // if the lower bound is less than the upper bound,
                        // pixel is invalid if its value is not between the bound
                        if (lowBound < upBound)
                        {
                            if (!(pixel_r_h >= lowBound && pixel_r_h <= upBound)) { pixel_r_h = -1; }
                            if (!(pixel_g_s >= lowBound && pixel_g_s <= upBound)) { pixel_g_s = -1; }
                            if (!(pixel_b_v >= lowBound && pixel_b_v <= upBound)) { pixel_b_v = -1; }
                            if (!(pixel_alp >= lowBound && pixel_alp <= upBound)) { pixel_alp = -1; }
                        }
                        // if the lower bound is greater than the upper bound,
                        // pixel is invalid if its value IS between the bounds
                        else if (lowBound > upBound)
                        {
                            if (pixel_r_h <= lowBound && pixel_r_h >= upBound) { pixel_r_h = -1; }
                            if (pixel_g_s <= lowBound && pixel_g_s >= upBound) { pixel_g_s = -1; }
                            if (pixel_b_v <= lowBound && pixel_b_v >= upBound) { pixel_b_v = -1; }
                            if (pixel_alp <= lowBound && pixel_alp >= upBound) { pixel_alp = -1; }
                        }
                    }
                    
                    // Setting col to inval col if pixel == -1
                    Color color;
                    if (useRGB)
                    {
                        int a, r, g, b;
                        if (pixel_alp == -1) { a = (int)(255 * InvalidColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT)); }
                        else
                        {
                            a = (int)(circ ?
                            Helper.circ(ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT) + pixel_alp / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255 :
                            Helper.inclusiveMod(ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT) + pixel_alp / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255);
                        }

                        if (pixel_r_h == -1) { r = (int)(255 * InvalidColorRed.GetValueOrDefault(Constants.COLOR_DEFAULT)); }
                        else
                        {
                            r = (int)(circ ?
                            Helper.circ(ColorRed.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_r_h / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255 :
                            Helper.inclusiveMod(ColorRed.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_r_h / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255);
                        }

                        if (pixel_g_s == -1) { g = (int)(255 * InvalidColorGreen.GetValueOrDefault(Constants.COLOR_DEFAULT)); }
                        else
                        {
                            g = (int)(circ ?
                            Helper.circ(ColorGreen.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_g_s / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255 :
                            Helper.inclusiveMod(ColorGreen.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_g_s / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255);
                        }
                        
                        if (pixel_b_v == -1) { b = (int)(255 * InvalidColorBlue.GetValueOrDefault(Constants.COLOR_DEFAULT)); }
                        else
                        {
                            b = (int)(circ ?
                            Helper.circ(ColorBlue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_b_v / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255 :
                            Helper.inclusiveMod(ColorBlue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_b_v / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255);
                        }
                        
                        // Apply the Color factors
                        r = (int)(r * ColorFactorR.GetValueOrDefault(Constants.COLORFACT_DEFAULT));
                        g = (int)(g * ColorFactorG.GetValueOrDefault(Constants.COLORFACT_DEFAULT));
                        b = (int)(b * ColorFactorB.GetValueOrDefault(Constants.COLORFACT_DEFAULT));

                        // Validate the Colors (range 0-255)
                        if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
                        if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
                        if (b > 255) { b = 255; } else if (b < 0) { b = 0; }

                        color = Color.FromArgb(a, r, g, b);
                    }
                    else
                    {
                        double a, h, s, v;
                        if (pixel_alp == -1) { a = InvalidColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT); }
                        else
                        {
                            a = circ ?
                            Helper.circ(ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT) + pixel_alp / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) :
                            Helper.inclusiveMod(ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT) + pixel_alp / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1);
                        }

                        if (pixel_r_h == -1) { h = InvalidColorHue.GetValueOrDefault(Constants.COLOR_DEFAULT); }
                        else
                        {
                            h = Helper.inclusiveMod(ColorHue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_r_h / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 360;
                        }

                        if (pixel_g_s == -1) { s = InvalidColorSaturation.GetValueOrDefault(Constants.COLOR_DEFAULT); }
                        else
                        {
                            s = circ ?
                            Helper.circ(ColorSaturation.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_g_s / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) :
                            Helper.inclusiveMod(ColorSaturation.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_g_s / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1);
                        }

                        if (pixel_b_v == -1) { v = InvalidColorValue.GetValueOrDefault(Constants.COLOR_DEFAULT); }
                        else
                        {
                            v = circ ?
                            Helper.circ(ColorValue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_b_v / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) :
                            Helper.inclusiveMod(ColorValue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_b_v / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1);
                        }

                        color = Helper.ConvertHSV2RGB(h, s, v);
                        int r, g, b;
                        // Apply the Color factors
                        r = (int)(color.R * ColorFactorR.GetValueOrDefault(Constants.COLORFACT_DEFAULT));
                        g = (int)(color.G * ColorFactorG.GetValueOrDefault(Constants.COLORFACT_DEFAULT));
                        b = (int)(color.B * ColorFactorB.GetValueOrDefault(Constants.COLORFACT_DEFAULT));

                        // Validate the Colors (range 0-255)
                        if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
                        if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
                        if (b > 255) { b = 255; } else if (b < 0) { b = 0; }

                        // Console.WriteLine(r.ToString() + " " + g.ToString() + " " + b.ToString());

                        // Update Color
                        color = Color.FromArgb((int)(255*a), r, g, b);
                    }
                    // Console.WriteLine(color.R.ToString() + " " + color.G.ToString() + " " + color.B.ToString());
                    // Append the pixel to the image bitmap
                    image.SetPixel(x, y, color);
                }
            }
            return image;
        }
        #endregion

        #region Methods for multi-threaded Generating
        /// <summary>
        /// Generates an Image of this State with a given a Size and a Function for the Color calculation. If max_threads = -1 the max number is used
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <param name="max_threads">The maximum number of threads this will use</param>
        /// <param name="path_out">The Path to save the image at</param>
        /// <returns>the filepath of the generated image</returns>
        /// <exception cref="DirectoryNotFoundException">thrown if path_out does not exist</exception>
        public string GenerateImage(GenerationArgs args, int max_threads, string path_out = @"")
        {
            // Generate the image
            Bitmap image = GetBitmap(args, max_threads);

            // Creating filename and path, checking if directory exists
            string path = path_out == "" ? AppDomain.CurrentDomain.BaseDirectory + "Output" : path_out;
            if (!Directory.Exists(path)) { throw new DirectoryNotFoundException("The Directory " + path + " was not found."); }
            path += Path.DirectorySeparatorChar + (string.IsNullOrEmpty(Name) ? Constants.STATENAME_DEFAULT : Name);

            // Edit the filename so that it's unique
            path = Helper.ValidFileName(path);

            // Save the image
            image.Save(path + @".png", System.Drawing.Imaging.ImageFormat.Png);

            return path + @".png";
        }

        /// <summary>
        /// Generates the Bitmap object of the state, given a size and Function for Color calculation. If max_threads = -1 the max number is used
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <param name="max_threads">The maximum number of threads this will use</param>
        /// <returns>The generated Bitmap</returns>
        public Bitmap GetBitmap(GenerationArgs args, int max_threads)
        {
            // Parsing GenerationArgs Size
            Size size = new Size(args.Size[0], args.Size[1]);

            // Set the number 
            int threads_num = max_threads;
            if (max_threads == -1 || max_threads > Environment.ProcessorCount) { threads_num = Environment.ProcessorCount; }
            else if (max_threads < 1) { threads_num = 1; }

            // Create instance of Bitmap for pixel data and array of Bitmaps for Threads to work on
            Bitmap image = new Bitmap(size.Width, size.Height);
            Bitmap[] partial_images = new Bitmap[threads_num];

            // Create the Threads
            Thread[] threads = new Thread[threads_num];

            // Run the threads
            for (int i = 0; i < threads_num; i++)
            {
                // index needs to be made local for the lambda function
                int local_i = i;

                threads[local_i] = new Thread(new ThreadStart(() => GetPartialBitmap(args, local_i, threads_num, out partial_images[local_i])));
                threads[local_i].Start();
            }

            // Join the Threads and put all partial Bitmaps together
            Graphics gr = Graphics.FromImage(image);
            for (int i = 0; i < threads_num; i++)
            {
                threads[i].Join();
                gr.DrawImage(partial_images[i], i * (size.Width / threads_num), 0, partial_images[i].Width, partial_images[i].Height); 
            }

            // return the Bitmap
            return image;
        }

        /// <summary>
        /// Generates part of the Bitmap object of the state, given a size and Function for Color calculation and index of the thread and number of max threads.
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <param name="idx">The number of the Thread</param>
        /// <param name="max">The Total Number of Threads</param>
        /// <param name="image">The Bitmap where the Data should be stored</param>
        private void GetPartialBitmap(GenerationArgs args, int idx, int max, out Bitmap image)
        {
            // Parsing GenerationArgs
            Size size = new Size(args.Size[0], args.Size[1]);

            bool invalGlobal = args.InvalidColorGlobal ?? true;
            bool circ = args.Circular ?? true;
            bool useRGB = args.UseRGB ?? false;


            Function? HueFunc = null;
            Function? SatFunc = null;
            Function? ValFunc = null;

            Function? RedFunc = null;
            Function? GreFunc = null;
            Function? BluFunc = null;

            Function? AlpFunc = null;

            if (useRGB)
            {
                if (!string.IsNullOrEmpty(args.RedFunction))
                {
                    RedFunc = new Function(args.RedFunction);
                    if (args.AddOns != null) RedFunc.LoadAddOns(args.AddOns.ToArray());
                }
                if (!string.IsNullOrEmpty(args.GreenFunction))
                {
                    GreFunc = new Function(args.GreenFunction);
                    if (args.AddOns != null) GreFunc.LoadAddOns(args.AddOns.ToArray());
                }
                if (!string.IsNullOrEmpty(args.BlueFunction))
                {
                    BluFunc = new Function(args.BlueFunction);
                    if (args.AddOns != null) BluFunc.LoadAddOns(args.AddOns.ToArray());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(args.HueFunction))
                {
                    HueFunc = new Function(args.HueFunction);
                    if (args.AddOns != null) HueFunc.LoadAddOns(args.AddOns.ToArray());
                }

                if (!string.IsNullOrEmpty(args.SaturationFunction))
                {
                    SatFunc = new Function(args.SaturationFunction);
                    if (args.AddOns != null) SatFunc.LoadAddOns(args.AddOns.ToArray());
                }

                if (!string.IsNullOrEmpty(args.ValueFunction))
                {
                    ValFunc = new Function(args.ValueFunction);
                    if (args.AddOns != null) ValFunc.LoadAddOns(args.AddOns.ToArray());
                }
            }

            if (!string.IsNullOrEmpty(args.AlphaFunction))
            {
                AlpFunc = new Function(args.AlphaFunction);
                if (args.AddOns != null) AlpFunc.LoadAddOns(args.AddOns.ToArray());
            }

            // setting the Width of the Bitmap and the first pixel to generate
            int partial_width = size.Width / max;
            int first_px = idx * partial_width;
            if (idx == max - 1) { partial_width = size.Width - first_px; }
            
            // setting the Bitmap
            image = new Bitmap(partial_width, size.Height);

            // Iterate over every pixel
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < partial_width; x++)
                {
                    // Calculate actual x,y values x_ & y_ (Implementing Scaling and rotation)
                    // shift the Rotation Center to Origin
                    double x_1 = XFactor.GetValueOrDefault(Constants.XYFACTOR_DEFAULT) * (x + first_px) + XRotationCenter.GetValueOrDefault(Constants.XYROTCENTER_DEFAULT);
                    double y_1 = YFactor.GetValueOrDefault(Constants.XYFACTOR_DEFAULT) * y + YRotationCenter.GetValueOrDefault(Constants.XYROTCENTER_DEFAULT);
                    // calculate the rotation in radiants
                    double rotrad = 2 * Math.PI * Rotation.GetValueOrDefault(Constants.ROTATION_DEFAULT) / 360;
                    double x_ = x_1 * Math.Cos(rotrad) - y_1 * Math.Sin(rotrad)  // Apply rotation
                        + X0.GetValueOrDefault(Constants.XY0_DEFAULT)                      // Shift X0 to Origin
                        - size.Width / 2;                                        // Shift Origin to middle of screen

                    double y_ = x_1 * Math.Sin(rotrad) + y_1 * Math.Cos(rotrad)  // Apply rotation
                        + Y0.GetValueOrDefault(Constants.XY0_DEFAULT)                      // Shift Y0 to Origin
                        - size.Height / 2;                                       // Shift Origin to middle of Screen

                    // Validating Mod by making it double.Epsilon if it's less or equal to 0
                    if (Mod <= 0) { Mod = double.Epsilon; }

                    // Creating Instance of the pixel
                    double pixel_r_h = 0, // red or hue
                        pixel_g_s = 0,    // green or saturation
                        pixel_b_v = 0,    // blue or value
                        pixel_alp = 0;    // alpha

                    // this whole section feels like terrible bad practice
                    bool all_inval = invalGlobal;
                    do
                    {
                        // just a container variable
                        double n;

                        // the try catches possible Errors and sets the value

                        // Red or Hue
                        try
                        {
                            if ((useRGB ? RedFunc : HueFunc) == null) { pixel_r_h = 0; }
                            else
                            {
                                n = (useRGB ? RedFunc : HueFunc).Evaluate(x_, y_, Parameters, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                                pixel_r_h = n.IsFinite() ? Helper.mod(n, Mod.GetValueOrDefault(Constants.NUM_DEFAULT)) : -1;
                            }
                        }
                        catch (Exception) { pixel_r_h = -1; }
                        if (all_inval && (pixel_r_h == -1)) { break; }

                        // Green or Saturation
                        try
                        {
                            if ((useRGB ? GreFunc : SatFunc) == null) { pixel_g_s = 0; }
                            else
                            {
                                n = (useRGB ? GreFunc : SatFunc).Evaluate(x_, y_, Parameters, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                                pixel_g_s = n.IsFinite() ? Helper.mod(n, Mod.GetValueOrDefault(Constants.NUM_DEFAULT)) : -1;
                            }
                        }
                        catch (Exception) { pixel_g_s = -1; }
                        if (all_inval && (pixel_g_s == -1)) { break; }

                        // Blue or Value
                        try
                        {
                            if ((useRGB ? BluFunc : ValFunc) == null) { pixel_b_v = 0; }
                            else
                            {
                                n = (useRGB ? BluFunc : ValFunc).Evaluate(x_, y_, Parameters, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                                pixel_b_v = n.IsFinite() ? Helper.mod(n, Mod.GetValueOrDefault(Constants.NUM_DEFAULT)) : -1;
                            }
                        }
                        catch (Exception) { pixel_b_v = -1; }
                        if (all_inval && (pixel_b_v == -1)) { break; }

                        // Alpha
                        try
                        {
                            if (AlpFunc == null) { pixel_alp = 0; }
                            else
                            {
                                n = AlpFunc.Evaluate(x_, y_, Parameters, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                                pixel_alp = n.IsFinite() ? Helper.mod(n, Mod.GetValueOrDefault(Constants.NUM_DEFAULT)) : -1;
                            }
                        }
                        catch (Exception) { pixel_alp = -1; }
                        if (all_inval &= (pixel_alp == -1)) { break; }
                    } while (false);

                    // if all inval is true set all values to -1
                    if (all_inval)
                    {
                        pixel_r_h = -1;
                        pixel_g_s = -1;
                        pixel_b_v = -1;
                        pixel_alp = -1;
                    }


                    // Only check for lower and upper bounds if... 
                    if (!all_inval && !(ModLimLow == 0 && ModLimUp == Mod))
                    {
                        // Setting Pixel to -1 if out of lower and upper bounds
                        double lowBound = Helper.inclusiveMod(ModLimLow.GetValueOrDefault(Constants.LIMLOW_DEFAULT), Mod.GetValueOrDefault(Constants.NUM_DEFAULT));
                        double upBound = Helper.inclusiveMod(ModLimUp.GetValueOrDefault(Constants.LIMHIGH_DEFAULT) - 0.0001, Mod.GetValueOrDefault(Constants.NUM_DEFAULT));

                        // If Bounds are equal, the pixel is automatically invalid
                        if (lowBound == upBound)
                        {
                            pixel_r_h = -1;
                            pixel_g_s = -1;
                            pixel_b_v = -1;
                            pixel_alp = -1;
                        }

                        // if the lower bound is less than the upper bound,
                        // pixel is invalid if its value is not between the bound
                        if (lowBound < upBound)
                        {
                            if (!(pixel_r_h >= lowBound && pixel_r_h <= upBound)) { pixel_r_h = -1; }
                            if (!(pixel_g_s >= lowBound && pixel_g_s <= upBound)) { pixel_g_s = -1; }
                            if (!(pixel_b_v >= lowBound && pixel_b_v <= upBound)) { pixel_b_v = -1; }
                            if (!(pixel_alp >= lowBound && pixel_alp <= upBound)) { pixel_alp = -1; }
                        }
                        // if the lower bound is greater than the upper bound,
                        // pixel is invalid if its value IS between the bounds
                        else if (lowBound > upBound)
                        {
                            if (pixel_r_h <= lowBound && pixel_r_h >= upBound) { pixel_r_h = -1; }
                            if (pixel_g_s <= lowBound && pixel_g_s >= upBound) { pixel_g_s = -1; }
                            if (pixel_b_v <= lowBound && pixel_b_v >= upBound) { pixel_b_v = -1; }
                            if (pixel_alp <= lowBound && pixel_alp >= upBound) { pixel_alp = -1; }
                        }
                    }

                    // Setting col to inval col if pixel == -1
                    Color color;
                    if (useRGB)
                    {
                        int a, r, g, b;
                        if (pixel_alp == -1) { a = (int)(255 * InvalidColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT)); }
                        else
                        {
                            a = (int)(circ ?
                            Helper.circ(ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT) + pixel_alp / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255 :
                            Helper.inclusiveMod(ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT) + pixel_alp / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255);
                        }

                        if (pixel_r_h == -1) { r = (int)(255 * InvalidColorRed.GetValueOrDefault(Constants.COLOR_DEFAULT)); }
                        else
                        {
                            r = (int)(circ ?
                            Helper.circ(ColorRed.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_r_h / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255 :
                            Helper.inclusiveMod(ColorRed.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_r_h / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255);
                        }

                        if (pixel_g_s == -1) { g = (int)(255 * InvalidColorGreen.GetValueOrDefault(Constants.COLOR_DEFAULT)); }
                        else
                        {
                            g = (int)(circ ?
                            Helper.circ(ColorGreen.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_g_s / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255 :
                            Helper.inclusiveMod(ColorGreen.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_g_s / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255);
                        }

                        if (pixel_b_v == -1) { b = (int)(255 * InvalidColorBlue.GetValueOrDefault(Constants.COLOR_DEFAULT)); }
                        else
                        {
                            b = (int)(circ ?
                            Helper.circ(ColorBlue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_b_v / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255 :
                            Helper.inclusiveMod(ColorBlue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_b_v / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 255);
                        }

                        // Apply the Color factors
                        r = (int)(r * ColorFactorR.GetValueOrDefault(Constants.COLORFACT_DEFAULT));
                        g = (int)(g * ColorFactorG.GetValueOrDefault(Constants.COLORFACT_DEFAULT));
                        b = (int)(b * ColorFactorB.GetValueOrDefault(Constants.COLORFACT_DEFAULT));

                        // Validate the Colors (range 0-255)
                        if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
                        if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
                        if (b > 255) { b = 255; } else if (b < 0) { b = 0; }

                        color = Color.FromArgb(a, r, g, b);
                    }
                    else
                    {
                        double a, h, s, v;
                        if (pixel_alp == -1) { a = InvalidColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT); }
                        else
                        {
                            a = circ ?
                            Helper.circ(ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT) + pixel_alp / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) :
                            Helper.inclusiveMod(ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT) + pixel_alp / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1);
                        }

                        if (pixel_r_h == -1) { h = InvalidColorHue.GetValueOrDefault(Constants.COLOR_DEFAULT); }
                        else
                        {
                            h = Helper.mod(ColorHue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_r_h / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) * 360;
                        }

                        if (pixel_g_s == -1) { s = InvalidColorSaturation.GetValueOrDefault(Constants.COLOR_DEFAULT); }
                        else
                        {
                            s = circ ?
                            Helper.circ(ColorSaturation.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_g_s / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) :
                            Helper.inclusiveMod(ColorSaturation.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_g_s / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1);
                        }

                        if (pixel_b_v == -1) { v = InvalidColorValue.GetValueOrDefault(Constants.COLOR_DEFAULT); }
                        else
                        {
                            v = circ ?
                            Helper.circ(ColorValue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_b_v / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1) :
                            Helper.inclusiveMod(ColorValue.GetValueOrDefault(Constants.COLOR_DEFAULT) + pixel_b_v / Mod.GetValueOrDefault(Constants.NUM_DEFAULT), 1);
                        }

                        color = Helper.ConvertHSV2RGB(h, s, v);
                        int r, g, b;
                        // Apply the Color factors
                        r = (int)(color.R * ColorFactorR.GetValueOrDefault(Constants.COLORFACT_DEFAULT));
                        g = (int)(color.G * ColorFactorG.GetValueOrDefault(Constants.COLORFACT_DEFAULT));
                        b = (int)(color.B * ColorFactorB.GetValueOrDefault(Constants.COLORFACT_DEFAULT));

                        // Validate the Colors (range 0-255)
                        if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
                        if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
                        if (b > 255) { b = 255; } else if (b < 0) { b = 0; }

                        // Update Color
                        color = Color.FromArgb((int)(255 * a), r, g, b);
                    }

                    // Append the pixel to the image bitmap
                    image.SetPixel(x, y, color);
                }
            }
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Serializes the State to Json
        /// </summary>
        /// <returns>The Serialized State as string.</returns>
        public string ToJson()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                WriteIndented = true,
            };
            return JsonSerializer.Serialize(this, options);
        }
        
        public static State FromJson(string json_string)
        {
            JsonSerializerOptions option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
            };
            return JsonSerializer.Deserialize<State>(json_string);
            /*
            State result = new State();
            
            JsonDocument jd = JsonDocument.Parse(json_str);
            JsonElement root = jd.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new Exception("Error: Expected ObjectType RootElement in JsonText");
            }

            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                AllowTrailingCommas = true,
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter(),
                },
            };

            foreach (JsonProperty prop in root.EnumerateObject())
            {
                // handle Name seperately
                if (prop.Name == "Name") { result.Name = prop.Value.GetString(); continue; }

                // handle Parameters
                if (prop.Name == "Parameters") 
                { 
                    int i = 0;
                    foreach (JsonElement el in prop.Value.EnumerateArray())
                    {
                        if (el.TryGetDouble(out double a)) { result.Parameters[i] = a; }
                        else { throw new Exception("Error: Expected a double but got a " + el.GetType().ToString()); }
                        i++;
                    }
                }

                // handle wrong keys
                if (!PropertyStrings.ContainsKey(prop.Name))
                {
                    throw new Exception("Error: Unexpected property found: " + prop.Name);
                }

                if (PropertyStrings[prop.Name] >= StateProperty.i0) { throw new Exception("Error: Setting Parameters with i0-i9 is not supported. Use Parameter Array instead."); }

                else
                {
                    StateProperty p = PropertyStrings[prop.Name];
                    if (prop.Value.TryGetDouble(out double a)) { result[p] = a; }
                    else { throw new Exception("Error: Expected a double but got a " + prop.GetType().ToString()); }
                }
            }
            return result;*/
        }

        /// <summary>
        /// Gets details about this state. Useful for debugging. 
        /// </summary>
        /// <returns>A formatted details string</returns>
        public string GetDebugInfo()
        {
            string result =
                string.IsNullOrEmpty(Name) ? "" : $"{"Name: ",-30} {Name} \n" +

                (X0 == null ? "" : $"{"X0 Coordinate: ",-30} {X0} \n") +
                (Y0 == null ? "" : $"{"Y0 Coordinate: ",-30} {Y0} \n") +
                (XRotationCenter == null ? "" : $"{"X Rotation Center: ",-30} {XRotationCenter} \n") +
                (YRotationCenter == null ? "" : $"{"Y Rotation Center: ",-30} {YRotationCenter} \n") +
                (XFactor == null ? "" : $"{"X-Factor: ",-30} {XFactor} \n") +
                (YFactor == null ? "" : $"{"Y-Factor: ",-30} {YFactor} \n") +
                (Rotation == null ? "" : $"{"Rotation: ",-30} {Rotation} \n") +

                (Mod == null ? "" : $"{"Modulus Number: ",-30} {Mod} \n") +
                (ModLimLow == null ? "" : $"{"Modulus Lower Limit: ",-30} {ModLimLow} \n") +
                (ModLimUp == null ? "" : $"{"Modulus Upper Limit: ",-30} {ModLimUp} \n") +

                (ColorHue == null ? "" : $"{"Color Hue: ",-30} {ColorHue} \n") +
                (ColorSaturation == null ? "" : $"{"Color Saturation: ",-30} {ColorSaturation} \n") +
                (ColorValue == null ? "" : $"{"Color Value: ",-30} {ColorValue} \n") +
                (InvalidColorHue == null ? "" : $"{"Invalid Color Hue: ",-30} {InvalidColorHue} \n") +
                (InvalidColorSaturation == null ? "" : $"{"Invalid Color Saturation: ",-30} {InvalidColorSaturation} \n") +
                (InvalidColorValue == null ? "" : $"{"Invalid Color Value: ",-30} {InvalidColorValue} \n") +

                (ColorBlue == null ? "" : $"{"Color Red: ",-30} {ColorBlue} \n") +
                (ColorGreen == null ? "" : $"{"Color Green: ",-30} {ColorGreen} \n") +
                (ColorBlue == null ? "" : $"{"Color Blue: ",-30} {ColorBlue} \n") +
                (InvalidColorRed == null ? "" : $"{"Invalid Color Red: ",-30} {InvalidColorRed} \n") +
                (InvalidColorGreen == null ? "" : $"{"Invalid Color Green: ",-30} {InvalidColorGreen} \n") +
                (InvalidColorBlue == null ? "" : $"{"Invalid Color Blue: ",-30} {InvalidColorBlue} \n") +

                $"{"Color Factors (R G B): ",-30} {ColorFactorR} {ColorFactorG} {ColorFactorB} \n" +

                (ColorAlpha == null ? "" : $"{"Color Alpha: ",-30} {ColorAlpha} \n") +
                (InvalidColorAlpha == null ? "" : $"{"Invalid Color Alpha: ",-30} {InvalidColorAlpha} \n") +
                
                $"{"Parameters: ",-30} {Parameters[0]} {Parameters[1]} {Parameters[2]} {Parameters[3]} {Parameters[4]} {Parameters[5]} {Parameters[6]} {Parameters[7]} {Parameters[8]} {Parameters[9]}";

            return result;
        }
        
        /// <summary>
        /// Gets the default Value for the specified property
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static double GetDefaultValue(StateProperty prop)
        {
            switch (prop)
            {
                case StateProperty.X0:
                case StateProperty.Y0: return Constants.XY0_DEFAULT;
                
                case StateProperty.XRotationCenter:
                case StateProperty.YRotationCenter: return Constants.XYROTCENTER_DEFAULT;

                case StateProperty.XFactor:
                case StateProperty.YFactor: return Constants.XYFACTOR_DEFAULT;

                case StateProperty.Rotation: return Constants.ROTATION_DEFAULT;
                case StateProperty.Mod: return Constants.NUM_DEFAULT;
                case StateProperty.ModLimLow: return Constants.LIMLOW_DEFAULT;
                case StateProperty.ModLimHigh: return Constants.LIMHIGH_DEFAULT;


                case StateProperty.ColorHue:
                case StateProperty.ColorSaturation:
                case StateProperty.ColorValue:
                case StateProperty.InvalidColorHue:
                case StateProperty.InvalidColorSaturation:
                case StateProperty.InvalidColorValue:
                case StateProperty.ColorRed:
                case StateProperty.ColorGreen:
                case StateProperty.ColorBlue:
                case StateProperty.InvalidColorRed:
                case StateProperty.InvalidColorGreen:
                case StateProperty.InvalidColorBlue: return Constants.COLOR_DEFAULT;

                case StateProperty.ColorFactorR:
                case StateProperty.ColorFactorG:
                case StateProperty.ColorFactorB: return Constants.COLORFACT_DEFAULT;

                case StateProperty.ColorAlpha: 
                case StateProperty.InvalidColorAlpha: return Constants.ALPHA_DEFAULT;

                default: return 0;


            }
        }

        /// <summary>
        /// Fills the Dictionary containing Strong to StateProperty Values
        /// </summary>
        private static void FillPropertyStringDict()
        {
            for (StateProperty prop = 0; prop < StateProperty.i9; prop++)
            {
                PropertyStrings.Add(prop.ToString(), prop);
            }
            PropStringFilled = true;
        }
        #endregion
    }

    /// <summary>
    /// Enum for indexing state properties
    /// </summary>
    public enum StateProperty : int
    {
        X0,
        Y0,
        XRotationCenter, 
        YRotationCenter,
        XFactor,
        YFactor,
        Rotation,
        
        Mod,
        ModLimLow,
        ModLimHigh,
        
        
        ColorHue,
        ColorSaturation,
        ColorValue,
        InvalidColorHue,
        InvalidColorSaturation,
        InvalidColorValue,

        ColorRed,
        ColorGreen,
        ColorBlue,
        InvalidColorRed,
        InvalidColorGreen,
        InvalidColorBlue,

        ColorFactorR,
        ColorFactorG,
        ColorFactorB,

        ColorAlpha,
        InvalidColorAlpha,
        
        i0, i1, i2, i3, i4, i5, i6, i7, i8, i9
    }
}
