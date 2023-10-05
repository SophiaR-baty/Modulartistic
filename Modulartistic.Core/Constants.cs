using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Modulartistic.Core
{
    public static class Constants
    {
        // For State
        public const string STATENAME_DEFAULT = "State";   // default for state name

        public const double XY0_DEFAULT = 0;                // default for x0 and y0
        public const double XYROTCENTER_DEFAULT = 0;        // default for x_rot_center and y_rot_center
        public const double XYFACTOR_DEFAULT = 1;           // default for xfact and yfact
        public const double ROTATION_DEFAULT = 0;           // default for rotation

        public const double NUM_DEFAULT = 500;              // default for num
        public const double LIMLOW_DEFAULT = 0;             // default for limlow
        public const double LIMHIGH_DEFAULT = 500;          // default for limhigh

        public const double COLOR_HUE_DEFAULT = 0;              // default for all color values (rgb, hsv)
        public const double COLOR_SATURATION_DEFAULT = 1;              // default for all color values (rgb, hsv)
        public const double COLOR_VALUE_DEFAULT = 1;              // default for all color values (rgb, hsv)
        public const double COLOR_RED_DEFAULT = 1;              // default for all color values (rgb, hsv)
        public const double COLOR_GREEN_DEFAULT = 1;              // default for all color values (rgb, hsv)
        public const double COLOR_BLUE_DEFAULT = 1;              // default for all color values (rgb, hsv)
        public const double COLOR_ALPHA_DEFAULT = 1;              // default for all color values (rgb, hsv)

        public const double INV_COLOR_HUE_DEFAULT = 0;              // default for all color values (rgb, hsv)
        public const double INV_COLOR_SATURATION_DEFAULT = 0;              // default for all color values (rgb, hsv)
        public const double INV_COLOR_VALUE_DEFAULT = 0;              // default for all color values (rgb, hsv)
        public const double INV_COLOR_RED_DEFAULT = 0;              // default for all color values (rgb, hsv)
        public const double INV_COLOR_GREEN_DEFAULT = 0;              // default for all color values (rgb, hsv)
        public const double INV_COLOR_BLUE_DEFAULT = 0;              // default for all color values (rgb, hsv)
        public const double INV_COLOR_ALPHA_DEFAULT = 1;              // default for all color values (rgb, hsv)

        public const double COLORFACT_DEFAULT = 1;          // default for color factors

        // For StateSequence
        public const string STATESEQUENCE_NAME_DEFAULT = "animation";   // default for statesequence name
        public const string SCENE_NAME_DEFAULT = "scene";   // default for state scene
        
        // For GenerationData
        public const string GENERATIONDATA_NAME_DEFAULT = "generation_data";   // default for generationdata

        // For GenerationArgs
        public const uint FRAMERATE_DEFAULT = 12;                    // default for framerate
        public const bool INVALIDCOLORGLOBAL_DEFAULT = false;        // default invalcolglobal
        public const bool CIRCULAR_DEFAULT = true;                   // default circular
        public const bool USERGB_DEFAULT = false;                    // default usergb

        // For StateTimeline
        public const string STATETIMELINE_NAME_DEFAULT = "timeline";   // default for statetimeline name

        // For Config
        public const string OUTPUT_CONFIG_KEY = "output_path";
        public const string INPUT_CONFIG_KEY = "input_path";
        public const string DEMO_CONFIG_KEY = "demo_path";
        public const string FFMPEG_CONFIG_KEY = "ffmpeg_path";
        public const string ADDON_CONFIG_KEY = "addon_path";

        public static readonly string OUTPUT_FOLDER_DEFAULT = AppDomain.CurrentDomain.BaseDirectory + "output";
        public static readonly string INPUT_FOLDER_DEFAULT = AppDomain.CurrentDomain.BaseDirectory + "input";
        public static readonly string ADDONS_FOLDER_DEFAULT = AppDomain.CurrentDomain.BaseDirectory + "addons";
        public static readonly string DEMOS_FOLDER_DEFAULT = AppDomain.CurrentDomain.BaseDirectory + "demofiles";
        public static readonly string FFMPEG_FOLDER_DEFAULT = "";
        public static readonly string CONFIG_FILE_PATH = AppDomain.CurrentDomain.BaseDirectory + "pathconfig.json";


    }
}
