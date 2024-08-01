using System;
using System.Collections.Generic;
using System.Linq;
using NCalc;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using FFMpegCore.Extensions.SkiaSharp;
using FFMpegCore.Pipes;
using Modulartistic.Core;
using NAudio.Wave;
using System.Reflection;
using FFMpegCore;
using Modulartistic.Drawing;
using System.ComponentModel;
using System.Text.Json;
using System.Xml.Linq;
using Json.Schema;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace Modulartistic.AudioGeneration
{
    public class AudioAnimationBuilder
    {
        #region private fields

        // object containing the audioanalysis
        private AudioAnalysis _analysis;

        // number of total frames
        private int _framecount;
        
        #endregion

        #region public properties

        [JsonIgnore]
        public string InputFile { get; set; }

        public StateOptions Options { get; set; }

        public State State { get; set; }

        /* Beat Properties -> Add after beat implemented
        public Easing BeatEasing { get; set; }
        public double BeatLength { get; set; }
        public double BeatPeak { get; set; }

        */
        [JsonIgnore]
        public Dictionary<string, string> StatePropertyFunctions { get; set; }

        #endregion

        #region constructors

        public AudioAnimationBuilder(string path) : this() 
        {
            InputFile = path;
        }

        public AudioAnimationBuilder() 
        {
            Options = new StateOptions();
            State = new State();
            StatePropertyFunctions = new Dictionary<string, string>();
        }

        #endregion

        #region Generation methods

        private IEnumerable<IVideoFrame> EnumerateFrames(GenerationOptions options)
        {
            // parses GenerationArgs
            uint framerate = Options.Framerate;

            Progress? visProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Audio visualization...", _framecount);
            // loops through the scenes
            for (int i = 0; i < _framecount; i++)
            {
                foreach (KeyValuePair<string, string> kvp in StatePropertyFunctions)
                {
                    State[kvp.Key] = EvaluateFunction(kvp.Value, _analysis, i);
                }
                yield return new BitmapVideoFrameWrapper(State.GetBitmap(Options, options));
                visProgress?.IncrementProgress();
            }
            options.ProgressReporter?.RemoveTask(visProgress);
        }

        private string GenerateFrames(GenerationOptions options, string frames_dir)
        {
            // parses GenerationArgs
            uint framerate = Options.Framerate;

            // create Directory for frames if not exist
            if (!Directory.Exists(frames_dir)) { Directory.CreateDirectory(frames_dir); }

            Progress? visProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Audio visualization...", _framecount);
            // loops through the scenes
            for (int i = 0; i < _framecount; i++)
            {
                foreach (KeyValuePair<string, string> kvp in StatePropertyFunctions)
                {
                    State[kvp.Key] = EvaluateFunction(kvp.Value, _analysis, i);
                }
                State.Name = "Frame_" + i.ToString().PadLeft(_framecount.ToString().Length, '0');
                RegisterAnalysisExtras(i);

                State.GenerateImage(Options, options, frames_dir);
                visProgress?.IncrementProgress();
            }
            options.ProgressReporter?.RemoveTask(visProgress);

            return frames_dir;
        }

        private async Task CreateAnimation(GenerationOptions options, AnimationFormat type, string absolute_out_filepath)
        {
            // parsing framerate and setting piping source
            uint framerate = Options.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(options))
            {
                FrameRate = framerate, // set source frame rate
            };


            switch (type)
            {
                case AnimationFormat.Gif:
                    // parsing size
                    System.Drawing.Size size = new System.Drawing.Size(Options.Width, Options.Height);

                    // generate the gif file
                    try
                    {
                        await FFMpegArguments
                        .FromPipeInput(videoFramesSource)
                        .OutputToFile(absolute_out_filepath + @".gif", false, options => options
                            .WithGifPaletteArgument(0, size, (int)framerate)
                            .WithFramerate(framerate))
                        .ProcessAsynchronously();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error generating animation. ", e);
                    }
                    break;
                case AnimationFormat.Mp4:
                    // generate the mp4 file
                    try
                    {
                        await FFMpegArguments
                        .FromPipeInput(videoFramesSource)
                        .OutputToFile(absolute_out_filepath + @".mp4", false, options => options
                            // .WithVideoCodec(VideoCodec.LibX265)
                            // .WithVideoBitrate(16000) // find a balance between quality and file size
                            .WithFramerate(framerate))
                        .ProcessAsynchronously();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error generating animation. ", e);
                    }
                    break;
                default:
                    throw new NotImplementedException("only gif and mp4 are supported for animation cration");
            }
        }

        private async Task CreateAnimationFromFolder(GenerationOptions options, AnimationFormat type, string folder)
        {
            // Creating the image list
            List<string> imgPaths = Directory.GetFiles(folder).ToList();

            // Enumerater for image files
            IEnumerable<IVideoFrame> EnumerateFrames()
            {
                // loops through the all img paths
                Progress? joinProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid().ToString()}", $"Joining frames", imgPaths.Count);
                for (int i = 0; i < imgPaths.Count; i++)
                {
                    joinProgress?.IncrementProgress();
                    yield return new BitmapVideoFrameWrapper(new Bitmap(imgPaths[i]));
                }
                options.ProgressReporter?.RemoveTask(joinProgress);
            }

            uint framerate = Options.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames())
            {
                FrameRate = framerate, // set source frame rate
            };


            switch (type)
            {
                case AnimationFormat.Gif:
                    // parsing size
                    System.Drawing.Size size = new System.Drawing.Size(Options.Width, Options.Height);

                    // generate the gif file
                    try
                    {
                        await FFMpegArguments
                        .FromPipeInput(videoFramesSource)
                        .OutputToFile(folder + @".gif", false, options => options
                            .WithGifPaletteArgument(0, size, (int)framerate)
                            .WithFramerate(framerate))
                        .ProcessAsynchronously();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error generating animation. ", e);
                    }
                    break;
                case AnimationFormat.Mp4:
                    // generate the mp4 file
                    try
                    {
                        await FFMpegArguments
                        .FromPipeInput(videoFramesSource)
                        .OutputToFile(folder + @".mp4", false, options => options
                            // .WithVideoCodec(VideoCodec.LibX265)
                            // .WithVideoBitrate(16000) // find a balance between quality and file size
                            .WithFramerate(framerate))
                        .ProcessAsynchronously();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error generating animation. ", e);
                    }
                    break;
                default:
                    throw new NotImplementedException("only gif and mp4 are supported for animation cration");
            }
        }

        public async Task<string> GenerateAnimation(GenerationOptions options, bool decibelScale, string out_dir)
        {
            // check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // parse options
            bool keepframes = options.KeepAnimationFrames;
            AnimationFormat type = options.AnimationFormat;

            AnalyseAudio(decibelScale);

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, Path.GetFileNameWithoutExtension(InputFile));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.GetValidFileName(file_path_out);

            if (keepframes)
            {
                // the folder where the frames are saved
                string folder = GenerateFrames(options, file_path_out);
                await CreateAnimationFromFolder(options, type, folder);
            }
            else
            {
                await CreateAnimation(options, type, file_path_out);
            }

            return file_path_out + $".{Helper.GetAnimationFormatExtension(type)}";
        }

        #endregion

        #region other methods

        /// <summary>
        /// Print debug information about the audioanalysis like min and max values as well as averages
        /// </summary>
        /// <param name="options"></param>
        /// <param name="decibelScale"></param>
        public void PrintDebug(GenerationOptions options, bool decibelScale)
        {
            ILogger? logger = options.Logger;

            AnalyseAudio(decibelScale);

            logger?.LogDebug($"Min PeakMax: {_analysis.MinPeakMax()}");
            logger?.LogDebug($"Max PeakMax: {_analysis.MaxPeakMax()}");
            logger?.LogDebug($"Avg PeakMax: {_analysis.AvgPeakMax()}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min PeakMin: {_analysis.MinPeakMin()}");
            logger?.LogDebug($"Max PeakMin: {_analysis.MaxPeakMin()}");
            logger?.LogDebug($"Avg PeakMin: {_analysis.AvgPeakMin()}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min SubBass: {_analysis.MinSubBass()}");
            logger?.LogDebug($"Max SubBass: {_analysis.MaxSubBass()}");
            logger?.LogDebug($"Avg SubBass: {_analysis.AvgSubBass()}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Bass: {_analysis.MinBass()}");
            logger?.LogDebug($"Max Bass: {_analysis.MaxBass()}");
            logger?.LogDebug($"Avg Bass: {_analysis.AvgBass()}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Lower Midrange: {_analysis.MinLowerMidrange()}");
            logger?.LogDebug($"Max Lower Midrange: {_analysis.MaxLowerMidrange()}");
            logger?.LogDebug($"Avg Lower Midrange: {_analysis.AvgLowerMidrange()}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Midrange: {_analysis.MinMidrange()}");
            logger?.LogDebug($"Max Midrange: {_analysis.MaxMidrange()}");
            logger?.LogDebug($"Avg Midrange: {_analysis.AvgMidrange()}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Upper Midrange: {_analysis.MinUpperMidrange()}");
            logger?.LogDebug($"Max Upper Midrange: {_analysis.MaxUpperMidrange()}");
            logger?.LogDebug($"Avg Upper Midrange: {_analysis.AvgUpperMidrange()}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Presence: {_analysis.MinPresence()}");
            logger?.LogDebug($"Max Presence: {_analysis.MaxPresence()}");
            logger?.LogDebug($"Avg Presence: {_analysis.AvgPresence()}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Brilliance: {_analysis.MinBrilliance()}");
            logger?.LogDebug($"Max Brilliance: {_analysis.MaxBrilliance()}");
            logger?.LogDebug($"Avg Brilliance: {_analysis.AvgBrilliance()}");

        }

        /// <summary>
        /// do audio analysis and fill the
        /// </summary>
        /// <param name="decibelScale"></param>
        /// <param name="extraseconds"></param>
        public void AnalyseAudio(bool decibelScale, double extraseconds = 3)
        {
            uint framerate = Options.Framerate;
            _analysis = new AudioAnalysis(InputFile, (int)framerate, decibelScale);
            _framecount = (int)((_analysis.AudioLength.TotalSeconds + extraseconds) * framerate);
        }

        /// <summary>
        /// Registers Extra functions and parameters for state functions used for pixel calculation
        /// </summary>
        /// <param name="idx"></param>
        public void RegisterAnalysisExtras(int idx)
        {
            foreach (MethodInfo mInfo in typeof(AudioAnalysis).GetMethods())
            {
                if (mInfo.IsSpecialName) { continue; }
                State.AddExtraFunction(_analysis, mInfo);
            }

            double time = idx / Options.Framerate;
            State.SetExtraParameter("time", time);
            State.SetExtraParameter("frame", idx);
        }
        
        /// <summary>
        /// Evaluate an expression, used for calculating StatePropertyFunctions
        /// </summary>
        /// <param name="func"></param>
        /// <param name="analysis"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        private double EvaluateFunction(string func, AudioAnalysis analysis, int idx)
        {
            Expression expression = new Expression(func);
            Helper.ExprRegisterStateOptions(ref expression, Options);
            Helper.ExprRegisterStateProperties(ref expression, State);

            double time = idx/Options.Framerate;
            expression.Parameters["time"] = time;
            expression.Parameters["frame"] = idx;

            foreach (MethodInfo mInfo in typeof(AudioAnalysis).GetMethods())
            {
                if (mInfo.IsSpecialName) { continue; }
                Helper.RegisterMethod(expression, analysis, mInfo);
            }

            double result = Convert.ToDouble(expression.Evaluate());
            return result;
        }

        #endregion

        #region json methods

        /// <summary>
        /// Loads audioAnimationBuilder properties from a JSON element and updates the builder accordingly.
        /// </summary>
        /// <param name="element">The JSON element containing the properties to be loaded.</param>
        /// <exception cref="Exception">Thrown when a JSON property value is neither a string nor a number.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when a JSON property name does not match any state property.</exception>
        /// <remarks>
        /// This method takes the State and Options properties of the JSON Element and iterates over theri properties to load the State and StateOptions objects. 
        /// If a property value is a string, it is treated as an expression and evaluated. If it is a number, it is directly assigned.
        /// If the Expression has errors it is assumed that it contains custom Analysis functions in which case the string is mapped to the <see cref="AudioAnimationBuilder.StatePropertyFunctions"/>
        /// </remarks>
        public void LoadJson(JsonElement element)
        {
            JsonElement optionsElement = element.GetProperty(nameof(Options));
            Options = StateOptions.FromJson(optionsElement);


            JsonElement stateElement = element.GetProperty(nameof(State));
            State = State.FromJson(stateElement, Options);
            foreach (JsonProperty elem in stateElement.EnumerateObject())
            {
                switch (elem.Name)
                {
                    case nameof(State.Name):
                        break;
                    case nameof(State.X0):
                    case nameof(State.Y0):
                    case nameof(State.XRotationCenter):
                    case nameof(State.YRotationCenter):
                    case nameof(State.XFactor):
                    case nameof(State.YFactor):
                    case nameof(State.Rotation):
                    case nameof(State.Mod):
                    case nameof(State.ModLowerLimit):
                    case nameof(State.ModUpperLimit):
                    case nameof(State.ColorRedHue):
                    case nameof(State.ColorGreenSaturation):
                    case nameof(State.ColorBlueValue):
                    case nameof(State.ColorAlpha):
                    case nameof(State.InvalidColorRedHue):
                    case nameof(State.InvalidColorGreenSaturation):
                    case nameof(State.InvalidColorBlueValue):
                    case nameof(State.InvalidColorAlpha):
                    case nameof(State.ColorFactorRedHue):
                    case nameof(State.ColorFactorGreenSaturation):
                    case nameof(State.ColorFactorBlueValue):
                        if (elem.Value.ValueKind == JsonValueKind.String)
                        {
                            // if value is string type evaluate
                            Expression expr = new Expression(elem.Value.GetString());
                            Helper.ExprRegisterStateProperties(ref expr, State);
                            Helper.ExprRegisterStateOptions(ref expr, Options);
                            if (expr.HasErrors()) 
                            { 
                                StatePropertyFunctions[elem.Name] = elem.Value.GetString() ?? "";
                            }
                        }
                        break;
                    case nameof(State.Parameters):
                        // iterate over Parameters
                        int i = 0;
                        foreach (JsonElement param_elem in elem.Value.EnumerateArray())
                        {
                            if (param_elem.ValueKind == JsonValueKind.String)
                            {
                                // if value is string type evaluate
                                Expression expr = new Expression(param_elem.GetString());
                                Helper.ExprRegisterStateProperties(ref expr, State);
                                Helper.ExprRegisterStateOptions(ref expr, Options);
                                if (expr.HasErrors())
                                {
                                    string name = $"i{i}";
                                    StatePropertyFunctions[name] = elem.Value.GetString() ?? "";
                                }
                            }
                            i++;
                        }
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{elem.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="State"/> instance from a JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the state properties to be loaded.</param>
        /// <param name="opts">The state options used for expression evaluation if any value is a string.</param>
        /// <returns>A new <see cref="State"/> instance with properties loaded from the provided JSON element.</returns>
        /// <remarks>
        /// This method initializes a new <see cref="State"/> instance and then loads its properties from the provided JSON element
        /// using the <see cref="LoadJson(JsonElement, StateOptions)"/> method.
        /// </remarks>
        public static AudioAnimationBuilder FromJson(JsonElement element)
        {
            AudioAnimationBuilder builder = new AudioAnimationBuilder();
            builder.LoadJson(element);

            return builder;
        }

        /// <summary>
        /// Loads <see cref="AudioAnimationBuilder"/> from a JSON file.
        /// </summary>
        /// <param name="file">The path to the JSON file.</param>
        /// <returns>A new instance of <see cref="AudioAnimationBuilder"/> populated with the data from the JSON file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the JSON file contains an unrecognized property type.</exception>
        /// <exception cref="JsonException">Thrown if there is an error parsing the JSON file.</exception>
        /// <remarks>
        /// This method reads the JSON file, parses it into a <see cref="JsonDocument"/>, and creates a <see cref="AudioAnimationBuilder"/> instance from the root JSON element.
        /// </remarks>
        public static AudioAnimationBuilder FromFile(string file)
        {
            if (!File.Exists(file)) { throw new FileNotFoundException($"The file '{file}' does not exist.", file); }

            try
            {
                using (var stream = File.OpenRead(file))
                {
                    using (var jdoc = JsonDocument.Parse(stream))
                    {
                        return FromJson(jdoc.RootElement);
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Error parsing JSON in file '{file}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing file '{file}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Uses the AudioAnimationBuilder schema to validate if a specified JSON element is a valid <see cref="AudioAnimationBuilder"/> object.
        /// </summary>
        /// <param name="root">The <see cref="JsonElement"/> to validate.</param>
        /// <returns><see cref="Json.Schema.EvaluationResults"/> containing the results of the validation.</returns>
        /// <exception cref="Exception">Thrown if the validation fails and the JSON is not in the correct format.</exception>
        private static EvaluationResults ValidateAudioAnimationBuilderJson(JsonElement root)
        {
            // load jsonSchema
            JsonSchema schema = JsonSchema.FromText(Schema.GetAudioAnimationBuilderSchema());

            // validate
            EvaluationResults result = schema.Evaluate(root);

            // return evaluation results
            return result;
        }

        /// <summary>
        /// Serializes the current <see cref="AudioAnimationBuilder"/> object to a JSON string.
        /// </summary>
        /// <returns>The serialized <see cref="AudioAnimationBuilder"/> object as a JSON string.</returns>
        /// <remarks>
        /// This method uses <see cref="JsonSerializer"/> with options to ignore null values, format the JSON with indentation, and use a custom dictionary converter.
        /// </remarks>
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter(),
                },
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            JsonObject root = JsonSerializer.SerializeToNode(this, options).AsObject();
            foreach (KeyValuePair<string, string> kvp in StatePropertyFunctions)
            {
                root[nameof(Core.State)][kvp.Key] = kvp.Value;
            }

            return JsonSerializer.Serialize(this, options);
        }

        /// <summary>
        /// Saves the current <see cref="AudioAnimationBuilder"/> object as a JSON file.
        /// </summary>
        /// <param name="out_dir">The directory where the JSON file should be saved.</param>
        /// <param name="name">The name of the JSON file (excluding extension).</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the specified directory does not exist.</exception>
        /// <remarks>
        /// This method checks if the directory exists and validates the filename to avoid overwriting existing files. The file is saved with a ".json" extension.
        /// </remarks>
        public void SaveJson(string out_dir, string name)
        {
            // check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, name);

            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.GetValidFileName(file_path_out);

            // Write contents and Save the file
            File.WriteAllText(file_path_out + @".json", ToJson());
        }
        
        #endregion
    }
}
