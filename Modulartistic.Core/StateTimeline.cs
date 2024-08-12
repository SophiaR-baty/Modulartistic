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
using Modulartistic.Common;

namespace Modulartistic.Core
{
    /// <summary>
    /// StateTimeline consisting of a Base State and several timed StateEvents
    /// </summary>
    public class StateTimeline
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of this state timeline.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the name of the state sequence.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Length of this state timeline in milliseconds.
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// Gets or sets the Base State of this state timeline
        /// </summary>
        public State Base { get; set; }

        /// <summary>
        /// Gets or sets a List of <see cref="StateEvent"/> objects of this state timeline
        /// </summary>
        public List<StateEvent> Events { get; set; }

        /// <summary>
        /// Gets the Length of this state timeline in seconds.
        /// </summary>
        [JsonIgnore]
        public double LengthInSeconds { get => Length / 1000.0; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTimeline"/> class with zero duration an empty list of <see cref="StateEvent"/> objects, a default Base <see cref="State"/> and a specified name
        /// </summary>
        /// <param name="name">The name of this state sequence.</param>
        public StateTimeline(string name) : this()
        {
            Name = name == "" ? Constants.StateTimeline.STATETIMELINE_NAME_DEFAULT : name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTimeline"/> class with zero duration an empty list of <see cref="StateEvent"/> objects, a default Base <see cref="State"/> and the default name.
        /// </summary>
        public StateTimeline()
        {
            Name = Constants.StateTimeline.STATETIMELINE_NAME_DEFAULT;
            Length = 0;
            Base = new State();
            Events = new List<StateEvent>();
        }

        #endregion

        #region Methods to get lengths

        /// <summary>
        /// Calculates the total number of frames that will be created for the animation based on the specified framerate.
        /// </summary>
        /// <param name="framerate">The framerate (frames per second) for the animation.</param>
        /// <returns>
        /// An <see cref="int"/> representing the total number of frames.
        /// </returns>
        public int TotalFrameCount(uint framerate)
        {
            return (int)(framerate * Length / 1000);
        }

        #endregion

        #region Animation Generation

        /// <summary>
        /// Enumerates the frames of this state timline as <see cref="IVideoFrame"/> objects.
        /// </summary>
        /// <param name="args">The <see cref="StateOptions"/> used for frame generation.</param>
        /// <param name="options">The <see cref="GenerationOptions"/> used for frame generation.</param>
        /// <returns>
        /// An <see cref="IEnumerable{IVideoFrame}"/> that represents the frames of the animation.
        /// </returns>
        private IEnumerable<IVideoFrame> EnumerateFrames(StateOptions args, GenerationOptions options)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate;

            // loops through the frames
            List<StateEvent> activeEvents = new List<StateEvent>();
            ulong frames = framerate * Length / 1000;
            Progress? timelineProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Generating scenes of {Name}", frames);
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
                timelineProgress?.IncrementProgress();
                yield return new BitmapVideoFrameWrapper(FrameState.GetBitmap(args, options));
            }
            options.ProgressReporter?.RemoveTask(timelineProgress);
        }

        /// <summary>
        /// Generates all frames and saves them in the specified output directory.
        /// </summary>
        /// <param name="args">The <see cref="StateOptions"/> used for frame generation.</param>
        /// <param name="options">The <see cref="GenerationOptions"/> used for frame generation.</param>
        /// <param name="frames_dir">The directory where the frames will be saved.</param>
        /// <returns>
        /// The path to the directory where the frames were saved.
        /// </returns>
        private string GenerateFrames(StateOptions args, GenerationOptions options, string frames_dir)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate;

            // create Directory for frames if not exist
            if (!Directory.Exists(frames_dir)) { Directory.CreateDirectory(frames_dir); }

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
            Progress? framesProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Generating frames of {Name}", frames);
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
                FrameState.GenerateImage(args, options, frames_dir);
                framesProgress?.IncrementProgress();
            }
            options.ProgressReporter?.RemoveTask(framesProgress);

            return frames_dir;
        }

        /// <summary>
        /// Generates Frames, creates an animation from the generated frames and saves it to the specified file.
        /// </summary>
        /// <param name="args">The <see cref="StateOptions"/> used for frame generation.</param>
        /// <param name="options">The <see cref="GenerationOptions"/> used for animation creation.</param>
        /// <param name="type">The format of the animation to be created (e.g., GIF or MP4).</param>
        /// <param name="absolute_out_filepath">The absolute path to the file where the animation will be saved.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="Exception">Thrown if an error occurs during animation generation.</exception>
        private async Task CreateAnimation(StateOptions args, GenerationOptions options, AnimationFormat type, string absolute_out_filepath)
        {
            // parsing framerate and setting piping source
            uint framerate = args.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(args, options))
            {
                FrameRate = framerate, // set source frame rate
            };

            switch (type)
            {
                case AnimationFormat.Gif:
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
                    break;
                case AnimationFormat.Mp4:
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
                    break;
                default:
                    throw new NotImplementedException("only gif and mp4 are supported for animation cration");
            }
        }

        /// <summary>
        /// Creates an animation from previously generated frames located in the specified folder.
        /// </summary>
        /// <param name="args">The <see cref="StateOptions"/> used for frame generation.</param>
        /// <param name="options">The <see cref="GenerationOptions"/> used for animation creation.</param>
        /// <param name="type">The format of the animation to be created (e.g., GIF or MP4).</param>
        /// <param name="folder">The folder containing the generated frames.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        private async Task CreateAnimationFromFolder(StateOptions args, GenerationOptions options, AnimationFormat type, string folder)
        {
            // Creating the image list
            List<string> imgPaths = Directory.GetFiles(folder).ToList();
            // Enumerater for image files
            IEnumerable<IVideoFrame> EnumerateFrames()
            {
                // loops through the all img paths
                Progress? joinProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Joining frames of {Name}", imgPaths.Count);
                for (int i = 0; i < imgPaths.Count; i++)
                {
                    joinProgress?.IncrementProgress();
                    yield return new BitmapVideoFrameWrapper(new Bitmap(imgPaths[i]));
                }
                options.ProgressReporter?.RemoveTask(joinProgress);
            }

            uint framerate = args.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames())
            {
                FrameRate = framerate, // set source frame rate
            };

            switch (type)
            {
                case AnimationFormat.Gif:
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
                    break;
                case AnimationFormat.Mp4:
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
                    break;
                default:
                    throw new NotImplementedException("only gif and mp4 are supported for animation cration");
            }
        }

        /// <summary>
        /// Generates an animation and saves it to the specified output directory.
        /// </summary>
        /// <param name="args">The <see cref="StateOptions"/> used for frame generation and animation creation.</param>
        /// <param name="options">The <see cref="GenerationOptions"/> used for animation creation.</param>
        /// <param name="out_dir">The directory where the animation will be saved.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. Returns the path to the generated animation file.
        /// </returns>
        public async Task<string> GenerateAnimation(StateOptions args, GenerationOptions options, string out_dir)
        {
            // check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // parse options
            bool keepframes = options.KeepAnimationFrames;
            AnimationFormat type = options.AnimationFormat;

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.GetValidFileName(file_path_out);

            if (keepframes)
            {
                // the folder where the frames are saved
                string folder = GenerateFrames(args, options, file_path_out);
                await CreateAnimationFromFolder(args, options, type, folder);
            }
            else
            {
                await CreateAnimation(args, options, type, file_path_out);
            }

            return file_path_out + $".{Helper.GetAnimationFormatExtension(type)}";
        }

        #endregion

        #region JSON Methods

        /// <summary>
        /// Determines whether the specified JSON element is a valid representation of a <see cref="StateTimeline"/>.
        /// </summary>
        /// <param name="element">The JSON element to validate.</param>
        /// <returns>
        /// <c>true</c> if the JSON element is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Loads the properties of this <see cref="StateTimeline"/> from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the state timeline data.</param>
        /// <param name="opts">The <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <exception cref="KeyNotFoundException">Thrown if a property is not found in the JSON element.</exception>
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
        /// Creates a new <see cref="StateTimeline"/> instance from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the state timeline data.</param>
        /// <param name="opts">The <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <returns>
        /// A new <see cref="StateSequence"/> instance populated with data from the JSON element.
        /// </returns>
        /// <exception cref="KeyNotFoundException">Thrown if a property is not found in the JSON element.</exception>
        public static StateTimeline FromJson(JsonElement element, StateOptions opts)
        {
            StateTimeline stateTimeline = new StateTimeline();
            stateTimeline.LoadJson(element, opts);

            return stateTimeline;
        }
        
        #endregion
    }

    /// <summary>
    /// Represents an event that can occur at a specific time with various attributes like attack, decay, and release times.
    /// </summary>
    public class StateEvent : IndexableBase
    {
        #region Properties and fields

        // All times in milliseconds!

        /// <summary>
        /// Start time of the event in milliseconds.
        /// </summary>
        public uint StartTime { get; set; }

        /// <summary>
        /// Duration of the event in milliseconds.
        /// </summary>
        public uint Length { get; set; }


        /// <summary>
        /// Attack time in milliseconds.
        /// </summary>
        public uint AttackTime { get; set; }

        /// <summary>
        /// Easing function used during the attack phase.
        /// </summary>
        [JsonIgnore]
        public Easing AttackEasing { get; set; }
        /// <summary>
        /// Easing type used during the attack phase.
        /// </summary>
        public EasingType AttackEasingType { get => AttackEasing.Type; set => AttackEasing = Easing.FromType(value); }


        /// <summary>
        /// Decay time in milliseconds.
        /// </summary>
        public uint DecayTime { get; set; }

        /// <summary>
        /// Easing function used during the decay phase.
        /// </summary>
        [JsonIgnore]
        public Easing DecayEasing { get; set; }

        /// <summary>
        /// Easing type used during the decay phase.
        /// </summary>
        public EasingType DecayEasingType { get => DecayEasing.Type; set => DecayEasing = Easing.FromType(value); }


        /// <summary>
        /// Release time in milliseconds.
        /// </summary>
        public uint ReleaseTime { get; set; }

        /// <summary>
        /// Easing function used during the release phase.
        /// </summary>
        [JsonIgnore]
        public Easing ReleaseEasing { get; set; }

        /// <summary>
        /// Easing type used during the release phase.
        /// </summary>
        public EasingType ReleaseEasingType { get => ReleaseEasing.Type; set => ReleaseEasing = Easing.FromType(value); }


        /// <summary>
        /// Dictionary of peak values for state properties during the event.
        /// </summary>
        public Dictionary<StateProperty, double> PeakValues { get; set; }

        /// <summary>
        /// Dictionary of sustain values for state properties during the event.
        /// </summary>
        public Dictionary<StateProperty, double> SustainValues { get; set; }

        private State pre_release_state;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateEvent"/> class with default values.
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

        #endregion

        #region Methods for Generation

        /// <summary>
        /// Calculates the current state of the event based on the provided time and base state.
        /// </summary>
        /// <param name="currentTime">The current time in milliseconds.</param>
        /// <param name="BaseState">The base state to use for calculating changes.</param>
        /// <returns>The state of the event at the given time.</returns>
        /// <exception cref="Exception">Thrown if the current time is before the start time of the event.</exception>
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
                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    result[i] = BaseState[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Determines if the event is still active at the given time.
        /// </summary>
        /// <param name="currentTime">The current time in milliseconds.</param>
        /// <returns><c>true</c> if the event is still active; otherwise, <c>false</c>.</returns>
        /// <exception cref="Exception">Thrown if the current time is before the start time of the event.</exception>
        public bool IsActive(uint currentTime)
        {
            long t_active = currentTime - StartTime;
            if (t_active < 0) { return false; }

            if (t_active >= Length + ReleaseTime) { return false; }
            else { return true; }
        }

        #endregion

        #region JSON Methods

        /// <summary>
        /// Checks if the provided JSON element is a valid representation of a <see cref="StateEvent"/>.
        /// </summary>
        /// <param name="element">The JSON element to validate.</param>
        /// <returns><c>true</c> if the JSON element is valid; otherwise, <c>false</c>.</returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Loads the properties of this <see cref="StateEvent"/> from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the event data.</param>
        /// <param name="opts">The <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <exception cref="KeyNotFoundException">Thrown if a property is not found in the JSON element.</exception>
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
        /// Creates a <see cref="StateEvent"/> instance from the provided JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the event data.</param>
        /// <param name="opts">The <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <returns>A new <see cref="StateEvent"/> instance.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a property is not found in the JSON element.</exception>
        public static StateEvent FromJson(JsonElement element, StateOptions opts)
        {
            StateEvent stateEvent = new StateEvent();
            stateEvent.LoadJson(element, opts);

            return stateEvent;
        }

        #endregion
    }
}
