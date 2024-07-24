using MathNet.Numerics;
using NAudio.Wave;
using NAudio.Dsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

namespace Modulartistic.AudioGeneration
{
    internal class AudioAnalysis
    {
        private string _filepath;
        private Frequencies[] _frequencies;
        private float[] _volumes;
        private TimeSpan _audio_length;


        public Frequencies[] Frequencies { get => _frequencies;  }
        public float[] Volumes { get => _volumes; }
        public TimeSpan AudioLength { get => _audio_length; }

        
        
        public AudioAnalysis(string filePath, int framerate)
        {
            _filepath = filePath;

            using (var reader = new AudioFileReader(_filepath))
            {
                _audio_length = reader.TotalTime;
                int datalength = (int)(_audio_length.TotalSeconds * framerate);
                _frequencies = new Frequencies[datalength];
                _volumes = new float[datalength];

                for (int i = 0; i < datalength; i++)
                {
                    _volumes[i] = GetVolumeAtTimestamp(reader, TimeSpan.FromMilliseconds(1000/framerate*i));
                    _frequencies[i] = GetFrequenciesAtTimestamp(reader, TimeSpan.FromMilliseconds(1000/framerate*i));
                }
            }
        }

        private float GetVolumeAtTimestamp(AudioFileReader reader, TimeSpan timestamp)
        {
            int sampleRate = reader.WaveFormat.SampleRate;
            int channels = reader.WaveFormat.Channels;

            // Move to the desired timestamp
            reader.CurrentTime = timestamp;

            float[] buffer = new float[sampleRate * channels];
            int samplesRead = reader.Read(buffer, 0, buffer.Length);

            // Calculate RMS (Root Mean Square) to get volume
            double sum = 0;
            for (int i = 0; i < samplesRead; i++)
            {
                sum += buffer[i] * buffer[i];
            }
            double rms = Math.Sqrt(sum / samplesRead);

            // Convert to decibels
            double decibels = 20 * Math.Log10(rms);

            return (float)decibels;
        }

        private Frequencies GetFrequenciesAtTimestamp(AudioFileReader reader, TimeSpan timestamp)
        {
            int sampleRate = reader.WaveFormat.SampleRate;
            int channels = reader.WaveFormat.Channels;

            // Move to the desired timestamp
            reader.CurrentTime = timestamp;

            float[] buffer = new float[sampleRate * channels];
            int samplesRead = reader.Read(buffer, 0, buffer.Length);

            // Perform FFT (Fast Fourier Transform) for frequency analysis
            Complex[] complexBuffer = new Complex[samplesRead];
            for (int i = 0; i < samplesRead; i++)
            {
                complexBuffer[i].X = (float)(buffer[i] * FastFourierTransform.HannWindow(i, samplesRead));
                complexBuffer[i].Y = 0;
            }

            FastFourierTransform.FFT(true, (int)Math.Log(samplesRead, 2.0), complexBuffer);

            float[] frequencies = new float[samplesRead / 2];
            for (int i = 0; i < samplesRead / 2; i++)
            {
                frequencies[i] = (float)Math.Sqrt(complexBuffer[i].X * complexBuffer[i].X + complexBuffer[i].Y * complexBuffer[i].Y);
            }

            // Define the frequency bands
            var freq = new Frequencies(frequencies, sampleRate);

            return freq;
        }

        
    }
}
