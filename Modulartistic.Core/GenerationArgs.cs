using System.Collections.Generic;

#nullable enable

namespace Modulartistic.Core
{
    public class GenerationArgs
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
        public int Width { get => m_width; set => m_width=value; }

        /// <summary>
        /// The Image/Animation Height
        /// </summary>
        public int Height { get => m_height; set => m_height=value; }

        /// <summary>
        /// The Framerate for Animations.
        /// </summary>
        public uint Framerate { get => m_framerate; set => m_framerate=value; }

        /// <summary>
        /// AddOns to load.
        /// </summary>
        public List<string> AddOns { get => m_addons; }


        /// <summary>
        /// The Function to calculate Hue.
        /// </summary>
        public string FunctionRH { get => m_func_r_h; set => m_func_r_h=value; }

        /// <summary>
        /// The Function to calculate Saturation.
        /// </summary>
        public string FunctionGS { get => m_func_g_s; set => m_func_g_s=value; }

        /// <summary>
        /// The Function to calculate Value.
        /// </summary>
        public string FunctionBV { get => m_func_b_v; set => m_func_b_v=value; }

        /// <summary>
        /// The Function to calculate Alpha.
        /// </summary>
        public string FunctionAlpha { get => m_func_alp; set => m_func_alp=value; }

        /// <summary>
        /// Whether to use all Invalid color values when only one function is invalid.
        /// </summary>
        public bool InvalidColorGlobal { get => m_inval_col_glob; set => m_inval_col_glob=value; }

        /// <summary>
        /// If true, values like alpha that range from 0-1 will have their max at num/2, otherwise at num.
        /// </summary>
        public bool Circular { get => m_circular; set => m_circular=value; }

        /// <summary>
        /// if true, uses rgb for functions instead of hsv.
        /// </summary>
        public bool UseRGB { get => m_use_rgb; set => m_use_rgb=value; }
        #endregion

        public GenerationArgs()
        {
            m_width = 500;
            m_height = 500;
            m_framerate = 12;
            m_addons = new List<string>();

            m_func_r_h = "";
            m_func_g_s = "";
            m_func_b_v = "";
            m_func_alp = "";

            m_inval_col_glob = true;
            m_circular = false;
            m_use_rgb = false;
        }

        public string GetDebugInfo()
        {
            string result =
            $"{"Size: ",-30} {Width}x{Height} \n" +
            $"{"Framerate: ",-30} {Framerate} \n";
            
            if (UseRGB)
            {
                result += $"{"RedFunction: ",-30} {FunctionRH} \n" +
                    $"{"GreenFunction: ",-30} {FunctionGS} \n" +
                    $"{"BlueFunction: ",-30} {FunctionBV} \n";
            }
            else
            {
                result += $"{"HueFunction: ",-30} {FunctionRH} \n" +
                    $"{"SaturationFunction: ",-30} {FunctionGS} \n" +
                    $"{"ValueFunction: ",-30} {FunctionBV} \n";
            }

            result += $"{"AlphaFunction: ",-30} {FunctionAlpha} \n" +

            $"{"InvalidColorGlobal: ",-30} {InvalidColorGlobal} \n" +
            $"{"Circular: ",-30} {Circular} \n" +
            $"{"UseRGB: ",-30} {UseRGB} \n";

            if (AddOns.Count != 0)
            {
                result += $"{"Addons: ",-30} \n";
                for (int i = 0; i < AddOns.Count; i++) { result += AddOns[i] + "\n"; }
            }

            return result;
        }
    }
}
