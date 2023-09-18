using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

#nullable enable

namespace Modulartistic
{
    internal static class ConfigurationManager
    {
        // Folders configuration -> put this in a config file
        private static string _output_folder_default = AppDomain.CurrentDomain.BaseDirectory + "output";
        private static string _input_folder_default = AppDomain.CurrentDomain.BaseDirectory + "input";
        private static string _addon_folder_default = AppDomain.CurrentDomain.BaseDirectory + "addons";
        private static string _demo_folder_default = AppDomain.CurrentDomain.BaseDirectory + "demofiles";
        private static string _ffmpeg_folder_default = "";

        private static string? _output_folder; 
        private static string? _input_folder; 
        private static string? _addon_folder; 
        private static string? _demo_folder; 
        private static string? _ffmpeg_folder;

        public static string OUTPUTFOLDER { get => _output_folder ?? _output_folder_default; }
        public static string INPUTFOLDER { get => _input_folder ?? _input_folder_default; }
        public static string ADDONFOLDER { get => _addon_folder ?? _addon_folder_default; }
        public static string DEMOFOLDER { get => _demo_folder ?? _demo_folder_default; }
        public static string FFMPEGFOLDER { get => _ffmpeg_folder_default; }

        public static void LoadConfigurationFile(string abs_file_path)
        {
            if (!File.Exists(abs_file_path))
            {
                throw new FileNotFoundException($"The specified File {abs_file_path} does not exist. ");
            }

            string jsontext = File.ReadAllText(abs_file_path);

            JsonDocument jd = JsonDocument.Parse(jsontext);
        }

        
    }
}
