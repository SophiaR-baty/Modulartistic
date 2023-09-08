using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic
{
    interface ICommand
    {
        public void ParseArguments(string[] args);

        public void PrintHelp();

        public Task<ErrorCode> Execute();
    }
}
