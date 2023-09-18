using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic
{
    class ConfigCommand : ICommand
    {
        public ConfigCommand(string[] strings)
        {
        }

        public async Task<ErrorCode> Execute()
        {
            throw new NotImplementedException();
        }

        public void ParseArguments(string[] args)
        {
            throw new NotImplementedException();
        }

        public void PrintHelp()
        {
            throw new NotImplementedException();
        }
    }
}
