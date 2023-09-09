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

        public const double COLOR_DEFAULT = 0;              // default for all color values (rgb, hsv and invals too)
        public const double COLORFACT_DEFAULT = 1;          // default for color factors
        public const double ALPHA_DEFAULT = 1;              // default for alpha

        // For StateSequence
        public const string STATESEQUENCE_NAME_DEFAULT = "animation";   // default for state name
        public const string SCENE_NAME_DEFAULT = "scene";   // default for state scene
        
        // For GenerationData
        public const string GENERATIONDATA_NAME_DEFAULT = "generation_data";   // default for state scene

        // For GenerationArgs
        public const uint FRAMERATE_DEFAULT = 12;                    // default for framerate
        public const bool INVALIDCOLORGLOBAL_DEFAULT = false;        // default invalcolglobal
        public const bool CIRCULAR_DEFAULT = true;                   // default circular
        public const bool USERGB_DEFAULT = false;                    // default usergb

        // Folders configuration -> put this in a config file
        private static string _output_folder = AppDomain.CurrentDomain.BaseDirectory + "output";
        private static string _input_folder = AppDomain.CurrentDomain.BaseDirectory + "input";
        private static string _addon_folder = AppDomain.CurrentDomain.BaseDirectory + "addons";
        private static string _demo_folder = AppDomain.CurrentDomain.BaseDirectory + "demofiles";

        private static string _ffmpeg_folder = ""; // implement this once config files work

        public static string OUTPUTFOLDER { get => _output_folder; set => _output_folder = value; }
        public static string INPUTFOLDER { get => _input_folder; set => _input_folder = value; }
        public static string ADDONFOLDER { get => _addon_folder; set => _addon_folder = value; }
        public static string DEMOFOLDER { get => _demo_folder; set => _demo_folder = value; }
        public static string FFMPEGFOLDER { get => _ffmpeg_folder; set => _ffmpeg_folder = value; }

    }
}
