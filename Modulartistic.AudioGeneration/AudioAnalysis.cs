using MathNet.Numerics;
using NAudio.Wave;
using NAudio.Dsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AudioGeneration
{
    internal class AudioAnalysis
    {
        private int _bufferSize;
        private int _fft_buffer;
        private TimeSpan _audio_length;
        private int _audio_buffer_size;
        
        public AudioAnalysis() { }

        public float GetVolumeAtTimestamp(string filePath, TimeSpan timestamp)
        {
            using (var reader = new AudioFileReader(filePath))
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
        }

        public float[] GetFrequencyAtTimestamp(string filePath, TimeSpan timestamp)
        {
            using (var reader = new AudioFileReader(filePath))
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

                return frequencies;
            }
        }
    }
}
