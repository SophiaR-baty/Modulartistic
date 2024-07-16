using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public interface IProgressReporter
    {
        event EventHandler<double> ProgressChanged;


        public void ReportProgress(double progress);

    }
}
