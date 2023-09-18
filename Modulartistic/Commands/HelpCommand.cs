using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic
{
    class HelpCommand : ICommand
    {
        public async Task<ErrorCode> Execute()
        {
            PrintHelp();
            return ErrorCode.Help;
        }

        public void ParseArguments(string[] args)
        {
            // maybe add arguments to get help about specific things
            throw new NotImplementedException();
        }

        public void PrintHelp()
        {
            Helper.CreateDemos();
            Helper.PrintUsage();
        }
    }
}
