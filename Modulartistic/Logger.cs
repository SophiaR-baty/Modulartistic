using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulartistic.Core;
using Spectre.Console;
using static System.Net.Mime.MediaTypeNames;

namespace Modulartistic
{
    internal class Logger : ILogger, IDisposable
    {
        private string _file;
        private StreamWriter _writer;

        public bool WriteDebugToConsole {  get; set; }

        public Logger(string logfilepath, bool writeDebugToConsole = false)
        {
            if (!File.Exists(logfilepath))
            {
                File.Create(logfilepath);
            }
            _file = logfilepath;
            _writer = new StreamWriter(_file, true);
        }
        
        public void Dispose()
        {
            _writer.Write("---------------------------------------------\n");
            _writer.Close();
            _writer.Dispose();
        }

        public void Log(string message)
        {
            AnsiConsole.MarkupInterpolated($"{message}\n");
            WriteToLogFile($"     LOG: {message}");
        }

        public void LogDebug(string message)
        {
            if (WriteDebugToConsole) AnsiConsole.MarkupInterpolated($"[dim]{message}[/]\n");
            WriteToLogFile($"   DEBUG: {message}");
        }

        public void LogError(string message)
        {
            AnsiConsole.MarkupInterpolated($"[red]{message}[/]\n");
            WriteToLogFile($"   ERROR: {message}");
        }

        public void LogException(Exception e)
        {
            AnsiConsole.WriteException(e);
            WriteToLogFile($"   ERROR: {e.Message}");
        }

        public void LogInfo(string message)
        {
            AnsiConsole.MarkupInterpolated($"[blue]{message}[/]\n");
            WriteToLogFile($"    INFO: {message}");
        }

        public void LogWarning(string message)
        {
            AnsiConsole.MarkupInterpolated($"[yellow]{message}[/]\n");
            WriteToLogFile($" WARNING: {message}");
        }

        private void WriteToLogFile(string message)
        {
            _writer.Write(DateTime.Now.ToString("yyyyMMdd-HHmmss") + message + "\n");
        }
    }
}
