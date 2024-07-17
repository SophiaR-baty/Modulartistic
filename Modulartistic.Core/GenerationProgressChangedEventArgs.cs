using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public class GenerationProgressChangedEventArgs : EventArgs
    {
        public GenerationProgressChangedEventArgs(long progress) 
        { 
            GenerationProgress = progress;
        }

        public long GenerationProgress { get; set; }
    }
}
