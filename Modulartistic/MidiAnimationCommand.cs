using Modulartistic.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic
{
    class MidiAnimationCommand : ICommand
    {
        private string midi_path;
        private string json_path;
        private string out_path;

        private bool faster;
        private bool mp4;
        private bool show;
        private bool debug;
        private bool keepframes;
        private bool createjson;

        private ErrorCode error_code;

        public MidiAnimationCommand(string[] args) : this()
        {
            ParseArguments(args);
        }
        public MidiAnimationCommand()
        {
            midi_path = "";
            json_path = "";
            out_path = "";

            faster = false;
            mp4 = false;
            show = false;
            debug = false;
            keepframes = false;
            createjson = false;

            error_code = ErrorCode.Success;
        }
        
        public async Task<ErrorCode> Execute()
        {
            if (error_code != ErrorCode.Success) { return error_code; }

            GenerationDataFlags flags = GenerationDataFlags.None;
            if (show) { flags |= GenerationDataFlags.Show; }
            if (debug) { flags |= GenerationDataFlags.Debug; }
            if (faster) { flags |= GenerationDataFlags.Faster; }
            if (mp4) { flags |= GenerationDataFlags.MP4; }
            if (keepframes) { flags |= GenerationDataFlags.KeepFrames; }

            GenerationData GD = new GenerationData();
            StateTimelineTemplate STT;
            if (json_path == "") { STT = StateTimelineTemplate.GetDefaultTemplate(); }
            else { STT = StateTimelineTemplate.LoadJson(json_path); }
            StateTimeline ST = MidiAnimationCreator.CreateStateTimeline(midi_path, STT);
            GD.Add(STT.Metadata);
            GD.Add(ST);

            if (createjson) { GD.SaveJson(out_path); }
            else { await GD.GenerateAll(flags, out_path); }
            
            return error_code;
        }

        public void ParseArguments(string[] args)
        {
            bool expect_midi = true;
            bool accept_json = false;
            bool accept_outdir = false;
            bool accept_flags = false;

            if (args.Contains("--help") || args.Contains("-h") || args.Contains("-?"))
            {
                PrintHelp();
                error_code = ErrorCode.Help;
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (expect_midi && File.Exists(arg) && arg.EndsWith(".mid"))
                {
                    midi_path = arg;
                    accept_json = true;
                    accept_outdir = true;
                    accept_flags = true;
                    continue;
                }
                if (accept_json && File.Exists(arg) && arg.EndsWith(".json"))
                {
                    json_path = arg;
                    accept_json = false;
                    continue;
                }
                if (accept_outdir && Directory.Exists(arg))
                {
                    out_path = arg;
                    accept_json = false;
                    accept_outdir = false;
                }
                if (accept_flags)
                {
                    if (arg == "--debug") { debug = true; }
                    else if (arg == "--faster") { faster = true; }
                    else if (arg == "--show") { show = true; }
                    else if (arg == "--mp4") { mp4 = true; }
                    else if (arg == "--keepframes") { keepframes = true; }
                    else if (arg == "--json") { createjson = true; }
                    else
                    {
                        Console.Error.WriteLine($"Unexpected Flag or Argument: {arg} \nUse -? for help. ");
                        error_code = ErrorCode.UnexpectedArgument;
                        return;
                    }

                    accept_json = false;
                    accept_outdir = false;
                }

                Console.Error.WriteLine($"Unexpected Flag or Argument: {arg} \nUse -? for help. ");
                error_code = ErrorCode.UnexpectedArgument;
                return;
            }
        }

        public void PrintHelp()
        {
            throw new NotImplementedException();
        }
    }
}
