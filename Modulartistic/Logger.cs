using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulartistic.Core;
using Spectre.Console;

namespace Modulartistic
{
    internal class Logger : ILogger
    {
        public void Log(string message)
        {
            AnsiConsole.MarkupInterpolated($"[yellow]{message}[/]\n");
        }
    }
}
