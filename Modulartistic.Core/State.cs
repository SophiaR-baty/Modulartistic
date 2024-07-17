using System;
using Modulartistic.Drawing;
using MathNet.Numerics;
using System.Text.Json.Serialization;
using Antlr4.Runtime.Misc;
using System.Text.Json;
using NCalc;
using Json.Schema;
using System.Reflection;
using System.Xml.Linq;

#nullable enable

namespace Modulartistic.Core
{
    /// <summary>
    /// State Class to contain Data about Image States/Frames for Image Generation.
    /// </summary>
    public class State : IndexableBase
    {
        #region Static
        private static Dictionary<string, StateProperty> PropertyStrings = new Dictionary<string, StateProperty>();
        private static bool PropStringFilled = false;
        #endregion

        #region Fields
        private string m_name;              // the name of this state

        private double m_x0;                // the x-coordinate in the middle of the image
        private double m_y0;                // the y-coordinate in the middle of the image
        private double m_x_rot_center;      // the x-coordinate of the point around which is rotated
        private double m_y_rot_center;      // the y-coordinate of the point around which is rotated
        private double m_x_factor;          // value by which x-coordinates are multiplied
        private double m_y_factor;          // value by which y-coordinates are multiplied
        private double m_rotation;          // rotation in degrees

        private double m_num;               // num which all functions are taken modulo by
        private double m_lim_low;           // if a function mod num is less than this, assign the inval value
        private double m_lim_high;          // if a function mod num is greater than this, assign the inval value
                                            // if lim_low > lim_high -> effects are reversed

        private double m_r_h;               // the hue offset or constant value
        private double m_g_s;               // the saturation offset or constant value
        private double m_b_v;               // the value offset or constant value

        private double m_inv_r_h;           // the hue invalid value
        private double m_inv_g_s;           // the saturation invalid value
        private double m_inv_b_v;           // the value invalid value

        private double m_factor_r_h;        // the red color factor
        private double m_factor_g_s;        // the green color factor
        private double m_factor_b_v;        // the blue color factor

        private double m_alp;               // the alpha offset or constant value
        private double m_inv_alp;           // the alpha invalid value

        private double[] m_parameters;
        #endregion

        #region Properties
        /// <summary>
        /// The Name of the state
        /// </summary>
        public string Name { get => m_name; set => m_name = value; }

        /// <summary>
        /// The x-Coordinate that will be in the middle of the screen. 
        /// </summary>
        public double X0 { get => m_x0; set => m_x0 = value; }
        /// <summary>
        /// The y-Coordinate that will be in the middle of the screen. 
        /// </summary>
        public double Y0 { get => m_y0; set => m_y0 = value; }
        /// <summary>
        /// The x-Coordinate around which is rotated. 
        /// </summary>
        public double XRotationCenter { get => m_x_rot_center; set => m_x_rot_center = value; }
        /// <summary>
        /// The y-Coordinate around which is rotated. 
        /// </summary>
        public double YRotationCenter { get => m_y_rot_center; set => m_y_rot_center = value; }
        /// <summary>
        /// The factor by which the x-Coordinates will be scaled.
        /// </summary>
        public double XFactor { get => m_x_factor; set => m_x_factor = value; }
        /// <summary>
        /// The factor by which the y-coordinates will be scaled.
        /// </summary>
        public double YFactor { get => m_y_factor; set => m_y_factor = value; }
        /// <summary>
        /// The Amount of degrees the image will be rotated.
        /// </summary>
        public double Rotation { get => m_rotation; set => m_rotation = value; }


        /// <summary>
        /// The Modulus Number by which all functions are taken modulo.
        /// </summary>
        public double Mod { get => m_num; set => m_num = value; }
        /// <summary>
        /// The Lower Limit of the Modulus number. Values below will be treated as invalid.
        /// </summary>
        public double ModLowerLimit { get => m_lim_low; set => m_lim_low = value; }
        /// <summary>
        /// The Upper Limit of the Modulus number. Values above will be treated as invalid.
        /// </summary>
        public double ModUpperLimit { get => m_lim_high; set => m_lim_high = value; }


        /// <summary>
        /// The Red or Hue Offset or Constant.
        /// </summary>
        public double ColorRedHue { get => m_r_h; set => m_r_h = value; }
        /// <summary>
        /// The Green or Saturation Offset or Constant. Has to be from 0-1.
        /// </summary>
        public double ColorGreenSaturation { get => m_g_s; set => m_g_s = value; }
        /// <summary>
        /// The Blue or Value Offset or Constant. Has to be from 0-1.
        /// </summary>
        public double ColorBlueValue { get => m_b_v; set => m_b_v = value; }


        /// <summary>
        /// The Red or Hue Value for invalid results.
        /// </summary>
        public double InvalidColorRedHue { get => m_inv_r_h; set => m_inv_r_h = value; }
        /// <summary>
        /// The Green or Saturation Value for invalid results. Has to be from 0-1.
        /// </summary>
        public double InvalidColorGreenSaturation { get => m_inv_g_s; set => m_inv_g_s = value; }
        /// <summary>
        /// The Blue or Value Value for invalid results. Has to be from 0-1.
        /// </summary>
        public double InvalidColorBlueValue { get => m_inv_b_v; set => m_inv_b_v = value; }

        /// <summary>
        /// Factor by which red or hue is scaled at the end.
        /// </summary>
        public double ColorFactorRedHue { get => m_factor_r_h; set => m_factor_r_h = value; }
        /// <summary>
        /// Factor by which green or saturation is scaled at the end.
        /// </summary>
        public double ColorFactorGreenSaturation { get => m_factor_g_s; set => m_factor_g_s = value; }
        /// <summary>
        /// Factor by which blue or value is scaled at the end.
        /// </summary>
        public double ColorFactorBlueValue { get => m_factor_b_v; set => m_factor_b_v = value; }


        /// <summary>
        /// The Alpha Offset or Constant.
        /// </summary>
        public double ColorAlpha { get => m_alp; set => m_alp = value; }
        /// <summary>
        /// The Alpha Value for invalid results.
        /// </summary>
        public double InvalidColorAlpha { get => m_inv_alp; set => m_inv_alp = value; }


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
                    case StateProperty.X0: return X0;
                    case StateProperty.Y0: return Y0;
                    case StateProperty.XRotationCenter: return XRotationCenter;
                    case StateProperty.YRotationCenter: return YRotationCenter;
                    case StateProperty.XFactor: return XFactor;
                    case StateProperty.YFactor: return YFactor;
                    case StateProperty.Rotation: return Rotation;

                    case StateProperty.Mod: return Mod;
                    case StateProperty.ModLowerLimit: return ModLowerLimit;
                    case StateProperty.ModUpperLimit: return ModUpperLimit;

                    case StateProperty.ColorRedHue: return ColorRedHue;
                    case StateProperty.ColorGreenSaturation: return ColorGreenSaturation;
                    case StateProperty.ColorBlueValue: return ColorBlueValue;
                    case StateProperty.InvalidColorRedHue: return InvalidColorRedHue;
                    case StateProperty.InvalidColorGreenSaturation: return InvalidColorGreenSaturation;
                    case StateProperty.InvalidColorBlueValue: return InvalidColorBlueValue;

                    case StateProperty.ColorFactorRedHue: return ColorFactorRedHue;
                    case StateProperty.ColorFactorGreenSaturation: return ColorFactorGreenSaturation;
                    case StateProperty.ColorFactorBlueValue: return ColorFactorBlueValue;

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
                    case StateProperty.ModLowerLimit: ModLowerLimit = value; break;
                    case StateProperty.ModUpperLimit: ModUpperLimit = value; break;

                    case StateProperty.ColorRedHue: ColorRedHue = value; break;
                    case StateProperty.ColorGreenSaturation: ColorGreenSaturation = value; break;
                    case StateProperty.ColorBlueValue: ColorBlueValue = value; break;
                    case StateProperty.InvalidColorRedHue: InvalidColorRedHue = value; break;
                    case StateProperty.InvalidColorGreenSaturation: InvalidColorGreenSaturation = value; break;
                    case StateProperty.InvalidColorBlueValue: InvalidColorBlueValue = value; break;

                    case StateProperty.ColorFactorRedHue: ColorFactorRedHue = value; break;
                    case StateProperty.ColorFactorGreenSaturation: ColorFactorGreenSaturation = value; break;
                    case StateProperty.ColorFactorBlueValue: ColorFactorBlueValue = value; break;

                    case StateProperty.ColorAlpha: ColorAlpha = value; break;
                    case StateProperty.InvalidColorAlpha: InvalidColorAlpha = value; break;

                    default: Parameters[(int)p - (int)StateProperty.i0] = value; break;
                }
            }
        }
        #endregion

        #region Constructors
        public State()
        {
            if (!PropStringFilled) { FillPropertyStringDict(); }
            
            m_name = Constants.State.STATENAME_DEFAULT;

            m_x0 = Constants.State.XY0_DEFAULT;
            m_y0 = Constants.State.XY0_DEFAULT;
            m_x_rot_center = Constants.State.XYROTCENTER_DEFAULT;
            m_y_rot_center = Constants.State.XYROTCENTER_DEFAULT;
            m_x_factor = Constants.State.XYFACTOR_DEFAULT;
            m_y_factor = Constants.State.XYFACTOR_DEFAULT;
            m_rotation = Constants.State.ROTATION_DEFAULT;

            m_num = Constants.State.NUM_DEFAULT;
            m_lim_low = Constants.State.LIMLOW_DEFAULT;
            m_lim_high = Constants.State.LIMHIGH_DEFAULT;

            m_r_h = Constants.State.COLOR_R_H_DEFAULT;
            m_g_s = Constants.State.COLOR_G_S_DEFAULT;
            m_b_v = Constants.State.COLOR_B_V_DEFAULT;
            
            m_inv_r_h = Constants.State.INV_COLOR_R_H_DEFAULT;
            m_inv_g_s = Constants.State.INV_COLOR_G_S_DEFAULT;
            m_inv_b_v = Constants.State.INV_COLOR_B_V_DEFAULT;

            m_alp = Constants.State.COLOR_ALP_DEFAULT;
            m_inv_alp = Constants.State.INV_COLOR_ALP_DEFAULT;

            m_factor_r_h = Constants.State.COLORFACT_DEFAULT;
            m_factor_g_s = Constants.State.COLORFACT_DEFAULT;
            m_factor_b_v = Constants.State.COLORFACT_DEFAULT;

            m_parameters = new double[10];
        }

        /// <summary>
        /// Constructs a new State that has default values.
        /// </summary>
        public State(string name) : this()
        {
            Name = name == "" ? Constants.State.STATENAME_DEFAULT : name;
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
                    (double)idx/maxidx);
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
        private void GetPartialBitmap(StateOptions args, out Bitmap image, int idx = 0, int max = 0)
        {
            #region Setting and validating State Properties
            // setting and validating State Properties
            // mod Properties
            double mod = Mod;
            if (mod <= 0) { mod = double.Epsilon; }
            double modliml = ModLowerLimit;
            double modlimu = ModUpperLimit;
            double lowBound = Helper.InclusiveMod(modliml, mod);
            double upBound = Helper.InclusiveMod(modlimu, mod);
            // coordinate Properties
            double x0 = X0;
            double y0 = Y0;
            double xrotc = XRotationCenter;
            double yrotc = YRotationCenter;
            double xfact = XFactor;
            double yfact = YFactor;
            // calculate the rotation in radiants and its cos and sin
            double rotrad = 2 * Math.PI * Rotation / 360;
            double sinrot = Math.Sin(rotrad);
            double cosrot = Math.Cos(rotrad);
            // color Properties
            double col_r_h = ColorRedHue;
            double col_g_s = ColorGreenSaturation;
            double col_b_v = ColorBlueValue;
            double col_alp = ColorAlpha;
            // invalid Color Properties
            double inv_col_r_h = InvalidColorRedHue;
            double inv_col_g_s = InvalidColorGreenSaturation;
            double inv_col_b_v = InvalidColorBlueValue;
            double inv_col_alp = InvalidColorAlpha;
            // Color Factors
            double colfact_r_h = ColorFactorRedHue;
            double colfact_g_s = ColorFactorGreenSaturation;
            double colfact_b_v = ColorFactorBlueValue;
            #endregion

            #region parsing GenerationArgs (Functions and size)
            // Parsing GenerationArgs
            Size size = new Size(args.Width, args.Height);
            bool invalGlobal = args.InvalidColorGlobal;
            bool circ = args.CircularMod;
            bool useRGB = args.UseRGB;
            // if rgb is not used c_r_h will be expected to be an input from 0-360, however for calculation a value 0-1 is expected
            if (!useRGB) { col_r_h /= 360; }
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
            if (!string.IsNullOrEmpty(args.FunctionRedHue))
            {
                Func_R_H = new Function(args.FunctionRedHue);
                Func_R_H.RegisterStateProperties(this, args);
                if (args.AddOns != null) Func_R_H.LoadAddOns(args.AddOns.ToArray());
                Func_R_H_null = false;
            }
            if (!string.IsNullOrEmpty(args.FunctionGreenSaturation))
            {
                Func_G_S = new Function(args.FunctionGreenSaturation);
                Func_G_S.RegisterStateProperties(this, args);
                if (args.AddOns != null) Func_G_S.LoadAddOns(args.AddOns.ToArray());
                Func_G_S_null = false;
            }
            if (!string.IsNullOrEmpty(args.FunctionBlueValue))
            {
                Func_B_V = new Function(args.FunctionBlueValue);
                Func_B_V.RegisterStateProperties(this, args);
                if (args.AddOns != null) Func_B_V.LoadAddOns(args.AddOns.ToArray());
                Func_B_V_null = false;
            }

            if (!string.IsNullOrEmpty(args.FunctionAlpha))
            {
                Func_Alp = new Function(args.FunctionAlpha);
                Func_Alp.RegisterStateProperties(this, args);
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
                    void calculatePixelValue(Function? func, double offset, bool circ, out double pixel_val)
                    {
                        double n;
                        // not trying to catch exceptions here anymore! 
                        // if there is an exception, it has to do with the function and/or addons
                        // addons shouldnt throw anyway and if there is an Exception elsewhere the program may stop
                        n = mod*offset + func.Evaluate(x_, y_);
                        pixel_val = n.IsFinite() ? (circ ? Helper.CircularMod(n, mod) : Helper.Mod(n, mod)) : -1;
                    }

                    // calculate color values
                    bool all_inval = invalGlobal;
                    // Yes, using gotos now (instead of do{}while(false)),
                    // I think its more readable and shouldn't come with any risks

                    if (!Func_R_H_null) { calculatePixelValue(Func_R_H, col_r_h, circ && useRGB, out pixel_r_h); }
                    else { pixel_r_h = mod * col_r_h; }
                    if (all_inval && (pixel_r_h == -1)) { goto FinishCalculation; }

                    if (!Func_G_S_null) { calculatePixelValue(Func_G_S, col_g_s, circ, out pixel_g_s); }
                    else { pixel_g_s = mod * col_g_s; }
                    if (all_inval && (pixel_g_s == -1)) { goto FinishCalculation; }

                    if (!Func_B_V_null) { calculatePixelValue(Func_B_V, col_b_v, circ, out pixel_b_v); }
                    else { pixel_b_v = mod * col_b_v; }
                    if (all_inval && (pixel_b_v == -1)) { goto FinishCalculation; }

                    if (!Func_Alp_null) { calculatePixelValue(Func_Alp, col_alp, circ, out pixel_alp); }
                    else { pixel_alp = mod * col_alp; }
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

                    if (invalGlobal && (pixel_alp == -1 || pixel_b_v == -1 || pixel_g_s == -1 || pixel_r_h == -1))
                    {
                        pixel_r_h = -1;
                        pixel_g_s = -1;
                        pixel_b_v = -1;
                        pixel_alp = -1;
                    }
                    #endregion

                    Color color;

                    #region Setting Color in RGB Mode
                    // Setting col to inval col if pixel == -1
                    if (useRGB)
                    {
                        int a, r, g, b;
                        if (pixel_alp == -1) { a = (int)(255 * inv_col_alp); }
                        else
                        {
                            a = (int)(Helper.InclusiveMod(pixel_alp / mod, 1) * 255);
                        }

                        if (pixel_r_h == -1) { r = (int)(255 * inv_col_r_h); }
                        else
                        {

                            r = (int)(Helper.InclusiveMod(pixel_r_h / mod, 1) * 255);
                        }

                        if (pixel_g_s == -1) { g = (int)(255 * inv_col_g_s); }
                        else
                        {
                            g = (int)(Helper.InclusiveMod(pixel_g_s / mod, 1) * 255);
                        }

                        if (pixel_b_v == -1) { b = (int)(255 * inv_col_b_v); }
                        else
                        {
                            b = (int)(Helper.InclusiveMod(pixel_b_v / mod, 1) * 255);
                        }

                        // Validate the Colors (range 0-255)
                        if (r > 255) { r = 255; } else if (r < 0) { r = 0; }
                        if (g > 255) { g = 255; } else if (g < 0) { g = 0; }
                        if (b > 255) { b = 255; } else if (b < 0) { b = 0; }

                        color = new Color(r, g, b, a);

                        // Apply colorfactors
                        float h, s, v;
                        h = color.GetHue() * (float)colfact_r_h;
                        s = color.GetSaturation() * (float)colfact_g_s;
                        v = color.GetValue() * (float)colfact_b_v;

                        // Validate the Colors (range 0-255)
                        if (s > 1) { s = 1; } else if (g < 0) { s = 0; }
                        if (v > 1) { v = 1; } else if (b < 0) { v = 0; }

                        color = Color.FromHSV(h, s, v, a / 255);
                    }
                    #endregion

                    #region Setting Color in HSV Mode
                    else
                    {
                        double a, h, s, v;
                        if (pixel_alp == -1) { a = inv_col_alp; }
                        else
                        {
                            a = Helper.InclusiveMod(pixel_alp / mod, 1);
                        }

                        if (pixel_r_h == -1) { h = inv_col_r_h; }
                        else
                        {
                            // this used inclusive mod before, which caused problems with animations. In case that happens in any other sections refer to this comment :)
                            h = Helper.Mod(pixel_r_h / mod, 1) * 360;
                        }

                        if (pixel_g_s == -1) { s = inv_col_g_s; }
                        else
                        {
                            s = Helper.InclusiveMod(pixel_g_s / mod, 1);
                        }

                        if (pixel_b_v == -1) { v = inv_col_b_v; }
                        else
                        {
                            v = Helper.InclusiveMod(pixel_b_v / mod, 1);
                        }

                        color = Color.FromHSV((float)h, (float)s, (float)v);

                        // Apply the Color factors
                        int r, g, b;
                        r = (int)(color.R * colfact_r_h);
                        g = (int)(color.G * colfact_g_s);
                        b = (int)(color.B * colfact_b_v);

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
        /// <param name="out_dir">The Path to save the image at</param>
        /// <returns>the filepath of the generated image</returns>
        /// <exception cref="DirectoryNotFoundException">thrown if path_out does not exist</exception>
        public string GenerateImage(StateOptions args, int max_threads, string out_dir)
        {
            // If out-dir is empty set to default, then check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.State.STATENAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);

            // Generate the image
            Bitmap image = GetBitmap(args, max_threads);
            // Save the image
            image.Save(file_path_out + @".png");

            return file_path_out + @".png";
        }

        public string GenerateImage(StateOptions args, GenerationOptions options)
        {
            string out_dir = options.OutputPath;
            int max_threads = options.MaxThreads;
            if (max_threads < 1) { max_threads = 1; }

            // If out-dir is empty set to default, then check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.State.STATENAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);

            // Generate the image
            Bitmap image = GetBitmap(args, max_threads);
            // Save the image
            image.Save(file_path_out + @".png");

            return file_path_out + @".png";
        }

        public string GenerateImage(StateOptions args, string out_dir, GenerationOptions options)
        {
            int max_threads = options.MaxThreads;
            if (max_threads < 1) { max_threads = 1; }

            // If out-dir is empty set to default, then check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.State.STATENAME_DEFAULT : Name));
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
        public Bitmap GetBitmap(StateOptions args, int max_threads)
        {
            if (max_threads == 0 || max_threads == 1)
            {
                GetPartialBitmap(args, out Bitmap image);
                return image;
            }
            else
            {
                // Parsing GenerationArgs Size
                Size size = new Size(args.Width, args.Height);

                // Set the number 
                int threads_num = max_threads;
                if (max_threads <= -1 || max_threads > Environment.ProcessorCount) { threads_num = Environment.ProcessorCount; }
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
            }
        }
        #endregion

        #region Other Methods
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

        #region json
        /// <summary>
        /// Returns true if the passed JsonElement is a valid State representation
        /// </summary>
        /// <param name="element">JsonElement for State</param>
        /// <returns></returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);

        }

        /// <summary>
        /// Load State properties from Json
        /// </summary>
        /// <param name="element">Json Element for State</param>
        /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void LoadJson(JsonElement element, StateOptions opts)
        {
            foreach (JsonProperty elem in element.EnumerateObject())
            {
                switch (elem.Name)
                {
                    case nameof(Name):
                        this[elem.Name] = elem.Value.GetString();
                        break;
                    case nameof(X0):
                    case nameof(Y0):
                    case nameof(XRotationCenter):
                    case nameof(YRotationCenter):
                    case nameof(XFactor):
                    case nameof(YFactor):
                    case nameof(Rotation):
                    case nameof(Mod):
                    case nameof(ModLowerLimit):
                    case nameof(ModUpperLimit):
                    case nameof(ColorRedHue):
                    case nameof(ColorGreenSaturation):
                    case nameof(ColorBlueValue):
                    case nameof(ColorAlpha):
                    case nameof(InvalidColorRedHue):
                    case nameof(InvalidColorGreenSaturation):
                    case nameof(InvalidColorBlueValue):
                    case nameof(InvalidColorAlpha):
                    case nameof(ColorFactorRedHue):
                    case nameof(ColorFactorGreenSaturation):
                    case nameof(ColorFactorBlueValue):
                        this[elem.Name] = GetValueOrEvaluate(elem, opts);
                        break;
                    case nameof(Parameters):
                        // iterate over Parameters
                        int i = 0;
                        foreach (JsonElement param_elem in elem.Value.EnumerateArray())
                        {
                            double param_value;
                            if (param_elem.ValueKind == JsonValueKind.String)
                            {
                                // if value is string type evaluate
                                Expression expr = new Expression(param_elem.GetString());
                                Helper.ExprRegisterStateProperties(ref expr, this);
                                Helper.ExprRegisterStateOptions(ref expr, opts);
                                param_value = (double)expr.Evaluate();
                            }
                            else if (param_elem.ValueKind == JsonValueKind.Number)
                            {
                                // if value is double type simply get value
                                param_value = param_elem.GetDouble();
                            }
                            else { throw new Exception($"Element must be string or number. "); }

                            // set Parameter value
                            Parameters[i] = param_value;
                            i++;
                        }
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{elem.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Load State from Json
        /// </summary>
        /// <param name="element">Json Element for State</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public static State FromJson(JsonElement element, StateOptions opts)
        {
            State state = new State();
            state.LoadJson(element, opts);

            return state;
        }

        /// <summary>
        /// Get Value from JsonProperty, simply take the number value or try to evaluate if value is string
        /// </summary>
        /// <param name="element">The JsonProperty</param>
        /// <param name="opts">StateOptions to regeister</param>
        /// <exception cref="Exception">If the json value was neither number nor string</exception>
        private double GetValueOrEvaluate(JsonProperty element, StateOptions opts)
        {
            // retrieve the value beforehand
            double value;
            if (element.Value.ValueKind == JsonValueKind.String)
            {
                // if value is string type evaluate
                Expression expr = new Expression(element.Value.GetString());
                Helper.ExprRegisterStateProperties(ref expr, this);
                Helper.ExprRegisterStateOptions(ref expr, opts);
                value = (double)expr.Evaluate();
            }
            else if (element.Value.ValueKind == JsonValueKind.Number)
            {
                // if value is double type simply get value
                value = element.Value.GetDouble();
            }
            else { throw new Exception($"Property {element.Name} must be string or number. "); }

            return value;
        }
        #endregion
    }
}
