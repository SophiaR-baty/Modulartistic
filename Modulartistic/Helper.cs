using Modulartistic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic
{
    internal static class Helper
    {
        public static long CalculateMaxProgress(GenerationData data)
        {
            long maxProgress = 0;

            StateOptions currentOpts = new StateOptions();

            for (int i = 0; i < data.Count; i++)
            {
                object obj = data.Data[i];

                // if the object is StateOptions update the current StateOptions
                if (obj.GetType() == typeof(StateOptions))
                {
                    currentOpts = (StateOptions)obj;
                    maxProgress++;
                }

                // else if the object is a state, generate said state
                else if (obj.GetType() == typeof(State))
                {
                    maxProgress += currentOpts.Width * currentOpts.Height;
                }

                // if the object is a StateSequence, generate that StateSequence
                else if (obj.GetType() == typeof(StateSequence))
                {
                    StateSequence SS = (StateSequence)obj;
                    foreach (Scene s in SS.Scenes)
                    {
                        maxProgress += (long)(s.Length * currentOpts.Framerate * currentOpts.Width * currentOpts.Height);
                    }
                }

                // if the object is a StateTimeline
                else if (obj.GetType() == typeof(StateTimeline))
                {
                    StateTimeline ST = obj as StateTimeline;
                    maxProgress += ST.TotalFrameCount(currentOpts.Framerate) * currentOpts.Width * currentOpts.Height;
                }
                else
                {
                    throw new Exception();
                }
            }

            return maxProgress;
        }
    }
}
