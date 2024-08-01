using MathNet.Numerics;
using NAudio.Wave;
using NAudio.Dsp;
using System;
using NAudio.WaveFormRenderer;
using FFMpegCore;

namespace Modulartistic.AudioGeneration
{
    internal class AudioAnalysis
    {
        #region private fields

        // filepath to the analyzed file
        private string _filepath;

        // total length of the audio
        private TimeSpan _audio_length;

        // collection of audio frames containing audio data
        private AudioFrame[] _frames;

        // total number of frames
        private int _number_of_frames;

        // framerate
        private int _framerate;

        #region private fields for helper methods

        private float _maxPeakMax;
        private float _minPeakMax;
        private float _avgPeakMax;

        private float _maxPeakMin;
        private float _minPeakMin;
        private float _avgPeakMin;

        private float _maxSubBass;
        private float _minSubBass;
        private float _avgSubBass;

        private float _maxBass;
        private float _minBass;
        private float _avgBass; 
        
        private float _maxLowerMidrange;
        private float _minLowerMidrange;
        private float _avgLowerMidrange;

        private float _maxMidrange;
        private float _minMidrange;
        private float _avgMidrange; 
        
        private float _maxUpperMidrange;
        private float _minUpperMidrange;
        private float _avgUpperMidrange;

        private float _maxPresence;
        private float _minPresence;
        private float _avgPresence;

        private float _maxBrilliance;
        private float _minBrilliance;
        private float _avgBrilliance;

        #endregion

        #endregion

        #region public properties

        /// <summary>
        /// Get the length of the analyzed audio
        /// </summary>
        public TimeSpan AudioLength { get => _audio_length; }
        
        /// <summary>
        /// get the number of Frames
        /// </summary>
        public int FrameCount { get => _number_of_frames; }
        
        /// <summary>
        /// Get the frame array
        /// </summary>
        public AudioFrame[] Frames { get => _frames; }

        #endregion

        #region constructors

        /// <summary>
        /// Create a new audio analysis and fill the array of audioframes
        /// </summary>
        /// <param name="filePath">The file path to the audio file to analyze</param>
        /// <param name="framerate">The framerate used for animation generation, determines the total framecount</param>
        /// <param name="decibelScale">Wether or not to use decibel scale</param>
        public AudioAnalysis(string filePath, int framerate, bool decibelScale)
        {
            _filepath = filePath;
            _framerate = framerate;

            using (var reader = new AudioFileReader(_filepath))
            {
                _number_of_frames = (int)Math.Ceiling(reader.TotalTime.TotalSeconds * _framerate);
                _frames = new AudioFrame[_number_of_frames];
                _audio_length = reader.TotalTime;

                for (int i = 0; i < _number_of_frames; i++)
                {
                    _frames[i] = new AudioFrame();
                }


                // test different values for block size
                PeakProvider peakProvider = new RmsPeakProvider(200);
                // PeakProvider p2 = new AveragePeakProvider(1);

                int bytesPerSample = (reader.WaveFormat.BitsPerSample / 8);
                var samples = reader.Length / (bytesPerSample);
                var samplesPerPixel = (int)(samples / _number_of_frames);
                peakProvider.Init(reader.ToSampleProvider(), samplesPerPixel);

                FillPeaks(peakProvider, decibelScale);
                reader.Position = 0;
                FillFrequencyBands(reader, decibelScale);
            }

            InitializeMinMaxAvg();
        }

        #endregion

        #region private methods for audio analysis

        /// <summary>
        /// fill the min and max peaks of all audio frames
        /// </summary>
        /// <param name="peakProvider">a peak provider</param>
        /// <param name="decibelScale">whether to use decibel scale</param>
        private void FillPeaks(IPeakProvider peakProvider, bool decibelScale)
        {
            double dynamicRange = 48;

            int frame = 0;
            var currentPeak = peakProvider.GetNextPeak();
            if (decibelScale)
            {
                var decibelMax = 20 * Math.Log10(currentPeak.Max);
                if (decibelMax < 0 - dynamicRange) decibelMax = 0 - dynamicRange;
                var linear = (float)((dynamicRange + decibelMax) / dynamicRange);
                currentPeak = new PeakInfo(0 - linear, linear);
            }
            while (frame < _number_of_frames)
            {
                var nextPeak = peakProvider.GetNextPeak();
                if (decibelScale)
                {
                    var decibelMax = 20 * Math.Log10(nextPeak.Max);
                    if (decibelMax < 0 - dynamicRange) decibelMax = 0 - dynamicRange;
                    var linear = (float)((dynamicRange + decibelMax) / dynamicRange);
                    nextPeak = new PeakInfo(0 - linear, linear);
                }

                _frames[frame].PeakMin = currentPeak.Min;
                _frames[frame].PeakMax = currentPeak.Max;

                currentPeak = nextPeak;
                frame++;
            }
        }

        /// <summary>
        /// Fill the Frequency information of all Frames
        /// </summary>
        /// <param name="reader">an audio reader</param>
        /// <param name="decibelScale">whether or not to use decibel scale</param>
        private void FillFrequencyBands(AudioFileReader reader, bool decibelScale)
        {
            int bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
            var samples = reader.Length / bytesPerSample;
            var samplesPerPixel = (int)(samples / _number_of_frames);

            ISampleProvider provider = reader.ToSampleProvider();

            int sampleRate = reader.WaveFormat.SampleRate;
            int frame = 0;
            while (frame < _number_of_frames)
            {
                float[] buffer = new float[samplesPerPixel];
                int samplesRead = provider.Read(buffer, 0, buffer.Length);

                // Perform FFT (Fast Fourier Transform) for frequency analysis
                Complex[] complexBuffer = new Complex[samplesRead];
                for (int i = 0; i < samplesRead; i++)
                {
                    complexBuffer[i].X = (float)(buffer[i] * FastFourierTransform.HannWindow(i, samplesRead));
                    complexBuffer[i].Y = 0;
                }
                int m = (int)Math.Floor(Math.Log(samplesRead, 2.0));
                FastFourierTransform.FFT(true, m, complexBuffer);

                // convert values from fft to frequency bands
                float[] frequencies = new float[samplesRead / 2];
                for (int i = 0; i < samplesRead / 2; i++)
                {
                    frequencies[i] = (float)Math.Sqrt(complexBuffer[i].X * complexBuffer[i].X + complexBuffer[i].Y * complexBuffer[i].Y);
                }

                // Define the frequency bands we're interested in
                float[][] frequencyBands = new float[][]
                {
                    new float[] { 20, 60 },    // Example: Sub-bass
                    new float[] { 60, 250 },   // Example: Bass
                    new float[] { 250, 500 },  // Example: Low Midrange
                    new float[] { 500, 2000 }, // Example: Midrange
                    new float[] { 2000, 4000 },// Example: Upper Midrange
                    new float[] { 4000, 6000 },// Example: Presence
                    new float[] { 6000, 20000 }// Example: Brilliance
                };

                float[] bandMagnitudes = new float[frequencyBands.Length];

                // Sum the magnitudes of the frequencies within each band
                for (int i = 0; i < frequencies.Length; i++)
                {
                    float freq = (i * sampleRate) / (float)samplesRead;
                    for (int band = 0; band < frequencyBands.Length; band++)
                    {
                        if (freq >= frequencyBands[band][0] && freq < frequencyBands[band][1])
                        {
                            bandMagnitudes[band] += frequencies[i];
                            break;
                        }
                    }
                }

                _frames[frame].Frequencybands = new float[bandMagnitudes.Length];
                // Convert the summed magnitudes to decibels
                for (int band = 0; band < bandMagnitudes.Length; band++)
                {
                    if (decibelScale)
                        bandMagnitudes[band] = 20 * (float)Math.Log10(bandMagnitudes[band] + float.Epsilon); // Add float.Epsilon to avoid log(0)
                    _frames[frame].Frequencybands[band] = bandMagnitudes[band];
                }

                frame++;
            }
        }

        #endregion

        #region public methods for expression evaluation

        public float GetPeakMax(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].PeakMax;
        }

        public float GetPeakMin(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].PeakMin;
        }

        public float GetFrequency(int frame, int band)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].Frequencybands[band];
        }

        public float GetSubBass(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].Frequencybands[0];
        }

        public float GetBass(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].Frequencybands[1];
        }

        public float GetLowerMidrange(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].Frequencybands[2];
        }

        public float GetMidrange(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].Frequencybands[3];
        }

        public float GetUpperMidrange(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].Frequencybands[4];
        }

        public float GetPresence(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].Frequencybands[5];
        }

        public float GetBrilliance(int frame)
        {
            if (frame < 0 || frame >= _frames.Length)
                return 0;
            return _frames[frame].Frequencybands[6];
        }

        #region min, max and avg

        private void InitializeMinMaxAvg()
        {
            _maxPeakMax = Frames.Max(f => f.PeakMax);
            _minPeakMax = Frames.Min(f => f.PeakMax);
            _avgPeakMax = Frames.Average(f => f.PeakMax);

            _maxPeakMin = Frames.Max(f => f.PeakMin);
            _minPeakMin = Frames.Min(f => f.PeakMin);
            _avgPeakMin = Frames.Average(f => f.PeakMin);

            _maxSubBass = Frames.Max(f => f.Frequencybands[0]);
            _minSubBass = Frames.Min(f => f.Frequencybands[0]);
            _avgSubBass = Frames.Average(f => f.Frequencybands[0]);

            _maxBass = Frames.Max(f => f.Frequencybands[1]);
            _minBass = Frames.Min(f => f.Frequencybands[1]);
            _avgBass = Frames.Average(f => f.Frequencybands[1]);

            _maxLowerMidrange = Frames.Max(f => f.Frequencybands[2]);
            _minLowerMidrange = Frames.Min(f => f.Frequencybands[2]);
            _avgLowerMidrange = Frames.Average(f => f.Frequencybands[2]);

            _maxMidrange = Frames.Max(f => f.Frequencybands[3]);
            _minMidrange = Frames.Min(f => f.Frequencybands[3]);
            _avgMidrange = Frames.Average(f => f.Frequencybands[3]);

            _maxUpperMidrange = Frames.Max(f => f.Frequencybands[4]);
            _minUpperMidrange = Frames.Min(f => f.Frequencybands[4]);
            _avgUpperMidrange = Frames.Average(f => f.Frequencybands[4]);

            _maxPresence = Frames.Max(f => f.Frequencybands[5]);
            _minPresence = Frames.Min(f => f.Frequencybands[5]);
            _avgPresence = Frames.Average(f => f.Frequencybands[5]);

            _maxBrilliance = Frames.Max(f => f.Frequencybands[6]);
            _minBrilliance = Frames.Min(f => f.Frequencybands[6]);
            _avgBrilliance = Frames.Average(f => f.Frequencybands[6]);

        }

        #region PeakMax

        public float MaxPeakMax()
        {
            return _maxPeakMax;
        }

        public float MinPeakMax()
        {
            return _minPeakMax;
        }

        public float AvgPeakMax()
        {
            return _avgPeakMax;
        }

        #endregion

        #region PeakMin

        public float MaxPeakMin()
        {
            return _maxPeakMin;
        }

        public float MinPeakMin()
        {
            return _minPeakMin;
        }

        public float AvgPeakMin()
        {
            return _avgPeakMin;
        }

        #endregion

        #region SubBass

        public float MaxSubBass()
        {
            return _maxSubBass;
        }

        public float MinSubBass()
        {
            return _minSubBass;
        }

        public float AvgSubBass()
        {
            return _avgSubBass;
        }

        #endregion

        #region Bass

        public float MaxBass()
        {
            return _maxBass;
        }

        public float MinBass()
        {
            return _minBass;
        }

        public float AvgBass()
        {
            return _avgBass;
        }

        #endregion

        #region Lower Midrange

        public float MaxLowerMidrange()
        {
            return _maxLowerMidrange;
        }

        public float MinLowerMidrange()
        {
            return _minLowerMidrange;
        }

        public float AvgLowerMidrange()
        {
            return _avgLowerMidrange;
        }

        #endregion

        #region midrange

        public float MaxMidrange()
        {
            return _maxMidrange;
        }

        public float MinMidrange()
        {
            return _minMidrange;
        }

        public float AvgMidrange()
        {
            return _avgMidrange;
        }

        #endregion

        #region upper midrange

        public float MaxUpperMidrange()
        {
            return _maxUpperMidrange;
        }

        public float MinUpperMidrange()
        {
            return _minUpperMidrange;
        }

        public float AvgUpperMidrange()
        {
            return _avgUpperMidrange;
        }

        #endregion

        #region presence

        public float MaxPresence()
        {
            return _maxPresence;
        }

        public float MinPresence()
        {
            return _minPresence;
        }

        public float AvgPresence()
        {
            return _avgPresence;
        }

        #endregion

        #region brilliance

        public float MaxBrilliance()
        {
            return _maxBrilliance;
        }

        public float MinBrilliance()
        {
            return _minBrilliance;
        }

        public float AvgBrilliance()
        {
            return _avgBrilliance;
        }

        #endregion

        #endregion

        #endregion
    }
}
