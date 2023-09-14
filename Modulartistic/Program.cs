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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            
            Helper.CreateDirectories();
            ICommand command = new HelpCommand(); ;
            
            if (argv.Length == 0)
            {
                return (int)(await command.Execute());
            }

            // if first argument is generate
            if (argv[0] == "generate")
            {
                command = new GenerateCommand(argv[1..]);
            }

            // if the first argument is midi-animation
            if (argv[0] == "midi-animation")
            {
                command = new MidiAnimationCommand();
                //if (argv[0].EndsWith(".mid") && File.Exists(argv[0]) && argv[1].EndsWith(".json") && File.Exists(argv[1]))
                //{
                //    StateTimelineTemplate template = StateTimelineTemplate.LoadJson(argv[1]);
                //    MidiAnimationCreator.GenerateJson(argv[0], template);

                //    Console.WriteLine("Json has been created. \nCreate Animation now? (y?)");
                //    if (Console.Read() == 'y')
                //    {
                //        MidiAnimationCreator.GenerateAnimation(argv[0], template);
                //        Console.WriteLine("Done!");
                //    }
                //    else
                //    {
                //        Console.WriteLine("The Animation can be created later using the generated Json file.");
                //    }
                //    return 0;
                //}
                Console.Error.WriteLine("Not implemented yet...");
                return 1;
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
