using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AudioGeneration
{
    internal class Frequencies
    {
        /// <summary>
        /// 20-60Hz
        /// </summary>
        public float SubBass { get; set; }

        /// <summary>
        /// 60-250Hz
        /// </summary>
        public float Bass { get; set; }

        /// <summary>
        /// 250-500Hz
        /// </summary>
        public float LowMidrange { get; set; }

        /// <summary>
        /// 500-2000Hz
        /// </summary>
        public float Midrange { get; set; }

        /// <summary>
        /// 2-4kHz
        /// </summary>
        public float HighMidrange { get; set; }

        /// <summary>
        /// 4-6kHz
        /// </summary>
        public float Presence { get; set; }

        /// <summary>
        /// 6-20kHz (also Treble)
        /// </summary>
        public float Brilliance { get; set; }

        public Frequencies() { }

        public Frequencies(float[] frequencies, int sampleRate) 
        {
            SubBass = GetAveragePowerInBand(frequencies, sampleRate, 20, 60);
            Bass = GetAveragePowerInBand(frequencies, sampleRate, 60, 250);
            LowMidrange = GetAveragePowerInBand(frequencies, sampleRate, 250, 500);
            Midrange = GetAveragePowerInBand(frequencies, sampleRate, 500, 2000);
            HighMidrange = GetAveragePowerInBand(frequencies, sampleRate, 2000, 4000);
            Presence = GetAveragePowerInBand(frequencies, sampleRate, 4000, 6000);
            Brilliance = GetAveragePowerInBand(frequencies, sampleRate, 6000, 20000);
        }

        private float GetAveragePowerInBand(float[] frequencies, int sampleRate, int minFreq, int maxFreq)
        {
            int minIndex = (int)((minFreq / (float)sampleRate) * frequencies.Length * 2);
            int maxIndex = (int)((maxFreq / (float)sampleRate) * frequencies.Length * 2);

            float sum = 0;
            int count = 0;
            for (int i = minIndex; i < maxIndex && i < frequencies.Length; i++)
            {
                sum += frequencies[i];
                count++;
            }

            return count > 0 ? sum / count : 0;
        }
    }
}
