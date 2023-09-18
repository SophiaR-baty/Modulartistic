using System.Collections.Generic;

#nullable enable

namespace Modulartistic.Core
{
    public class GenerationArgs
    {
        #region Properties
        /// <summary>
        /// The Image/Animation Dimensions.
        /// </summary>
        public int[] Size { get; set; }
        /// <summary>
        /// The Framerate for Animations.
        /// </summary>
        public uint? Framerate { get; set; }
        /// <summary>
        /// AddOns to load.
        /// </summary>
        public List<string>? AddOns { get; set; }

        /// <summary>
        /// The Function to calculate Hue.
        /// </summary>
        public string? HueFunction { get; set; }
        /// <summary>
        /// The Function to calculate Saturation.
        /// </summary>
        public string? SaturationFunction { get; set; }
        /// <summary>
        /// The Function to calculate Value.
        /// </summary>
        public string? ValueFunction { get; set; }

        /// <summary>
        /// The Function to calculate Red.
        /// </summary>
        public string? RedFunction { get; set; }
        /// <summary>
        /// The Function to calculate Green.
        /// </summary>
        public string? GreenFunction { get; set; }
        /// <summary>
        /// The Function to calculate Blue.
        /// </summary>
        public string? BlueFunction { get; set; }

        /// <summary>
        /// The Function to calculate Alpha.
        /// </summary>
        public string? AlphaFunction { get; set; }

        /// <summary>
        /// Whether to use all Invalid color values when only one function is invalid.
        /// </summary>
        public bool? InvalidColorGlobal { get; set; }
        /// <summary>
        /// If true, values like alpha that range from 0-1 will have their max at num/2, otherwise at num.
        /// </summary>
        public bool? Circular { get; set; }
        /// <summary>
        /// if true, uses rgb for functions instead of hsv.
        /// </summary>
        public bool? UseRGB { get; set; }
        #endregion

        public GenerationArgs()
        {
            Size = new int[] { 500, 500 };
            Framerate = null;
            AddOns = null;

            HueFunction = null;
            SaturationFunction = null;
            ValueFunction = null;

            RedFunction = null;
            GreenFunction = null;
            BlueFunction = null;

            AlphaFunction = null;

            InvalidColorGlobal = null;
            Circular = null;
            UseRGB = null;
        }

        public string GetDebugInfo()
        {
            string result =
            $"{"Size: ",-30} {Size[0]}x{Size[1]} \n" +
            (Framerate == null ? "" : $"{"Framerate: ",-30} {Framerate} \n") +

            (HueFunction == null ? "" : $"{"HueFunction: ",-30} {HueFunction} \n") +
            (SaturationFunction == null ? "" : $"{"SaturationFunction: ",-30} {SaturationFunction} \n") +
            (ValueFunction == null ? "" : $"{"ValueFunction: ",-30} {ValueFunction} \n") +

            (RedFunction == null ? "" : $"{"RedFunction: ",-30} {RedFunction} \n") +
            (GreenFunction == null ? "" : $"{"GreenFunction: ",-30} {GreenFunction} \n") +
            (BlueFunction == null ? "" : $"{"BlueFunction: ",-30} {BlueFunction} \n") +

            (AlphaFunction == null ? "" : $"{"AlphaFunction: ",-30} {AlphaFunction} \n") +

            (InvalidColorGlobal == null ? "" : $"{"InvalidColorGlobal: ",-30} {InvalidColorGlobal} \n") +
            (Circular == null ? "" : $"{"Circular: ",-30} {Circular} \n") +
            (UseRGB == null ? "" : $"{"UseRGB: ",-30} {UseRGB} \n");

            if (AddOns != null)
            {
                result += $"{"Addons: ",-30} \n";
                for (int i = 0; i < AddOns.Count; i++) { result += AddOns[i] + "\n"; }
            }

            return result;
        }
    }
}
