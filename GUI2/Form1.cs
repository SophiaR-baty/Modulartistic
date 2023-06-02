using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modulartistic;


namespace GUI2
{
    public partial class Form1 : Form
    {

        List<State> states;
        GenerationArgs genArgs;

        public Form1()
        {
            states = new List<State>();
            genArgs = new GenerationArgs();

            InitializeComponent();

            widthInput.Text = "500";
            heightInput.Text = "500";
            framerateInput.Text = "12";
            functionInput.Text = "x*y";


            statesListBox.SelectedIndexChanged += OnSelectedStateChanged;


            // Validating Events
            numInput.Validating += ValidateDouble;
            modLimLowInput.Validating += ValidateDouble;
            modLimUpInput.Validating += ValidateDouble;

            x0Input.Validating += ValidateDouble;
            y0Input.Validating += ValidateDouble;
            xFactInput.Validating += ValidateDouble;
            yFactInput.Validating += ValidateDouble;
            rotationInput.Validating += ValidateDouble;

            colMinInput.Validating += ValidateDouble;
            colValInput.Validating += ValidateDouble;
            colSatInput.Validating += ValidateDouble;
            colAlphaInput.Validating += ValidateDouble;
            invalColAInput.Validating += ValidateDouble;
            invalColBInput.Validating += ValidateDouble;
            invalColGInput.Validating += ValidateDouble;
            invalColRInput.Validating += ValidateDouble;
            colFactRInput.Validating += ValidateDouble;
            colFactGInput.Validating += ValidateDouble;
            colFactBInput.Validating += ValidateDouble;

            para0Input.Validating += ValidateDouble;
            para1Input.Validating += ValidateDouble;
            para2Input.Validating += ValidateDouble;
            para3Input.Validating += ValidateDouble;
            para4Input.Validating += ValidateDouble;
            para5Input.Validating += ValidateDouble;
            para6Input.Validating += ValidateDouble;
            para7Input.Validating += ValidateDouble;
            para8Input.Validating += ValidateDouble;
            para9Input.Validating += ValidateDouble;

            widthInput.Validating += ValidateInt;
            heightInput.Validating += ValidateInt;
            framerateInput.Validating += ValidateUInt;

            // Validated Events
            numInput.Validated += OnNumInputValidated;
            modLimLowInput.Validated += OnModLimLowInputValidated;
            modLimUpInput.Validated += OnModLimUpInputValidated;

            x0Input.Validated += OnX0InputValidated;
            y0Input.Validated += OnY0Validated;
            xFactInput.Validated += OnXFactValidated;
            yFactInput.Validated += OnYFactValidated;
            rotationInput.Validated += OnRotationValidated;

            colMinInput.Validated += OnColMinInputValidated;
            colValInput.Validated += OnColValInputValidated;
            colSatInput.Validated += OnColSatInputValidated;
            colAlphaInput.Validated += OnColAlphaInputValidated;
            invalColAInput.Validated += OnInvalColAInputValidated;
            invalColBInput.Validated += OnInvalColBInputValidated;
            invalColGInput.Validated += OnInvalColGInputValidated;
            invalColRInput.Validated += OnInvalColRInputValidated;
            colFactRInput.Validated += OnColFactRInputValidated;
            colFactGInput.Validated += OnColFactGInputValidated;
            colFactBInput.Validated += OnColFactBInputValidated;

            para0Input.Validated += OnPara0InputValidated;
            para1Input.Validated += OnPara1InputValidated;
            para2Input.Validated += OnPara2InputValidated;
            para3Input.Validated += OnPara3InputValidated;
            para4Input.Validated += OnPara4InputValidated;
            para5Input.Validated += OnPara5InputValidated;
            para6Input.Validated += OnPara6InputValidated;
            para7Input.Validated += OnPara7InputValidated;
            para8Input.Validated += OnPara8InputValidated;
            para9Input.Validated += OnPara9InputValidated;

            widthInput.Validated += OnWidthInputValidated;
            heightInput.Validated += OnHeightInputValidated;
            framerateInput.Validated += OnFramerateInputValidated;
            functionInput.Validated += OnFunctionInputValidated;
            addOnInput.Validated += OnAddOnsInputValidated;

            DeactivateAll();
        }

        private void DeactivateAll()
        {
            numInput.Enabled = false;
            modLimLowInput.Enabled = false;
            modLimUpInput.Enabled = false;

            x0Input.Enabled = false;
            y0Input.Enabled = false;
            xFactInput.Enabled = false;
            yFactInput.Enabled = false;
            rotationInput.Enabled = false;

            colMinInput.Enabled = false;
            colValInput.Enabled = false;
            colSatInput.Enabled = false;
            colAlphaInput.Enabled = false;
            invalColAInput.Enabled = false;
            invalColBInput.Enabled = false;
            invalColGInput.Enabled = false;
            invalColRInput.Enabled = false;
            colFactRInput.Enabled = false;
            colFactGInput.Enabled = false;
            colFactBInput.Enabled = false;

            para0Input.Enabled = false;
            para1Input.Enabled = false;
            para2Input.Enabled = false;
            para3Input.Enabled = false;
            para4Input.Enabled = false;
            para5Input.Enabled = false;
            para6Input.Enabled = false;
            para7Input.Enabled = false;
            para8Input.Enabled = false;
            para9Input.Enabled = false;
        }

        private void ActivateAll()
        {
            numInput.Enabled = true;
            modLimLowInput.Enabled = true;
            modLimUpInput.Enabled = true;

            x0Input.Enabled = true;
            y0Input.Enabled = true;
            xFactInput.Enabled = true;
            yFactInput.Enabled = true;
            rotationInput.Enabled = true;

            colMinInput.Enabled = true;
            colValInput.Enabled = true;
            colSatInput.Enabled = true;
            colAlphaInput.Enabled = true;
            invalColAInput.Enabled = true;
            invalColBInput.Enabled = true;
            invalColGInput.Enabled = true;
            invalColRInput.Enabled = true;
            colFactRInput.Enabled = true;
            colFactGInput.Enabled = true;
            colFactBInput.Enabled = true;

            para0Input.Enabled = true;
            para1Input.Enabled = true;
            para2Input.Enabled = true;
            para3Input.Enabled = true;
            para4Input.Enabled = true;
            para5Input.Enabled = true;
            para6Input.Enabled = true;
            para7Input.Enabled = true;
            para8Input.Enabled = true;
            para9Input.Enabled = true;
        }

        private void ValidateDouble(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(((TextBox)sender).Text, out _))
            {
                errorProvider1.SetError((Control)sender, "Value is not a valid number. ");
                e.Cancel = true;
            }
            else { errorProvider1.SetError((Control)sender, ""); }
        }

        private void ValidateInt(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(((TextBox)sender).Text, out _))
            {
                errorProvider1.SetError((Control)sender, "Value is not a valid number. ");
                e.Cancel = true;
            }
            else { errorProvider1.SetError((Control)sender, ""); }
        }

        private void ValidateUInt(object sender, CancelEventArgs e)
        {
            if (!uint.TryParse(((TextBox)sender).Text, out _))
            {
                errorProvider1.SetError((Control)sender, "Value is not a valid number. ");
                e.Cancel = true;
            }
            else { errorProvider1.SetError((Control)sender, ""); }
        }

        private void ValidateAddOns(object sender, CancelEventArgs e)
        {
            List<string> paths = ((TextBox)sender).Text.Split('\n').ToList<string>();
            if (!paths.All((s) => File.Exists(s)))
            {
                errorProvider1.SetError((Control)sender, "At least one of the addOns is not a valid file. ");
                e.Cancel = true;
            }
            else { errorProvider1.SetError((Control)sender, ""); }
        }



        private void OnSelectedStateChanged(object sender, EventArgs e)
        {
            State s = states[((ListBox)sender).SelectedIndex];

            numInput.Text = s.Mod.ToString();
            modLimLowInput.Text = s.ModLimLow.ToString();
            modLimUpInput.Text = s.ModLimUp.ToString();

            x0Input.Text = s.X0.ToString();
            y0Input.Text = s.Y0.ToString();
            xFactInput.Text = s.XZoom.ToString();
            yFactInput.Text = s.YZoom.ToString();
            rotationInput.Text = s.Rotation.ToString();

            colMinInput.Text = s.ColorMinimum.ToString();
            colValInput.Text = s.ColorValue.ToString();
            colSatInput.Text = s.ColorSaturation.ToString();
            colAlphaInput.Text = s.ColorAlpha.ToString();
            invalColAInput.Text = s.InvalidColor[3].ToString();
            invalColBInput.Text = s.InvalidColor[2].ToString();
            invalColGInput.Text = s.InvalidColor[1].ToString();
            invalColRInput.Text = s.InvalidColor[0].ToString();
            colFactRInput.Text = s.ColorFactors[0].ToString();
            colFactGInput.Text = s.ColorFactors[1].ToString();
            colFactBInput.Text = s.ColorFactors[2].ToString();

            para0Input.Text = s.Parameters[0].ToString();
            para1Input.Text = s.Parameters[1].ToString();
            para2Input.Text = s.Parameters[2].ToString();
            para3Input.Text = s.Parameters[3].ToString();
            para4Input.Text = s.Parameters[4].ToString();
            para5Input.Text = s.Parameters[5].ToString();
            para6Input.Text = s.Parameters[6].ToString();
            para7Input.Text = s.Parameters[7].ToString();
            para8Input.Text = s.Parameters[8].ToString();
            para9Input.Text = s.Parameters[9].ToString();
        }

        private void addStateButton_Click(object sender, EventArgs e)
        {
            if (statesListBox.Items.Count == 0)
            {
                ActivateAll();
            }

            State s = new State();
            s.Name = "State";
            states.Add(s);
            statesListBox.Items.Add(s.Name);
            statesListBox.SelectedIndex = statesListBox.Items.Count - 1;
        }

        #region Parse Text To State (Validated) Events
        private void OnNumInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Mod = double.Parse(((TextBox)sender).Text);
        }
        private void OnModLimLowInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ModLimLow = double.Parse(((TextBox)sender).Text);
        }
        private void OnModLimUpInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ModLimUp = double.Parse(((TextBox)sender).Text);
        }

        private void OnX0InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.X0 = double.Parse(((TextBox)sender).Text);
        }
        private void OnY0Validated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Y0 = double.Parse(((TextBox)sender).Text);
        }
        private void OnXFactValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.XZoom = double.Parse(((TextBox)sender).Text);
        }
        private void OnYFactValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.YZoom = double.Parse(((TextBox)sender).Text);
        }
        private void OnRotationValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Rotation = double.Parse(((TextBox)sender).Text);
        }

        private void OnColMinInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ColorMinimum = double.Parse(((TextBox)sender).Text);
        }
        private void OnColValInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ColorValue = double.Parse(((TextBox)sender).Text);
        }
        private void OnColSatInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ColorSaturation = double.Parse(((TextBox)sender).Text);
        }
        private void OnColAlphaInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ColorAlpha = double.Parse(((TextBox)sender).Text);
        }
        private void OnInvalColAInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.InvalidColor[3] = double.Parse(((TextBox)sender).Text);
        }
        private void OnInvalColRInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.InvalidColor[0] = double.Parse(((TextBox)sender).Text);
        }
        private void OnInvalColGInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.InvalidColor[1] = double.Parse(((TextBox)sender).Text);
        }
        private void OnInvalColBInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.InvalidColor[2] = double.Parse(((TextBox)sender).Text);
        }
        private void OnColFactRInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ColorFactors[0] = double.Parse(((TextBox)sender).Text);
        }
        private void OnColFactGInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ColorFactors[1] = double.Parse(((TextBox)sender).Text);
        }
        private void OnColFactBInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.ColorFactors[2] = double.Parse(((TextBox)sender).Text);
        }

        private void OnPara0InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[0] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara1InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[1] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara2InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[2] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara3InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[3] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara4InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[4] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara5InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[5] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara6InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[6] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara7InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[7] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara8InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[8] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara9InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states[idx];
            s.Parameters[9] = double.Parse(((TextBox)sender).Text);
        }

        private void OnWidthInputValidated(object sender, EventArgs e)
        {
            genArgs.Size[0] = int.Parse(((TextBox)sender).Text);
        }
        private void OnHeightInputValidated(object sender, EventArgs e)
        {
            genArgs.Size[1] = int.Parse(((TextBox)sender).Text);
        }
        private void OnFramerateInputValidated(object sender, EventArgs e)
        {
            genArgs.Framerate = uint.Parse(((TextBox)sender).Text);
        }
        private void OnFunctionInputValidated(object sender, EventArgs e)
        {
            genArgs.Function = ((TextBox)sender).Text;
        }
        private void OnAddOnsInputValidated(object sender, EventArgs e)
        {
            genArgs.AddOns = addOnInput.Text.Split('\n').ToList();
            genArgs.AddOns.RemoveAll((s) => s == "");
        }
        #endregion

        // Generate Button
        private void generateImageButton_Click(object sender, EventArgs e)
        {
            Size size = new Size(genArgs.Size[0], genArgs.Size[1]);
            Function func = new Function(genArgs.Function);
            func.LoadAddOns(genArgs.AddOns.ToArray());

            int idx = statesListBox.SelectedIndex;

            Bitmap bm = states[idx].GetBitmap(size, func);

            pictureBox1.Image = bm;
        }


        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {
        }

        private void label10_Click(object sender, EventArgs e)
        {
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }


    }
}
