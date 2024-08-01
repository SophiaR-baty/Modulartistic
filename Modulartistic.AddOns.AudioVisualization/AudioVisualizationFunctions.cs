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

        #region Memory fields
        private static bool initialized = false;
        private static ConcurrentDictionary<string, AudioAnalysis> audios;

        private static int Framerate;
        #endregion

        #region public methods for expressions

        public static double GetPeakMax(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);
            if (analysis is null || frame < 0 || frame >= analysis.Frames.Length) { return double.NaN; }

            return analysis.Frames[frame].PeakMax;
        }

        public static double GetPeakMin(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);
            if (analysis is null || frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].PeakMin;
        }

        public static double GetSubBass(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);
            if (analysis is null || frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[0];
        }

        public static double GetBass(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);
            if (analysis is null || frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[1];
        }

        public static double GetLowerMidrange(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);
            if (analysis is null || frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[2];
        }

        public static double GetMidrange(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);
            if (analysis is null || frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[3];
        }

        public static double GetUpperMidrange(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);
            if (analysis is null || frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[4];
        }

        public static double GetPresence(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);
            if (analysis is null || frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[5];
        }

        public static double GetBrilliance(int frame, string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel); 
            if (analysis is null || frame < 0 || frame >= analysis.FrameCount) { return double.NaN; }

            return analysis.Frames[frame].Frequencybands[6];
        }






        public static double GetMaxPeakMax(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxPeakMax ?? double.NaN;
        }

        public static double GetMinPeakMax(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinPeakMax ?? double.NaN;
        }

        public static double GetAvgPeakMax(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgPeakMax ?? double.NaN;
        }

        public static double GetMaxPeakMin(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxPeakMin ?? double.NaN;
        }

        public static double GetMinPeakMin(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinPeakMin ?? double.NaN;
        }

        public static double GetAvgPeakMin(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgPeakMin ?? double.NaN;
        }

        public static double GetMaxSubBass(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxSubBass ?? double.NaN;
        }

        public static double GetMinSubBass(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinSubBass ?? double.NaN;
        }

        public static double GetAvgSubBass(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgSubBass ?? double.NaN;
        }

        public static double GetMaxBass(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxBass ?? double.NaN;
        }

        public static double GetMinBass(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinBass ?? double.NaN;
        }

        public static double GetAvgBass(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgBass ?? double.NaN;
        }

        public static double GetMaxLowerMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxLowerMidrange ?? double.NaN;
        }

        public static double GetMinLowerMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinLowerMidrange ?? double.NaN;
        }

        public static double GetAvgLowerMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgLowerMidrange ?? double.NaN;
        }

        public static double GetMaxMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxMidrange ?? double.NaN;
        }

        public static double GetMinMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinMidrange ?? double.NaN;
        }

        public static double GetAvgMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgMidrange ?? double.NaN;
        }

        public static double GetMaxUpperMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxUpperMidrange ?? double.NaN;
        }

        public static double GetMinUpperMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinUpperMidrange ?? double.NaN;
        }

        public static double GetAvgUpperMidrange(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgUpperMidrange ?? double.NaN;
        }

        public static double GetMaxPresence(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxPresence ?? double.NaN;
        }

        public static double GetMinPresence(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinPresence ?? double.NaN;
        }

        public static double GetAvgPresence(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgPresence ?? double.NaN;
        }

        public static double GetMaxBrilliance(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MaxBrilliance ?? double.NaN;
        }

        public static double GetMinBrilliance(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.MinBrilliance ?? double.NaN;
        }

        public static double GetAvgBrilliance(string abs_path_to_file, bool use_decibel = false)
        {
            LoadAudio(abs_path_to_file, out AudioAnalysis? analysis, use_decibel);

            return analysis?.AvgBrilliance ?? double.NaN;
        }

        #endregion

        private static void LoadAudio(string key, out AudioAnalysis? analysis, bool use_decibel)
        {
            if (!audios.ContainsKey(key)) 
            {
                if (!initialized) { throw new Exception("AddOn has not been initialized"); }
                if (!Path.IsPathRooted(key) || !File.Exists(key)) { analysis = null; return; }

                audios.TryAdd(key, new AudioAnalysis(key, Framerate, use_decibel));
            } 
            while (!audios[key].FinishedAnalysis) { continue; }
            analysis = audios[key];
        }
    }
}
