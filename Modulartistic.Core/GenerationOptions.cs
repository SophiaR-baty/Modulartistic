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
        
        public int MaxThreads => _maxthreads;
        public bool PrintDebugInfo => _debug;
        public bool KeepAnimationFrames => _keepframes;
        public AnimationFormat AnimationFormat => _aniformat;
        public ILogger? Logger => _logger;
        public IProgressReporter? ProgressReporter => _progressReporter;
        public IPathProvider? PathProvider => _pathProvider;

        public GenerationOptions() 
        {
        }
    }
}
