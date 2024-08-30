using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnimatedGif;
using System.Text.Json;
using System.Text.Json.Serialization;
using FFMpegCore.Pipes;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Extensions.SkiaSharp;
using SkiaSharp;
using System.Threading.Tasks;
using Modulartistic.Drawing;
using System.Reflection;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using Modulartistic.Common;

namespace Modulartistic.Core
{
    /// <summary>
    /// Represents a sequence of <see cref="Scene"/> objects in an animation or graphical sequence. A StateSequence contains multiple <see cref="Scene"/> objects containing <see cref="State"/>s and provides functionality to generate frames and animations between these.
    /// </summary>
    public class StateSequence
    {
        #region Fields & properties

        /// <summary>
        /// Gets or sets the name of this state sequence.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the name of the state sequence.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of scenes in this state sequence.
        /// </summary>
        /// <value>
        /// A <see cref="List{Scene}"/> containing the scenes in this state sequence.
        /// </value>
        public List<Scene> Scenes { get; set; }

        /// <summary>
        /// Gets the number of scenes in this state sequence.
        /// </summary>
        /// <value>
        /// An <see cref="int"/> representing the number of scenes.
        /// </value>
        [JsonIgnore]
        public int Count
        {
            get => Scenes.Count;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateSequence"/> class with the specified scenes and name.
        /// </summary>
        /// <param name="sequence">An enumerable of <see cref="Scene"/> objects representing the scenes in this state sequence.</param>
        /// <param name="name">The name of this state sequence. Defaults to an empty string.</param>
        public StateSequence(IEnumerable<Scene> sequence, string name = "")
        {
            Scenes = sequence.ToList();
            Name = name == "" ? Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT : name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateSequence"/> class with the specified name and an empty list of scenes.
        /// </summary>
        /// <param name="name">The name of this state sequence.</param>
        public StateSequence(string name) : this()
        {
            Name = name == "" ? Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT : name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateSequence"/> class with an empty list of scenes and the default name.
        /// </summary>
        public StateSequence()
        {
            Scenes = new List<Scene>();
            Name = Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT;
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
            int total = 0;
            for (int i = 0; i < Count; i++)
            {
                total += (int)(Scenes[i].Length * framerate);
            }
            return total;
        }

        /// <summary>
        /// Calculates the total length of this state sequence in seconds.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> representing the total length of the state sequence in seconds.
        /// </returns>
        public double LengthInSeconds()
        {
            double total = 0;
            for (int i = 0; i < Count; i++)
            {
                total += Scenes[i].Length;
            }
            return total;
        }

        /// <summary>
        /// Gets the number of frames for the <see cref="Scene"/> at a specific index based on the specified framerate.
        /// </summary>
        /// <param name="idx">The index of the scene in the sequence.</param>
        /// <param name="framerate">The framerate (frames per second) for the animation.</param>
        /// <returns>
        /// An <see cref="int"/> representing the number of frames for the scene at the specified index.
        /// </returns>
        public int Framecount(int idx, int framerate)
        {
            return (int)Scenes[idx].Length * framerate;
        }

        #endregion

        #region Animation Generation

        /// <summary>
        /// Enumerates the frames of this state sequence as <see cref="IVideoFrame"/> objects.
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

            Progress? sequenceProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Generating scenes of {Name}", Count);
            // loops through the scenes
            for (int i = 0; i < Count; i++)
            {
                Scene current = Scenes[i];
                Scene next = Scenes[(i + 1) % Scenes.Count];

                // iterate over all Frames and create the corresponding images
                int frames = (int)(current.Length * framerate);
                Progress? sceneProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Generating frames for scene {i + 1}", frames);
                for (int j = 0; j < frames; j++)
                {
                    State frameState = new State(current.State, next.State, current.Easing, j, frames);
                    sceneProgress?.IncrementProgress();
                    yield return new BitmapVideoFrameWrapper(frameState.GetBitmap(args, parameters, options));
                }
                options.ProgressReporter?.RemoveTask(sceneProgress);
                sequenceProgress?.IncrementProgress();
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

            // create Directory for frames if not exist
            if (!Directory.Exists(frames_dir)) { Directory.CreateDirectory(frames_dir); }

            Progress? sequenceProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid().ToString()}", $"Generating scenes of {Name}", Count);
            // loops through the scenes
            for (int i = 0; i < Count; i++)
            {
                Scene current = Scenes[i];
                Scene next = Scenes[(i + 1) % Scenes.Count];

                // creates the directory for the scene
                string scene_out_dir = Helper.GetValidFileName(Path.Combine(frames_dir, current.State.Name == "" ? Constants.StateSequence.SCENE_NAME_DEFAULT : current.State.Name));
                if (!Directory.Exists(scene_out_dir)) { Directory.CreateDirectory(scene_out_dir); }

                // iterate over all Frames and create the corresponding images
                int frames = (int)(current.Length * framerate);
                Progress? sceneProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid().ToString()}", $"Generating frames for scene {i + 1}", frames);
                for (int j = 0; j < frames; j++)
                {
                    State frameState = new State(current.State, next.State, current.Easing, j, frames);
                    frameState.GenerateImage(args, parameters, options, scene_out_dir);
                    sceneProgress?.IncrementProgress();
                }
                options.ProgressReporter?.RemoveTask(sceneProgress);

                sequenceProgress?.IncrementProgress();
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
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(args, parameters,options))
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
            List<string> imgPaths = new List<string>();
            List<string> sceneDirs = new List<string>();

            // loop through all Scenes to get all images in the correct order
            for (int i = 0; i < Count; i++)
            {
                // Define the scenDir of the current scene
                string sceneDir = Path.Combine(folder, Scenes[i].State.Name == "" ? Constants.StateSequence.SCENE_NAME_DEFAULT : Scenes[i].State.Name);

                // In case of identically named Scenes convert Name to Name_n
                if (sceneDirs.Contains(sceneDir))
                {
                    int j;
                    for (j = 1; sceneDirs.Contains(sceneDir + "_" + j); j++) { }
                    sceneDir = sceneDir + "_" + j;
                }

                // Add sceneDir to the List and Get Images from sceneDir
                sceneDirs.Add(sceneDir);
                imgPaths.AddRange(Directory.GetFiles(sceneDir));
            }

            // Enumerater for image files
            IEnumerable<IVideoFrame> EnumerateFrames()
            {
                // loops through the all img paths
                Progress? joinProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid().ToString()}", $"Joining frames of {Name}", imgPaths.Count);
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
                    case nameof(Scenes):
                        Scenes.Clear();
                        foreach (JsonElement jScene in jSeqProp.Value.EnumerateArray())
                        {
                            Scene scene = Scene.FromJson(jScene, opts);
                            Scenes.Add(scene);
                        }
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{jSeqProp.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="StateSequence"/> instance from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the state sequence data.</param>
        /// <param name="opts">The <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <returns>
        /// A new <see cref="StateSequence"/> instance populated with data from the JSON element.
        /// </returns>
        /// <exception cref="KeyNotFoundException">Thrown if a property is not found in the JSON element.</exception>
        public static StateSequence FromJson(JsonElement element, StateOptions opts)
        {
            StateSequence stateSequence = new StateSequence();
            stateSequence.LoadJson(element, opts);

            return stateSequence;
        }
        #endregion
    }

    /// <summary>
    /// Represents a scene in an <see cref="StateSequence"/> animation . A scene includes the starting state, the duration of the scene, and an easing function to transition to the next scene.
    /// </summary>
    public class Scene
    {
        #region Properties

        /// <summary>
        /// Gets or sets the starting state of this scene.
        /// </summary>
        /// <value>
        /// The <see cref="Core.State"/> object representing the beginning state of the scene.
        /// </value>
        public State State { get; set; }

        /// <summary>
        /// Gets or sets the length of this scene in seconds. This determines the duration before transitioning to the next scene.
        /// </summary>
        /// <value>
        /// A <see cref="double"/> representing the length of the scene in seconds.
        /// </value>
        public double Length { get; set; }

        /// <summary>
        /// Gets the easing function used to transform this scene to the next scene.
        /// </summary>
        /// <value>
        /// An <see cref="Core.Easing"/> object representing the easing function.
        /// </value>
        [JsonIgnore]
        public Easing Easing { get; private set; }

        /// <summary>
        /// Gets or sets the type of easing function used to transition to the next scene.
        /// </summary>
        /// <value>
        /// An <see cref="Core.EasingType"/> enumeration value representing the type of easing function.
        /// </value>
        public EasingType EasingType 
        { 
            get => Easing.Type; 
            set => Easing = Easing.FromType(value); 
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class with default values.
        /// </summary>
        public Scene()
        {
            State = new State();
            Length = 1;
            EasingType = EasingType.Linear;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class with the specified state, length, and easing function.
        /// </summary>
        /// <param name="state">The starting <see cref="Core.State"/> of this scene.</param>
        /// <param name="length">The length of this scene in seconds.</param>
        /// <param name="easing">The <see cref="Core.Easing"/> function used to transform this scene to the next scene.</param>
        public Scene(State state, double length, Easing easing)
        {
            State = state;
            Length = length;
            Easing = easing;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class with the specified state, length, and easing function.
        /// </summary>
        /// <param name="state">The starting <see cref="Core.State"/> of this scene.</param>
        /// <param name="length">The length of this scene in seconds.</param>
        /// <param name="easingType">The <see cref="Core.EasingType"/> used to transform this scene to the next scene.</param>
        public Scene(State state, double length, EasingType easingType)
        {
            State = state;
            Length = length;
            EasingType = easingType;
        }

        #endregion

        #region JSON Methods

        /// <summary>
        /// Validates whether the given <see cref="JsonElement"/> represents a valid <see cref="Scene"/> representation.
        /// </summary>
        /// <param name="element">The <see cref="JsonElement"/> to validate.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="element"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Loads the scene properties from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the scene properties.</param>
        /// <param name="opts">The current <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if a property in the JSON element does not exist on the <see cref="Scene"/> type.
        /// </exception>
        public void LoadJson(JsonElement element, StateOptions opts)
        {
            foreach (JsonProperty jSceneProp in element.EnumerateObject())
            {
                switch (jSceneProp.Name)
                {
                    case nameof(State):
                        this.State = State.FromJson(jSceneProp.Value, opts);
                        break;
                    case nameof(Length):
                        this.Length = jSceneProp.Value.GetDouble();
                        break;
                    case nameof(EasingType):
                        this.EasingType = Enum.Parse<EasingType>(jSceneProp.Value.GetString());
                        break;
                    default:
                        throw new KeyNotFoundException($"Property '{jSceneProp.Name}' does not exist on type '{GetType().Name}'.");
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="Scene"/> instance from the specified JSON element.
        /// </summary>
        /// <param name="element">The JSON element containing the scene properties.</param>
        /// <param name="opts">The current <see cref="StateOptions"/> used for evaluating state properties.</param>
        /// <returns>
        /// A new <see cref="Scene"/> instance populated with the data from the JSON element.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if a property in the JSON element does not exist on the <see cref="Scene"/> type.
        /// </exception>
        public static Scene FromJson(JsonElement element, StateOptions opts)
        {
            Scene scene = new Scene();
            scene.LoadJson(element, opts);

            return scene;
        }
        
        #endregion
    }
}