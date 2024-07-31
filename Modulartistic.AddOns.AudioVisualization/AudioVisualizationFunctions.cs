using System.Collections.Concurrent;

namespace Modulartistic.AddOns.AudioVisualization
{
    [AddOn]
    public class AudioVisualizationFunctions
    {
        #region Memory fields
        private static bool initialized = false;
        private static ConcurrentDictionary<string, AudioAnalysis> audios;
        #endregion


        public double GetPeakMax(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].PeakMax;
        }

        public double GetPeakMin(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].PeakMin;
        }

        public double GetSubBass(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].Frequencybands[0];
        }

        public double GetBass(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].Frequencybands[1];
        }

        public double GetLowerMidrange(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].Frequencybands[2];
        }

        public double GetMidrange(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].Frequencybands[3];
        }

        public double GetUpperMidrange(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].Frequencybands[4];
        }

        public double GetPresence(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].Frequencybands[5];
        }

        public double GetBrilliance(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].Frequencybands[6];
        }

        public double GetMaxPeakMax(int frame, string abs_path_to_file, int framerate, bool use_decibel = false)
        {
            if (!initialized) { Initialize(); }
            if (!Path.IsPathRooted(abs_path_to_file) || !File.Exists(abs_path_to_file)) { return double.NaN; }

            LoadAudio(abs_path_to_file, out AudioAnalysis analysis, framerate, use_decibel);

            return analysis.Frames[frame].Frequencybands[6];
        }






        private static void LoadAudio(string key, out AudioAnalysis analysis, int framerate, bool use_decibel)
        {
            if (audios.ContainsKey(key)) { analysis = audios[key]; }
            else
            {
                analysis = new AudioAnalysis(key, framerate, use_decibel);
                if (audios.Count > 3) { audios.Clear(); }
                audios.TryAdd(key, analysis);
            }
        }


        private static void Initialize()
        {
            audios = new ConcurrentDictionary<string, AudioAnalysis>();
            initialized = true;
        }
    }
}
