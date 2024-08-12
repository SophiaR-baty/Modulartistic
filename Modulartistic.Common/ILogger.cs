using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Common
{
    /// <summary>
    /// Defines the contract for logging messages of various severity levels.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a general message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Log(string message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        void LogError(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        void LogWarning(string message);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        void LogInfo(string message);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        void LogDebug(string message);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        void LogException(Exception e);
    }
}
