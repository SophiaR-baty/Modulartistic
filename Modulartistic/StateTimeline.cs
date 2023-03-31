using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using AnimatedGif;
using System.IO;
using System.Text.Json.Serialization;

namespace Modulartistic
{
    /// <summary>
    /// StateTimeline consisting of a BaseStates and several timed StateEvents
    /// </summary>
    public class StateTimeline
    {
        #region properties
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
        #endregion

        #region constructors
        /// <summary>
        /// Creates an Empty StateTimeline
        /// </summary>
        public StateTimeline()
        {
            Name = "";
            Length = 0;
            Base = new State();
            Events = new List<StateEvent>();
        }

        /// <summary>
        /// Creates an Empty StateTimeline
        /// </summary>
        /// <param name="name">The name of the TimeLine</param>
        public StateTimeline(string name)
        {
            Name = name;
            Length = 0;
            Base = new State();
            Events = new List<StateEvent>();
        }
        #endregion

        #region methods
        public int TotalFrameCount(uint framerate)
        {
            return (int)(framerate * Length / 1000);
        }

        public double LengthInSeconds()
        {
            return Length / 1000;
        }
        #endregion

        #region Methods for animation Creation
        /// <summary>
        /// Creates an Animation for this StateTimeline
        /// </summary>
        /// <param name="args">The GenrationArgs containing Size and Function and Framerate Data</param>
        /// <param name="path_out">The directory to generate in, if empty or not specified wwill be set to Output, if not found DirectoryNotFoundException will be thrown</param>
        /// <exception cref="DirectoryNotFoundException">If path_out is not found</exception>
        public void GenerateAnimation(GenerationArgs args, string path_out)
        {
            uint framerate = args.Framerate;

            // Creating filename and path
            // Make path
            string path = path_out == "" ? AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + @"Output" : path_out;
            if (!Directory.Exists(path)) { throw new DirectoryNotFoundException("The Directory " + path + " was not found."); }
            path += Path.DirectorySeparatorChar + (Name == "" ? "Timeline" : Name);

            // Validate and Create the Output Path
            path = Modulartistic.Helper.ValidFileName(path);
            Directory.CreateDirectory(path);

            // Order Events
            Events = Events.OrderBy((a) => a.StartTime).ToList();

            // set Length if Length == 0
            if (Length == 0)
            {
                Length = Events.Max(x => x.StartTime + x.Length + x.ReleaseTime) + 500;
            }

            List<StateEvent> activeEvents = new List<StateEvent>();

            // iterate over all frames
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

                for (StateProperty j = StateProperty.Mod; j <= StateProperty.i9; j++)
                {
                    FrameState[j] = (Base[j] + states.Sum(state => state[j])) / (states.Count + 1);
                }

                // Create Image of state
                FrameState.Name = "Frame_" + i.ToString().PadLeft(frames.ToString().Length, '0');
                FrameState.GenerateImage(args, path);
            }

            // Generate the gif
            CreateGif(framerate, path);
        }

        /// <summary>
        /// Generates the GIF
        /// </summary>
        /// <param name="framerate">The framerate in frames per second</param>
        /// <param name="folder">The folder where th images are, Will also be the name of the animation</param>
        private void CreateGif(double framerate, string folder)
        {
            // Creating the image list
            List<string> imgPaths = Directory.GetFiles(folder).ToList();

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
    }

    /// <summary>
    /// The StateEvent with Envelope Properties
    /// </summary>
    public class StateEvent
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
        public string AttackEasingType { get => AttackEasing.Type; set => AttackEasing = Modulartistic.Easing.FromString(value); }
        /// <summary>
        /// DecayTime in Millisecond
        /// </summary>
        public uint DecayTime { get; set; }
        /// <summary>
        /// Easing Function the Decay uses
        /// </summary>
        [JsonIgnore]
        public Easing DecayEasing { get; set; }
        public string DecayEasingType { get => DecayEasing.Type; set => DecayEasing = Modulartistic.Easing.FromString(value); }
        /// <summary>
        /// ReleaseTime in Milliseconds
        /// </summary>
        public uint ReleaseTime { get; set; }
        /// <summary>
        /// Easing Function the Release uses
        /// </summary>
        [JsonIgnore]
        public Easing ReleaseEasing { get; set; }
        public string ReleaseEasingType { get => ReleaseEasing.Type; set => ReleaseEasing = Modulartistic.Easing.FromString(value); }

        /// <summary>
        /// Dictionary of Peak Values
        /// </summary>
        public Dictionary<StateProperty, double> PeakValues { get; set; }
        /// <summary>
        /// Dictionary of Susatain Values
        /// </summary>
        public Dictionary<StateProperty, double> SustainValues { get; set; }
        #endregion

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
            long t_active = currentTime - StartTime;
            if (t_active < 0) { throw new Exception(); }

            State result = new State();
            // Attack
            if (t_active < AttackTime)
            {
                Easing easing = AttackEasing;
                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    if (!PeakValues.ContainsKey(i)) { result[i] = BaseState[i]; }
                    else { result[i] = easing.Ease(BaseState[i], PeakValues[i], Convert.ToInt32(t_active / 2), Convert.ToInt32(AttackTime / 2)); }
                }
            }
            // Decay
            else if (t_active < AttackTime + DecayTime)
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
                            Convert.ToInt32((t_active - AttackTime) / 2),
                            Convert.ToInt32(DecayTime / 2));
                    }
                }
            }
            // Sustain
            else if (t_active < AttackTime + Length + DecayTime)
            {
                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    result[i] = SustainValues.ContainsKey(i) ? SustainValues[i] : BaseState[i];
                }
            }
            // Release
            else if (t_active < AttackTime + Length + DecayTime + ReleaseTime)
            {
                Easing easing = ReleaseEasing;
                for (StateProperty i = 0; i < StateProperty.i9; i++)
                {
                    if (!SustainValues.ContainsKey(i) && !PeakValues.ContainsKey(i)) { result[i] = BaseState[i]; }
                    else
                    {
                        result[i] = easing.Ease(
                            SustainValues.ContainsKey(i) ? SustainValues[i] : PeakValues[i],
                            BaseState[i],
                            Convert.ToInt32((t_active - AttackTime - Length - DecayTime) / 2),
                            Convert.ToInt32(ReleaseTime / 2));
                    }
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
        /// Whether this StateEvent is still Active
        /// </summary>
        /// <param name="currentTime">The Current Time</param>
        /// <returns>bool</returns>
        /// <exception cref="Exception">If Activation Time is after Current Time</exception>
        public bool IsActive(uint currentTime)
        {
            long t_active = currentTime - StartTime;
            if (t_active < 0) { throw new Exception(); }

            if (t_active >= AttackTime + Length + DecayTime + ReleaseTime) { return false; }
            else { return true; }
        }
    }
}
