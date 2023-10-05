using FFMpegCore;
using Modulartistic.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modulartistic
{
    internal class Program
    {
        static async Task<int> Main(string[] argv)
        {
            // FFMpegCore.GlobalFFOptions.Configure(new FFMpegCore.FFOptions { BinaryFolder = @"D:\Downloads\ffmpeg-2023-09-07-git-9c9f48e7f2-full_build\bin", TemporaryFilesFolder = "/tmp" });
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            
            Helper.CreateDirectories();
            ICommand command = new HelpCommand(); ;
            try
            {
                PathConfig.LoadConfigurationFile(Constants.CONFIG_FILE_PATH);
            }
            catch { }

            GlobalFFOptions.Configure(options => options.BinaryFolder = PathConfig.FFMPEGFOLDER);


            if (argv.Length == 0)
            {
                return (int)(await command.Execute());
            }

            // if first argument is generate
            if (argv[0] == "generate")
            {
                command = new GenerateCommand(argv[1..]);
            }

            if (argv[0] == "config")
            {
                command = new ConfigCommand(argv[1..]);
            }

            // if the first argument is midi-animation
            if (argv[0] == "midi-animation")
            {
                command = new MidiAnimationCommand(argv[1..]);
            }

            // if the first argument is test
            // test is used to test a StateTimelineTemplate
            if (argv[0] == "test")
            {
                Console.Error.WriteLine("Not implemented yet...");
                
                
                //if (argv[0] == "test" && argv[1].EndsWith(".json") && File.Exists(argv[1]))
                //{
                //    StateTimelineTemplate template = StateTimelineTemplate.LoadJson(argv[1]);
                //    template.GenerateTests("", Path.GetFileNameWithoutExtension(argv[1]) + "_tests");
                //}
                 

                return 1;
            }

            return (int)(await command.Execute());
        }
    }
}
