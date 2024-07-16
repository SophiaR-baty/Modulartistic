using Spectre.Console;
using CommandLine;
using FFMpegCore.Arguments;
using Json.Schema;
using Modulartistic.Core;
using CommandLine.Text;
using FFMpegCore;
using FFMpegCore.Helpers;

namespace Modulartistic
{
    internal class Program
    {
        static readonly CommandLinePathProvider PathProvider = new CommandLinePathProvider();

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
            [Option('d', "debug", HelpText = "Prints useful information while generating")]
            public bool Debug { get; set; }

            [Option('k', "keepframes", HelpText = "Saves individual frames for Animations")]
            public bool KeepFrames { get; set; }

            [Option("faster", HelpText = "Speeds up generating images by utilizing multiple threads. 1 and 0 both utilize 1 thread, -1 will use the maximum available", Default = -1)]
            public int? Faster {  get; set; }

            [Option('f', "animation-format", HelpText = "What file format to save animations in (mp4 or gif)")]
            public string AnimationFormat { get; set; }

            [Option('o', "output", HelpText = "Path to the directory where output files should be saved to")]
            public string OutputDirectory {  get; set; }

            [Option('i', "input", HelpText = "one or more input files, must be valid json files")]
            public IEnumerable<string> InputFiles { get; set; }
        }

        private static int RunGenerateAndReturnExitCode(GenerateOptions options)
        {
            // setting global options
            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = PathProvider.GetFFmpegPath() });

            foreach (string file in options.InputFiles) 
            {
                try
                {
                    GenerationData generationData = GenerationData.FromFile(file);
                    Task task = generationData.GenerateAll(options.OutputDirectory);
                    task.Wait();
                }
                catch (Exception ex) 
                {
                    if (options.Debug)
                    {
                        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                    }
                    else
                    {
                        // AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                        AnsiConsole.MarkupInterpolated($"[red]{ex.Message}[/]");
                    }
                    
                    // AnsiConsole.Markup($"[red]{ex.Message}[/]");
                }
            }

            return 0;

        }

    }
}
