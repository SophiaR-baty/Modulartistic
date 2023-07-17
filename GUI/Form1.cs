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
using GUI;
using Modulartistic;


namespace GUI2
{
    public partial class Form1 : Form
    {

        StateSequence states;
        GenerationArgs genArgs;
        string last_tmp_animation;
        FunctionDictionary dict;

        public Form1()
        {
            
            
            // create instances of genArgs and states
            states = new StateSequence("Animation");
            genArgs = new GenerationArgs();

            // initialize Components
            InitializeComponent();
            errorProvider1.Icon = SystemIcons.WinLogo;

            // set default GenArgs
            widthInput.Text = "500";
            heightInput.Text = "500";
            framerateInput.Text = "12";
            functionInput.Text = "x*y";

            // add SeletedStateChangedEvent
            statesListBox.SelectedIndexChanged += OnSelectedStateChanged;

            // add easings to combobox
            foreach (string easingtype in Easing.ImplementedEasingTypes)
            {
                easingComboBox.Items.Add(easingtype);
            }

            // Add validation related events
            AddValidatedEvents();
            AddValidatingEvents();

            // Add initial State
            AddState();


            functionInput.AutoCompleteMode = AutoCompleteMode.Suggest;
            functionInput.AutoCompleteSource = AutoCompleteSource.CustomSource;
            dict = new FunctionDictionary();
            functionInput.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            foreach (string word in dict.GetDictionary())
            {
                functionInput.AutoCompleteCustomSource.Add(word);
            }
        }

        private void AddState()
        {
            State s = new State();
            s.Name = "New State";

            Scene sc = new Scene();
            sc.State = s;

            states.Scenes.Add(sc);

            statesListBox.Items.Add(s.Name);
            statesListBox.SelectedIndex = statesListBox.Items.Count - 1;
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

        private void AddValidatingEvents()
        {
            // Validating Events
            numInput.Validating += ValidateDoubleGrt0;
            modLimLowInput.Validating += ValidateDouble;
            modLimUpInput.Validating += ValidateDouble;

            x0Input.Validating += ValidateDouble;
            y0Input.Validating += ValidateDouble;
            xFactInput.Validating += ValidateDouble;
            yFactInput.Validating += ValidateDouble;
            rotationInput.Validating += ValidateDouble;

            colMinInput.Validating += ValidateDouble;
            colValInput.Validating += ValidateDoubleRng0To1;
            colSatInput.Validating += ValidateDoubleRng0To1;
            colAlphaInput.Validating += ValidateDoubleRng0To1;
            invalColAInput.Validating += ValidateDoubleRng0To1;
            invalColBInput.Validating += ValidateDoubleRng0To1;
            invalColGInput.Validating += ValidateDoubleRng0To1;
            invalColRInput.Validating += ValidateDoubleRng0To1;
            colFactRInput.Validating += ValidateDoubleNonNeg;
            colFactGInput.Validating += ValidateDoubleNonNeg;
            colFactBInput.Validating += ValidateDoubleNonNeg;

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

            animLengthInput.Validating += ValidateDoubleNonNeg;
            easingComboBox.Validating += ValidateEasing;

            widthInput.Validating += ValidateIntGrt0;
            heightInput.Validating += ValidateIntGrt0;
            framerateInput.Validating += ValidateUInt;
            addOnInput.Validating += ValidateAddOns;
        }

        private void AddValidatedEvents()
        {
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

            animLengthInput.Validated += OnAnimLengthInputValidated;
            easingComboBox.Validated += OnEasingComboBoxValidated;

            widthInput.Validated += OnWidthInputValidated;
            heightInput.Validated += OnHeightInputValidated;
            framerateInput.Validated += OnFramerateInputValidated;
            functionInput.Validated += OnFunctionInputValidated;
            addOnInput.Validated += OnAddOnsInputValidated;
        }

        #region Error
        private void SetError(Control control, string message)
        {
            control.BackColor = Color.LightCoral;
            errorToolTip.SetToolTip(control, message);
        }

        private string GetError(Control control)
        {
            return errorToolTip.GetToolTip(control);
        }

        private void ClearError(Control control)
        {
            control.BackColor = SystemColors.Window;
            errorToolTip.SetToolTip(control, "");
        }
        #endregion

        #region Validate evenets
        private void ValidateDoubleGrt0(object sender, CancelEventArgs e)
        {
            double val;
            if (!double.TryParse(((TextBox)sender).Text, out val))
            {
                SetError((Control)sender, "Value contains invalid characters. Please enter a real number. ");
                e.Cancel = true;
                return;
            }
            else if (val <= 0)
            {
                SetError((Control)sender, "Number may not be less or equal to 0. ");
                e.Cancel = true;
                return;
            }
            else
            {
                ClearError((Control)sender);
                return;
            }
        }

        private void ValidateDoubleNonNeg(object sender, CancelEventArgs e)
        {
            double val;
            if (!double.TryParse(((TextBox)sender).Text, out val))
            {
                SetError((Control)sender, "Value contains invalid characters. Please enter a real number. ");
                e.Cancel = true;
                return;
            }
            else if (val < 0)
            {
                SetError((Control)sender, "Number may not be less or equal to 0. ");
                e.Cancel = true;
                return;
            }
            else
            {
                ClearError((Control)sender);
                return;
            }
        }

        private void ValidateDoubleRng0To1(object sender, CancelEventArgs e)
        {
            double val;
            if (!double.TryParse(((TextBox)sender).Text, out val))
            {
                SetError((Control)sender, "Value contains invalid characters. Please enter a real number. ");
                e.Cancel = true;
                return;
            }
            else if (val < 0 || val > 1)
            {
                SetError((Control)sender, "Number may not be less than 0 or greater than 1. ");
                e.Cancel = true;
                return;
            }
            else
            {
                ClearError((Control)sender);
                return;
            }
        }

        private void ValidateDouble(object sender, CancelEventArgs e)
        {
            double val;
            if (!double.TryParse(((TextBox)sender).Text, out val))
            {
                SetError((Control)sender, "Value contains invalid characters. Please enter a real number. ");
                e.Cancel = true;
                return;
            }
            else
            {
                ClearError((Control)sender);
                return;
            }
        }



        private void ValidateInt(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(((TextBox)sender).Text, out _))
            {
                SetError((Control)sender, "Value contains invalid characters. Please enter an Integer. ");
                e.Cancel = true;
                return;
            }
            else
            {
                ClearError((Control)sender);
            }
        }

        private void ValidateIntGrt0(object sender, CancelEventArgs e)
        {
            int val;
            if (!int.TryParse(((TextBox)sender).Text, out val))
            {
                SetError((Control)sender, "Value contains invalid characters. Please enter an Integer. ");
                e.Cancel = true;
                return;
            }
            else if (val <= 0)
            {
                SetError((Control)sender, "This Number may not be less than or equal to 0. ");
                e.Cancel = true;
                return;
            }
            else
            {
                ClearError((Control)sender);
            }
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
            paths.RemoveAll(p => String.IsNullOrEmpty(p) || p == "\n");
            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i].Replace("\n\r", "");
                if (string.IsNullOrEmpty(path)) continue;
                if (!File.Exists(path))
                {
                    errorProvider1.SetError((Control)sender, "At least one of the addOns is not a valid file. :" + path + ":");
                    e.Cancel = true;
                    break;
                }
                else { errorProvider1.SetError((Control)sender, ""); }
            }
        }

        private void ValidateEasing(object sender, CancelEventArgs e)
        {
            try
            {
                Easing s = Easing.FromString(((ComboBox)sender).Text);
            }
            catch
            {
                errorProvider1.SetError((Control)sender, "The Easing Type does not exist");
                e.Cancel = true;
            }
        }
        #endregion


        private void OnSelectedStateChanged(object sender, EventArgs e)
        {
            if (states.Count == 0) { return; }

            Scene sc = states.Scenes[((ListBox)sender).SelectedIndex];
            State s = sc.State;

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

            animLengthInput.Text = sc.Length.ToString();
            easingComboBox.Text = sc.EasingType;
        }

        private void addStateButton_Click(object sender, EventArgs e)
        {
            // activate all Textboxes and buttons
            if (statesListBox.Items.Count == 0)
            {
                ActivateAll();
                remStateButton.Enabled = true;
            }

            AddState();
        }

        private void remStateButton_Click(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            if (idx > 0) { statesListBox.SelectedIndex = idx - 1; }
            states.Scenes.RemoveAt(idx);
            statesListBox.Items.RemoveAt(idx);

            if (states.Count == 0)
            {
                DeactivateAll();
                remStateButton.Enabled = false;
            }
        }

        #region Parse Text To State (Validated) Events
        private void OnNumInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Mod = double.Parse(((TextBox)sender).Text);
        }
        private void OnModLimLowInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ModLimLow = double.Parse(((TextBox)sender).Text);
        }
        private void OnModLimUpInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ModLimUp = double.Parse(((TextBox)sender).Text);
        }

        private void OnX0InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.X0 = double.Parse(((TextBox)sender).Text);
        }
        private void OnY0Validated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Y0 = double.Parse(((TextBox)sender).Text);
        }
        private void OnXFactValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.XZoom = double.Parse(((TextBox)sender).Text);
        }
        private void OnYFactValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.YZoom = double.Parse(((TextBox)sender).Text);
        }
        private void OnRotationValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Rotation = double.Parse(((TextBox)sender).Text);
        }

        private void OnColMinInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ColorMinimum = double.Parse(((TextBox)sender).Text);
        }
        private void OnColValInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ColorValue = double.Parse(((TextBox)sender).Text);
        }
        private void OnColSatInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ColorSaturation = double.Parse(((TextBox)sender).Text);
        }
        private void OnColAlphaInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ColorAlpha = double.Parse(((TextBox)sender).Text);
        }
        private void OnInvalColAInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.InvalidColor[3] = double.Parse(((TextBox)sender).Text);
        }
        private void OnInvalColRInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.InvalidColor[0] = double.Parse(((TextBox)sender).Text);
        }
        private void OnInvalColGInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.InvalidColor[1] = double.Parse(((TextBox)sender).Text);
        }
        private void OnInvalColBInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.InvalidColor[2] = double.Parse(((TextBox)sender).Text);
        }
        private void OnColFactRInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ColorFactors[0] = double.Parse(((TextBox)sender).Text);
        }
        private void OnColFactGInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ColorFactors[1] = double.Parse(((TextBox)sender).Text);
        }
        private void OnColFactBInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.ColorFactors[2] = double.Parse(((TextBox)sender).Text);
        }

        private void OnPara0InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[0] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara1InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[1] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara2InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[2] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara3InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[3] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara4InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[4] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara5InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[5] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara6InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[6] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara7InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[7] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara8InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[8] = double.Parse(((TextBox)sender).Text);
        }
        private void OnPara9InputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            State s = states.Scenes[idx].State;
            s.Parameters[9] = double.Parse(((TextBox)sender).Text);
        }

        private void OnAnimLengthInputValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            Scene sc = states.Scenes[idx];
            sc.Length = double.Parse(((TextBox)sender).Text);
        }

        private void OnEasingComboBoxValidated(object sender, EventArgs e)
        {
            int idx = statesListBox.SelectedIndex;
            Scene sc = states.Scenes[idx];
            try
            {
                sc.EasingType = ((ComboBox)sender).Text;
            }
            catch { }
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
            dict = new FunctionDictionary();
            List<string> files = addOnInput.Text.Split('\n').ToList();
            files.RemoveAll((s) => s == "");
            /*
            foreach (string file in files)
            {
                if (Directory.Dr
            }
            */
            genArgs.AddOns = addOnInput.Text.Split('\n').ToList();
            genArgs.AddOns.RemoveAll((s) => s == "");


            foreach (string dll in genArgs.AddOns)
            {
                dict.LoadAddOn(dll);
            }

            functionInput.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            foreach (string word in dict.GetDictionary())
            {
                functionInput.AutoCompleteCustomSource.Add(word);
            }
        }
        #endregion

        // Generate Button
        private void generateImageButton_Click(object sender, EventArgs e)
        {
            DeleteLastTmpAnimation();

            int idx = statesListBox.SelectedIndex;
            Bitmap bm = states.Scenes[idx].State.GetBitmap(genArgs, -1);

            UpdatePictureBoxPicture(bm);
        }

        // generate sequence
        private void generateAnimationButton_Click(object sender, EventArgs e)
        {
            DeleteLastTmpAnimation();

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp");
            }
            string path_out = AppDomain.CurrentDomain.BaseDirectory + "tmp";
            last_tmp_animation = states.GenerateAnimation(genArgs, -1, path_out);

            UpdatePictureBoxPicture(last_tmp_animation);
        }

        private void saveImageButton_Click(object sender, EventArgs e)
        {
            SaveImage();
        }


        private void DeleteLastTmpAnimation()
        {
            if (File.Exists(last_tmp_animation)) { File.Delete(last_tmp_animation); }
            last_tmp_animation = "";
        }

        private void UpdatePictureBoxPicture(Bitmap bm)
        {
            pictureBox1.Image = bm;
        }

        private void UpdatePictureBoxPicture(string filename)
        {
            pictureBox1.ImageLocation = filename;
        }

        private void SaveImage()
        {
            // create the dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // create the output dir and set as initial directory
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Output"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Output");
            }
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Output";


            // if saving an image
            if (String.IsNullOrEmpty(last_tmp_animation))
            {
                saveFileDialog.Title = "Save Image";
                saveFileDialog.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png";
                saveFileDialog.DefaultExt = ".png";
                saveFileDialog.FileName = "State";
            }
            // if saving animation
            else if (File.Exists(last_tmp_animation))
            {
                saveFileDialog.Title = "Save Animation";
                saveFileDialog.Filter = "Gif Image (.gif)|*.gif";
                saveFileDialog.DefaultExt = ".gif";
                saveFileDialog.FileName = "Animation";
            }
            else
            {
                throw new FileNotFoundException("A temporary file that should exist is missing. ");
            }

            // save the image
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog.FileName);
            }
        }


        #region unused


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

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        #endregion

    }
}
