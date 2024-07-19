using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message);
        void LogWarning(string message);
        void LogInfo(string message);
        void LogDebug(string message);
        void LogException(Exception e);
    }
}
