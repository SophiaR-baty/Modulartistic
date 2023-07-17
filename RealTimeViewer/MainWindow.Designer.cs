namespace RealTimeViewer
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainToolStrip = new System.Windows.Forms.ToolStrip();
            funcLabel = new System.Windows.Forms.ToolStripLabel();
            funcTextbox = new System.Windows.Forms.ToolStripTextBox();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            picBox = new System.Windows.Forms.PictureBox();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            zoomLabel = new System.Windows.Forms.ToolStripLabel();
            xZoomLabel = new System.Windows.Forms.ToolStripLabel();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBox).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // mainToolStrip
            // 
            mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { funcLabel, funcTextbox, toolStripSeparator1 });
            mainToolStrip.Location = new System.Drawing.Point(0, 0);
            mainToolStrip.Name = "mainToolStrip";
            mainToolStrip.Size = new System.Drawing.Size(800, 25);
            mainToolStrip.TabIndex = 0;
            mainToolStrip.Text = "toolStrip1";
            // 
            // funcLabel
            // 
            funcLabel.Name = "funcLabel";
            funcLabel.Size = new System.Drawing.Size(54, 22);
            funcLabel.Text = "Function";
            // 
            // funcTextbox
            // 
            funcTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            funcTextbox.Name = "funcTextbox";
            funcTextbox.Size = new System.Drawing.Size(150, 25);
            funcTextbox.Text = "x*y";
            funcTextbox.Validating += funcTextbox_Validating;
            funcTextbox.Validated += funcTextbox_Validated;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // picBox
            // 
            picBox.BackColor = System.Drawing.SystemColors.Control;
            picBox.Dock = System.Windows.Forms.DockStyle.Fill;
            picBox.Location = new System.Drawing.Point(0, 25);
            picBox.Name = "picBox";
            picBox.Size = new System.Drawing.Size(800, 485);
            picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            picBox.TabIndex = 1;
            picBox.TabStop = false;
            picBox.MouseClick += picBox_MouseClick;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { zoomLabel, xZoomLabel, toolStripSeparator2 });
            toolStrip1.Location = new System.Drawing.Point(0, 485);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(800, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // zoomLabel
            // 
            zoomLabel.Name = "zoomLabel";
            zoomLabel.Size = new System.Drawing.Size(45, 22);
            zoomLabel.Text = "Zoom: ";
            // 
            // xZoomLabel
            // 
            xZoomLabel.Name = "xZoomLabel";
            xZoomLabel.Size = new System.Drawing.Size(35, 22);
            xZoomLabel.Text = "100%";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLight;
            ClientSize = new System.Drawing.Size(800, 510);
            Controls.Add(toolStrip1);
            Controls.Add(picBox);
            Controls.Add(mainToolStrip);
            Name = "MainWindow";
            Text = "Modulartistic Realtime Viewer";
            mainToolStrip.ResumeLayout(false);
            mainToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBox).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripTextBox funcTextbox;
        private System.Windows.Forms.ToolStripLabel funcLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.PictureBox picBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel zoomLabel;
        private System.Windows.Forms.ToolStripLabel xZoomLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
