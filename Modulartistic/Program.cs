using Spectre.Console;
using CommandLine;
using FFMpegCore.Arguments;
using Json.Schema;
using Modulartistic.Core;
using CommandLine.Text;
using FFMpegCore;
using FFMpegCore.Helpers;
using Antlr4.Runtime.Misc;

namespace Modulartistic
{
    internal class Program
    {
        static readonly CommandLinePathProvider PathProvider = new CommandLinePathProvider();
        static readonly Logger Logger = new Logger();

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ConfigOptions,GenerateOptions>(args)
                .MapResult(
                    (ConfigOptions opts) => RunConfigAndReturnExitCode(opts), 
                    (GenerateOptions opts) => RunGenerateAndReturnExitCode(opts),
                    errs => 1);
        }

        [Verb("config", HelpText = "configure paths for the application")]
        class ConfigOptions
        {
            [Option("ffmpeg-path", HelpText = "Sets the path to the FFmpeg binary")]
            public string ?FFmpegPath { get; set; }

            [Option("addons-path", HelpText = "Sets the path to directories AddOns are located in")]
            public string ?AddOnsPath { get; set; }

            [Option("show-config", Default = false, HelpText = "List all configured options")]
            public bool ShowConfig { get; set; }
        }

        private static int RunConfigAndReturnExitCode(ConfigOptions opts)
        {
            // sets FFmpeg Path
            if (opts.FFmpegPath is not null)
            {
                try
                {
                    PathProvider.SetFFmpegPath(opts.FFmpegPath);
                }
                catch (FileNotFoundException e)
                {
                    AnsiConsole.Markup($"[red]{e.Message}[/]");
                }
            }

            // sets FFmpeg Path
            if (opts.AddOnsPath is not null)
            {
                try
                {
                    PathProvider.SetAddonPath(opts.AddOnsPath);
                }
                catch (DirectoryNotFoundException e)
                {
                    AnsiConsole.Markup($"[red]{e.Message}[/]");
                }
            }

            // list configs
            if (opts.ShowConfig)
            {
                AnsiConsole.Markup($"[blue]AddOns: [/]{PathProvider.GetAddonPath()}\n");

                try
                {
                    AnsiConsole.Markup($"[blue]FFmpeg: [/]{PathProvider.GetFFmpegPath()}\n");
                }
                catch (Exception e)
                {
                    AnsiConsole.Markup($"[red]{e.Message}[/]\n");
                }

                AnsiConsole.Markup($"[blue]Demo: [/]{PathProvider.GetDemoPath()}\n");
            }

            return 0;
        }



        [Verb("generate", HelpText = "generate a <generation_data>.json file")]
        class GenerateOptions
        {
            [Option('d', "debug", Default = false, HelpText = "Prints useful information while generating")]
            public bool Debug { get; set; }

            [Option('k', "keepframes", Default = false, HelpText = "Saves individual frames for Animations")]
            public bool KeepFrames { get; set; }

            [Option('f', "faster", Default = -1, HelpText = "Speeds up generating images by utilizing multiple threads. 1 and 0 both utilize 1 thread, -1 will use the maximum available")]
            public int? Faster {  get; set; }

            [Option('a', "animation-format", Default = "gif", HelpText = "What file format to save animations in (mp4 or gif)")]
            public string AnimationFormat { get; set; }

            [Option('o', "output", Default = ".", HelpText = "Path to the directory where output files should be saved to")]
            public string OutputDirectory {  get; set; }

            [Option('i', "input", Required = true, HelpText = "one or more input files, must be valid json files")]
            public IEnumerable<string> InputFiles { get; set; }
        }

        private static int RunGenerateAndReturnExitCode(GenerateOptions options)
        {
            // setting global options
            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = Path.GetDirectoryName(PathProvider.GetFFmpegPath()) });
            
            string[] files = options.InputFiles.ToArray();
            // checking if input files specified
            if (files.Length == 0)
            {
                Logger.LogError("No input files specified. please specify at least one input file to generate using the -i or --input option. ");
                return -1;
            }

            int errorcode = 0;
            ProgressReporter reporter = new ProgressReporter();
            AnsiConsole.Progress()
                .HideCompleted(true)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),    // Task description
                    new ProgressBarColumn(),        // Progress bar
                    new PercentageColumn(),         // Percentage
                    new RemainingTimeColumn(),      // Remaining time
                    new SpinnerColumn()
                })
                .Start(
                ctx =>
                {
                    
                    Dictionary<string, ProgressTask> tasks = new Dictionary<string, ProgressTask>();
                    reporter.TaskAdded += (sender, args) => 
                    {
                        ProgressTask a = ctx.AddTask(args.Description, true, args.MaxProgress);
                        tasks.Add(args.Key, a);
                    };
                    reporter.ProgressChanged += (sender, args) => { tasks[args.Key].Value = args.CurrentProgress; };
                    reporter.TaskRemoved += (sender, args) => { tasks[args.Key].StopTask(); };

                    int length = files.Length;
                    Core.Progress? loopProgress = null;
                    if (length > 1) { reporter.AddTask("fileloop", "Generating all files...", length); }
                    foreach (string file in files)
                    {
                        if (!File.Exists(file))
                        {
                            Logger.LogError($"The specified file {file} does not exist. ");
                            Logger.LogInfo($"Skipping file {file}");
                            loopProgress?.IncrementProgress();
                            continue;
                        }

                        try
                        {
                            GenerationData generationData = GenerationData.FromFile(file);

                            GenerationOptions genOptions = new GenerationOptions()
                            {
                                KeepAnimationFrames = options.KeepFrames,
                                AnimationFormat = options.AnimationFormat == "mp4" ? AnimationFormat.Mp4 : AnimationFormat.Gif,
                                MaxThreads = options.Faster ?? 1,
                                PrintDebugInfo = options.Debug,
                                Logger = new Logger(),
                                ProgressReporter = reporter,
                            };

                            Task task = generationData.GenerateAll(genOptions, options.OutputDirectory);
                            task.Wait();

                            loopProgress?.IncrementProgress();
                        }
                        catch (Exception ex)
                        {
                            if (options.Debug) { Logger.LogException(ex); } else { Logger.LogError(ex.Message); }
                            Logger.LogInfo($"Skipping file {file}");
                            loopProgress?.IncrementProgress();
                            continue;
                        }
                    }
                    reporter.RemoveTask(loopProgress?.Key);



                }
            );


            

            return 0;

        }

    }
}
