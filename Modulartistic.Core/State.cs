using System;
using Modulartistic.Drawing;
using System.Threading;
using System.IO;
using MathNet.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

#nullable enable

namespace Modulartistic.Core
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
        public double this[StateProperty p]
        {
            get
            {
                switch (p)
                {
                    // default is parameters
                    case StateProperty.X0: return X0 ?? Constants.XY0_DEFAULT;
                    case StateProperty.Y0: return Y0 ?? Constants.XY0_DEFAULT;
                    case StateProperty.XRotationCenter: return XRotationCenter ?? Constants.XYROTCENTER_DEFAULT;
                    case StateProperty.YRotationCenter: return YRotationCenter ?? Constants.XYROTCENTER_DEFAULT;
                    case StateProperty.XFactor: return XFactor ?? Constants.XYFACTOR_DEFAULT;
                    case StateProperty.YFactor: return YFactor ?? Constants.XYFACTOR_DEFAULT;
                    case StateProperty.Rotation: return Rotation ?? Constants.ROTATION_DEFAULT;

                    case StateProperty.Mod: return Mod ?? Constants.NUM_DEFAULT;
                    case StateProperty.ModLimLow: return ModLimLow ?? Constants.LIMLOW_DEFAULT; 
                    case StateProperty.ModLimHigh: return ModLimUp ?? Constants.LIMHIGH_DEFAULT;
                    
                    case StateProperty.ColorHue: return ColorHue ?? Constants.COLOR_DEFAULT;
                    case StateProperty.ColorSaturation: return ColorSaturation ?? Constants.COLOR_DEFAULT;
                    case StateProperty.ColorValue: return ColorValue ?? Constants.COLOR_DEFAULT;
                    case StateProperty.InvalidColorHue: return InvalidColorHue ?? Constants.COLOR_DEFAULT;
                    case StateProperty.InvalidColorSaturation: return InvalidColorSaturation ?? Constants.COLOR_DEFAULT;
                    case StateProperty.InvalidColorValue: return InvalidColorValue ?? Constants.COLOR_DEFAULT;

                    case StateProperty.ColorRed: return ColorRed ?? Constants.COLOR_DEFAULT;
                    case StateProperty.ColorGreen: return ColorGreen ?? Constants.COLOR_DEFAULT;
                    case StateProperty.ColorBlue: return ColorBlue ?? Constants.COLOR_DEFAULT;
                    case StateProperty.InvalidColorRed: return InvalidColorRed ?? Constants.COLOR_DEFAULT;
                    case StateProperty.InvalidColorGreen: return InvalidColorGreen ?? Constants.COLOR_DEFAULT;
                    case StateProperty.InvalidColorBlue: return InvalidColorBlue ?? Constants.COLOR_DEFAULT;

                    case StateProperty.ColorFactorR: return ColorFactorR ?? Constants.COLORFACT_DEFAULT;
                    case StateProperty.ColorFactorG: return ColorFactorG ?? Constants.COLORFACT_DEFAULT;
                    case StateProperty.ColorFactorB: return ColorFactorB ?? Constants.COLORFACT_DEFAULT;

                    case StateProperty.ColorAlpha: return ColorAlpha ?? Constants.ALPHA_DEFAULT;
                    case StateProperty.InvalidColorAlpha: return InvalidColorAlpha ?? Constants.ALPHA_DEFAULT;

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

                    default: Parameters[(int)p - (int)StateProperty.i0] = value; break;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new State that has default values.
        /// </summary>
        public State(string name = "")
        {
            if (!PropStringFilled) { FillPropertyStringDict(); }

            Name = name == "" ? Constants.STATENAME_DEFAULT : name;

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
            for (StateProperty prop = 0; prop <= StateProperty.i9; prop++)
            {
                this[prop] = easing.Ease(
                    startState[prop], 
                    endState[prop], 
                    idx, 
                    maxidx);
            }
        }
        #endregion

        #region Methods for Generating Image
        /// <summary>
        /// Generates part of the Bitmap object of the state, given a size and Function for Color calculation and index of the thread and number of max threads.
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <param name="idx">The number of the Thread</param>
        /// <param name="max">The Total Number of Threads</param>
        /// <param name="image">The Bitmap where the Data should be stored</param>
        private void GetPartialBitmap(GenerationArgs args, out Bitmap image, int idx = 0, int max = 0)
        {
            #region Setting and validating State Properties
            // setting and validating State Properties
            // mod Properties
            double mod = Mod.GetValueOrDefault(Constants.NUM_DEFAULT);
            if (mod <= 0) { mod = double.Epsilon; }
            double modliml = ModLimLow.GetValueOrDefault(Constants.LIMLOW_DEFAULT);
            double modlimu = ModLimUp.GetValueOrDefault(Constants.LIMHIGH_DEFAULT);
            double lowBound = Helper.inclusiveMod(modliml, mod);
            double upBound = Helper.inclusiveMod(modlimu, mod);
            // coordinate Properties
            double x0 = X0.GetValueOrDefault(Constants.XY0_DEFAULT);
            double y0 = Y0.GetValueOrDefault(Constants.XY0_DEFAULT);
            double xrotc = XRotationCenter.GetValueOrDefault(Constants.XYROTCENTER_DEFAULT);
            double yrotc = YRotationCenter.GetValueOrDefault(Constants.XYROTCENTER_DEFAULT);
            double xfact = XFactor.GetValueOrDefault(Constants.XYFACTOR_DEFAULT);
            double yfact = YFactor.GetValueOrDefault(Constants.XYFACTOR_DEFAULT);
            // calculate the rotation in radiants and its cos and sin
            double rotrad = 2 * Math.PI * Rotation.GetValueOrDefault(Constants.ROTATION_DEFAULT) / 360;
            double sinrot = Math.Sin(rotrad);
            double cosrot = Math.Cos(rotrad);
            // color Properties
            double colr = ColorRed.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double colg = ColorGreen.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double colb = ColorBlue.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double colh = ColorHue.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double cols = ColorSaturation.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double colv = ColorValue.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double cola = ColorAlpha.GetValueOrDefault(Constants.ALPHA_DEFAULT);
            // invalid Color Properties
            double inv_colr = InvalidColorRed.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double inv_colg = InvalidColorGreen.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double inv_colb = InvalidColorBlue.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double inv_colh = InvalidColorHue.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double inv_cols = InvalidColorSaturation.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double inv_colv = InvalidColorValue.GetValueOrDefault(Constants.COLOR_DEFAULT);
            double inv_cola = InvalidColorAlpha.GetValueOrDefault(Constants.COLOR_DEFAULT);
            // Color Factors
            double colfactr = ColorFactorR.GetValueOrDefault(Constants.COLORFACT_DEFAULT);
            double colfactg = ColorFactorG.GetValueOrDefault(Constants.COLORFACT_DEFAULT);
            double colfactb = ColorFactorB.GetValueOrDefault(Constants.COLORFACT_DEFAULT);
            #endregion

            #region parsing GenerationArgs (Functions and size)
            // Parsing GenerationArgs
            Size size = new Size(args.Size[0], args.Size[1]);
            bool invalGlobal = args.InvalidColorGlobal.GetValueOrDefault(Constants.INVALIDCOLORGLOBAL_DEFAULT);
            bool circ = args.Circular.GetValueOrDefault(Constants.CIRCULAR_DEFAULT);
            bool useRGB = args.UseRGB.GetValueOrDefault(Constants.USERGB_DEFAULT);
            // instanciate the functions
            Function? Func_R_H = null;
            Function? Func_G_S = null;
            Function? Func_B_V = null;
            Function? Func_Alp = null;
            bool Func_R_H_null = true;
            bool Func_G_S_null = true;
            bool Func_B_V_null = true;
            bool Func_Alp_null = true;
            // parse the functions
            if (useRGB)
            {
                if (!string.IsNullOrEmpty(args.RedFunction))
                {
                    Func_R_H = new Function(args.RedFunction);
                    if (args.AddOns != null) Func_R_H.LoadAddOns(args.AddOns.ToArray());
                    Func_R_H_null = false;
                }
                if (!string.IsNullOrEmpty(args.GreenFunction))
                {
                    Func_G_S = new Function(args.GreenFunction);
                    if (args.AddOns != null) Func_G_S.LoadAddOns(args.AddOns.ToArray());
                    Func_G_S_null = false;
                }
                if (!string.IsNullOrEmpty(args.BlueFunction))
                {
                    Func_B_V = new Function(args.BlueFunction);
                    if (args.AddOns != null) Func_B_V.LoadAddOns(args.AddOns.ToArray());
                    Func_B_V_null = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(args.HueFunction))
                {
                    Func_R_H = new Function(args.HueFunction);
                    if (args.AddOns != null) Func_R_H.LoadAddOns(args.AddOns.ToArray());
                    Func_R_H_null = false;
                }

                if (!string.IsNullOrEmpty(args.SaturationFunction))
                {
                    Func_G_S = new Function(args.SaturationFunction);
                    if (args.AddOns != null) Func_G_S.LoadAddOns(args.AddOns.ToArray());
                    Func_G_S_null = false;
                }

                if (!string.IsNullOrEmpty(args.ValueFunction))
                {
                    Func_B_V = new Function(args.ValueFunction);
                    if (args.AddOns != null) Func_B_V.LoadAddOns(args.AddOns.ToArray());
                    Func_B_V_null = false;
                }
            }
            if (!string.IsNullOrEmpty(args.AlphaFunction))
            {
                Func_Alp = new Function(args.AlphaFunction);
                if (args.AddOns != null) Func_Alp.LoadAddOns(args.AddOns.ToArray());
                Func_Alp_null = false;
            }
            #endregion

            #region Setting values for partial creation
            int partial_width;
            int first_px;
            if (max == 0)
            {
                partial_width = size.Width;
                first_px = 0;
            }
            else
            {
                // setting the Width of the Bitmap and the first pixel to generate
                partial_width = size.Width / max;
                first_px = idx * partial_width;
                if (idx == max - 1) { partial_width = size.Width - first_px; }
            }
            #endregion

            #region Coordinate Calculation (unchanging for x and y)
            // get rotation center via inverse
            double rx1 = xrotc + xfact * (-xrotc - size.Width / 2.0) + x0;
            double ry1 = yrotc + -yfact * (-yrotc - size.Height / 2.0) + y0;

            // static summands for coordinate calculation
            double add_to_x = -xrotc - size.Width + 2 * x0 / xfact + first_px;
            double add_to_y = -yrotc - size.Height - 2 * y0 / yfact;
            double add_to_x_2 = xrotc - rx1 * cosrot + ry1 * sinrot;
            double add_to_y_2 = yrotc - rx1 * sinrot - ry1 * cosrot;
            #endregion

            // Create instance of Bitmap for pixel data
            image = new Bitmap(partial_width, (int)size.Height);

            // Iterate over every pixel
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < partial_width; x++)
                {
                    #region Coordinate Calculation (dependent on x and y)
                    double x1 = xfact * (x + add_to_x);
                    double y1 = -yfact * (y + add_to_y);

                    double x_ = x1 * cosrot - y1 * sinrot + add_to_x_2;
                    double y_ = x1 * sinrot + y1 * cosrot + add_to_y_2;
                    #endregion

                    // Creating Instance of the pixel
                    double pixel_r_h = 0, // red or hue
                        pixel_g_s = 0,    // green or saturation
                        pixel_b_v = 0,    // blue or value
                        pixel_alp = 0;    // alpha

                    #region Evaluating Functions
                    void calculatePixelValue(Function? func, out double pixel_val)
                    {
                        double n;
                        // not trying to catch exceptions here anymore! 
                        // if there is an exception, it has to do with the function and/or addons
                        // addons shouldnt throw anyway and if there is an Exception elsewhere the program may stop
                        n = func.Evaluate(x_, y_, Parameters, mod);
                        pixel_val = n.IsFinite() ? Helper.mod(n, mod) : -1;
                    }

                    // calculate color values
                    bool all_inval = invalGlobal;
                    // Yes, using gotos now (instead of do{}while(false)),
                    // I think its more readable and shouldn't come with any risks

                    if (!Func_R_H_null) { calculatePixelValue(Func_R_H, out pixel_r_h); }
                    if (all_inval && (pixel_r_h == -1)) { goto FinishCalculation; }

                    if (!Func_G_S_null) { calculatePixelValue(Func_G_S, out pixel_g_s); }
                    if (all_inval && (pixel_g_s == -1)) { goto FinishCalculation; }

                    if (!Func_B_V_null) { calculatePixelValue(Func_B_V, out pixel_b_v); }
                    if (all_inval && (pixel_b_v == -1)) { goto FinishCalculation; }

                    if (!Func_Alp_null) { calculatePixelValue(Func_Alp, out pixel_alp); }
                    if (all_inval &= (pixel_alp == -1)) { goto FinishCalculation; }

                FinishCalculation:

                    // if bounds are equal, all colors are invalid
                    all_inval |= lowBound == upBound;
                    // if all inval is true set all values to -1
                    if (all_inval)
                    {
                        pixel_r_h = -1;
                        pixel_g_s = -1;
                        pixel_b_v = -1;
                        pixel_alp = -1;
                    }
                    #endregion

                    #region Checking Bounds
                    // Setting Pixel to -1 if out of lower and upper bounds
                    // Only check for lower and upper bounds if... 
                    if (!all_inval && !(modliml == 0 && modlimu == Mod))
                    {
                        // if the lower bound is less than the upper bound,
                        // pixel is invalid if its value is not between the bound
                        if (lowBound < upBound)
                        {
                            if (!Func_R_H_null && !(pixel_r_h >= lowBound && pixel_r_h <= upBound)) { pixel_r_h = -1; }
                            if (!Func_G_S_null && !(pixel_g_s >= lowBound && pixel_g_s <= upBound)) { pixel_g_s = -1; }
                            if (!Func_B_V_null && !(pixel_b_v >= lowBound && pixel_b_v <= upBound)) { pixel_b_v = -1; }
                            if (!Func_Alp_null && !(pixel_alp >= lowBound && pixel_alp <= upBound)) { pixel_alp = -1; }
                        }
                        // if the lower bound is greater than the upper bound,
                        // pixel is invalid if its value IS between the bounds
                        else if (lowBound > upBound)
                        {
                            if (!Func_R_H_null && pixel_r_h <= lowBound && pixel_r_h >= upBound) { pixel_r_h = -1; }
                            if (!Func_G_S_null && pixel_g_s <= lowBound && pixel_g_s >= upBound) { pixel_g_s = -1; }
                            if (!Func_B_V_null && pixel_b_v <= lowBound && pixel_b_v >= upBound) { pixel_b_v = -1; }
                            if (!Func_Alp_null && pixel_alp <= lowBound && pixel_alp >= upBound) { pixel_alp = -1; }
                        }
                    }
                    #endregion

                    #region Setting Color in RGB Mode
                    // Setting col to inval col if pixel == -1
                    Color color;
                    if (useRGB)
                    {
                        int a, r, g, b;
                        if (pixel_alp == -1) { a = (int)(255 * inv_cola); }
                        else
                        {
                            a = (int)(circ ?
                            Helper.circ(cola + pixel_alp / mod, 1) * 255 :
                            Helper.inclusiveMod(cola + pixel_alp / mod, 1) * 255);
                        }

                        if (pixel_r_h == -1) { r = (int)(255 * inv_colr); }
                        else
                        {
                            r = (int)(circ ?
                            Helper.circ(colr + pixel_r_h / mod, 1) * 255 :
                            Helper.inclusiveMod(colr + pixel_r_h / mod, 1) * 255);
                        }

                        if (pixel_g_s == -1) { g = (int)(255 * inv_colg); }
                        else
                        {
                            g = (int)(circ ?
                            Helper.circ(colg + pixel_g_s / mod, 1) * 255 :
                            Helper.inclusiveMod(colg + pixel_g_s / mod, 1) * 255);
                        }

                        if (pixel_b_v == -1) { b = (int)(255 * inv_colb); }
                        else
                        {
                            b = (int)(circ ?
                            Helper.circ(colb + pixel_b_v / mod, 1) * 255 :
                            Helper.inclusiveMod(colb + pixel_b_v / mod, 1) * 255);
                        }

                        // Apply the Color factors
                        r = (int)(r * colfactr);
                        g = (int)(g * colfactg);
                        b = (int)(b * colfactb);

                        // Validate the Colors (range 0-255)
                        if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
                        if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
                        if (b > 255) { b = 255; } else if (b < 0) { b = 0; }

                        color = new Color(r, g, b, a);
                    }
                    #endregion

                    #region Setting Color in HSV Mode
                    else
                    {
                        double a, h, s, v;
                        if (pixel_alp == -1) { a = inv_cola; }
                        else
                        {
                            a = circ ?
                            Helper.circ(cola + pixel_alp / mod, 1) :
                            Helper.inclusiveMod(cola + pixel_alp / mod, 1);
                        }

                        if (pixel_r_h == -1) { h = inv_colh; }
                        else
                        {
                            // this used inclusive mod before, which caused problems with animations. In case that happens in any other sections refer to this comment :)
                            h = Helper.mod(colh / 360 + pixel_r_h / mod, 1) * 360;
                        }

                        if (pixel_g_s == -1) { s = inv_cols; }
                        else
                        {
                            s = circ ?
                            Helper.circ(cols + pixel_g_s / mod, 1) :
                            Helper.inclusiveMod(cols + pixel_g_s / mod, 1);
                        }

                        if (pixel_b_v == -1) { v = inv_colv; }
                        else
                        {
                            v = circ ?
                            Helper.circ(colv + pixel_b_v / mod, 1) :
                            Helper.inclusiveMod(colv + pixel_b_v / mod, 1);
                        }

                        color = Color.FromHSV((float)h, (float)s, (float)v);
                        int r, g, b;
                        // Apply the Color factors
                        r = (int)(color.R * colfactr);
                        g = (int)(color.G * colfactg);
                        b = (int)(color.B * colfactb);

                        // Validate the Colors (range 0-255)
                        if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
                        if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
                        if (b > 255) { b = 255; } else if (b < 0) { b = 0; }

                        // Update Color
                        color = new Color(r, g, b, (int)(255 * a));
                    }
                    #endregion

                    // set the pixel on the image bitmap
                    image.SetPixel(x, y, color);

                }
            }
        }

        /// <summary>
        /// Generates an Image of this State with a given a Size and a Function for the Color calculation. If max_threads = -1 the max number is used
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <param name="max_threads">The maximum number of threads this will use</param>
        /// <param name="path_out">The Path to save the image at</param>
        /// <returns>the filepath of the generated image</returns>
        /// <exception cref="DirectoryNotFoundException">thrown if path_out does not exist</exception>
        public string GenerateImage(GenerationArgs args, int max_threads, string out_dir = @"")
        {
            // If out-dir is empty set to default, then check if it exists
            out_dir = out_dir == "" ? Constants.OUTPUTFOLDER : out_dir;
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.STATENAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);

            // Generate the image
            Bitmap image = GetBitmap(args, max_threads);
            // Save the image
            image.Save(file_path_out + @".png");

            return file_path_out + @".png";
        }

        /// <summary>
        /// Generates the Bitmap object of the state, given a size and Function for Color calculation. If max_threads = -1 the max number is used
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function Data</param>
        /// <param name="max_threads">The maximum number of threads this will use</param>
        /// <returns>The generated Bitmap</returns>
        public Bitmap GetBitmap(GenerationArgs args, int max_threads)
        {
            if (max_threads == 0 || max_threads == 1) 
            {
                GetPartialBitmap(args, out Bitmap image);
                return image;
            }
            else
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

                    threads[local_i] = new Thread(new ThreadStart(() => GetPartialBitmap(args, out partial_images[local_i], local_i, threads_num)));
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

                // return the Bitmap
                return image;
            }
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Gets details about this state. Useful for debugging. 
        /// </summary>
        /// <returns>A formatted details string</returns>
        public string GetDetailsString()
        {
            const int padding = -30;
            string details = string.IsNullOrEmpty(Name) ? "" : $"{"Name: ", padding} {Name} \n"; 

            if (X0 != null) { details += $"{"X0 Coordinate: ", padding} {X0} \n"; }
            if (Y0 != null) { details += $"{"Y0 Coordinate: ", padding} {Y0} \n"; }
            if (XRotationCenter != null) { details += $"{"X Rotation Center: ", padding} {XRotationCenter} \n"; }
            if (YRotationCenter != null) { details += $"{"Y Rotation Center: ", padding} {YRotationCenter} \n"; }
            if (XFactor != null) { details += $"{"X Factor: ", padding} {XFactor} \n"; }
            if (YFactor != null) { details += $"{"Y Factor: ", padding} {YFactor} \n"; }
            if (Rotation != null) { details += $"{"Rotation: ", padding} {Rotation} \n"; }

            if (Mod != null) { details += $"{"Modulus Number: ", padding} {Mod} \n"; }
            if (ModLimLow != null) { details += $"{"Modulus Lower Limit: ", padding} {ModLimLow} \n"; }
            if (ModLimUp != null) { details += $"{"Modulus Upper Limit: ", padding} {ModLimUp} \n"; }

            if (ColorHue != null) { details += $"{"Hue Offset: ", padding} {ColorHue} \n"; }
            if (ColorSaturation != null) { details += $"{"Saturation Offset: ", padding} {ColorSaturation} \n"; }
            if (ColorValue != null) { details += $"{"Color Value Offset: ", padding} {ColorValue} \n"; }

            if (InvalidColorHue != null) { details += $"{"Invalid Hue: ", padding} {InvalidColorHue} \n"; }
            if (InvalidColorSaturation != null) { details += $"{"Invalid Saturation: ", padding} {InvalidColorSaturation} \n"; }
            if (InvalidColorValue != null) { details += $"{"Invalid Color Value: ", padding} {InvalidColorValue} \n"; }

            if (ColorRed != null) { details += $"{"Red Offset: ", padding} {ColorRed} \n"; }
            if (ColorGreen != null) { details += $"{"Green Offset: ", padding} {ColorGreen} \n"; }
            if (ColorBlue != null) { details += $"{"Blue Offset: ", padding} {ColorBlue} \n"; }
            
            if (InvalidColorRed != null) { details += $"{"Invalid Red: ",padding} {InvalidColorRed} \n"; }
            if (InvalidColorGreen != null) { details += $"{"Invalid Green: ",padding} {InvalidColorGreen} \n"; }
            if (InvalidColorBlue != null) { details += $"{"Invalid Blue: ",padding} {InvalidColorBlue} \n"; }
            
            if (ColorAlpha != null) { details += $"{"Alpha Offset: ", padding} {ColorAlpha} \n"; }
            if (InvalidColorAlpha != null) { details += $"{"Invalid Alpha Offset: ", padding} {InvalidColorAlpha} \n"; }

            if (ColorFactorR != null || ColorFactorG != null || ColorFactorB != null) { details += $"{"Color Factors (R G B): ", padding} {this[StateProperty.ColorFactorR]} {this[StateProperty.ColorFactorG]} {this[StateProperty.ColorFactorB]} \n"; }
            details += $"{"Parameters: ",-30} {Parameters[0]} {Parameters[1]} {Parameters[2]} {Parameters[3]} {Parameters[4]} {Parameters[5]} {Parameters[6]} {Parameters[7]} {Parameters[8]} {Parameters[9]}";

            return details;
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
        /// Fills the Dictionary containing String to StateProperty Values
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
