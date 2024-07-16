using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulartistic.Core;
using Spectre.Console;

namespace Modulartistic
{
    internal class ProgressReporter : IProgressReporter
    {
        public event EventHandler<double>? ProgressChanged;

        private ProgressContext _progressContext;
        private Stack<ProgressTask> _tasks;
        

        public ProgressReporter(ProgressContext ctx) 
        { 
            _progressContext = ctx;
            _tasks = new Stack<ProgressTask?>();

            ProgressChanged += (sender, e) => {  };
        }

        public void AddProgressTask(string description, double maxValue = 100)
        {
            ProgressTask task = _progressContext.AddTask(description, maxValue: maxValue);
            ProgressChanged += (sender, e) => task.Value(e);

            _tasks.Push(task);
        }

        public bool IsFinished => _tasks.Count == 0;

        public void ReportProgress(double progress)
        {
            if (_tasks.Count != 0 && _tasks.Peek().IsFinished) 
            {
                ProgressTask task = _tasks.Pop();
                ProgressChanged -= (sender, e) => task.Value(e);
            }

            ProgressChanged?.Invoke(this, progress);
        }
    }
}
