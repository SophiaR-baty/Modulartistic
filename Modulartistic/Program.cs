using NCalc;
using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Modulartistic
{
    internal class Program
    {
        static int Main(string[] argv)
        {
            if (argv.Length == 0 || argv[0] == "-?")
            {
                GenerationData GD = new GenerationData();
                GD.Add(new GenerationArgs());

                State S1 = new State();
                S1.Name = "State_1";
                GD.Add(S1);


                StateSequence SS = new StateSequence();
                SS.Name = "StateSequence";
                State S2 = new State();
                S2.Name = "State_2";
                S2.Mod = 100;
                Scene SC1 = new Scene(S1, 3, "Linear");
                Scene SC2 = new Scene(S2, 3, "Linear");
                SS.Scenes.Add(SC1);
                SS.Scenes.Add(SC2);
                GD.Add(SS);

                StateTimeline ST = new StateTimeline();
                State Base = new State();
                Base.Name = "Base";
                ST.Base = Base;
                ST.Length = 5000;
                ST.Events.Add(new StateEvent());
                ST.Name = "StateTimeline";
                GD.Add(ST);

                GD.SaveJson("Output\\modulartistic_demo.json");
                
                
                PrintUsage();
                return 1;
            }

            else if (argv.Length == 1)
            {
                if (argv[0].EndsWith(".json") && File.Exists(argv[0]))
                {
                    GenerationData gd = new GenerationData();
                    gd.LoadJson(argv[0]);

                    gd.GenerateAll();
                    Console.WriteLine("Done!");
                    return 0;
                }
                else
                {
                    PrintUsage();
                    return 1;
                }
            }

            else if (argv.Length == 2)
            {
                if (argv[0] == "test" && argv[1].EndsWith(".json") && File.Exists(argv[1]))
                {
                    StateTimelineTemplate template = StateTimelineTemplate.LoadJson(argv[1]);
                    template.GenerateTests("", Path.GetFileNameWithoutExtension(argv[1]) + "_tests");
                }
                
                
                if (argv[0].EndsWith(".json") && File.Exists(argv[0]) && Directory.Exists(argv[1]))
                {
                    GenerationData gd = new GenerationData();
                    gd.LoadJson(argv[0]);

                    gd.GenerateAll(argv[1]);
                    Console.WriteLine("Done");
                    return 0;
                }
                else if (argv[0].EndsWith(".mid") && File.Exists(argv[0]) && argv[1].EndsWith(".json") && File.Exists(argv[1]))
                {
                    StateTimelineTemplate template = StateTimelineTemplate.LoadJson(argv[1]);
                    string filename = Modulartistic.Helper.ValidFileName("Output\\" + Path.GetFileNameWithoutExtension(argv[0]));
                    MidiAnimationCreator.GenerateJson(argv[0], template, filename + ".json");

                    Console.WriteLine("Json has been created. \nCreate Animation now? (y/n)");
                    if (Console.Read() == 'y')
                    {
                        MidiAnimationCreator.GenerateAnimation(argv[0], template);
                        Console.WriteLine("Done!");
                    }
                    else
                    {
                        Console.WriteLine("The Animation can be created later using the generated Json file.");
                    }
                    return 0;
                }
                else
                {
                    PrintUsage();
                    return 1;
                }
            }

            else if (argv.Length == 3)
            {
                if (
                    argv[0].EndsWith(".mid") && File.Exists(argv[0])
                    && argv[1].EndsWith(".json") && File.Exists(argv[1])
                    && Directory.Exists(argv[2])
                    )
                {
                    StateTimelineTemplate template = StateTimelineTemplate.LoadJson(argv[1]);
                    MidiAnimationCreator.GenerateJson(argv[0], template, argv[2]);

                    Console.WriteLine("Json has been created. \nCreate Animation now? (y/n)");
                    if (Console.Read() == 'y')
                    {
                        MidiAnimationCreator.GenerateAnimation(argv[0], template, argv[2]);
                        Console.WriteLine("Done!");
                    }
                    else
                    {
                        Console.WriteLine("The Animation can be created later using the generated Json file.");
                    }
                    return 0;
                }
            }

            PrintUsage();
            return 1;
        }

        public static void PrintUsage()
        {
            Console.WriteLine("Usage: <not implemented>");
        }
    }
}
