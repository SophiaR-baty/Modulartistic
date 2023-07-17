using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Modulartistic
{
    internal class Program
    {
        static int Main(string[] argv)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            // If no arguments were given
            if (argv.Length == 0)
            {
                // if the demo file does not already exist, create it
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "demofiles" + Path.DirectorySeparatorChar + "modulartistic_demo"))
                {
                    // add standard generation data
                    GenerationData GD = new GenerationData();
                    GD.Add(new GenerationArgs());

                    // add standard State
                    State S1 = new State();
                    S1.Name = "State_1";
                    GD.Add(S1);

                    // add a StateSequence with 2 States
                    StateSequence SS = new StateSequence();
                    SS.Name = "StateSequence";
                    // create the second state
                    State S2 = new State();
                    S2.Name = "State_2";
                    S2.ColorMinimum = 360;
                    // create the 2 scenes and add them
                    Scene SC1 = new Scene(S1, 3, "Linear");
                    Scene SC2 = new Scene(S2, 3, "Linear");
                    SS.Scenes.Add(SC1);
                    SS.Scenes.Add(SC2);
                    // add the StateSequence
                    GD.Add(SS);

                    // Add a StateTimeline
                    StateTimeline ST = new StateTimeline();
                    // Add a Base State (standard state)
                    State Base = new State();
                    Base.Name = "Base";
                    ST.Base = Base;
                    // Make the timeline 5 seconds long
                    ST.Length = 5000;
                    // Add one StateEvent
                    StateEvent SE = new StateEvent();
                    SE.StartTime = 1000;
                    SE.AttackTime = 200;
                    SE.DecayTime = 200;
                    SE.ReleaseTime = 600;
                    SE.PeakValues.Add(StateProperty.ColorMinimum, 360);
                    SE.Length = 1000;
                    ST.Events.Add(SE);
                    ST.Name = "StateTimeline";
                    GD.Add(ST);

                    // Save the demo file
                    GD.Name = "modulartistic_demo";
                    GD.SaveJson();
                }

                // and print usage
                PrintUsage();
                return 1;
            }
            
            // if the first argument is generate
            if (argv[0] == "generate")
            {
                // if there is nothing after the generate
                if (argv.Length == 1)
                {
                    // if the demo file does not already exist, create it
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "demofiles" + Path.DirectorySeparatorChar + "modulartistic_demo"))
                    {
                        // add standard generation data
                        GenerationData GD = new GenerationData();
                        GD.Add(new GenerationArgs());

                        // add standard State
                        State S1 = new State();
                        S1.Name = "State_1";
                        GD.Add(S1);

                        // add a StateSequence with 2 States
                        StateSequence SS = new StateSequence();
                        SS.Name = "StateSequence";
                        // create the second state
                        State S2 = new State();
                        S2.Name = "State_2";
                        S2.ColorMinimum = 360;
                        // create the 2 scenes and add them
                        Scene SC1 = new Scene(S1, 3, "Linear");
                        Scene SC2 = new Scene(S2, 3, "Linear");
                        SS.Scenes.Add(SC1);
                        SS.Scenes.Add(SC2);
                        // add the StateSequence
                        GD.Add(SS);

                        // Add a StateTimeline
                        StateTimeline ST = new StateTimeline();
                        // Add a Base State (standard state)
                        State Base = new State();
                        Base.Name = "Base";
                        ST.Base = Base;
                        // Make the timeline 5 seconds long
                        ST.Length = 5000;
                        // Add one StateEvent
                        StateEvent SE = new StateEvent();
                        SE.StartTime = 1000;
                        SE.AttackTime = 200;
                        SE.DecayTime = 200;
                        SE.ReleaseTime = 600;
                        SE.PeakValues.Add(StateProperty.ColorMinimum, 360);
                        SE.Length = 1000;
                        ST.Events.Add(SE);
                        ST.Name = "StateTimeline";
                        GD.Add(ST);

                        // Save the demo file
                        GD.Name = "modulartistic_demo";
                        GD.SaveJson();
                    }

                    // then generate for the demo file
                    Console.WriteLine("Generating Demo File...");
                    GenerationData gd = new GenerationData();
                    gd.LoadJson(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Output" + Path.DirectorySeparatorChar + "modulartistic_demo.json");

                    gd.GenerateAll(GenerationDataFlags.None);
                    Console.WriteLine("Done!");

                    // And Print Help
                    PrintUsage();
                    stopwatch.Stop();
                    Console.WriteLine("Program took " + stopwatch.Elapsed.ToString());
                    return 0;
                }

                // if there is any --help or -h or -? after the generate
                else if (argv[1..].Contains("--help") || argv[1..].Contains("-h") || argv[1..].Contains("-?"))
                {
                    PrintUsage();
                    return 0;
                }

                // if the second argument is a filename
                else if (argv[1].EndsWith(".json") && File.Exists(argv[1]))
                {
                    // if there are no more arguments after that
                    if (argv.Length == 2)
                    {
                        // Generate Specified GenerationData
                        Console.WriteLine("Generating Images and Animations for {0}. ", argv[1]);
                        GenerationData gd = new GenerationData();
                        gd.LoadJson(argv[1]);

                        gd.GenerateAll(GenerationDataFlags.None);
                        Console.WriteLine("Done!");
                        stopwatch.Stop();
                        Console.WriteLine("Program took " + stopwatch.Elapsed.ToString());
                        return 0;
                    }
                    
                    // if there is a valid directory name
                    if (Directory.Exists(argv[2]))
                    {
                        // if there are no more arguments after that
                        if (argv.Length == 3)
                        {
                            // Generate Specified GenerationData
                            Console.WriteLine("Generating Images and Animations for {0} in {1}. ", argv[1], argv[2]);
                            GenerationData gd = new GenerationData();
                            gd.LoadJson(argv[1]);

                            gd.GenerateAll(GenerationDataFlags.None, argv[2]);
                            Console.WriteLine("Done!");
                            stopwatch.Stop();
                            Console.WriteLine("Program took " + stopwatch.Elapsed.ToString());
                            return 0;
                        }

                        // if there are flags after that
                        else
                        {
                            GenerationDataFlags flags = GenerationDataFlags.None;
                            for (int i = 3; i < argv.Length; i++)
                            {
                                if (argv[i] == "--show") { flags |= GenerationDataFlags.Show; }
                                else if (argv[i] == "--debug") { flags |= GenerationDataFlags.Debug; }
                                else if (argv[i] == "--faster") { flags |= GenerationDataFlags.Faster; }
                                else
                                {
                                    Console.Error.WriteLine("Unrecognized Flag: {0} \nUse -? for help. ");
                                    return 1;
                                }
                            }
                            // Generate Specified GenerationData
                            Console.WriteLine("Generating Images and Animations for {0} in {1}. ", argv[1], argv[2]);
                            GenerationData gd = new GenerationData();
                            gd.LoadJson(argv[1]);

                            gd.GenerateAll(flags, argv[2]);
                            Console.WriteLine("Done!");
                            stopwatch.Stop();
                            Console.WriteLine("Program took " + stopwatch.Elapsed.ToString());
                            return 0;
                        }
                    }

                    // if there are flags after that
                    else
                    {
                        
                        GenerationDataFlags flags = GenerationDataFlags.None;
                        for (int i = 2; i < argv.Length; i++)
                        {
                            if (argv[i] == "--show") { flags |= GenerationDataFlags.Show; }
                            else if (argv[i] == "--debug") { flags |= GenerationDataFlags.Debug; }
                            else if (argv[i] == "--faster") { flags |= GenerationDataFlags.Faster; }
                            else
                            {
                                Console.Error.WriteLine("Unrecognized Flag: {0} \nUse -? for help. ");
                                return 1;
                            }
                        }

                        // Generate Specified GenerationData
                        Console.WriteLine("Generating Images and Animations for {0}. ", argv[1]);
                        GenerationData gd = new GenerationData();
                        gd.LoadJson(argv[1]);

                        gd.GenerateAll(flags);
                        Console.WriteLine("Done!");
                        stopwatch.Stop();
                        Console.WriteLine("Program took " + stopwatch.Elapsed.ToString());
                        return 0;
                    }
                }
                
                // if there are valid flags
                else
                {
                    // if the demo file does not already exist, create it
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "demofiles" + Path.DirectorySeparatorChar + "modulartistic_demo"))
                    {
                        // add standard generation data
                        GenerationData GD = new GenerationData();
                        GD.Add(new GenerationArgs());

                        // add standard State
                        State S1 = new State();
                        S1.Name = "State_1";
                        GD.Add(S1);

                        // add a StateSequence with 2 States
                        StateSequence SS = new StateSequence();
                        SS.Name = "StateSequence";
                        // create the second state
                        State S2 = new State();
                        S2.Name = "State_2";
                        S2.ColorMinimum = 360;
                        // create the 2 scenes and add them
                        Scene SC1 = new Scene(S1, 3, "Linear");
                        Scene SC2 = new Scene(S2, 3, "Linear");
                        SS.Scenes.Add(SC1);
                        SS.Scenes.Add(SC2);
                        // add the StateSequence
                        GD.Add(SS);

                        // Add a StateTimeline
                        StateTimeline ST = new StateTimeline();
                        // Add a Base State (standard state)
                        State Base = new State();
                        Base.Name = "Base";
                        ST.Base = Base;
                        // Make the timeline 5 seconds long
                        ST.Length = 5000;
                        // Add one StateEvent
                        StateEvent SE = new StateEvent();
                        SE.StartTime = 1000;
                        SE.AttackTime = 200;
                        SE.DecayTime = 200;
                        SE.ReleaseTime = 600;
                        SE.PeakValues.Add(StateProperty.ColorMinimum, 360);
                        SE.Length = 1000;
                        ST.Events.Add(SE);
                        ST.Name = "StateTimeline";
                        GD.Add(ST);

                        // Save the demo file
                        GD.Name = "modulartistic_demo";
                        GD.SaveJson();
                    }

                    GenerationDataFlags flags = GenerationDataFlags.None;
                    for (int i = 1; i < argv.Length; i++)
                    {
                        if (argv[i] == "--show") { flags |= GenerationDataFlags.Show; }
                        else if (argv[i] == "--debug") { flags |= GenerationDataFlags.Debug; }
                        else if (argv[i] == "--faster") { flags |= GenerationDataFlags.Faster; }
                        else
                        {
                            Console.Error.WriteLine("Unrecognized Flag: {0} \nUse -? for help. ");
                            return 1;
                        }
                    }

                    // then generate for the demo file
                    Console.WriteLine("Generating Demo File...");
                    GenerationData gd = new GenerationData();
                    gd.LoadJson(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Output" + Path.DirectorySeparatorChar + "modulartistic_demo.json");

                    gd.GenerateAll(flags);
                    Console.WriteLine("Done!");
                    stopwatch.Stop();
                    Console.WriteLine("Program took " + stopwatch.Elapsed.ToString());
                    return 0;
                }
            }

            // if the first argument is template
            if (argv[0] == "template")
            {
                Console.Error.WriteLine("Not implemented yet...");
                return 1;
            }

            return 1;

            if (argv.Length == 1)
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

            Console.WriteLine("Modulartistic.exe <generationData.json> <path to folder>");
            Console.WriteLine("Generates all images and animations defined in generationData.json. They will be created at the specified folder. \n");

            Console.WriteLine("Modulartistic.exe <midiFile.mid> <timeline_template.json>");
            Console.WriteLine("Creates an animation for a specified midi File based on a given template. \n");

            Console.WriteLine("Modulartistic.exe <midiFile.mid> <timeline_template.json> <path to folder>");
            Console.WriteLine("Creates an animation for a specified midi File based on a given template in the specified folder. \n");

            Console.WriteLine("For more information on how to create templates and generationData visit <URL here>");
        }
    }
}
