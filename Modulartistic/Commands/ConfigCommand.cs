using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulartistic.Core;

namespace Modulartistic
{
    class ConfigCommand : ICommand
    {
        private ErrorCode error_code;


        public ConfigCommand(string[] args) : this()
        {
            ParseArguments(args);
        }

        public ConfigCommand()
        {
            error_code = 0;
        }


        public async Task<ErrorCode> Execute()
        {
            if (error_code != ErrorCode.Success)
            {
                return error_code;
            }
            
            PathConfig.SaveConfigurationFile(Constants.CONFIG_FILE_PATH);
            return error_code;
        }

        public void ParseArguments(string[] args)
        {
            CurrentParameter current_pm = CurrentParameter.ExpectKey;

            if (args.Contains("--help") || args.Contains("-h") || args.Contains("-?"))
            {
                PrintHelp();
                error_code = ErrorCode.Help;
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (current_pm)
                {
                    case CurrentParameter.ExpectKey:
                        {
                            if (arg == "--output" || arg == "-o") 
                            { 
                                current_pm = CurrentParameter.ExpectOutputPath; 
                                break;
                            }
                            if (arg == "--input" || arg == "-i")
                            {
                                current_pm = CurrentParameter.ExpectInputPath;
                                break;
                            }
                            if (arg == "--addons" || arg == "-a")
                            {
                                current_pm = CurrentParameter.ExpectAddonsPath;
                                break;
                            }
                            if (arg == "--demos" || arg == "-d")
                            {
                                current_pm = CurrentParameter.ExpectDemosPath;
                                break;
                            }
                            if (arg == "--ffmpeg" || arg == "-f")
                            {
                                current_pm = CurrentParameter.ExpectFFmpegPath;
                                break;
                            }

                            Console.Error.WriteLine($"Unexpected Flag or Argument: {arg} \nUse -? for help. ");
                            error_code = ErrorCode.UnexpectedArgument;
                            return;
                        }
                    case CurrentParameter.ExpectOutputPath:
                        {
                            if (!Directory.Exists(arg))
                            {
                                Console.Error.WriteLine($"Directory not found: {arg} \nUse -? for help. ");
                                error_code = ErrorCode.DirectoryNotFound;
                                return;
                            }

                            PathConfig.OUTPUTFOLDER = arg;
                            current_pm = CurrentParameter.ExpectKey;
                            break;
                        }
                    case CurrentParameter.ExpectInputPath:
                        {
                            if (!Directory.Exists(arg))
                            {
                                Console.Error.WriteLine($"Directory not found: {arg} \nUse -? for help. ");
                                error_code = ErrorCode.DirectoryNotFound;
                                return;
                            }

                            PathConfig.INPUTFOLDER = arg;
                            current_pm = CurrentParameter.ExpectKey;
                            break;
                        }
                    case CurrentParameter.ExpectAddonsPath:
                        {
                            if (!Directory.Exists(arg))
                            {
                                Console.Error.WriteLine($"Directory not found: {arg} \nUse -? for help. ");
                                error_code = ErrorCode.DirectoryNotFound;
                                return;
                            }

                            PathConfig.ADDONFOLDER = arg;
                            current_pm = CurrentParameter.ExpectKey;
                            break;
                        }
                    case CurrentParameter.ExpectDemosPath:
                        {
                            if (!Directory.Exists(arg))
                            {
                                Console.Error.WriteLine($"Directory not found: {arg} \nUse -? for help. ");
                                error_code = ErrorCode.DirectoryNotFound;
                                return;
                            }

                            PathConfig.DEMOFOLDER = arg;
                            current_pm = CurrentParameter.ExpectKey;
                            break;
                        }
                    case CurrentParameter.ExpectFFmpegPath:
                        {
                            if (!Directory.Exists(arg))
                            {
                                Console.Error.WriteLine($"Directory not found: {arg} \nUse -? for help. ");
                                error_code = ErrorCode.DirectoryNotFound;
                                return;
                            }

                            PathConfig.FFMPEGFOLDER = arg;
                            current_pm = CurrentParameter.ExpectKey;
                            
                            break;
                        }
                    default:
                        break;
                }

                if (error_code == ErrorCode.UnexpectedArgument) { }
            }

                
        }

        public void PrintHelp()
        {
            Console.WriteLine("config [<key> <path>]");
            Console.WriteLine("the 'config' command will set default paths. \n");

            Console.WriteLine("<midifile.mid>          - the key of the path to configure");
            Console.WriteLine("<path>                  - a single absolute filepath");
            Console.WriteLine("argument types must be specified in this order! \n");

            Console.WriteLine("keys: ");
            Console.WriteLine(" --output or -o  - configure the default output directory");
            Console.WriteLine(" --input or -i   - configure the default input directory");
            Console.WriteLine(" --addons or -a  - configure the default addons directory");
            Console.WriteLine(" --demos or -d   - configure the default demos directory");
            Console.WriteLine(" --ffmpeg or -f  - set the location of ffmpeg binaries \n");

            Console.WriteLine("Visit https://github.com/MaxGeo543/Modulartistic for more information. \n");
        }

        private enum CurrentParameter
        {
            ExpectKey,
            ExpectOutputPath,
            ExpectInputPath,
            ExpectAddonsPath,
            ExpectDemosPath,
            ExpectFFmpegPath,
        }
    }
}
