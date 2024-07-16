namespace Modulartistic.Core
{
    /// <summary>
    /// Constants and default values
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Constants and default values for State Objects
        /// </summary>
        public static class State
        {
            public const string STATENAME_DEFAULT = "State";   // default for state name

            public const double XY0_DEFAULT = 0;                // default for x0 and y0
            public const double XYROTCENTER_DEFAULT = 0;        // default for x_rot_center and y_rot_center
            public const double XYFACTOR_DEFAULT = 1;           // default for xfact and yfact
            public const double ROTATION_DEFAULT = 0;           // default for rotation

            public const double NUM_DEFAULT = 500;              // default for num
            public const double LIMLOW_DEFAULT = 0;             // default for limlow
            public const double LIMHIGH_DEFAULT = 500;          // default for limhigh

            public const double COLOR_R_H_DEFAULT = 0;              // default for all color values (rgb, hsv)
            public const double COLOR_G_S_DEFAULT = 1;              // default for all color values (rgb, hsv)
            public const double COLOR_B_V_DEFAULT = 1;              // default for all color values (rgb, hsv)
            public const double COLOR_ALP_DEFAULT = 1;              // default for all color values (rgb, hsv)

            public const double INV_COLOR_R_H_DEFAULT = 0;              // default for all color values (rgb, hsv)
            public const double INV_COLOR_G_S_DEFAULT = 0;              // default for all color values (rgb, hsv)
            public const double INV_COLOR_B_V_DEFAULT = 0;              // default for all color values (rgb, hsv)
            public const double INV_COLOR_ALP_DEFAULT = 1;              // default for all color values (rgb, hsv)

            public const double COLORFACT_DEFAULT = 1;          // default for color factors
        }

        /// <summary>
        /// Constants and default values for StateSequence Objects
        /// </summary>
        public static class StateSequence
        {
            public const string STATESEQUENCE_NAME_DEFAULT = "animation";   // default for statesequence name
            public const string SCENE_NAME_DEFAULT = "scene";   // default for state scene
        }

        /// <summary>
        /// Constants and default values for GenerationData Objects
        /// </summary>
        public static class GenerationData
        {
            public const string GENERATIONDATA_NAME_DEFAULT = "generation_data";   // default for generationdata
        }

        /// <summary>
        /// Constants and default values for StateOptions Objects
        /// </summary>
        public static class StateOptions
        {
            public const int WIDTH_DEFAULT = 500;
            public const int HEIGHT_DEFAULT = 500;

            public const uint FRAMERATE_DEFAULT = 12;                    // default for framerate
            public const bool INVALIDCOLORGLOBAL_DEFAULT = false;        // default invalcolglobal
            public const bool CIRCULAR_DEFAULT = true;                   // default circular
            public const bool USERGB_DEFAULT = false;                    // default usergb

            public const string FUNC_R_H_DEFAULT = "x*y";
            public const string FUNC_G_S_DEFAULT = "";
            public const string FUNC_B_V_DEFAULT = "";
            public const string FUNC_ALP_DEFAULT = "";
        }

        /// <summary>
        /// Constants and default values for StateTimeline Objects
        /// </summary>
        public static class StateTimeline
        {
            public const string STATETIMELINE_NAME_DEFAULT = "timeline";   // default for statetimeline name
        }

        /// <summary>
        /// Constants and default values for Config
        /// </summary>
        public static class Config
        {
            public const string GENERATIONDATA_SCHEMA_FILE = "modulartistic_generation_data_schema.json";
        }
    }
}
