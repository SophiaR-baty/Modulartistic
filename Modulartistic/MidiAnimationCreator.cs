using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using NCalc;
using System.IO;

namespace Modulartistic
{
    public class MidiAnimationCreator
    {
        public static void GenerateJson(string midi_path, int channel)
        {
            
            var midiFile = MidiFile.Read(midi_path);
            TempoMap tmpMap = midiFile.GetTempoMap();

            StateTimeline ST = new StateTimeline("Test");
            ST.Length = 10000;
            ST.Base = new State()
            {
                Mod = 960,
                ModLimLow = 0,
                ModLimUp = 50,
                X0 = 0, Y0 = 0,
                XZoom = 1, YZoom = 1,
                Rotation = 0,
                ColorMinimum = 0,
                ColorAlpha = 1,
                ColorSaturation = 0, 
                ColorValue = 1,
                InvalidColor = new double[] { 1, 0, 0, 0,},
                ColorFactors = new double[] {1, 1, 1,},
                Parameters = new List<double>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
                Name = "Base",
            };

            List<Note> notes = midiFile.GetNotes().ToList();
            for (int i = 0; i < notes.Count;) 
            {
                Note note = notes[i];
                Console.WriteLine("{0} {1} {2} {3}", note.Time, note.Length, note.NoteNumber, note.Velocity);
                if (note.Channel == channel)
                {
                    StateEvent SE = new StateEvent();
                    SE.AttackTime = 10;
                    SE.DecayTime = 3500;
                    SE.ReleaseTime = 100;
                    SE.AttackEasing = Easing.Linear();
                    SE.DecayEasing = Easing.Linear();
                    SE.ReleaseEasing = Easing.Linear();
                    SE.Length = (uint)((TimeSpan)(MetricTimeSpan)note.LengthAs(TimeSpanType.Metric, tmpMap)).TotalMilliseconds;
                    
                    SE.PeakValues[StateProperty.ModLimLow] = (note.Velocity % 255) * 4;
                    SE.PeakValues[StateProperty.ModLimUp] = (note.Velocity % 255) * 4 + 50;
                    SE.PeakValues[StateProperty.ColorSaturation] = 1;
                    
                    SE.StartTime = (uint)((TimeSpan)(MetricTimeSpan)note.TimeAs(TimeSpanType.Metric, tmpMap)).TotalMilliseconds;

                    ST.Events.Add(SE);
                }
                i++;
            }
            ST.GenerateAnimation(new GenerationArgs(), "");
        }

        public static StateTimeline CreateStateTimeline(string midi_path, StateTimelineTemplate template, string path_out)
        {
            var midiFile = MidiFile.Read(midi_path);
            TempoMap tmpMap = midiFile.GetTempoMap();

            StateTimeline timeline = new StateTimeline();
            timeline.Length = (uint)((TimeSpan)(MetricTimeSpan)midiFile.GetDuration(TimeSpanType.Metric)).TotalMilliseconds;

            // Debugging
            Console.WriteLine(((TimeSpan)(MetricTimeSpan)midiFile.GetDuration(TimeSpanType.Metric)).ToString());

            if (!template.OrderAndValidate()) { throw new Exception("For each channel only 1 EventType must be given"); }

            timeline.Base = template.Base;

            List<Note> notes = midiFile.GetNotes().ToList();
            foreach (StateEventType eventType in template.Events)
            {
                for (int i = 0; i < notes.Count;  i++)
                {
                    Note note = notes[i];
                    Console.WriteLine(note.Channel);
                    // Console.WriteLine(note.Channel);
                    if (note.Channel == eventType.Channel)
                    {
                        Dictionary<string, double> mappings = new Dictionary<string, double>()
                        {
                            { "Length", ((TimeSpan)(MetricTimeSpan)note.LengthAs(TimeSpanType.Metric, tmpMap)).TotalMilliseconds},
                            { "NoteNumber", note.NoteNumber },
                            { "Octave", note.Octave },
                            { "Velocity", note.Velocity },
                            { "OffVelocity", note.OffVelocity },
                            { "StartTime", ((TimeSpan)(MetricTimeSpan)note.TimeAs(TimeSpanType.Metric, tmpMap)).TotalMilliseconds },
                            { "EndTime", ((TimeSpan)(MetricTimeSpan)note.EndTimeAs(TimeSpanType.Metric, tmpMap)).TotalMilliseconds }
                        };

                        StateEvent SE = new StateEvent();
                        SE.AttackTime = eventType.AttackTime;
                        SE.DecayTime = eventType.DecayTime;
                        SE.ReleaseTime = eventType.ReleaseTime;
                        SE.AttackEasingType = eventType.AttackEasingType;
                        SE.DecayEasingType = eventType.DecayEasingType; ;
                        SE.ReleaseEasingType = eventType.ReleaseEasingType;
                        SE.Length = (uint)((TimeSpan)(MetricTimeSpan)note.LengthAs(TimeSpanType.Metric, tmpMap)).TotalMilliseconds;

                        foreach (KeyValuePair<StateProperty, string> keyValuePair in eventType.SustainValueMappings)
                        {
                            Expression exp = new Expression(keyValuePair.Value);
                            foreach (KeyValuePair<string, double> mapping in mappings)
                            {
                                exp.Parameters[mapping.Key] = mapping.Value;
                            }
                            SE.SustainValues[keyValuePair.Key] = Convert.ToDouble(exp.Evaluate());
                        }

                        foreach (KeyValuePair<StateProperty, string> keyValuePair in eventType.PeakValueMappings)
                        {
                            Expression exp = new Expression(keyValuePair.Value);
                            foreach (KeyValuePair<string, double> mapping in mappings)
                            {
                                exp.Parameters[mapping.Key] = mapping.Value;
                            }
                            SE.PeakValues[keyValuePair.Key] = Convert.ToDouble(exp.Evaluate());
                        }

                        SE.StartTime = (uint)((TimeSpan)(MetricTimeSpan)note.TimeAs(TimeSpanType.Metric, tmpMap)).TotalMilliseconds;

                        timeline.Events.Add(SE);
                    }
                }
            }

            return timeline;
        }

        public static void GenerateAnimation(string midi_path, StateTimelineTemplate template, string path_out = "") 
        {
            StateTimeline timeline = CreateStateTimeline(midi_path, template, path_out);
            timeline.Name = Path.GetFileNameWithoutExtension(midi_path);
            Console.WriteLine("Creating Animation with " + timeline.TotalFrameCount(template.Metadata.Framerate) + " Frames");
            timeline.GenerateAnimation(template.Metadata, path_out);
        }

        public static void GenerateJson(string midi_path, StateTimelineTemplate template, string file_out )
        {
            GenerationData gd = new GenerationData();
            gd.Add(template.Metadata);
            StateTimeline timeline = CreateStateTimeline(midi_path, template, file_out);
            timeline.Name = midi_path[..midi_path.IndexOf('.')];
            gd.Add(timeline);

            gd.SaveJson(file_out);
        }
    }
}
