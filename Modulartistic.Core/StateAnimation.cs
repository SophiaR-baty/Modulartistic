using FFMpegCore.Extensions.SkiaSharp;
using FFMpegCore.Pipes;
using FFMpegCore;
using Modulartistic.Common;
using Modulartistic.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using FFMpegCore.Enums;
using System.Globalization;

namespace Modulartistic.Core
{
    public class StateAnimation
    {
        #region Fields & properties

        // name of the animation
        private string m_name;

        // maximum duration in seconds
        private double m_duration;

        // when this condition will evaluate to true the animation ends
        private string m_end_condition;

        // the base state
        private State m_state;

        /// <summary>
        /// Gets or sets the name of this state animation.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the name of the state animation.
        /// </value>
        public string Name { get => m_name ?? ""; set => m_name = value; }

        /// <summary>
        /// Gets or sets the Maximum Duration of the state animation in seconds. Set to -1 for infinite.
        /// </summary>
        /// <value cref="double">Maximum Duration in seconds</value>
        public double MaxDuration { get => m_duration; set => m_duration = value; }

        /// <summary>
        /// Gets or sets the EndCondition of the animation. When this condition evaluates to true, the animation ends.
        /// </summary>
        /// <value cref="string">An expression to be evaluated at each frame</value>
        public string EndCondition { get => m_end_condition; set => m_end_condition = value; }

        /// <summary>
        /// Gets or sets the base state of the animation
        /// </summary>
        public State BaseState { get => m_state; set => m_state = value; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAnimation"/> class with specified values.
        /// </summary>
        public StateAnimation(State s, double max_length, string end_condition, string name = "")
        {
            m_name = name == "" ? Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT : name;
            m_duration = max_length;
            m_end_condition = end_condition;
            m_state = s;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAnimation"/> class with specified values.
        /// </summary>
        public StateAnimation(State s, string name) : this(s)
        {
            m_name = name == "" ? Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT : name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAnimation"/> class with a specified bas state.
        /// </summary>
        public StateAnimation(State s) : this()
        {
            m_state = s;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAnimation"/> class with default values.
        /// </summary>
        public StateAnimation()
        {
            m_name = Constants.StateAnimation.STATEANIMATION_NAME_DEFATULT;
            m_duration = 60.0;
            m_end_condition = "";
            m_state = new State();
        }

        #endregion

        #region Animation Generation

        /// <summary>
        /// Enumerates the frames of this state animation as <see cref="IVideoFrame"/> objects.
        /// </summary>
        /// <param name="args">The <see cref="StateOptions"/> used for frame generation.</param>
        /// <param name="options">The <see cref="GenerationOptions"/> used for frame generation.</param>
        /// <returns>
        /// An <see cref="IEnumerable{IVideoFrame}"/> that represents the frames of the animation.
        /// </returns>
        private IEnumerable<IVideoFrame> EnumerateFrames(StateOptions args, object?[] parameters, GenerationOptions options)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate;

            int max_frames = -1;
            if (m_duration > 0)
            {
                max_frames = (int)(m_duration * framerate);
            }
            int frame = 0;

            Progress? sequenceProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid().ToString()}", $"Generating scenes of {Name}", max_frames);
            while (max_frames < 0 || frame < max_frames)
            {
                State frameState = m_state;
                yield return new BitmapVideoFrameWrapper(frameState.GetBitmap(args, parameters, options, m_end_condition, out object end));

                frame++;
                sequenceProgress?.SetProgress(frame);
                if (Convert.ToBoolean(end)) { break; }
            }
            options.ProgressReporter?.RemoveTask(sequenceProgress);
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
        private string GenerateFrames(StateOptions args, object?[] parameters, GenerationOptions options, string frames_dir)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate;

            int max_frames = -1;
            if (m_duration > 0)
            {
                max_frames = (int)(m_duration * framerate);
            }
            int frame = 0;

            // create Directory for frames if not exist
            if (!Directory.Exists(frames_dir)) { Directory.CreateDirectory(frames_dir); }

            Progress? sequenceProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid().ToString()}", $"Generating scenes of {Name}", max_frames <= 0 ? 100 : max_frames);
            while (max_frames < 0 || frame < max_frames)
            {
                State frameState = m_state;
                frameState.Name = "Frame_" + frame.ToString();
                frameState.GenerateImage(args, parameters, options, frames_dir, m_end_condition, out object end);
                
                frame++;
                sequenceProgress?.SetProgress(frame % sequenceProgress ?.MaxProgress ?? 0);
                if (Convert.ToBoolean(end)) { break; }
            }
            options.ProgressReporter?.RemoveTask(sequenceProgress);

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
        private async Task CreateAnimation(StateOptions args, object?[] parameters, GenerationOptions options, AnimationFormat type, string absolute_out_filepath)
        {
            // parsing framerate and setting piping source
            uint framerate = args.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(args, parameters, options))
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
            var compareInfo = CultureInfo.CurrentCulture.CompareInfo;

            // Sort using CompareInfo to get a natural sort order
            imgPaths.Sort(new Helper.NaturalStringComparer());

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
        public async Task<string> GenerateAnimation(StateOptions args, object?[] parameters, GenerationOptions options, string out_dir)
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
                string folder = GenerateFrames(args, parameters, options, file_path_out);
                await CreateAnimationFromFolder(args, options, type, folder);
            }
            else
            {
                await CreateAnimation(args, parameters, options, type, file_path_out);
            }

            return file_path_out + $".{Helper.GetAnimationFormatExtension(type)}";
        }

        #endregion

        #region JSON Methods

        /// <summary>
        /// Determines whether the specified JSON element is a valid representation of a <see cref="StateSequence"/>.
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
        /// Loads the properties of this <see cref="StateSequence"/> from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the state sequence data.</param>
        /// <param name="opts">The <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <exception cref="KeyNotFoundException">Thrown if a property is not found in the JSON element.</exception>
        public void LoadJson(JsonElement element, StateOptions opts)
        {
            foreach (JsonProperty jSeqProp in element.EnumerateObject())
            {
                switch (jSeqProp.Name)
                {
                    case nameof(Name):
                        Name = jSeqProp.Value.GetString();
                        break;
                    case nameof(MaxDuration):
                        MaxDuration = jSeqProp.Value.GetDouble();
                        break;
                    case nameof(EndCondition):
                        EndCondition = jSeqProp.Value.GetString();
                        break;
                    case nameof(BaseState):
                        BaseState = State.FromJson(jSeqProp.Value, opts);
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{jSeqProp.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="StateAnimation"/> instance from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the state animation data.</param>
        /// <param name="opts">The <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <returns>
        /// A new <see cref="StateAnimation"/> instance populated with data from the JSON element.
        /// </returns>
        /// <exception cref="KeyNotFoundException">Thrown if a property is not found in the JSON element.</exception>
        public static StateAnimation FromJson(JsonElement element, StateOptions opts)
        {
            StateAnimation stateAnimation = new StateAnimation();
            stateAnimation.LoadJson(element, opts);

            return stateAnimation;
        }
        #endregion
    }
}
