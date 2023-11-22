using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Modulartistic.Core;

namespace Modulartistic
{
    public class GenerateCommand : ICommand
    {
        private bool faster;
        private bool mp4;
        private bool show;
        private bool debug;
        private bool keepframes;
        private List<string> filenames_json;
        private string output_dir;

        private ErrorCode error_code;

        public GenerateCommand(string[] args) : this()
        {
            ParseArguments(args);
        }
        
        public GenerateCommand()
        {
            faster = false;
            mp4 = false;
            show = false;
            debug = false;
            keepframes = false;
            filenames_json = new List<string>();
            output_dir = PathConfig.OUTPUTFOLDER;

            error_code = 0;
        }

        public async Task<ErrorCode> Execute()
        {
            if (error_code != ErrorCode.Success) { return error_code; }

            GenerationDataFlags flags = GenerationDataFlags.None;
            if (show) { flags |= GenerationDataFlags.Show; }
            if (debug) { flags |= GenerationDataFlags.Debug; }
            if (faster) { flags |= GenerationDataFlags.Faster; }
            if (mp4) { flags |= GenerationDataFlags.MP4; }
            if (keepframes) { flags |= GenerationDataFlags.KeepFrames; }

            if (filenames_json.Count == 0)
            {
                Helper.CreateDemos();

                string demofolder = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "demofiles";
                filenames_json.AddRange(Directory.GetFiles(demofolder));

                Console.WriteLine("Generating Images and Animations for demofiles. ");
            }

            foreach (string filename in filenames_json)
            {
                Console.WriteLine($"Generating Images and Animations for {filename} in {output_dir}. ");

                GenerationData gd = new GenerationData();
                try
                {
                    gd.LoadJson(filename);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Error loading the json file {filename}. ");
                    Console.Error.WriteLine(e.Message);
                    return ErrorCode.JsonParsingError;
                }

                try
                {
                    await gd.GenerateAll(flags, output_dir);
                    gd.SaveJson(output_dir);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Error while generating images and animation for {filename}. ");
                    Console.Error.WriteLine(e.Message);
                    return ErrorCode.GenerationError;
                }
            }
            
            Console.WriteLine("Done!");
            return error_code;
        }

        public void ParseArguments(string[] args)
        {
            bool accept_file = true;
            bool accept_dir = true;
            bool accept_flags = true;

            if (args.Contains("--help") || args.Contains("-h") || args.Contains("-?"))
            {
                PrintHelp();
                error_code = ErrorCode.Help;
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (accept_file && File.Exists(arg) && arg.EndsWith(".json"))
                {
                    filenames_json.Add(arg);
                    continue;
                }
                if (accept_dir && Directory.Exists(arg))
                {
                    output_dir = arg;
                    accept_file = false;
                    accept_dir = false;
                    continue;
                }
                if (accept_flags)
                {
                    if (arg == "--debug") { debug = true; }
                    else if (arg == "--faster") { faster = true; }
                    else if (arg == "--show") { show = true; }
                    else if (arg == "--mp4") { mp4 = true; }
                    else if (arg == "--keepframes") { keepframes = true; }
                    else
                    {
                        Console.Error.WriteLine($"Unexpected Flag or Argument: {arg} \nUse -? for help. ");
                        error_code = ErrorCode.UnexpectedArgument;
                        return;
                    }

                    accept_file = false;
                    accept_dir = false;
                    continue;
                }

                Console.Error.WriteLine($"Unexpected Flag or Argument: {arg} \nUse -? for help. ");
                error_code = ErrorCode.UnexpectedArgument;
                return;
            }
        }

        public void PrintHelp()
        {
            Console.WriteLine("generate [<generationData.json>] [output_directory] [<flags>]");
            Console.WriteLine("the 'generate' command will generate images and animations from 'generationData.json' files. \n");
            
            Console.WriteLine("[<generationData.json>] - 0 or more json files containing generationData");
            Console.WriteLine("[output_directory]      - a single existing directory to generate images and animations in. If omitted they will be generated in '{app_location}/Output'. ");
            Console.WriteLine("[<flags>]               - 0 or more flags. ");
            Console.WriteLine("argument types must be specified in this order! \n");

            Console.WriteLine("flags: ");
            Console.WriteLine(" --debug   - Print additional information");
            Console.WriteLine(" --faster  - Use multiple cores");
            Console.WriteLine(" --show    - Open generated images and animations");
            Console.WriteLine(" --mp4     - Generate mp4 instead of gif \n");

            Console.WriteLine("Visit https://github.com/MaxGeo543/Modulartistic for more information. \n");
        }
    }
}
