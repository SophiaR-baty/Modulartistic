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

namespace Modulartistic.AudioGeneration
{
    public class AudioAnimationBuilder
    {
        #region private fields

        private AudioAnalysis _analysis;

        private int _framecount;
        #endregion


        #region public properties

        public string InputFile { get; set; }

        public StateOptions Options { get; set; }

        public State State { get; set; }

        public Dictionary<string, string> StatePropertyFunctions { get; set; }

        #endregion

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

        public void PrintDebug(GenerationOptions options, bool decibelScale)
        {
            ILogger? logger = options.Logger;

            AnalyseAudio(decibelScale);

            logger?.LogDebug($"Min PeakMax: {_analysis.Frames.Min(f => f.PeakMax)}");
            logger?.LogDebug($"Max PeakMax: {_analysis.Frames.Max(f => f.PeakMax)}");
            logger?.LogDebug($"Avg PeakMax: {_analysis.Frames.Average(f => f.PeakMax)}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min PeakMin: {_analysis.Frames.Min(f => f.PeakMin)}");
            logger?.LogDebug($"Max PeakMin: {_analysis.Frames.Max(f => f.PeakMin)}");
            logger?.LogDebug($"Avg PeakMin: {_analysis.Frames.Average(f => f.PeakMin)}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min SubBass: {_analysis.Frames.Min(f => f.Frequencybands[0])}");
            logger?.LogDebug($"Max SubBass: {_analysis.Frames.Max(f => f.Frequencybands[0])}");
            logger?.LogDebug($"Max SubBass: {_analysis.Frames.Average(f => f.Frequencybands[0])}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Bass: {_analysis.Frames.Min(f => f.Frequencybands[1])}");
            logger?.LogDebug($"Max Bass: {_analysis.Frames.Max(f => f.Frequencybands[1])}");
            logger?.LogDebug($"Avg Bass: {_analysis.Frames.Average(f => f.Frequencybands[1])}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Lower Midrange: {_analysis.Frames.Min(f => f.Frequencybands[2])}");
            logger?.LogDebug($"Max Lower Midrange: {_analysis.Frames.Max(f => f.Frequencybands[2])}");
            logger?.LogDebug($"Avg Lower Midrange: {_analysis.Frames.Average(f => f.Frequencybands[2])}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Midrange: {_analysis.Frames.Min(f => f.Frequencybands[3])}");
            logger?.LogDebug($"Max Midrange: {_analysis.Frames.Max(f => f.Frequencybands[3])}");
            logger?.LogDebug($"Avg Midrange: {_analysis.Frames.Average(f => f.Frequencybands[3])}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Upper Midrange: {_analysis.Frames.Min(f => f.Frequencybands[4])}");
            logger?.LogDebug($"Max Upper Midrange: {_analysis.Frames.Max(f => f.Frequencybands[4])}");
            logger?.LogDebug($"Avg Upper Midrange: {_analysis.Frames.Average(f => f.Frequencybands[4])}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Presence: {_analysis.Frames.Min(f => f.Frequencybands[5])}");
            logger?.LogDebug($"Max Presence: {_analysis.Frames.Max(f => f.Frequencybands[5])}");
            logger?.LogDebug($"Avg Presence: {_analysis.Frames.Average(f => f.Frequencybands[5])}");
            logger?.LogDebug("");
            logger?.LogDebug($"Min Brilliance: {_analysis.Frames.Min(f => f.Frequencybands[6])}");
            logger?.LogDebug($"Max Brilliance: {_analysis.Frames.Max(f => f.Frequencybands[6])}");
            logger?.LogDebug($"Avg Brilliance: {_analysis.Frames.Average(f => f.Frequencybands[6])}");
        }

        public void AnalyseAudio(bool decibelScale, double extraseconds = 3)
        {
            uint framerate = Options.Framerate;
            _analysis = new AudioAnalysis(InputFile, (int)framerate, decibelScale);
            _framecount = (int)((_analysis.AudioLength.TotalSeconds + extraseconds) * framerate);
        }

        public void RegisterAnalysisExtras(int idx)
        {
            foreach (MethodInfo mInfo in typeof(AudioAnalysis).GetMethods())
            {
                if (mInfo.IsSpecialName) { continue; }
                State.AddExtraFunction(_analysis, mInfo);
            }

            double time = idx / Options.Framerate;
            State.AddExtraParameter("time", time);
            State.AddExtraParameter("frame", idx);
        }

        private double EvaluateFunction(string func, AudioAnalysis analysis, int idx)
        {
            Expression expression = new Expression(func);
            Helper.ExprRegisterStateOptions(ref expression, Options);
            Helper.ExprRegisterStateProperties(ref expression, State);

            double time = idx/Options.Framerate;
            expression.Parameters["time"] = time;
            expression.Parameters["frame"] = idx;

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetPeakMax))
                {
                    float result = analysis.GetPeakMax(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetPeakMin))
                {
                    float result = analysis.GetPeakMin(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetSubBass))
                {
                    float result = analysis.GetSubBass(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetBass))
                {
                    float result = analysis.GetBass(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetLowerMidrange))
                {
                    float result = analysis.GetLowerMidrange(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetMidrange))
                {
                    float result = analysis.GetMidrange(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetUpperMidrange))
                {
                    float result = analysis.GetUpperMidrange(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetPresence))
                {
                    float result = analysis.GetPresence(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            expression.EvaluateFunction += delegate (string name, FunctionArgs args)
            {
                if (name == nameof(analysis.GetBrilliance))
                {
                    float result = analysis.GetBrilliance(Convert.ToInt32(args.Parameters[0].Evaluate()));
                    args.Result = result;
                }
            };

            double result = Convert.ToDouble(expression.Evaluate());
            return result;
        }

        #endregion

        #region json methods

        public void LoadJson(JsonElement element, StateOptions opts)
        {
            JsonElement stateElement = element.GetProperty(nameof(State));
            foreach (JsonProperty elem in stateElement.EnumerateObject())
            {
                switch (elem.Name)
                {
                    case nameof(State.Name):
                        State[elem.Name] = elem.Value.GetString();
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
                        // retrieve the value beforehand
                        double value;
                        if (elem.Value.ValueKind == JsonValueKind.String)
                        {
                            // if value is string type evaluate
                            Expression expr = new Expression(elem.Value.GetString());
                            Helper.ExprRegisterStateProperties(ref expr, State);
                            Helper.ExprRegisterStateOptions(ref expr, opts);
                            if (expr.HasErrors()) 
                            { 
                                StatePropertyFunctions[elem.Name] = elem.Value.GetString() ?? "";
                                break;
                            }
                            value = (double)expr.Evaluate();
                        }
                        else if (elem.Value.ValueKind == JsonValueKind.Number)
                        {
                            // if value is double type simply get value
                            value = elem.Value.GetDouble();
                        }
                        else { throw new Exception($"Property {elem.Name} must be string or number. "); }
                        State[elem.Name] = value;
                        break;
                    case nameof(State.Parameters):
                        // iterate over Parameters
                        int i = 0;
                        foreach (JsonElement param_elem in elem.Value.EnumerateArray())
                        {
                            double param_value;
                            if (param_elem.ValueKind == JsonValueKind.String)
                            {
                                // if value is string type evaluate
                                Expression expr = new Expression(param_elem.GetString());
                                Helper.ExprRegisterStateProperties(ref expr, State);
                                Helper.ExprRegisterStateOptions(ref expr, opts);
                                if (expr.HasErrors())
                                {
                                    StatePropertyFunctions[elem.Name] = elem.Value.GetString() ?? "";
                                    i++;
                                    continue;
                                }
                                param_value = (double)expr.Evaluate();
                            }
                            else if (param_elem.ValueKind == JsonValueKind.Number)
                            {
                                // if value is double type simply get value
                                param_value = param_elem.GetDouble();
                            }
                            else { throw new Exception($"Element must be string or number. "); }

                            // set Parameter value
                            State.Parameters[i] = param_value;
                            i++;
                        }
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{elem.Name}' does not exist on type '{GetType().Name}'.");
                }
            }

            JsonElement optionsElement = element.GetProperty(nameof(StateOptions));
            Options = StateOptions.FromJson(optionsElement);
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
        public static State FromJson(JsonElement element, StateOptions opts)
        {
            State state = new State();
            state.LoadJson(element, opts);

            return state;
        }

        /// <summary>
        /// Uses the GenerationData schema to validate if a specified JSON element is a valid <see cref="GenerationData"/> object.
        /// </summary>
        /// <param name="root">The <see cref="JsonElement"/> to validate.</param>
        /// <returns><see cref="Json.Schema.EvaluationResults"/> containing the results of the validation.</returns>
        /// <exception cref="Exception">Thrown if the validation fails and the JSON is not in the correct format.</exception>
        private static EvaluationResults ValidateGenerationDataJson(JsonElement root)
        {
            // load jsonSchema
            JsonSchema schema = JsonSchema.FromText(Schemas.GetGenerationDataSchema());

            // validate
            EvaluationResults result = schema.Evaluate(root);

            // return evaluation results
            return result;
        }

        /// <summary>
        /// Serializes the current <see cref="GenerationData"/> object to a JSON string.
        /// </summary>
        /// <returns>The serialized <see cref="GenerationData"/> object as a JSON string.</returns>
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
            return JsonSerializer.Serialize(this, options);
        }

        /// <summary>
        /// Saves the current <see cref="GenerationData"/> object as a JSON file.
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

        public static AudioAnimationBuilder FromJson(JsonElement element)
        {
            AudioAnimationBuilder data = new AudioAnimationBuilder();
            data.LoadJson(element);

            return data;
        }

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


        #endregion
    }
}
