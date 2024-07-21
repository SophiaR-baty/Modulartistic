using Modulartistic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic
{
    internal class ProgressReporter : IProgressReporter
    {
        private Dictionary<string, Progress> _tasks;
        public Dictionary<string, Progress> Tasks { get => _tasks; private set => _tasks = value; }

        public event EventHandler<Progress>? TaskAdded;
        public event EventHandler<Progress>? TaskRemoved;
        public event EventHandler<Progress>? ProgressChanged;

        public ProgressReporter() 
        { 
            _tasks = new Dictionary<string, Progress>();
        }

        public Progress AddTask(string key, string description, double maxProgress = 100)
        {
            Progress p = new Progress(key, description, maxProgress);
            p.ProgressChanged += (s, e) => ProgressChanged?.Invoke(this, p);
            Tasks.Add(key, p);
            TaskAdded?.Invoke(this, p);
            return p;
        }

        public void RemoveTask(Progress? progress)
        {
            if (progress is null) return;
            progress.ProgressChanged -= (s, e) => ProgressChanged?.Invoke(this, progress);
            Tasks.Remove(progress.Key);
            TaskRemoved?.Invoke(this, progress);
        }
    }
}
