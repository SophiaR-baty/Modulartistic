using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    /// <summary>
    /// Defines the contract for providing paths required by the application.
    /// </summary>
    public interface IPathProvider
    {
        /// <summary>
        /// Gets the path to the add-ons directory.
        /// </summary>
        /// <returns>A string representing the path to the add-ons directory.</returns>
        public string GetAddonPath();

        /// <summary>
        /// Gets the path to the backup directory.
        /// </summary>
        /// <returns>A string representing the path to the backup directory.</returns>
        public string GetBackUpPath();

        /// <summary>
        /// Gets the path to the FFmpeg executable.
        /// </summary>
        /// <returns>A string representing the path to the FFmpeg executable.</returns>
        public string GetFFmpegPath();
    }
}
