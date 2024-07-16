using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public interface IPathProvider
    {
        public string GetAddonPath();
        public string GetDemoPath();
        public string GetFFmpegPath();
    }
}
