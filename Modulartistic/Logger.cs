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
            AnsiConsole.MarkupInterpolated($"{message}\n");
        }

        public void LogDebug(string message)
        {
            AnsiConsole.MarkupInterpolated($"[dim]{message}[/]\n");
        }

        public void LogError(string message)
        {
            AnsiConsole.MarkupInterpolated($"[red]{message}[/]\n");
        }

        public void LogException(Exception e)
        {
            AnsiConsole.WriteException(e);
        }

        public void LogInfo(string message)
        {
            AnsiConsole.MarkupInterpolated($"[blue]{message}[/]\n");
        }

        public void LogWarning(string message)
        {
            AnsiConsole.MarkupInterpolated($"[yellow]{message}[/]\n");
        }
    }
}
