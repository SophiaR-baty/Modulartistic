﻿using Spectre.Console;
using CommandLine;
using FFMpegCore.Arguments;
using Json.Schema;
using Modulartistic.Core;
using CommandLine.Text;
using FFMpegCore;
using FFMpegCore.Helpers;
using Antlr4.Runtime.Misc;
using Modulartistic.AudioGeneration;
using System.Diagnostics;

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
                errorcode = Parser.Default.ParseArguments<ConfigOptions, GenerateOptions, AudioGenerateOptions>(args)
                .MapResult(
                    (ConfigOptions opts) => RunConfigAndReturnExitCode(opts),
                    (GenerateOptions opts) => RunGenerateAndReturnExitCode(opts),
                    (AudioGenerateOptions opts) => RunAudioGenerateAndReturnExitCode(opts),
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
                    Core.Progress? loopProgress = null;
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
                            GenerationData generationData = GenerationData.FromFile(file);

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


                    double elapsed = tasks["fileloop"].ElapsedTime.Value.TotalMilliseconds;
                    Logger.LogInfo($"Time elapsed: {elapsed}");
                }
            );

            return errorcode;
        }




        [Verb("audio-visualize", HelpText = "generate for audio")]
        class AudioGenerateOptions
        {
            [Option('d', "debug", Default = false, HelpText = "Prints useful information while generating")]
            public bool Debug { get; set; }

            [Option('B', "decibel", Default = false, HelpText = "Use decibel scale for audio values")]
            public bool DecibelScale { get; set; }

            [Option('k', "keepframes", Default = false, HelpText = "Saves individual frames for Animations")]
            public bool KeepFrames { get; set; }

            [Option('f', "faster", Default = -1, HelpText = "Speeds up generating images by utilizing multiple threads. 1 and 0 both utilize 1 thread, -1 will use the maximum available")]
            public int? Faster { get; set; }

            [Option('o', "output", Default = ".", HelpText = "Path to the directory where output files should be saved to")]
            public string OutputDirectory { get; set; }

            [Option('i', "input", Required = true, HelpText = "one or more input files, must be valid json files or audio files")]
            public IEnumerable<string> InputFiles { get; set; }
        }

        private static int RunAudioGenerateAndReturnExitCode(AudioGenerateOptions options)
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
                    Core.Progress? fileloopProgress = null;
                    { fileloopProgress = reporter.AddTask("fileloop", "Generating all files...", length); }

                    bool jsonTemplateLoaded = false;
                    foreach (string file in files)
                    {
                        try
                        {
                            if (Path.GetExtension(file) == ".json")
                            {
                                // load json template
                                jsonTemplateLoaded = true;
                            }
                            else
                            {
                                // initializing builder
                                AudioAnimationBuilder builder = new AudioAnimationBuilder(file);

                                #region debug values for builder

                                // configuring builder (implement json template)
                                builder.State = new State()
                                {
                                    X0 = 100,
                                    Y0 = 100,
                                    ColorBlueValue = 0,
                                    ColorGreenSaturation = 0,
                                    ColorRedHue = 0,
                                };
                                builder.Options = new StateOptions()
                                {
                                    Framerate = 3,
                                    FunctionBlueValue = "if(y < 50/2*(GetSubBass(int(frame + x/3))/0.059932906 + GetBass(int(frame + x/3))/0.14595887), 400, 0)",
                                    FunctionGreenSaturation = "if(y < 50/3*(GetLowerMidrange(int(frame + x/3))/0.01345104 + GetMidrange(int(frame + x/3))/0.017425036 + GetUpperMidrange(int(frame + x/3))/0.008142932), 400, 0)",
                                    FunctionRedHue = "if(y < 50/2*(GetPresence(int(frame + x/3))/0.005128569 + GetBrilliance(int(frame + x/3))/0.17560594), 400, 0)",
                                    UseRGB = true,
                                    Height = 200,
                                    Width = 200,
                                };

                                #endregion

                                // seting generation options
                                GenerationOptions opts = new GenerationOptions()
                                {
                                    KeepAnimationFrames = options.KeepFrames,
                                    Logger = Logger,
                                    MaxThreads = options.Faster ?? -1,
                                    AnimationFormat = AnimationFormat.Mp4,
                                    PathProvider = PathProvider,
                                    PrintDebugInfo = options.Debug,
                                    ProgressReporter = reporter,
                                };

                                bool decibelScale = options.DecibelScale;

                                builder.PrintDebug(opts, decibelScale);

                                if (true || jsonTemplateLoaded)
                                {
                                    // creating animation
                                    Task<string> task = builder.GenerateAnimation(opts, decibelScale, options.OutputDirectory);
                                    task.Wait();

                                    // merging with sound
                                    // ffmpeg -i audio_1.mp4 -i audio.mp3 -c:v copy -c:a aac -strict experimental output.mp4
                                    try
                                    {
                                        string tmpName = $"vid_{Path.GetFileNameWithoutExtension(task.Result)}";

                                        Logger?.LogInfo("Start merging video with audio");
                                        ProcessStartInfo startInfo = new ProcessStartInfo
                                        {
                                            FileName = PathProvider.GetFFmpegPath(),
                                            Arguments = $"-i \"{task.Result}\" -i \"{file}\" -c:v copy -c:a aac -strict experimental {tmpName}.mp4 -y",
                                            RedirectStandardOutput = false,
                                            RedirectStandardError = false,
                                            UseShellExecute = false,
                                            CreateNoWindow = true
                                        };
                                        Logger?.LogDebug($"{PathProvider.GetFFmpegPath()} -i \"{task.Result}\" -i \"{file}\" -c:v copy -c:a aac -strict experimental {tmpName}.mp4 -y");
                                        Process? process = Process.Start(startInfo);
                                        process?.WaitForExit();

                                        if (process?.ExitCode != 0)
                                        {
                                            Logger?.LogError($"Process exited with Code {process?.ExitCode}");
                                            Logger?.LogDebug($"{process?.StandardError.ReadToEnd()}");
                                            Logger?.LogError($"{process?.StandardOutput.ReadToEnd()}");
                                        }
                                        else
                                        {
                                            Logger?.LogDebug($"Process exited with Code {process?.ExitCode}");
                                            File.Move($"{tmpName}.mp4", task.Result, true);
                                        }
                                        process?.Dispose();

                                    }
                                    catch (Exception ex)
                                    {
                                        Logger?.LogException(ex);
                                    }
                                }
                            }
                            fileloopProgress?.IncrementProgress();
                        }
                        catch (Exception e)
                        {
                            if (options.Debug)
                            {
                                Logger?.LogException(e);
                            }
                            else
                            {
                                Logger?.LogError(e.Message);
                            }
                        }
                    }
                    reporter.RemoveTask(fileloopProgress);
                });
            
            

            return errorcode;
        }

    }
}
