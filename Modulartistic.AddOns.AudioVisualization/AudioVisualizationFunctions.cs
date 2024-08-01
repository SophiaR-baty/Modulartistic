using Modulartistic.Core;
using System.Collections.Concurrent;

namespace Modulartistic.AddOns.AudioVisualization
{
    [AddOn]
    public static class AudioVisualizationFunctions
    {
        public static void Initialize(State s, StateOptions sOpts, GenerationOptions gOpts)
        {
            if (initialized) { return; }

            Framerate = (int)sOpts.Framerate;
            gOpts.Logger?.LogDebug($"{nameof(AudioVisualizationFunctions)} initialized");
            initialized = true;
        }
        static AudioVisualizationFunctions() 
        {
            audios = new ConcurrentDictionary<string, AudioAnalysis>();
        }

        #region Memory fields
        private static bool initialized = false;
        private static ConcurrentDictionary<string, AudioAnalysis> audios;

        private static int Framerate;
        #endregion

        #region public methods for expressions

        public static double GetPeakMax(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }
            
            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.Frames.Length) { return double.NaN; }

            return analysis.Frames[frame].PeakMax;
        }

        public static double GetPeakMin(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].PeakMin;
        }

        public static double GetSubBass(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[0];
        }

        public static double GetBass(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[1];
        }

        public static double GetLowerMidrange(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[2];
        }

        public static double GetMidrange(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[3];
        }

        public static double GetUpperMidrange(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[4];
        }

        public static double GetPresence(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[5];
        }

        public static double GetBrilliance(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);
            if (frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[6];
        }






        public static double GetMaxPeakMax(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxPeakMax;
        }

        public static double GetMinPeakMax(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinPeakMax;
        }

        public static double GetAvgPeakMax(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgPeakMax;
        }

        public static double GetMaxPeakMin(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxPeakMin;
        }

        public static double GetMinPeakMin(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinPeakMin;
        }

        public static double GetAvgPeakMin(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgPeakMin;
        }

        public static double GetMaxSubBass(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxSubBass;
        }

        public static double GetMinSubBass(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinSubBass;
        }

        public static double GetAvgSubBass(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgSubBass;
        }

        public static double GetMaxBass(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxBass;
        }

        public static double GetMinBass(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinBass;
        }

        public static double GetAvgBass(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgBass;
        }

        public static double GetMaxLowerMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxLowerMidrange;
        }

        public static double GetMinLowerMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinLowerMidrange;
        }

        public static double GetAvgLowerMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgLowerMidrange;
        }

        public static double GetMaxMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxMidrange;
        }

        public static double GetMinMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinMidrange;
        }

        public static double GetAvgMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgMidrange;
        }

        public static double GetMaxUpperMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxUpperMidrange;
        }

        public static double GetMinUpperMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinUpperMidrange;
        }

        public static double GetAvgUpperMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgUpperMidrange;
        }

        public static double GetMaxPresence(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxPresence;
        }

        public static double GetMinPresence(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinPresence;
        }

        public static double GetAvgPresence(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgPresence;
        }

        public static double GetMaxBrilliance(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MaxBrilliance;
        }

        public static double GetMinBrilliance(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.MinBrilliance;
        }

        public static double GetAvgBrilliance(string abs_path_to_file, bool use_decibel = false)
        {
            if (!initialized) { throw new Exception("AddOn has not been initialized"); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, use_decibel);

            return analysis.AvgBrilliance;
        }

        #endregion

        private static void LoadAudio(string key, out AudioAnalysis analysis, bool use_decibel)
        {
            if (audios.ContainsKey(key)) 
            {
                while (!audios[key].FinishedAnalysis) { continue; }
                analysis = audios[key];
            }
            else
            {
                audios.TryAdd(key, new AudioAnalysis(key, Framerate, use_decibel));
                analysis = new AudioAnalysis(key, Framerate, use_decibel);
                if (audios.Count > 3) { audios.Clear(); }
            }
        }
    }
}
