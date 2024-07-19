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

        public Progress RemoveTask(string key)
        {
            if (key == null) return null;
            
            Progress p;
            if (Tasks.TryGetValue(key, out p))
            {
                p.ProgressChanged -= (s, e) => ProgressChanged?.Invoke(this, p);
                Tasks.Remove(key);
                TaskRemoved?.Invoke(this, p);
                return p;
            }
            else { return null; }
            
        }
    }
}
