using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Common
{
    /// <summary>
    /// Defines an interface for reporting progress on tasks, including adding, removing, and tracking progress.
    /// </summary>
    public interface IProgressReporter
    {
        /// <summary>
        /// Gets the collection of current tasks being tracked by the reporter.
        /// </summary>
        /// <value>
        /// A dictionary where the key is a unique identifier for the task and the value is the <see cref="Progress"/> instance representing the task.
        /// </value>
        public Dictionary<string, Progress> Tasks { get; }

        /// <summary>
        /// Occurs when a new task is added to the progress reporter.
        /// </summary>
        /// <remarks>
        /// This event is triggered when a new <see cref="Progress"/> task is added.
        /// </remarks>
        public event EventHandler<Progress> TaskAdded;

        /// <summary>
        /// Occurs when a task is removed from the progress reporter.
        /// </summary>
        /// <remarks>
        /// This event is triggered when a <see cref="Progress"/> task is removed.
        /// </remarks>
        public event EventHandler<Progress> TaskRemoved;

        /// <summary>
        /// Occurs when the progress of a task changes.
        /// </summary>
        /// <remarks>
        /// This event is triggered when the progress value of a <see cref="Progress"/> task is updated.
        /// </remarks>
        public event EventHandler<Progress> ProgressChanged;

        /// <summary>
        /// Adds a new task to the progress reporter.
        /// </summary>
        /// <param name="key">A unique key to identify the task.</param>
        /// <param name="description">A description of the task.</param>
        /// <param name="maxProgress">The maximum progress value for the task (default is 100).</param>
        /// <returns>The <see cref="Progress"/> instance representing the newly added task.</returns>
        public Progress AddTask(string key, string description, double maxProgress=100);

        /// <summary>
        /// Removes a task from the progress reporter.
        /// </summary>
        /// <param name="progress">The <see cref="Progress"/> instance representing the task to be removed.</param>
        public void RemoveTask(Progress progress);
    }

    /// <summary>
    /// Represents a progress tracker with events for progress changes and task completion.
    /// </summary>
    public class Progress
    {
        #region Events

        /// <summary>
        /// Occurs when the progress value changes.
        /// </summary>
        public event EventHandler<double>? ProgressChanged;

        /// <summary>
        /// Occurs when the task is finished.
        /// </summary>
        public event EventHandler? TaskFinished;

        #endregion

        #region private Fields

        private double _maxprogress;
        private double _progress;
        private string _key;
        private string _description;

        private double _updateprogress_at;
        private double _updateprogress;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the maximum value of progress.
        /// </summary>
        public double MaxProgress { get => _maxprogress; }

        /// <summary>
        /// Gets the current progress value.
        /// </summary>
        public double CurrentProgress { get => _progress; }

        /// <summary>
        /// Gets a value indicating whether the task has finished.
        /// </summary>
        public bool HasFinished { get; private set; }

        /// <summary>
        /// Gets the description of the progress.
        /// </summary>
        public string Description { get => _description; }

        /// <summary>
        /// Gets the key associated with the progress.
        /// </summary>
        public string Key { get => _key; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Progress"/> class with the specified key, description, and optional maximum progress value.
        /// </summary>
        /// <param name="key">The unique key for the progress.</param>
        /// <param name="description">A description of the progress.</param>
        /// <param name="maxProgress">The maximum value of progress (default is 100).</param>
        public Progress(string key, string description, double maxProgress=100)
        {
            _key = key;
            _description = description;
            _maxprogress = maxProgress;
            _progress = 0;

            _updateprogress = 0;
            _updateprogress_at = maxProgress/100;

            HasFinished = false;
            ProgressChanged += (s, progress) => 
            { 
                if (progress >= _maxprogress) { TaskFinished?.Invoke(this, new EventArgs()); }
                HasFinished = true;
            };
        }

        #endregion

        #region Methods for changing Progress

        /// <summary>
        /// Sets the current progress value and raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        /// <param name="progress">The new progress value.</param>
        public void SetProgress(double progress)
        {
            _progress = progress;
            _updateprogress = progress;


            while (_updateprogress >= _updateprogress_at)
            {
                _updateprogress -= _updateprogress_at;
                ProgressChanged?.Invoke(this, progress);
            }
            
        }

        /// <summary>
        /// Increments the current progress value by 1 and raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        public void IncrementProgress()
        {
            _progress++;
            _updateprogress++;
            while (_updateprogress >= _updateprogress_at)
            {
                _updateprogress -= _updateprogress_at;
                ProgressChanged?.Invoke(this, _progress);
            }
        }

        #endregion
    }
}
