using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public interface IProgressReporter
    {
        public Dictionary<string, Progress> Tasks { get; }

        public event EventHandler<Progress> TaskAdded;
        public event EventHandler<Progress> TaskRemoved;
        public event EventHandler<Progress> ProgressChanged;

        public Progress AddTask(string key, string description, double maxProgress=100);
        public Progress RemoveTask(string key);
    }

    public class Progress
    {
        public event EventHandler<double> ProgressChanged;
        public event EventHandler TaskFinished;

        private double _maxprogress;
        private double _progress;
        private string _key;
        private string _description;

        public double MaxProgress { get => _maxprogress; }
        public double CurrentProgress { get => _progress; }
        public bool HasFinished { get; private set; }
        public string Description { get => _description; }
        public string Key { get => _key; }

        public Progress(string key, string description, double maxProgress=100)
        {
            _key = key;
            _description = description;
            _maxprogress = maxProgress;
            _progress = 0;
            HasFinished = false;
            ProgressChanged += (s, progress) => 
            { 
                if (progress >= _maxprogress) { TaskFinished?.Invoke(this, new EventArgs()); }
                HasFinished = true;
            };
        }

        public void SetProgress(double progress)
        {
            _progress = progress;
            ProgressChanged?.Invoke(this, progress);
        }

        public void IncrementProgress()
        {
            _progress++;
            ProgressChanged?.Invoke(this, _progress);
        }
    }
}
