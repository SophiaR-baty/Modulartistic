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
        private string filename_json;
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
            filename_json = "demofiles/state_example_1.json";
            output_dir = "output";

            error_code = 0;
        }

        public async Task<ErrorCode> Execute()
        {
            if (error_code != ErrorCode.Success) { return error_code; }

            GenerationDataFlags flags = GenerationDataFlags.None;
            if (show) { flags |= GenerationDataFlags.Show; }
            else if (debug) { flags |= GenerationDataFlags.Debug; }
            else if (faster) { flags |= GenerationDataFlags.Faster; }
            else if (mp4) { flags |= GenerationDataFlags.MP4; }

            Console.WriteLine($"Generating Images and Animations for {filename_json} in {output_dir}. ");

            GenerationData gd = new GenerationData();
            try
            {
                gd.LoadJson(filename_json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error loading the json file {filename_json}. ");
                Console.Error.WriteLine(e.Message);
                return ErrorCode.JsonParsingError;
            }

            try
            {
                await gd.GenerateAll(flags, output_dir);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error while generating. ");
                Console.Error.WriteLine(e.Message);
                return ErrorCode.GenerationError;
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
                Helper.PrintUsage();
                error_code = ErrorCode.Help;
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (accept_file && File.Exists(arg) && arg.EndsWith(".json"))
                {
                    filename_json = arg;
                    accept_file = false;
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
                    else
                    {
                        Console.Error.WriteLine($"Unexpected Flag or Argument: {arg} \nUse -? for help. ");
                        error_code = ErrorCode.UnexpectedArgument;
                        return;
                    }
                }
            }
        }
    }
}
