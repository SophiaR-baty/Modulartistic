using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AudioGeneration
{
    public class AudioFrame
    {
        private float _peakMax;
        private float _peakMin;
        private bool _isbeat;
        private float[] _frequencybands;

        public AudioFrame()
        {
            _peakMax = 0;
            _peakMin = 0;
            _isbeat = false;
            _frequencybands = new float[6];
        }

        public bool Isbeat { get => _isbeat; set => _isbeat = value; }
        public float[] Frequencybands { get => _frequencybands; set => _frequencybands = value; }
        public float PeakMax { get => _peakMax; set => _peakMax = value; }
        public float PeakMin { get => _peakMin; set => _peakMin = value; }
    }
}
