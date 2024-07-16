using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;
using NCalc;
using Modulartistic.Core;

namespace Modulartistic.MidiGeneration
{
    public class StateTimelineTemplate
    {
        #region Properties
        public string Name { get; set; }
        public StateOptions Metadata { get; set; }
        public State Base { get; set; }
        public List<StateEventType> Events { get; set; }
        #endregion

        private readonly Dictionary<string, double> testMappings = new Dictionary<string, double>()
        {
            { "Length", 100},
            { "NoteNumber", 40 },
            { "Octave", 3 },
            { "Velocity", 64 },
            { "OffVelocity", 64 },
            { "StartTime", 10000 },
            { "EndTime", 10000 }
        };

        #region Constructors
        public StateTimelineTemplate()
        {
            Name = Constants.StateTimeline.STATETIMELINE_NAME_DEFAULT;
            Metadata = new StateOptions();
            Base = new State();
            Events = new List<StateEventType>();
        }
        #endregion

        #region GenerationTesting Method(s)
        public void GenerateTests(string path_out, string name)
        {
            if (!OrderAndValidate()) { throw new Exception("For each channel only 1 EventType must be given"); }

            // Creating filename and path
            // Make path
            string path = path_out == "" ? AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + @"Output" : path_out;
            if (!Directory.Exists(path)) { throw new DirectoryNotFoundException("The Directory " + path + " was not found."); }
            path += Path.DirectorySeparatorChar + (name == "" ? "timelinetemplate_tests" : name + "_tests");

            // Validate and Create the Output Path
            path = Helper.ValidFileName(path);
            Directory.CreateDirectory(path);

            // Generate the Base State
            Base.GenerateImage(Metadata, 1, path);

            foreach (StateEventType e in Events)
            {
                // PeakValues
                State FrameState = new State()
                {
                    Name = "ch" + e.Channel + "_peak",
                };
                for (StateProperty j = 0; j <= StateProperty.i9; j++)
                {
                    Expression exp = new Expression(e.PeakValueMappings.ContainsKey(j) ? e.PeakValueMappings[j] : Base[j].ToString());
                    foreach (KeyValuePair<string, double> keyValuePair in testMappings)
                    {
                        exp.Parameters[keyValuePair.Key] = keyValuePair.Value;
                    }

                    FrameState[j] = Convert.ToDouble(exp.Evaluate());
                }
                FrameState.GenerateImage(Metadata, 1, path);

                // SustainValues
                FrameState = new State()
                {
                    Name = "ch" + e.Channel + "_sustain",
                };
                for (StateProperty j = 0; j <= StateProperty.i9; j++)
                {
                    Expression exp = new Expression(e.SustainValueMappings.ContainsKey(j) ? e.SustainValueMappings[j] : Base[j].ToString());
                    foreach (KeyValuePair<string, double> keyValuePair in testMappings)
                    {
                        exp.Parameters[keyValuePair.Key] = keyValuePair.Value;
                    }

                    FrameState[j] = Convert.ToDouble(exp.Evaluate());
                }
                FrameState.GenerateImage(Metadata, 1, path);
            }
        }

        public bool OrderAndValidate()
        {
            // Order Events
            Events = Events.OrderBy((a) => a.Channel).ToList();

            // Validate
            return !Events.Any(e1 => (Events.Count(e2 => (e1.Channel == e2.Channel)) > 1));
        }
        #endregion

        #region Json Serialization
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter(),
                },
            };
            return JsonSerializer.Serialize(this, options);
            // return Newtonsoft.Json.JsonConvert.SerializeObject(this.data, Newtonsoft.Json.Formatting.Indented);
        }

        public void SaveJson(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, ToJson());
        }

        public static StateTimelineTemplate LoadJson(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified File " + path + " does not exist. ");
            }

            string jsontext = File.ReadAllText(path);

            JsonDocument jd = JsonDocument.Parse(jsontext);


            JsonElement root = jd.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new Exception("Error: Expected ArrayType RootElement in Json File " + path);
            }

            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter()
                },
            };

            StateTimelineTemplate Template = JsonSerializer.Deserialize<StateTimelineTemplate>(root.GetRawText(), options);
            return Template;
        }
        #endregion

        public static StateTimelineTemplate GetDefaultTemplate()
        {
            StateTimelineTemplate STT = new StateTimelineTemplate();
            STT.Name = "default_template";
            STT.Metadata = new StateOptions()
            {
                FunctionRedHue = "x*y",
                Width = 640,
                Height = 360,
                Framerate = 12,
                CircularMod = true,
                InvalidColorGlobal = true,
            };
            STT.Base = new State()
            {
                Name = "Base",
                Mod = 960,
                ModUpperLimit = 50,
                ColorGreenSaturation = 1,
                InvalidColorAlpha = 1,
                InvalidColorRedHue = 0,
                InvalidColorGreenSaturation = 0,
                InvalidColorBlueValue = 0,
            };

            StateEventType SET = new StateEventType();
            SET.Channel = 0;
            SET.AttackTime = 10;
            SET.AttackEasingType = "Linear";
            SET.DecayTime = 3500;
            SET.DecayEasingType = "Linear";
            SET.ReleaseTime = 100;
            SET.ReleaseEasingType = "Linear";
            SET.PeakValueMappings.Add(StateProperty.ModLowerLimit, "4*Velocity");
            SET.PeakValueMappings.Add(StateProperty.ModUpperLimit, "4*Velocity+50");
            SET.PeakValueMappings.Add(StateProperty.ColorGreenSaturation, "1");

            STT.Events.Add(SET);

            return STT;
        }
    }

    public class StateEventType
    {
        #region Properties
        /// <summary>
        /// The Channel This EventType shall be mapped to.
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// AttackTime in Milliseconds
        /// </summary>
        public uint AttackTime { get; set; }

        /// <summary>
        /// The Type of Easing used for Attack
        /// </summary>
        public string AttackEasingType { get; set; }

        /// <summary>
        /// DecayTime in Millisecond
        /// </summary>
        public uint DecayTime { get; set; }

        /// <summary>
        /// The Type of Easing used for Decay
        /// </summary>
        public string DecayEasingType { get; set; }

        /// <summary>
        /// ReleaseTime in Milliseconds
        /// </summary>
        public uint ReleaseTime { get; set; }

        /// <summary>
        /// The Type of Easing used for Release
        /// </summary>
        public string ReleaseEasingType { get; set; }

        /// <summary>
        /// Dictionary of Peak Values
        /// </summary>
        public Dictionary<StateProperty, string> PeakValueMappings { get; set; }

        /// <summary>
        /// Dictionary of Susatain Values
        /// </summary>
        public Dictionary<StateProperty, string> SustainValueMappings { get; set; }
        #endregion

        /* Valid Mapping Names to use in Expressions for PeakValueMappings and SustainValueMappings are:  
         *  - Length
         *  - NoteNumber
         *  - Octave
         *  - Velocity
         *  - OffVelocity
         *  - StartTime
         *  - EndTime
         * And valid Function Names are those defined in NCalc plus eventually AddOns!
         * These are usable in PeakValueMappings and SustainValueMappings
         * 
         */

        public StateEventType()
        {
            Channel = 0;
            AttackTime = 100;
            AttackEasingType = "Linear";
            DecayTime = 100;
            DecayEasingType = "Linear";
            ReleaseTime = 100;
            ReleaseEasingType = "Linear";
            PeakValueMappings = new Dictionary<StateProperty, string>();
            SustainValueMappings = new Dictionary<StateProperty, string>();
        }
    }
}
