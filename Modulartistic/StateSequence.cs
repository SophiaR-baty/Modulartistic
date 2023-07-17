using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Diagnostics;
using AnimatedGif;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Modulartistic
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
            Name = "Sequence";
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

        #region Serialize and Deserialize
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, options);
        }
        #endregion

        #region gif/frame generation
        /// <summary>
        /// Generates all images/frames for the scene at a specified index, e.g. Animation from this scenes state to the next scenes state
        /// </summary>
        /// <param name="idx">index of the Scene to generate</param>
        /// <param name="args">The GenrationArgs containing Size and Function and Framerate Data</param>
        /// <param name="path_out">Where to create the images, if nothing specified it will be set to Output, will throw a DirectoryNotFoundException if directory does not exist.</param>
        public void GenerateScene(int idx, GenerationArgs args, string path_out)
        {
            uint framerate = args.Framerate;
            
            // Defining the start and endstates
            State startState = Scenes[idx].State;
            State endState = Scenes[(idx + 1)%Scenes.Count].State;

            // Make path
            if (path_out == "") { throw new ArgumentException("path_out must not be empty!"); }
            string path = path_out;
            path += Path.DirectorySeparatorChar + (startState.Name == "" ? "Scene" : startState.Name);

            // Validate and Create the Output Path
            path = Modulartistic.Helper.ValidFileName(path);
            Directory.CreateDirectory(path);

            // iterate over all Frames and create the corresponding images
            int frames = (int)(Scenes[idx].Length * framerate);
            for (int i = 0; i < frames; i++)
            {
                State frameState = new State(startState, endState, Scenes[idx].Easing, i, frames);
                frameState.GenerateImage(args, path);
            }
        }

        /// <summary>
        /// Generates an Animation for this StateSequence
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function and Framerate Data</param>
        /// <param name="path_out">the path where to create the animation, if not specified it will be set to Output, if not found an DirectoryNotFoundException is Thrown</param>
        public string GenerateAnimation(GenerationArgs args, string path_out = @"")
        {
            // Creating filename and path, checking if directory exists
            string path = path_out == "" ? AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + @"Output" : path_out;
            if (!Directory.Exists(path)) { throw new DirectoryNotFoundException("The Directory " + path + " was not found."); }
            path += Path.DirectorySeparatorChar + (Name == "" ? "Animation" : Name);

            // Validate and Create the Output Path
            path = Modulartistic.Helper.ValidFileName(path);
            Directory.CreateDirectory(path);

            // Generate Every Scene
            for (int i = 0; i < Count; i++)
            {
                GenerateScene(i, args, path);
            }

            // Generate the gif
            uint framerate = args.Framerate;
            CreateGif(framerate, path);

            return path + @".gif";
        }

        /// <summary>
        /// Creates the Gif of this state Sequence
        /// </summary>
        /// <param name="framerate">The framerate to generate with</param>
        /// <param name="folder">The folder that contains all Scenes/Frames, this will also be the filename</param>
        private void CreateGif(double framerate, string folder)
        {
            // Creating the image list
            List<string> imgPaths = new List<string>();
            List<string> sceneDirs = new List<string>();

            // loop through all Scenes to get all images in the correct order
            for (int i = 0; i < Count; i++)
            {
                // Define the scenDir of the current scene
                string sceneDir = folder + Path.DirectorySeparatorChar + (Scenes[i].State.Name == "" ? "Scene" : Scenes[i].State.Name);

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

            // Convert Framerate to delay
            int delay = (int)(1000.0 / framerate);

            // Create the gif
            using (var gif = AnimatedGif.AnimatedGif.Create(folder + ".gif", delay))
            {
                foreach (string file in imgPaths)
                {
                    var img = Image.FromFile(file);
                    gif.AddFrame(img, delay: -1, quality: GifQuality.Bit8);
                }
            }
        }


        #endregion

        #region multithreaded gif/frame generation
        /// <summary>
        /// Generates all images/frames for the scene at a specified index, e.g. Animation from this scenes state to the next scenes state. If max_threads = -1 uses the maximum number
        /// </summary>
        /// <param name="idx">index of the Scene to generate</param>
        /// <param name="args">The GenrationArgs containing Size and Function and Framerate Data</param>
        /// <param name="max_threads">The maximum number of Threads to create. If -1 uses the maximum number</param>
        /// <param name="path_out">Where to create the images, if nothing specified it will be set to Output, will throw a DirectoryNotFoundException if directory does not exist.</param>
        public void GenerateScene(int idx, GenerationArgs args, int max_threads, string path_out)
        {
            uint framerate = args.Framerate;

            // Defining the start and endstates
            State startState = Scenes[idx].State;
            State endState = Scenes[(idx + 1) % Scenes.Count].State;

            // Make path
            if (path_out == "") { throw new ArgumentException("path_out must not be empty!"); }
            string path = path_out;
            path += Path.DirectorySeparatorChar + (startState.Name == "" ? "Scene" : startState.Name);

            // Validate and Create the Output Path
            path = Modulartistic.Helper.ValidFileName(path);
            Directory.CreateDirectory(path);

            // iterate over all Frames and create the corresponding images
            int frames = (int)(Scenes[idx].Length * framerate);
            for (int i = 0; i < frames; i++)
            {
                State frameState = new State(startState, endState, Scenes[idx].Easing, i, frames);
                frameState.GenerateImage(args, max_threads, path);
            }
        }

        /// <summary>
        /// Generates an Animation for this StateSequence. If max_threads = -1 uses the maximum number
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function and Framerate Data</param>
        /// /// <param name="max_threads">The maximum number of Threads to create. If -1 uses the maximum number</param>
        /// <param name="path_out">the path where to create the animation, if not specified it will be set to Output, if not found an DirectoryNotFoundException is Thrown</param>
        public string GenerateAnimation(GenerationArgs args, int max_threads, string path_out = @"")
        {
            // Creating filename and path, checking if directory exists
            string path = path_out == "" ? AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + @"Output" : path_out;
            if (!Directory.Exists(path)) { throw new DirectoryNotFoundException("The Directory " + path + " was not found."); }
            path += Path.DirectorySeparatorChar + (Name == "" ? "Animation" : Name);

            // Validate and Create the Output Path
            path = Modulartistic.Helper.ValidFileName(path);
            Directory.CreateDirectory(path);

            // Generate Every Scene
            for (int i = 0; i < Count; i++)
            {
                GenerateScene(i, args, max_threads, path);
            }

            // Generate the gif
            uint framerate = args.Framerate;
            CreateGif(framerate, path);

            return path + @".gif";
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
                    Scenes[i].State.GetDetailsString() + "\n\n";
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
        public string EasingType { get => Easing.Type; set => Easing = Modulartistic.Easing.FromString(value); }
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