using System;
using System.IO;

namespace Modulartistic
{
    internal class Program
    {
        static int Main(string[] argv)
        {
            if (argv.Length == 0 || argv[0] == "-?")
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Output" + Path.DirectorySeparatorChar + "modulartistic_demo"))
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
                    GD.Name = "modulartistic_demo";
                    GD.SaveJson();
                }
                                
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
                    MidiAnimationCreator.GenerateJson(argv[0], template);

                    Console.WriteLine("Json has been created. \nCreate Animation now? (y?)");
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
            Console.WriteLine("Usage: ");
            Console.WriteLine("Modulartistic.exe <generationData.json>");
            Console.WriteLine("Generates all images and animations defined in generationData.json. They will be created at Output folder. \n");
           
            Console.WriteLine("Modulartistic.exe test <timeline_template.json>");
            Console.WriteLine("Generates tests for a timeline template defined in timeline_template.json. They will be created at Output folder. \n");

            Console.WriteLine("Modulartistic.exe test <generationData.json> <path to folder>");
            Console.WriteLine("Generates all images and animations defined in generationData.json. They will be created at the specified folder. \n");

            Console.WriteLine("Modulartistic.exe <midiFile.json> <timeline_template.json>");
            Console.WriteLine("Creates an animation for a specified midi File based on a given template. \n");

            Console.WriteLine("Modulartistic.exe <midiFile.json> <timeline_template.json> <path to folder>");
            Console.WriteLine("Creates an animation for a specified midi File based on a given template in the specified folder. \n");

            Console.WriteLine("For more information on how to create templates and generationData visit <URL here>");
        }
    }
}
