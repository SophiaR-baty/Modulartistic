using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Reflection;
using NCalc;
using Json.Schema;
using Modulartistic.AddOns;
using Modulartistic.Common;

#nullable enable

namespace Modulartistic.Core
{
    /// <summary>
    /// Represents the options for configuring state properties including image dimensions, framerate, and color functions.
    /// </summary>
    /// <remarks>
    /// The <see cref="StateOptions"/> class provides various settings to customize the behavior of state objects in an application.
    /// It includes properties for image dimensions, framerate, color functions, and additional settings related to color processing.
    /// This class also supports JSON serialization and deserialization for configuration purposes.
    /// </remarks>
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
        private List<FunctionParameter> m_params;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width of the image or animation.
        /// </summary>
        public int Width { get => m_width; set => m_width = value; }

        /// <summary>
        /// Gets or sets the height of the image or animation.
        /// </summary>
        public int Height { get => m_height; set => m_height = value; }

        /// <summary>
        /// Gets or sets the framerate for animations.
        /// </summary>
        public uint Framerate { get => m_framerate; set => m_framerate = value; }


        /// <summary>
        /// Gets or sets the function used to calculate the red or hue.
        /// </summary>
        public string FunctionRedHue { get => m_func_r_h; set => m_func_r_h = value; }

        /// <summary>
        /// Gets or sets the function used to calculate the green or saturation.
        /// </summary>
        public string FunctionGreenSaturation { get => m_func_g_s; set => m_func_g_s = value; }

        /// <summary>
        /// Gets or sets the function used to calculate the blue or value.
        /// </summary>
        public string FunctionBlueValue { get => m_func_b_v; set => m_func_b_v = value; }

        /// <summary>
        /// Gets or sets the function used to calculate the alpha value.
        /// </summary>
        public string FunctionAlpha { get => m_func_alp; set => m_func_alp = value; }


        /// <summary>
        /// Gets or sets a value indicating whether to use all invalid color values when only one function is invalid.
        /// </summary>
        public bool InvalidColorGlobal { get => m_inval_col_glob; set => m_inval_col_glob = value; }

        /// <summary>
        /// Gets or sets a value indicating whether values like alpha ranging from 0-1 will have their maximum at num/2 instead of num.
        /// </summary>
        public bool CircularMod { get => m_circular; set => m_circular = value; }

        /// <summary>
        /// Gets or sets a value indicating whether to use RGB for functions instead of HSV.
        /// </summary>
        public bool UseRGB { get => m_use_rgb; set => m_use_rgb = value; }

        /// <summary>
        /// Gets the list of add-ons to load.
        /// </summary>
        public List<string> AddOns { get => m_addons; }

        public List<FunctionParameter> Parameters { get => m_params; }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateOptions"/> class with default values.
        /// </summary>
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
            m_params = new List<FunctionParameter>();
        }

        #endregion

        public void RegisterAddOnsForParameters(GenerationOptions genOpts)
        {
            AddOnInitializationArgs args = new AddOnInitializationArgs()
            {
                Framerate = Framerate,
                Height = Height,
                Width = Width,
                Logger = genOpts.Logger,
                Guid = genOpts.GenerationDataGuid,
                ProgressReporter = genOpts.ProgressReporter,
            };
            foreach (var param in Parameters)
            {
                foreach (string addon in AddOns)
                {
                    param.LoadAddOn(addon, args);
                }

                param.IsStatic = param.CanEvaluate();
            }
        }

        #region Methods for debugging
        
        public void TestLoadingAddOns(GenerationOptions options)
        {
            ILogger? logger = options.Logger;
            IPathProvider pathProvider = options.PathProvider;

            foreach (string path in  m_addons)
            {
                string dll_path = Helper.GetAddOnPath(path, options.PathProvider);
                if (!File.Exists(dll_path))
                {
                    throw new FileNotFoundException($"Error loading AddOn: {dll_path} - file not found");
                }

                // log addons path name
                options.Logger?.LogDebug($"AddOns defined in {path}: ");

                // load assembly and iterate types
                Assembly testDLL = Assembly.LoadFile(dll_path);
                Type[] types = testDLL.GetTypes().Where(type => type.GetCustomAttribute(typeof(AddOnAttribute)) is not null).ToArray();
                if (types.Length == 0) options.Logger?.LogDebug($"  None");
                foreach (Type type in types)
                {
                    // log class name
                    options.Logger?.LogDebug($"  Functions in {type.Name}: ");

                    // gets all public static methods of the type
                    // -> only methods that should be exposed to the parser should be public static
                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    if (methodInfos.Length == 0) options.Logger?.LogDebug($"    None");
                    // iterates over all such methods
                    foreach (MethodInfo methodInfo in methodInfos)
                    {

                        // Get return type
                        string returnType = methodInfo.ReturnType.Name;

                        // Get method name
                        string methodName = methodInfo.Name;

                        // Get parameter information
                        var parameters = methodInfo.GetParameters();
                        string parametersString = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));

                        options.Logger?.LogDebug($"    {returnType} {methodName}({parametersString})");
                    }
                }
            }
        }
        
        #endregion

        #region JSON Methods

        /// <summary>
        /// Determines whether the specified <see cref="JsonElement"/> is a valid representation of <see cref="StateOptions"/>.
        /// </summary>
        /// <param name="element">The JSON element to validate.</param>
        /// <returns><c>true</c> if the JSON element is valid; otherwise, <c>false</c>.</returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Loads the <see cref="StateOptions"/> properties from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the state options data.</param>
        /// <exception cref="KeyNotFoundException">Thrown when a property in the JSON element does not match any known properties of <see cref="StateOptions"/>.</exception>
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
                    case nameof(Parameters):
                        Parameters.AddRange(elem.Value.EnumerateArray().Select((e, i) => new FunctionParameter(
                            e.GetProperty(nameof(FunctionParameter.Name)).GetString(), 
                            e.GetProperty(nameof(FunctionParameter.Expression)).GetString())));
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{elem.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="StateOptions"/> from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the state options data.</param>
        /// <returns>A new <see cref="StateOptions"/> instance populated with data from the JSON element.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when a property in the JSON element does not match any known properties of <see cref="StateOptions"/>.</exception>
        public static StateOptions FromJson(JsonElement element)
        {
            StateOptions stateOptions = new StateOptions();
            stateOptions.LoadJson(element);
            
            return stateOptions;
        }

        /// <summary>
        /// Gets the value of a JSON property or evaluates it if it's a string.
        /// </summary>
        /// <param name="elem">The JSON property element.</param>
        /// <returns>The evaluated value as a double.</returns>
        /// <exception cref="Exception">Thrown when the JSON property is neither a string nor a number.</exception>
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
