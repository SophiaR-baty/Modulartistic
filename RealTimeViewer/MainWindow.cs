using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modulartistic;

namespace RealTimeViewer
{
    public partial class MainWindow : Form
    {
        State state;
        GenerationArgs genArgs;

        public MainWindow()
        {
            InitializeComponent();

            genArgs = new GenerationArgs();
            state = new State();
            DrawState();
            picBox.MouseWheel += picBox_MouseWheel;
            
        }


        private void picBox_MouseClick(object sender, MouseEventArgs e)
        {


        }

        private void picBox_MouseWheel(object sender, MouseEventArgs e)
        {
            double zoom = state.XZoom * Math.Pow(2, e.Delta / 1000.0);
            state.XZoom = zoom;
            state.YZoom = zoom;
            xZoomLabel.Text = (100.0 / zoom).ToString() + "%";
            DrawState();
        }

        private void DrawState()
        {
            picBox.Image = state.GetBitmap(genArgs, -1);
        }

        private void funcTextbox_Validated(object sender, EventArgs e)
        {
            genArgs.Function = funcTextbox.Text;
            DrawState();
        }

        private void funcTextbox_Validating(object sender, CancelEventArgs e)
        {
            Function f = new Function(funcTextbox.Text);
            f.LoadAddOns(genArgs.AddOns.ToArray());
            if (!f.IsValid())
            {
                e.Cancel = true;
            }
        }
    }
}
