using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Modulartistic.Core
{
    public static class PathConfig
    {
        // Folders configuration -> put this in a config file
        

        private static string? _output_folder;
        private static string? _input_folder;
        private static string? _addon_folder;
        private static string? _demo_folder;
        private static string? _ffmpeg_folder;

        public static string OUTPUTFOLDER { get => _output_folder ?? Constants.OUTPUT_FOLDER_DEFAULT; set => _output_folder = value; }
        public static string INPUTFOLDER { get => _input_folder ?? Constants.INPUT_FOLDER_DEFAULT; set => _input_folder = value; }
        public static string ADDONFOLDER { get => _addon_folder ?? Constants.ADDONS_FOLDER_DEFAULT; set => _addon_folder = value; }
        public static string DEMOFOLDER { get => _demo_folder ?? Constants.DEMOS_FOLDER_DEFAULT; set => _demo_folder = value; }
        public static string FFMPEGFOLDER { get => _ffmpeg_folder ?? Constants.FFMPEG_FOLDER_DEFAULT; set => _ffmpeg_folder = value; }

        public static void LoadConfigurationFile(string abs_file_path)
        {
            // checking if file exists
            if (!File.Exists(abs_file_path))
            {
                throw new FileNotFoundException($"The specified File {abs_file_path} does not exist. ");
            }

            // reads json into JsonDocument object
            string jsontext = File.ReadAllText(abs_file_path);
            JsonDocument jd = JsonDocument.Parse(jsontext);

            JsonElement root = jd.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new Exception($"Error: Expected Object RootElement in Json File {abs_file_path} but got {root.ValueKind.ToString()}");
            }
            
            foreach (JsonProperty jprop in root.EnumerateObject())
            {
                if (jprop.Name == Constants.OUTPUT_CONFIG_KEY) { _output_folder = jprop.Value.GetString(); }
                else if (jprop.Name == Constants.INPUT_CONFIG_KEY) { _input_folder = jprop.Value.GetString(); }
                else if (jprop.Name == Constants.DEMO_CONFIG_KEY) { _demo_folder = jprop.Value.GetString(); }
                else if (jprop.Name == Constants.ADDON_CONFIG_KEY) { _addon_folder = jprop.Value.GetString(); }
                else if (jprop.Name == Constants.FFMPEG_CONFIG_KEY) { _ffmpeg_folder = jprop.Value.GetString(); }
                else { throw new Exception($"Invalid Property for Configuration File: {jprop.Name}"); }
            }
        }

        public static void SaveConfigurationFile(string abs_file_path)
        {
            JsonObject obj = new JsonObject();
            if (_output_folder != null && _output_folder != Constants.OUTPUT_FOLDER_DEFAULT) obj.Add(Constants.OUTPUT_CONFIG_KEY, _output_folder);
            if (_input_folder != null && _input_folder != Constants.INPUT_FOLDER_DEFAULT) obj.Add(Constants.INPUT_CONFIG_KEY, _input_folder);
            if (_demo_folder != null && _demo_folder != Constants.DEMOS_FOLDER_DEFAULT) obj.Add(Constants.DEMO_CONFIG_KEY, _demo_folder);
            if (_addon_folder != null && _addon_folder != Constants.ADDONS_FOLDER_DEFAULT) obj.Add(Constants.ADDON_CONFIG_KEY, _addon_folder);
            if (_ffmpeg_folder != null && _ffmpeg_folder != Constants.FFMPEG_FOLDER_DEFAULT) obj.Add(Constants.FFMPEG_CONFIG_KEY, _ffmpeg_folder);
            File.WriteAllText(abs_file_path, obj.ToJsonString());
        }
    }
}
