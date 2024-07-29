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
        private string _filepath;
        private TimeSpan _audio_length;
        private AudioFrame[] _frames;
        // total number of frames
        private int _number_of_frames;
        private int _framerate;

        public TimeSpan AudioLength { get => _audio_length; }
        public int FrameCount { get => _number_of_frames; }
        public AudioFrame[] Frames { get => _frames; }

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
        }

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
    }
}
