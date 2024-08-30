using Spectre.Console;
using CommandLine;
using FFMpegCore.Arguments;
using Json.Schema;
using Modulartistic.Core;
using CommandLine.Text;
using FFMpegCore;
using FFMpegCore.Helpers;
using Antlr4.Runtime.Misc;
using System.Diagnostics;
using Modulartistic.Common;

namespace Modulartistic
{
    internal class Program
    {
        static readonly CommandLinePathProvider PathProvider = new CommandLinePathProvider();
        static Logger? Logger;

        static int Main(string[] args)
        {
            int errorcode = 0;
            try
            {
                Logger = new Logger(PathProvider.GetLogFilePath());
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
                Logger?.Dispose();
                Logger = null;
            }

            // Combine the folder path and file name
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "generationdata_schema.json");
            try
            {
                // Write the content to the file, creating or overwriting as necessary
                File.WriteAllText(filePath, Schemas.GetGenerationDataSchema());
            }
            catch
            {
                Logger?.LogDebug($"Error writing JSON schema to {filePath}");
            }

            try
            {
                errorcode = Parser.Default.ParseArguments<ConfigOptions, GenerateOptions>(args)
                .MapResult(
                    (ConfigOptions opts) => RunConfigAndReturnExitCode(opts),
                    (GenerateOptions opts) => RunGenerateAndReturnExitCode(opts),
                    errs => 1);
            }
            catch (Exception e)
            {
                Logger?.LogException(e);
            }
            

            Logger?.Dispose();
            return errorcode;
        }

        [Verb("config", HelpText = "configure paths for the application")]
        class ConfigOptions
        {
            [Option("ffmpeg-path", HelpText = "Sets the path to the FFmpeg binary")]
            public string? FFmpegPath { get; set; }

            [Option("addons-path", HelpText = "Sets the path to directories AddOns are located in")]
            public string? AddOnsPath { get; set; }

            [Option("logfile-path", HelpText = "Sets the path to the logfile")]
            public string? LogFilePath { get; set; }

            [Option("show-config", Default = false, HelpText = "List all configured options")]
            public bool ShowConfig { get; set; }

            [Option('d', "debug", Default = false, HelpText = "Print Debug Info in case of Errors")]
            public bool Debug { get; set; }
        }

        private static int RunConfigAndReturnExitCode(ConfigOptions opts)
        {
            int errorcode = 0;
            
            // sets logfile path
            if (opts.LogFilePath is not null)
            {
                try
                {
                    PathProvider.SetLogFilePath(opts.LogFilePath);
                    Logger = new Logger(PathProvider.GetLogFilePath());
                }
                catch (Exception e)
                {
                    errorcode = 1;
                    if (Logger == null)
                    {
                        if (opts.Debug)
                        {
                            AnsiConsole.WriteException(e);
                        }
                        else
                        {
                            AnsiConsole.Markup($"[red]{e.Message}[/]");
                        }
                    }
                    else
                    {
                        if (opts.Debug)
                        {
                            Logger.LogError(e.Message);
                        }
                        else
                        {
                            Logger.LogException(e);
                        }
                    }
                }
            }

            // sets FFmpeg Path
            if (opts.FFmpegPath is not null)
            {
                try
                {
                    PathProvider.SetFFmpegPath(opts.FFmpegPath);
                }
                catch (FileNotFoundException e)
                {
                    errorcode = 1;
                    if (Logger == null)
                    {
                        if (opts.Debug)
                        {
                            AnsiConsole.WriteException(e);
                        }
                        else
                        {
                            AnsiConsole.Markup($"[red]{e.Message}[/]");
                        }
                    }
                    else
                    {
                        if (opts.Debug)
                        {
                            Logger.LogError(e.Message);
                        }
                        else
                        {
                            Logger.LogException(e);
                        }
                    }
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
                    errorcode = 1;
                    if (Logger == null)
                    {
                        if (opts.Debug)
                        {
                            AnsiConsole.WriteException(e);
                        }
                        else
                        {
                            AnsiConsole.Markup($"[red]{e.Message}[/]");
                        }
                    }
                    else
                    {
                        if (opts.Debug)
                        {
                            Logger.LogError(e.Message);
                        }
                        else
                        {
                            Logger.LogException(e);
                        }
                    }
                }
            }

            // list configs
            if (opts.ShowConfig)
            {
                try
                {
                    AnsiConsole.Markup($"[blue]AddOns: [/]{PathProvider.GetAddonPath()}\n");
                }
                catch (Exception e)
                {
                    errorcode = 1;
                    if (Logger == null)
                    {
                        if (opts.Debug)
                        {
                            AnsiConsole.WriteException(e);
                        }
                        else
                        {
                            AnsiConsole.Markup($"[red]{e.Message}[/]");
                        }
                    }
                    else
                    {
                        if (opts.Debug)
                        {
                            Logger.LogError(e.Message);
                        }
                        else
                        {
                            Logger.LogException(e);
                        }
                    }
                }

                try
                {
                    AnsiConsole.Markup($"[blue]FFmpeg: [/]{PathProvider.GetFFmpegPath()}\n");
                }
                catch (Exception e)
                {
                    errorcode = 1;
                    if (Logger == null)
                    {
                        if (opts.Debug)
                        {
                            AnsiConsole.WriteException(e);
                        }
                        else
                        {
                            AnsiConsole.Markup($"[red]{e.Message}[/]");
                        }
                    }
                    else
                    {
                        if (opts.Debug)
                        {
                            Logger.LogError(e.Message);
                        }
                        else
                        {
                            Logger.LogException(e);
                        }
                    }
                }

                try
                {
                    AnsiConsole.Markup($"[blue]BackUp: [/]{PathProvider.GetBackUpPath()}\n");
                }
                catch (Exception e)
                {
                    errorcode = 1;
                    if (Logger == null)
                    {
                        if (opts.Debug)
                        {
                            AnsiConsole.WriteException(e);
                        }
                        else
                        {
                            AnsiConsole.Markup($"[red]{e.Message}[/]");
                        }
                    }
                    else
                    {
                        if (opts.Debug)
                        {
                            Logger.LogError(e.Message);
                        }
                        else
                        {
                            Logger.LogException(e);
                        }
                    }
                }

                try
                {
                    AnsiConsole.Markup($"[blue]Log File: [/]{PathProvider.GetLogFilePath()}\n");
                }
                catch (Exception e)
                {
                    errorcode = 1;
                    if (Logger == null)
                    {
                        if (opts.Debug)
                        {
                            AnsiConsole.WriteException(e);
                        }
                        else
                        {
                            AnsiConsole.Markup($"[red]{e.Message}[/]");
                        }
                    }
                    else
                    {
                        if (opts.Debug)
                        {
                            Logger.LogError(e.Message);
                        }
                        else
                        {
                            Logger.LogException(e);
                        }
                    }
                }
            }

            return errorcode;
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
            Logger.WriteDebugToConsole = options.Debug;
            
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
                    Common.Progress? loopProgress = null;
                    { loopProgress = reporter.AddTask("fileloop", "Generating all files...", length); }
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
                            GenerationOptions genOptions = new GenerationOptions()
                            {
                                KeepAnimationFrames = options.KeepFrames,
                                AnimationFormat = options.AnimationFormat == "mp4" ? AnimationFormat.Mp4 : AnimationFormat.Gif,
                                MaxThreads = options.Faster ?? 1,
                                PrintDebugInfo = options.Debug,
                                Logger = Logger,
                                ProgressReporter = reporter,
                                PathProvider = PathProvider,
                            };

                            GenerationData generationData = GenerationData.FromFile(file, genOptions);

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
                    reporter.RemoveTask(loopProgress);

                    TimeSpan elapsed = tasks["fileloop"].ElapsedTime.Value;
                    Logger.LogInfo($"Time elapsed: {elapsed.ToString("hh':'mm':'ss':'ffff")}");
                }
            );

            return errorcode;
        }
    }
}
