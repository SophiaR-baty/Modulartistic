using System;
using System.Collections.Generic;
using System.Linq;
using AnimatedGif;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FFMpegCore.Extensions.SkiaSharp;
using FFMpegCore.Pipes;
using FFMpegCore;
using FFMpegCore.Enums;
using Modulartistic.Drawing;
using System.Reflection;
using System.Text.Json;
using static Modulartistic.Core.Constants;
using System.Xml.Linq;

namespace Modulartistic.Core
{
    /// <summary>
    /// StateTimeline consisting of a Base State and several timed StateEvents
    /// </summary>
    public class StateTimeline
    {
        #region Properties
        /// <summary>
        /// The Name of this StateTimeline
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Length in Milliseconds
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// Base State
        /// </summary>
        public State Base { get; set; }

        /// <summary>
        /// List of StateEvents with their start Timings (in Milliseconds)
        /// </summary>
        public List<StateEvent> Events { get; set; }

        [JsonIgnore]
        public double LengthInSeconds { get => Length / 1000.0; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an Empty StateTimeline
        /// </summary>
        public StateTimeline(string name) : this()
        {
            Name = name == "" ? Constants.StateTimeline.STATETIMELINE_NAME_DEFAULT : name;
        }

        /// <summary>
        /// Creates an Empty StateTimeline
        /// </summary>
        public StateTimeline()
        {
            Name = Constants.StateTimeline.STATETIMELINE_NAME_DEFAULT;
            Length = 0;
            Base = new State();
            Events = new List<StateEvent>();
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Gets the total amount of frames for a given framerate
        /// </summary>
        /// <param name="framerate">framerate</param>
        /// <returns></returns>
        public int TotalFrameCount(uint framerate)
        {
            return (int)(framerate * Length / 1000);
        }
        #endregion

        #region Animation Generation
        /// <summary>
        /// Enumerates Frames of this StateTimeline as IViedeoframes
        /// </summary>
        /// <param name="args">The GenerationArgs. </param>
        /// <param name="max_threads">The maximum number of threads to use. If -1 -> uses maximum number available. If 0 or 1 -> uses single thread algorithm. If > 1 uses at most that many threads. </param>
        /// <returns></returns>
        private IEnumerable<IVideoFrame> EnumerateFrames(StateOptions args, int max_threads)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate;

            // loops through the frames
            List<StateEvent> activeEvents = new List<StateEvent>();
            ulong frames = framerate * Length / 1000;
            for (uint i = 0; i < frames; i++)
            {
                // Get Time in Seconds and milliseconds
                double second = Convert.ToDouble(i) / Convert.ToDouble(framerate);
                uint time = Convert.ToUInt32(second * 1000);

                // Add events triggered on this tick to active events
                while (Events.Count > 0 && Events[0].StartTime <= time)
                {
                    activeEvents.Add(Events[0]);
                    Events.RemoveAt(0);
                }

                // Make a list with all current states of the active events
                List<State> states = new List<State>();
                for (int j = 0; j < activeEvents.Count; j++)
                {
                    StateEvent se = activeEvents[j];
                    uint activationTime = se.StartTime;

                    if (!se.IsActive(time)) { activeEvents.RemoveAt(j); }
                    states.Add(se.CurrentState(time, Base));
                }

                // Somehow combine all states into one single state
                State FrameState = new State();
                for (StateProperty j = 0; j <= StateProperty.i9; j++)
                {
                    FrameState[j] = (Base[j] + states.Sum(state => state[j])) / (states.Count + 1);
                }

                // Create Image of state
                FrameState.Name = "Frame_" + i.ToString().PadLeft(frames.ToString().Length, '0');
                if (max_threads == 0 || max_threads == 1) { yield return new BitmapVideoFrameWrapper(FrameState.GetBitmap(args, 1)); }
                else { yield return new BitmapVideoFrameWrapper(FrameState.GetBitmap(args, max_threads)); }
            }
        }

        /// <summary>
        /// Generates Animation for this StateTimeline and Saves it to a file. 
        /// </summary>
        /// <param name="args">The GenerationArgs</param>
        /// <param name="max_threads">The maximum number of threads to use. If -1 -> uses maximum number available. If 0 or 1 -> uses single thread algorithm. If > 1 uses at most that many threads. </param>
        /// <param name="type">The file type to be saved (mp4 or gif) </param>
        /// <param name="keepframes">Whether to keep single frames. Otherwise only the animation is saved. Not implemented yet -> must be false. </param>
        /// <param name="out_dir">Absolute path of a directory where to save the generated file. If this is an empty string the default output is used. </param>
        /// <returns>The absolute path of the generated file. </returns>
        /// <exception cref="DirectoryNotFoundException">thrown if out_dir doesn't exist</exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException">thrown if keepframes is true</exception>
        public async Task<string> GenerateAnimation(StateOptions args, GenerationOptions options)
        {
            string out_dir = options.OutputPath;
            AnimationFormat type = options.AnimationFormat;
            bool keepframes = options.KeepAnimationFrames;
            int max_threads = options.MaxThreads;
            
            // check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.StateTimeline.STATETIMELINE_NAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);

            // Order Events
            Events = Events.OrderBy((a) => a.StartTime).ToList();

            // set Length if Length == 0
            if (Length == 0)
            {
                Length = Events.Max(x => x.StartTime + x.Length + x.ReleaseTime) + 500;
            }

            switch (type)
            {
                case AnimationFormat.None:
                    {
                        throw new Exception("No AnimationType was specified. ");
                    }
                case AnimationFormat.Gif:
                    {
                        if (keepframes)
                        {
                            string folder = GenerateFrames(args, max_threads, file_path_out);
                            await CreateGif(args, folder);
                        }
                        else
                        {
                            await CreateGif(args, max_threads, file_path_out);
                        }
                        return file_path_out + @".gif";
                    }
                case AnimationFormat.Mp4:
                    {
                        if (keepframes)
                        {
                            string folder = GenerateFrames(args, max_threads, file_path_out);
                            await CreateMp4(args, folder);
                        }
                        else
                        {
                            await CreateMp4(args, max_threads, file_path_out);
                        }
                        return file_path_out + @".mp4";
                    }
                default:
                    {
                        throw new Exception("Unrecognized AnimationType");
                    }
            }
        }

        /// <summary>
        /// Create Animation for this StateTimeline and Saves it to a mp4 file. 
        /// </summary>
        /// <param name="args">The GenerationArgs</param>
        /// <param name="max_threads">The maximum number of threads to use. If -1 -> uses maximum number available. If 0 or 1 -> uses single thread algorithm. If > 1 uses at most that many threads. </param>
        /// <param name="absolute_out_filepath">Absolute path to file that shall be generated. </param>
        /// <returns></returns>
        /// <exception cref="Exception">If generation fails</exception>
        private async Task CreateMp4(StateOptions args, int max_threads, string absolute_out_filepath)
        {
            // parsing framerate and setting piping source
            uint framerate = args.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(args, max_threads))
            {
                FrameRate = framerate, // set source frame rate
            };

            // parsing size
            System.Drawing.Size size = new System.Drawing.Size(args.Width, args.Height);

            // generate the mp4 file
            try
            {
                await FFMpegArguments
                .FromPipeInput(videoFramesSource)
                .OutputToFile(absolute_out_filepath + @".mp4", false, options => options
                    .WithVideoCodec(VideoCodec.LibX265)
                    // .WithVideoBitrate(16000) // find a balance between quality and file size
                    .WithFramerate(framerate))
                .ProcessAsynchronously();
            }
            catch (Exception e)
            {
                throw new Exception("Error generating animation. ", e);
            }
        }

        /// <summary>
        /// Create Animation for this StateTimeline and Saves it to a gif file. 
        /// </summary>
        /// <param name="args">The GenerationArgs</param>
        /// <param name="max_threads">The maximum number of threads to use. If -1 -> uses maximum number available. If 0 or 1 -> uses single thread algorithm. If > 1 uses at most that many threads. </param>
        /// <param name="absolute_out_filepath">Absolute path to file that shall be generated. </param>
        /// <returns></returns>
        /// <exception cref="Exception">If generation fails</exception>
        private async Task CreateGif(StateOptions args, int max_threads, string absolute_out_filepath)
        {
            // parsing framerate and setting piping source
            uint framerate = args.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(args, max_threads))
            {
                FrameRate = framerate, // set source frame rate
            };

            // parsing size
            System.Drawing.Size size = new System.Drawing.Size(args.Width, args.Height);

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
        }

        /// <summary>
        /// Generate all frames and save them in the specified out_dir
        /// </summary>
        /// <param name="args">GenerationArgs</param>
        /// <param name="max_threads">Maximum number of threads to use. If -1 -> uses maximum number available. If 0 or 1 -> uses single thread algorithm. If > 1 uses at most that many threads. </param>
        /// <param name="out_dir">the output directory</param>
        /// <returns>returns outdir</returns>
        private string GenerateFrames(StateOptions args, int max_threads, string out_dir)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate;

            // create Directory for frames if not exist
            if (!Directory.Exists(out_dir)) { Directory.CreateDirectory(out_dir); }

            // Order Events
            List<StateEvent> event_list = Events.OrderBy(a => a.StartTime).ToList();

            // set Length if Length == 0
            if (Length == 0)
            {
                Length = Events.Max(x => x.StartTime + x.Length + x.ReleaseTime) + 1000;
            }

            // Initiate List for active states
            List<StateEvent> activeEvents = new List<StateEvent>();

            // iterate over all frames
            int frames = TotalFrameCount(framerate);
            for (int frame = 0; frame < frames; frame++)
            {
                // Get Time in Milliseconds
                uint time = (uint)((double)frame / framerate * 1000);
                // Console.WriteLine(time);
                // Add events triggered on this tick to active events
                while (event_list.Count > 0 && event_list[0].StartTime <= time)
                {
                    activeEvents.Add(event_list[0]);
                    event_list.RemoveAt(0);
                }

                // Make a list with all current states of the active events
                List<State> states = new List<State>();
                for (int j = 0; j < activeEvents.Count;)
                {
                    StateEvent se = activeEvents[j];
                    if (se.IsActive(time))
                    {
                        // Console.WriteLine(time);
                        states.Add(se.CurrentState(time, Base));
                    }
                    else
                    {
                        activeEvents.RemoveAt(j);
                        continue;
                    }
                    j++;
                }

                // Somehow combine all states into one single state
                State FrameState = new State();

                for (StateProperty j = 0; j <= StateProperty.i9; j++)
                {
                    if (states.Count == 0) { FrameState[j] = Base[j]; }
                    else { FrameState[j] = states.Sum(state => state[j]) - Base[j] * (states.Count - 1); }
                }

                // Create Image of state
                FrameState.Name = "Frame_" + frame.ToString().PadLeft(frames.ToString().Length, '0');
                FrameState.GenerateImage(args, max_threads, out_dir);
            }

            return out_dir;
        }

        /// <summary>
        /// Create Animation after having generated all frames beforehand and save as gif
        /// </summary>
        /// <param name="args">The GenerationArgs</param>
        /// <param name="folder">The absolute path to folder where the generated Scenes are</param>
        private async Task CreateGif(StateOptions args, string folder)
        {
            // Creating the image list
            List<string> imgPaths = Directory.GetFiles(folder).ToList();
            // Enumerater for image files
            IEnumerable<IVideoFrame> EnumerateFrames()
            {
                // loops through the all img paths
                for (int i = 0; i < imgPaths.Count; i++)
                {
                    yield return new BitmapVideoFrameWrapper(new Bitmap(imgPaths[i]));
                }
            }

            uint framerate = args.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames())
            {
                FrameRate = framerate, // set source frame rate
            };

            // parsing size
            System.Drawing.Size size = new System.Drawing.Size(args.Width, args.Height);

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
        }

        /// <summary>
        /// Create Animation after having generated all frames beforehand and save as mp4
        /// </summary>
        /// /// <param name="args">The GenerationArgs</param>
        /// <param name="folder">The absolute path to folder where the generated Scenes are</param>
        private async Task CreateMp4(StateOptions args, string folder)
        {
            // Creating the image list
            List<string> imgPaths = Directory.GetFiles(folder).ToList();

            // Enumerater for image files
            IEnumerable<IVideoFrame> EnumerateFrames()
            {
                // loops through the all img paths
                for (int i = 0; i < imgPaths.Count; i++)
                {
                    yield return new BitmapVideoFrameWrapper(new Bitmap(imgPaths[i]));
                }
            }

            uint framerate = args.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames())
            {
                FrameRate = framerate, // set source frame rate
            };

            // generate the mp4 file
            try
            {
                await FFMpegArguments
                .FromPipeInput(videoFramesSource)
                .OutputToFile(folder + @".mp4", false, options => options
                    .WithVideoCodec(VideoCodec.LibX265)
                    // .WithVideoBitrate(16000) // find a balance between quality and file size
                    .WithFramerate(framerate))
                .ProcessAsynchronously();
            }
            catch (Exception e)
            {
                throw new Exception("Error generating animation. ", e);
            }
        }

        #endregion

        #region json
        /// <summary>
        /// Returns true if the passed JsonElement is a valid StateTimeline representation
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Load StateTimeline properties from Json
        /// </summary>
        /// <param name="element">Json Element for GenerationOption</param>
        /// /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void LoadJson(JsonElement element, StateOptions opts)
        {
            foreach (JsonProperty jTLineProp in element.EnumerateObject())
            {
                switch (jTLineProp.Name)
                {
                    case nameof(Name):
                        Name = jTLineProp.Value.GetString();
                        break;
                    case nameof(Length):
                        Length = jTLineProp.Value.GetUInt32();
                        break;
                    case nameof(Base):
                        Base = State.FromJson(jTLineProp.Value, opts);
                        break;
                    case nameof(Events):
                        Events.Clear();
                        foreach (JsonElement jEvent in jTLineProp.Value.EnumerateArray())
                        {
                            StateEvent sEvent = StateEvent.FromJson(jEvent, opts);
                            Events.Add(sEvent);
                        }
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{jTLineProp.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Load StateTimeline from Json
        /// </summary>
        /// <param name="element">Json Element for GenerationOption</param>
        /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public static StateTimeline FromJson(JsonElement element, StateOptions opts)
        {
            StateTimeline stateTimeline = new StateTimeline();
            stateTimeline.LoadJson(element, opts);

            return stateTimeline;
        }
        #endregion
    }

    /// <summary>
    /// The StateEvent with Envelope Properties
    /// </summary>
    public class StateEvent : IndexableBase
    {
        #region Properties
        // All times in milliseconds!

        /// <summary>
        /// StartTime of Event in Milliseconds
        /// </summary>
        public uint StartTime { get; set; }

        /// <summary>
        /// Length of Event in Milliseconds
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// AttackTime in Milliseconds
        /// </summary>
        public uint AttackTime { get; set; }

        /// <summary>
        /// Easing Function the Attack uses
        /// </summary>
        [JsonIgnore]
        public Easing AttackEasing { get; set; }
        public EasingType AttackEasingType { get => AttackEasing.Type; set => AttackEasing = Easing.FromType(value); }
        
        /// <summary>
        /// DecayTime in Millisecond
        /// </summary>
        public uint DecayTime { get; set; }
        
        /// <summary>
        /// Easing Function the Decay uses
        /// </summary>
        [JsonIgnore]
        public Easing DecayEasing { get; set; }
        public EasingType DecayEasingType { get => DecayEasing.Type; set => DecayEasing = Easing.FromType(value); }
        
        /// <summary>
        /// ReleaseTime in Milliseconds
        /// </summary>
        public uint ReleaseTime { get; set; }
        
        /// <summary>
        /// Easing Function the Release uses
        /// </summary>
        [JsonIgnore]
        public Easing ReleaseEasing { get; set; }
        public EasingType ReleaseEasingType { get => ReleaseEasing.Type; set => ReleaseEasing = Easing.FromType(value); }

        /// <summary>
        /// Dictionary of Peak Values
        /// </summary>
        public Dictionary<StateProperty, double> PeakValues { get; set; }

        /// <summary>
        /// Dictionary of Susatain Values
        /// </summary>
        public Dictionary<StateProperty, double> SustainValues { get; set; }
        #endregion

        [JsonIgnore]
        private State pre_release_state;

        /// <summary>
        /// Creates a Standard StateEvent
        /// </summary>
        public StateEvent()
        {
            PeakValues = new Dictionary<StateProperty, double>();
            SustainValues = new Dictionary<StateProperty, double>();

            StartTime = 0;
            Length = 0;
            AttackTime = 0;
            AttackEasing = Easing.Linear();
            DecayTime = 0;
            DecayEasing = Easing.Linear();
            ReleaseTime = 0;
            ReleaseEasing = Easing.Linear();

            pre_release_state = new State();
        }

        /// <summary>
        /// Gets the Current State of this StateEvent
        /// </summary>
        /// <param name="currentTime">The Current Time (in Milliseconds)</param>
        /// <param name="BaseState">The BaseState</param>
        /// <returns>State</returns>
        /// <exception cref="Exception">if Activation Time is before Current time</exception>
        public State CurrentState(uint currentTime, State BaseState)
        {
            // Console.WriteLine(currentTime);
            long t_active = currentTime - StartTime;
            if (t_active < 0) { throw new Exception(); }

            State result = new State();
            // Attack
            if (t_active < Length && t_active < AttackTime)
            {
                Easing easing = AttackEasing;
                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    if (!PeakValues.ContainsKey(i)) { result[i] = BaseState[i]; }
                    else { result[i] = easing.Ease(BaseState[i], PeakValues[i], Convert.ToDouble(t_active)/(Convert.ToDouble(AttackTime))); }
                }

                pre_release_state = result;
            }
            // Decay
            else if (t_active < Length && t_active < AttackTime + DecayTime)
            {
                Easing easing = DecayEasing;
                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    if (!SustainValues.ContainsKey(i) && !PeakValues.ContainsKey(i))
                    {
                        result[i] = BaseState[i];
                    }
                    else
                    {
                        result[i] = easing.Ease(
                            PeakValues.ContainsKey(i) ? PeakValues[i] : BaseState[i],
                            SustainValues.ContainsKey(i) ? SustainValues[i] : BaseState[i],
                            Convert.ToDouble(t_active - AttackTime)/Convert.ToDouble(DecayTime));
                    }
                }

                pre_release_state = result;
            }
            // Sustain
            else if (t_active < Length)
            {
                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    result[i] = SustainValues.ContainsKey(i) ? SustainValues[i] : BaseState[i];
                }

                pre_release_state = result;
            }
            // Release
            else if (t_active > Length && t_active < AttackTime + Length + ReleaseTime)
            {
                Easing easing = ReleaseEasing;
                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    result[i] = easing.Ease(
                        pre_release_state[i],
                        BaseState[i],
                        Convert.ToDouble(t_active - AttackTime - Length)/Convert.ToDouble(ReleaseTime));
                }
            }
            else
            {
                // Console.WriteLine(currentTime);

                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    result[i] = BaseState[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Whether this StateEvent is still Active
        /// </summary>
        /// <param name="currentTime">The Current Time in Milliseconds</param>
        /// <returns>bool</returns>
        /// <exception cref="Exception">If Activation Time is after Current Time</exception>
        public bool IsActive(uint currentTime)
        {
            long t_active = currentTime - StartTime;
            if (t_active < 0) { return false; }

            if (t_active >= Length + ReleaseTime) { return false; }
            else { return true; }
        }

        #region json
        /// <summary>
        /// Returns true if the passed JsonElement is a valid StateEvent representation
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Load StateEvent properties from Json
        /// </summary>
        /// <param name="element">Json Element for GenerationOption</param>
        /// /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void LoadJson(JsonElement element, StateOptions opts)
        {
            foreach (JsonProperty jSEventProp in element.EnumerateObject())
            {
                switch (jSEventProp.Name)
                {
                    case nameof(StartTime):
                    case nameof(Length):
                    case nameof(AttackTime):
                    case nameof(DecayTime):
                    case nameof(ReleaseTime):
                        this[jSEventProp.Name] = jSEventProp.Value.GetUInt32();
                        break;
                    case nameof(AttackEasingType):
                    case nameof(DecayEasingType):
                    case nameof(ReleaseEasingType):
                        this[jSEventProp.Name] = Enum.Parse<EasingType>(jSEventProp.Value.GetString());
                        break;
                    case nameof(PeakValues):
                        PeakValues.Clear();
                        foreach (JsonProperty jEventval in jSEventProp.Value.EnumerateObject())
                        {
                            if (jEventval.Name == nameof(State.Parameters))
                            {
                                int i = 0;
                                foreach(JsonElement para in jEventval.Value.EnumerateArray())
                                {
                                    PeakValues.Add(Enum.Parse<StateProperty>("i" + i.ToString()), para.GetDouble());
                                    i++;
                                }
                            }
                            else
                            {
                                PeakValues.Add(Enum.Parse<StateProperty>(jEventval.Name), jEventval.Value.GetDouble());
                            }
                        }
                        break;
                    case nameof(SustainValues):
                        SustainValues.Clear();
                        foreach (JsonProperty jEventval in jSEventProp.Value.EnumerateObject())
                        {
                            if (jEventval.Name == nameof(State.Parameters))
                            {
                                int i = 0;
                                foreach (JsonElement para in jEventval.Value.EnumerateArray())
                                {
                                    SustainValues.Add(Enum.Parse<StateProperty>("i" + i.ToString()), para.GetDouble());
                                    i++;
                                }
                            }
                            else
                            {
                                SustainValues.Add(Enum.Parse<StateProperty>(jEventval.Name), jEventval.Value.GetDouble());
                            }
                        }
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{jSEventProp.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Load StateEvent from Json
        /// </summary>
        /// <param name="element">Json Element for GenerationOption</param>
        /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public static StateEvent FromJson(JsonElement element, StateOptions opts)
        {
            StateEvent stateEvent = new StateEvent();
            stateEvent.LoadJson(element, opts);

            return stateEvent;
        }
        #endregion
    }
}
