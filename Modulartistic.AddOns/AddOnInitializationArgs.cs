using Modulartistic.Common;

namespace Modulartistic.AddOns
{
    public class AddOnInitializationArgs
    {
        public ILogger? Logger { get; set; }
        public IProgressReporter? ProgressReporter { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public uint Framerate { get; set; }
        public Guid Guid { get; set; }

        public AddOnInitializationArgs() { }

    }
}
