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

namespace Modulartistic.Core
{
    /// <summary>
    /// A Sequence of States (in Scenes) to generate animation or batch images
    /// </summary>
    public class StateSequence
    {
        #region Fields & properties
        /// <summary>
        /// The Name of this State Sequence.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of Scenes.
        /// </summary>
        public List<Scene> Scenes { get; set; }

        /// <summary>
        /// Returns the Number of Scenes in this StateSequence
        /// </summary>
        [JsonIgnore]
        public int Count
        {
            get => Scenes.Count;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor of StateSequence with an Array of Scenes and a name.
        /// </summary>
        /// <param name="sequence">An Array of Scenes.</param>
        /// <param name="name">The Name of this StateSequence.</param>
        public StateSequence(Scene[] sequence, string name = "")
        {
            Scenes = sequence.ToList();
            Name = name == "" ? Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT : name;
        }
        /// <summary>
        /// Constructor for an empty StateSequence
        /// </summary>
        public StateSequence(string name) : this()
        {
            Name = name == "" ? Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT : name;
        }

        public StateSequence()
        {
            Scenes = new List<Scene>();
            Name = Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT;
        }
        #endregion

        #region Some Functions (Turn into properties maybe)
        /// <summary>
        /// Returns how many Frames will be created in total when starting the animation
        /// </summary>
        /// <param name="framerate">The framerate for which to calculate the frames</param>
        /// <returns>int</returns>
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
        /// The Total Length of this StateSequence in Seconds
        /// </summary>
        /// <returns>double, in Seconds</returns>
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
        /// Gets the amount of Frames to create for the scene at a specific index
        /// </summary>
        /// <param name="idx">the index for which to get the number of frames</param>
        /// <param name="framerate">The framerate for which to calculate</param>
        /// <returns>int</returns>
        public int Framecount(int idx, int framerate)
        {
            return (int)Scenes[idx].Length * framerate;
        }
        #endregion

        #region Animation Generation
        /// <summary>
        /// Enumerates Frames of this StateSequence as IViedeoframes
        /// </summary>
        /// <param name="args">The GenerationArgs. </param>
        /// <param name="max_threads">The maximum number of threads to use. If -1 -> uses maximum number available. If 0 or 1 -> uses single thread algorithm. If > 1 uses at most that many threads. </param>
        /// <returns></returns>
        private IEnumerable<IVideoFrame> EnumerateFrames(StateOptions args, int max_threads)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate;

            // loops through the scenes
            for (int i = 0; i < Count; i++)
            {
                Scene current = Scenes[i];
                Scene next = Scenes[(i + 1) % Scenes.Count];

                // iterate over all Frames and create the corresponding images
                int frames = (int)(current.Length * framerate);
                for (int j = 0; j < frames; j++)
                {
                    State frameState = new State(current.State, next.State, current.Easing, j, frames);
                    if (max_threads == 0 || max_threads == 1) { yield return new BitmapVideoFrameWrapper(frameState.GetBitmap(args, 1)); }
                    else { yield return new BitmapVideoFrameWrapper(frameState.GetBitmap(args, max_threads)); }
                }
            }
        }

        /// <summary>
        /// Create Animation for this StateSequence and Saves it to a mp4 file. 
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
        }


        /// <summary>
        /// Create Animation for this StateSequence and Saves it to a gif file. 
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

        public async Task<string> GenerateAnimation(StateOptions args, GenerationOptions options)
        {
            string out_dir = options.OutputPath;
            AnimationFormat type = options.AnimationFormat;
            bool keepframes = options.KeepAnimationFrames;
            int max_threads = options.MaxThreads;
            
            // check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.StateSequence.STATESEQUENCE_NAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);

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
                            // the folder where the frames are saved
                            string folder = GenerateFrames(args, file_path_out, options);
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
                            // the folder where the frames are saved
                            string folder = GenerateFrames(args, file_path_out, options);
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

            // loops through the scenes
            for (int i = 0; i < Count; i++)
            {
                Scene current = Scenes[i];
                Scene next = Scenes[(i + 1) % Scenes.Count];

                // creates the directory for the scene
                string scene_out_dir = Helper.ValidFileName(Path.Combine(out_dir, current.State.Name == "" ? Constants.StateSequence.SCENE_NAME_DEFAULT : current.State.Name));
                if (!Directory.Exists(scene_out_dir)) { Directory.CreateDirectory(scene_out_dir); }

                // iterate over all Frames and create the corresponding images
                int frames = (int)(current.Length * framerate);
                for (int j = 0; j < frames; j++)
                {
                    State frameState = new State(current.State, next.State, current.Easing, j, frames);
                    if (max_threads == 0 || max_threads == 1) { frameState.GenerateImage(args, 1, scene_out_dir); }
                    else { frameState.GenerateImage(args, max_threads, scene_out_dir); }
                }
            }

            return out_dir;
        }

        private string GenerateFrames(StateOptions args, string frames_dir, GenerationOptions options)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate;

            // create Directory for frames if not exist
            if (!Directory.Exists(frames_dir)) { Directory.CreateDirectory(frames_dir); }

            // loops through the scenes
            for (int i = 0; i < Count; i++)
            {
                Scene current = Scenes[i];
                Scene next = Scenes[(i + 1) % Scenes.Count];

                // creates the directory for the scene
                string scene_out_dir = Helper.ValidFileName(Path.Combine(frames_dir, current.State.Name == "" ? Constants.StateSequence.SCENE_NAME_DEFAULT : current.State.Name));
                if (!Directory.Exists(scene_out_dir)) { Directory.CreateDirectory(scene_out_dir); }

                // iterate over all Frames and create the corresponding images
                int frames = (int)(current.Length * framerate);
                for (int j = 0; j < frames; j++)
                {
                    State frameState = new State(current.State, next.State, current.Easing, j, frames);
                    frameState.GenerateImage(args, scene_out_dir, options);
                }
            }

            return frames_dir;
        }

        /// <summary>
        /// Create Animation after having generated all frames beforehand and save as gif
        /// </summary>
        /// <param name="framerate">The framerate</param>
        /// <param name="folder">The absolute path to folder where the generated Scenes are</param>
        private async Task CreateGif(StateOptions args, string folder)
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
        /// <param name="framerate">The framerate</param>
        /// <param name="folder">The absolute path to folder where the generated Scenes are</param>
        private async Task CreateMp4(StateOptions args, string folder)
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
        /// Returns true if the passed JsonElement is a valid StateSequence representation
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Load StateSequence properties from Json
        /// </summary>
        /// <param name="element">Json Element for GenerationOption</param>
        /// /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
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
        /// Load StateSequence from Json
        /// </summary>
        /// <param name="element">Json Element for StateSequence</param>
        /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public static StateSequence FromJson(JsonElement element, StateOptions opts)
        {
            StateSequence stateSequence = new StateSequence();
            stateSequence.LoadJson(element, opts);

            return stateSequence;
        }
        #endregion
    }

    /// <summary>
    /// Scene class consisting of a state, a length (in seconds) and an easing.
    /// </summary>
    public class Scene
    {
        #region Properties
        /// <summary>
        /// The (Beginning-) State of this scene.
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// The Length of this Scene in Seconds. (Time it takes to get to the next Scene)
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// Easing Function to Transform this to the next Scene.
        /// </summary>
        [JsonIgnore]
        public Easing Easing { get; private set; }

        public EasingType EasingType { get => Easing.Type; set => Easing = Easing.FromType(value); }
        #endregion

        #region Constructors
        /// <summary>
        /// Standard Constructor with 0 Args
        /// </summary>
        public Scene()
        {
            State = new State();
            Length = 1;
            EasingType = EasingType.Linear;
        }

        /// <summary>
        /// Standard constructor taking arguments for all 3 Properties
        /// </summary>
        /// <param name="state">The (Beginning-) State of this scene.</param>
        /// <param name="length">The Length of this Scene in Seconds. (Time it takes to get to the next Scene)</param>
        /// <param name="easing">Easing Function to Transform this to the next Scene.</param>
        public Scene(State state, double length, Easing easing)
        {
            State = state;
            Length = length;
            Easing = easing;
        }

        /// <summary>
        /// Standard constructor taking arguments for all 3 Properties
        /// </summary>
        /// <param name="state">The (Beginning-) State of this scene.</param>
        /// <param name="length">The Length of this Scene in Seconds. (Time it takes to get to the next Scene)</param>
        /// <param name="easingType">EasingType as EasingType enum to Transform this to the next Scene.</param>
        public Scene(State state, double length, EasingType easingType)
        {
            State = state;
            Length = length;
            EasingType = easingType;
        }
        #endregion

        #region json
        /// <summary>
        /// Returns true if the passed JsonElement is a valid State representation
        /// </summary>
        /// <param name="el">JsonElement for State</param>
        /// <returns></returns>
        public static bool IsJsonElementValid(JsonElement element)
        {
            return Schemas.IsElementValid(element, MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Load Scene properties from Json
        /// </summary>
        /// <param name="element">Json Element for State</param>
        /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
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
        /// Load Scene from Json
        /// </summary>
        /// <param name="element">Json Element for State</param>
        /// <param name="opts">Current StateOptions used for evaluating State Properties</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public static Scene FromJson(JsonElement element, StateOptions opts)
        {
            Scene scene = new Scene();
            scene.LoadJson(element, opts);

            return scene;
        }
        #endregion
    }
}