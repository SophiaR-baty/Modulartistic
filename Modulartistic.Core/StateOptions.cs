using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Reflection;
using NCalc;
using Json.Schema;

#nullable enable

namespace Modulartistic.Core
{
    public class StateOptions : IndexableBase
    {
        #region Fields
        private int m_width;
        private int m_height;
        private uint m_framerate;

        private string m_func_r_h;
        private string m_func_g_s;
        private string m_func_b_v;
        private string m_func_alp;

        private bool m_inval_col_glob;
        private bool m_circular;
        private bool m_use_rgb;

        private List<string> m_addons;
        #endregion


        #region Properties
        /// <summary>
        /// The Image/Animation Width.
        /// </summary>
        public int Width { get => m_width; set => m_width = value; }

        /// <summary>
        /// The Image/Animation Height
        /// </summary>
        public int Height { get => m_height; set => m_height = value; }

        /// <summary>
        /// The Framerate for Animations.
        /// </summary>
        public uint Framerate { get => m_framerate; set => m_framerate = value; }


        /// <summary>
        /// The Function to calculate Hue.
        /// </summary>
        public string FunctionRedHue { get => m_func_r_h; set => m_func_r_h = value; }

        /// <summary>
        /// The Function to calculate Saturation.
        /// </summary>
        public string FunctionGreenSaturation { get => m_func_g_s; set => m_func_g_s = value; }

        /// <summary>
        /// The Function to calculate Value.
        /// </summary>
        public string FunctionBlueValue { get => m_func_b_v; set => m_func_b_v = value; }

        /// <summary>
        /// The Function to calculate Alpha.
        /// </summary>
        public string FunctionAlpha { get => m_func_alp; set => m_func_alp = value; }


        /// <summary>
        /// Whether to use all Invalid color values when only one function is invalid.
        /// </summary>
        public bool InvalidColorGlobal { get => m_inval_col_glob; set => m_inval_col_glob = value; }

        /// <summary>
        /// If true, values like alpha that range from 0-1 will have their max at num/2, otherwise at num.
        /// </summary>
        public bool CircularMod { get => m_circular; set => m_circular = value; }

        /// <summary>
        /// If true, uses rgb for functions instead of hsv.
        /// </summary>
        public bool UseRGB { get => m_use_rgb; set => m_use_rgb = value; }

        /// <summary>
        /// AddOns to load.
        /// </summary>
        public List<string> AddOns { get => m_addons; }

        #endregion

        public StateOptions()
        {
            m_width = Constants.StateOptions.WIDTH_DEFAULT;
            m_height = Constants.StateOptions.HEIGHT_DEFAULT;
            m_framerate = Constants.StateOptions.FRAMERATE_DEFAULT;

            m_func_r_h = Constants.StateOptions.FUNC_R_H_DEFAULT;
            m_func_g_s = Constants.StateOptions.FUNC_G_S_DEFAULT;
            m_func_b_v = Constants.StateOptions.FUNC_B_V_DEFAULT;
            m_func_alp = Constants.StateOptions.FUNC_ALP_DEFAULT;

            m_inval_col_glob = Constants.StateOptions.INVALIDCOLORGLOBAL_DEFAULT;
            m_circular = Constants.StateOptions.CIRCULAR_DEFAULT;
            m_use_rgb = Constants.StateOptions.USERGB_DEFAULT;

            m_addons = new List<string>();
        }

        public string GetDebugInfo()
        {
            const int padding = -30;
            string result =
            $"{"Size: ",padding} {Width}x{Height} \n" +
            $"{"Framerate: ",padding} {Framerate} \n";

            if (UseRGB)
            {
                result += $"{"RedFunction: ",padding} {FunctionRedHue} \n" +
                    $"{"GreenFunction: ",-30} {FunctionGreenSaturation} \n" +
                    $"{"BlueFunction: ",padding} {FunctionBlueValue} \n";
            }
            else
            {
                result += $"{"HueFunction: ",padding} {FunctionRedHue} \n" +
                    $"{"SaturationFunction: ",padding} {FunctionGreenSaturation} \n" +
                    $"{"ValueFunction: ",padding} {FunctionBlueValue} \n";
            }

            result += $"{"AlphaFunction: ",padding} {FunctionAlpha} \n" +

            $"{"InvalidColorGlobal: ",padding} {InvalidColorGlobal} \n" +
            $"{"Circular: ",padding} {CircularMod} \n" +
            $"{"UseRGB: ",padding} {UseRGB} \n";

            if (AddOns.Count != 0)
            {
                result += $"{"Addons: ",padding} \n";
                for (int i = 0; i < AddOns.Count; i++) { result += AddOns[i] + "\n"; }
            }

            return result;
        }

        #region json
        /// <summary>
        /// Returns true if the passed JsonElement is a valid StateOptions representation
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Load StateOptions properties from Json
        /// </summary>
        /// <param name="element">Json Element for GenerationOption</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void LoadJson(JsonElement element)
        {
            foreach (JsonProperty elem in element.EnumerateObject())
            {
                switch (elem.Name) 
                {
                    case nameof(Width): 
                    case nameof(Height):
                        this[elem.Name] = (int)GetValueOrEvaluate(elem);
                        break;
                    case nameof(Framerate):
                        this[elem.Name] = (uint)GetValueOrEvaluate(elem);
                        break;
                    case nameof(FunctionRedHue):
                    case nameof(FunctionGreenSaturation):
                    case nameof(FunctionBlueValue):
                    case nameof(FunctionAlpha):
                        this[elem.Name] = elem.Value.GetString();
                        break;
                    case nameof(InvalidColorGlobal):
                    case nameof(CircularMod):
                    case nameof(UseRGB):
                        this[elem.Name] = elem.Value.GetBoolean();
                        break;
                    case nameof(AddOns):
                        AddOns.AddRange(elem.Value.EnumerateArray().Select((e, i) => e.GetString() ?? ""));
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{elem.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Load StateOptions from Json
        /// </summary>
        /// <param name="element">Json Element for GenerationOption</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public static StateOptions FromJson(JsonElement element)
        {
            StateOptions stateOptions = new StateOptions();
            stateOptions.LoadJson(element);
            
            return stateOptions;
        }

        /// <summary>
        /// Get Value from JsonProperty
        /// </summary>
        /// <param name="elem">The JsonProperty</param>
        /// <exception cref="Exception"></exception>
        private double GetValueOrEvaluate(JsonProperty elem)
        {

            // retrieve the value beforehand
            double value;
            if (elem.Value.ValueKind == JsonValueKind.String)
            {
                // if value is string type evaluate
                Expression expr = new Expression(elem.Value.GetString());
                Helper.ExprRegisterStateOptions(ref expr, this);
                value = (double)expr.Evaluate();
            }
            else if (elem.Value.ValueKind == JsonValueKind.Number)
            {
                // if value is double type simply get value
                value = elem.Value.GetDouble();
            }
            else { throw new Exception($"Property {elem.Name} must be string or number. "); }

            return value;
        }
        #endregion

    }
}
