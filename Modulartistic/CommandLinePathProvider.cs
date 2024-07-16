using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Modulartistic.Core;
using System.Reflection;
using static Modulartistic.Core.Constants;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Modulartistic
{
    /// <summary>
    /// Provides paths and capability to set paths to certain app directories
    /// </summary>
    internal class CommandLinePathProvider : IPathProvider
    {
        private Configuration _config;
        private readonly string _addonPathKey = "addonpath";
        private readonly string _ffmpegPathKey = "ffmpegpath";

        /// <summary>
        /// Creates a new CommandLinePathProvider
        /// </summary>
        public CommandLinePathProvider()
        {
            // load the configuration
            _config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Get the addon directory from app config. If add on directory has not been set, will implicitely set it to {appdir}/addons
        /// </summary>
        /// <returns>path to addons directory</returns>
        public string GetAddonPath()
        {
            // get app directory
            string appdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            if (appdir == string.Empty) { throw new Exception("Encountered error while getting app directory. "); }

            // set path it not currently set
            if (ConfigurationManager.AppSettings[_addonPathKey] == null) 
            { 
                SetAddonPath(Path.Combine(appdir, "addons")); 
            }
            // return path to addon directory
            return _config.AppSettings.Settings[_addonPathKey].Value;
        }

        /// <summary>
        /// Set the addon directory in app config
        /// </summary>
        /// <param name="path">Path to the addon directory. If path is not absolute, assumes current working directory as root</param>
        /// <exception cref="DirectoryNotFoundException">If the directory does not exist or wasn't found</exception>
        public void SetAddonPath(string path)
        {
            // if path is not absolute prepend cwd
            if (!Path.IsPathRooted(path)) { path = Path.Combine(Directory.GetCurrentDirectory(), path); }
            
            // check if directory exists
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The directory {path} does not exist.");
            }
            
            // set directory in config
            SetConfig(_addonPathKey, path);
        }

        /// <summary>
        /// Get the default demo path which is {appdir}/demos. if it doesn't exist, creates it
        /// </summary>
        /// <returns>Path to demo directory</returns>
        public string GetDemoPath()
        {
            // get app directory
            string appdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            if (appdir == string.Empty) { throw new Exception("Encountered error while getting app directory. "); }

            // get directory
            string dir = Path.Combine(appdir, "demos");
            
            // create if doesn't exist
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // return path
            return dir;
        }

        /// <summary>
        /// Get the path to the ffmpeg binary. If no path has been set, tries to find it, if it doesn't, throws an exception
        /// </summary>
        /// <returns>Path to ffmpeg binary</returns>
        /// <exception cref="KeyNotFoundException">If no path has been set and key is null</exception>
        public string GetFFmpegPath()
        {
            // check if key exists
            if (ConfigurationManager.AppSettings[_ffmpegPathKey] == null)
            {
                // try to find ffmpeg
                string ffmpegCommand = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
                string ffmpegPath = ExecuteCommand(ffmpegCommand, "ffmpeg");

                // add if ffmpeg found, otherwise throw an error
                if (ffmpegPath is not null) { SetConfig(_ffmpegPathKey, ffmpegPath); }
                else { throw new KeyNotFoundException($"FFmpeg path has not configured and couldn't be found automatically. "); }
            }
            
            // return path
            return _config.AppSettings.Settings[_ffmpegPathKey].Value;
        }

        /// <summary>
        /// Set path to ffmpeg binary in app config
        /// </summary>
        /// <param name="path">The path to the binary of ffmpeg</param>
        /// <exception cref="FileNotFoundException">If the file ffmpeg.exe is not found</exception>
        public void SetFFmpegPath(string path)
        {
            if (!Path.IsPathRooted(path)) { path = Path.Combine(Directory.GetCurrentDirectory(), path); }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file {path} does not exist.");
            }

            SetConfig(_ffmpegPathKey, path);
        }

        /// <summary>
        /// Checks if a key exists in App Config. If so changes the value, if not adds it
        /// </summary>
        /// <param name="key">key of config</param>
        /// <param name="value">value if config</param>
        private void SetConfig(string key, string value)
        {
            // Check if the key exists
            if (ConfigurationManager.AppSettings[key] != null)
            {
                // Update the value
                _config.AppSettings.Settings[key].Value = value;
            }
            else
            {
                // Add the key-value pair
                _config.AppSettings.Settings.Add(key, value);
            }

            // Save the changes in App.config file
            _config.Save(ConfigurationSaveMode.Modified);

            // Refresh the section to reflect changes in the ConfigurationManager
            ConfigurationManager.RefreshSection("appSettings");
        }

        static string ExecuteCommand(string command, string args)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd().Trim();
                        process.WaitForExit();

                        // Return the first line of the output, which is the path to ffmpeg
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while trying to find ffmpeg: " + ex.Message);
            }

            return null;
        }
    }
}
