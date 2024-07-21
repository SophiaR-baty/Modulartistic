using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    /// <summary>
    /// Represents options for configuring the generation of animations and images.
    /// </summary>
    public class GenerationOptions
    {
        #region private Fields

        private int _maxthreads;
        private bool _keepframes;
        private bool _debug;
        private AnimationFormat _aniformat;
        private ILogger? _logger;
        private IProgressReporter? _progressReporter;
        private IPathProvider _pathProvider;
        private Guid _generationDataGuid;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the maximum number of threads to use during generation.
        /// </summary>
        /// <value>
        /// The maximum number of threads. A value of -1 (or another negative value) indicates the use of the maximum number of available threads.
        /// </value>
        public int MaxThreads
        {
            get => _maxthreads;
            set => _maxthreads = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to keep the animation frames after generation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the animation frames should be kept; otherwise, <c>false</c>.
        /// </value>
        public bool KeepAnimationFrames
        {
            get => _keepframes;
            set => _keepframes = value;
        }

        public bool PrintDebugInfo
        {
            get => _debug;
            set => _debug = value;
        }

        /// <summary>
        /// Gets or sets the format for the animation output.
        /// </summary>
        /// <value>
        /// The <see cref="AnimationFormat"/> enum value specifying the format of the generated animation.
        /// </value>
        public AnimationFormat AnimationFormat
        {
            get => _aniformat;
            set => _aniformat = value;
        }

        /// <summary>
        /// Gets or sets the logger used to log messages during generation.
        /// </summary>
        /// <value>
        /// An instance of <see cref="ILogger"/> for logging purposes, or <c>null</c> if no logging is needed.
        /// </value>
        public ILogger? Logger
        {
            get => _logger;
            set => _logger = value;
        }

        /// <summary>
        /// Gets or sets the progress reporter used to report progress during generation.
        /// </summary>
        /// <value>
        /// An instance of <see cref="IProgressReporter"/> for reporting progress, or <c>null</c> if progress reporting is not needed.
        /// </value>
        public IProgressReporter? ProgressReporter
        {
            get => _progressReporter;
            set => _progressReporter = value;
        }

        /// <summary>
        /// Gets or sets the path provider used to manage file paths during generation.
        /// </summary>
        /// <value>
        /// An instance of <see cref="IPathProvider"/> for managing paths, or <c>null</c> if path management is not required.
        /// </value>
        public IPathProvider PathProvider
        {
            get => _pathProvider;
            set => _pathProvider = value;
        }

        /// <summary>
        /// Gets or sets the guid for a GenerationData object
        /// </summary>
        public Guid GenerationDataGuid
        {
            get => _generationDataGuid;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationOptions"/> class.
        /// </summary>
        public GenerationOptions() 
        { 
            _generationDataGuid = Guid.NewGuid();
        }

        #endregion
    }
}
