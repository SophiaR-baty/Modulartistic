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
        /// Constructor for an empty StateSequence
        /// </summary>
        public StateSequence()
        {
            Scenes = new List<Scene>();
            Name = Constants.STATESEQUENCE_NAME_DEFAULT;
        }

        /// <summary>
        /// Constructor of StateSequence with an Array of Scenes and a name.
        /// </summary>
        /// <param name="sequence">An Array of Scenes.</param>
        /// <param name="name">The Name of this StateSequence.</param>
        public StateSequence(Scene[] sequence, string name)
        {
            this.Scenes = sequence.ToList();
            Name = name;
        }

        public StateSequence(string name)
        {
            Scenes = new List<Scene>();
            Name = name;
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
        private IEnumerable<IVideoFrame> EnumerateFrames(GenerationArgs args, int max_threads)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT);

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
        private async Task CreateMp4(GenerationArgs args, int max_threads, string absolute_out_filepath)
        {
            // parsing framerate and setting piping source
            uint framerate = args.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT);
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(args, max_threads))
            {
                FrameRate = framerate, // set source frame rate
            };

            // parsing size
            System.Drawing.Size size = new System.Drawing.Size(args.Size[0], args.Size[1]);

            // generate the mp4 file
            try
            {
                await FFMpegArguments
                .FromPipeInput(videoFramesSource)
                .OutputToFile(absolute_out_filepath + @".mp4", false, options => options
                    .WithVideoCodec(VideoCodec.LibX265)
                    .WithVideoBitrate(16000) // find a balance between quality and file size
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
        private async Task CreateGif(GenerationArgs args, int max_threads, string absolute_out_filepath)
        {
            // parsing framerate and setting piping source
            uint framerate = args.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT);
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(args, max_threads))
            {
                FrameRate = framerate, // set source frame rate
            };

            // parsing size
            System.Drawing.Size size = new System.Drawing.Size(args.Size[0], args.Size[1]);

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
        /// Generates Animation for this StateSequence and Saves it to a file. 
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
        public async Task<string> GenerateAnimation(GenerationArgs args, int max_threads, AnimationType type, bool keepframes, string out_dir)
        {
            // If out-dir is empty set to default, then check if it exists
            out_dir = out_dir == "" ? Constants.OUTPUTFOLDER : out_dir;
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.STATESEQUENCE_NAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);
            
            // parse framerate from GenerationArgs
            uint framerate = args.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT);
            
            switch (type)
            {
                case AnimationType.None:
                    {
                        throw new Exception("No AnimationType was specified. ");
                    }
                case AnimationType.Gif:
                    {
                        if (keepframes)
                        {
                            string folder = GenerateFrames(args, max_threads, file_path_out);
                            CreateGif(args, folder);
                        }
                        else
                        {
                            await CreateGif(args, max_threads, file_path_out);
                        }
                        return file_path_out + @".gif";
                    }
                case AnimationType.Mp4:
                    {
                        if (keepframes) 
                        {
                            string folder = GenerateFrames(args, max_threads, file_path_out);
                            CreateMp4(args, folder);
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
        private string GenerateFrames(GenerationArgs args, int max_threads, string out_dir)
        {
            // parses GenerationArgs
            uint framerate = args.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT);

            // create Directory for frames if not exist
            if (!Directory.Exists(out_dir)) { Directory.CreateDirectory(out_dir); }

            // loops through the scenes
            for (int i = 0; i < Count; i++)
            {
                Scene current = Scenes[i];
                Scene next = Scenes[(i + 1) % Scenes.Count];

                // creates the directory for the scene
                string scene_out_dir = Helper.ValidFileName(Path.Combine(out_dir, current.State.Name == "" ? Constants.SCENE_NAME_DEFAULT : current.State.Name));
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

        /// <summary>
        /// Create Animation after having generated all frames beforehand and save as gif
        /// </summary>
        /// <param name="framerate">The framerate</param>
        /// <param name="folder">The absolute path to folder where the generated Scenes are</param>
        private void CreateGif(GenerationArgs args, string folder)
        {
            // Creating the image list
            List<string> imgPaths = new List<string>();
            List<string> sceneDirs = new List<string>();

            // loop through all Scenes to get all images in the correct order
            for (int i = 0; i < Count; i++)
            {
                // Define the scenDir of the current scene
                string sceneDir = Path.Combine(folder, Scenes[i].State.Name == "" ? Constants.SCENE_NAME_DEFAULT : Scenes[i].State.Name);

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
            uint framerate = args.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT);
            FFMpeg.JoinImageSequence(folder + @".gif", frameRate: framerate, imgPaths.ToArray());
        }

        /// <summary>
        /// Create Animation after having generated all frames beforehand and save as mp4
        /// </summary>
        /// <param name="framerate">The framerate</param>
        /// <param name="folder">The absolute path to folder where the generated Scenes are</param>
        private void CreateMp4(GenerationArgs args, string folder)
        {
            // Creating the image list
            List<string> imgPaths = new List<string>();
            List<string> sceneDirs = new List<string>();

            // loop through all Scenes to get all images in the correct order
            for (int i = 0; i < Count; i++)
            {
                // Define the scenDir of the current scene
                string sceneDir = Path.Combine(folder, Scenes[i].State.Name == "" ? Constants.SCENE_NAME_DEFAULT : Scenes[i].State.Name);

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
            uint framerate = args.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT);
            FFMpeg.JoinImageSequence(folder + @".mp4", frameRate: framerate, imgPaths.ToArray());
        }
        #endregion

        #region Other Methods
        /// <summary>
        /// Gets details about this StateSequence. Useful for debugging. 
        /// </summary>
        /// <returns>A formatted details string</returns>
        public string GetDetailsString(uint framerate = 12)
        {
            string result = string.Format(
                $"{"Name: ",-30} {Name} \n" +
                $"{"Scene Count: ",-30} {Count} \n" +
                $"{"Total Frame Count: ",-30} {TotalFrameCount(framerate)} \n" +
                $"{"Length in Seconds: ",-30} {LengthInSeconds()} \n\n" +
                $"{"Scenes: ",-30} \n\n"
                );
            for (int i = 0; i < Count; i++)
            {
                result +=
                    $"Scene {i}: \n" +
                    $"{"Easing: ",-30} {Scenes[i].EasingType} \n" +
                    Scenes[i].State.GetDebugInfo() + "\n\n";
            }

            return result;
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
        
        public string EasingType { get => Easing.Type; set => Easing = Easing.FromString(value); }
        #endregion

        #region Constructors
        /// <summary>
        /// Standard Constructor with 0 Args
        /// </summary>
        public Scene()
        {
            State = new State();
            Length = 1;
            EasingType = "Linear";
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
        /// <param name="easingType">EasingType as String to Transform this to the next Scene.</param>
        public Scene(State state, double length, string easingType)
        {
            State = state;
            Length = length;
            EasingType = easingType;
        }
        #endregion
    }
}