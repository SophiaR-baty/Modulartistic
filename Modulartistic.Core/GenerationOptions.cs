using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public class GenerationOptions
    {
        private int _maxthreads;
        private bool _debug;
        private bool _keepframes;
        private AnimationFormat _aniformat;
        private ILogger? _logger;
        private IProgressReporter? _progressReporter;
        private IPathProvider? _pathProvider;

        public int MaxThreads
        {
            get => _maxthreads;
            set => _maxthreads = value;
        }

        public bool PrintDebugInfo
        {
            get => _debug;
            set => _debug = value;
        }

        public bool KeepAnimationFrames
        {
            get => _keepframes;
            set => _keepframes = value;
        }

        public AnimationFormat AnimationFormat
        {
            get => _aniformat;
            set => _aniformat = value;
        }

        public ILogger? Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public IProgressReporter? ProgressReporter
        {
            get => _progressReporter;
            set => _progressReporter = value;
        }

        public IPathProvider? PathProvider
        {
            get => _pathProvider;
            set => _pathProvider = value;
        }

        public GenerationOptions() 
        {
        }
    }
}
