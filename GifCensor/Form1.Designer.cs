
namespace GifCensor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblSize = new System.Windows.Forms.Label();
            this.lblSel = new System.Windows.Forms.Label();
            this.txtPxlSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCensor = new System.Windows.Forms.Button();
            this.radioPixel = new System.Windows.Forms.RadioButton();
            this.radioSolid = new System.Windows.Forms.RadioButton();
            this.radioBlur = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBlurRad = new System.Windows.Forms.TextBox();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.chkDispProcessed = new System.Windows.Forms.CheckBox();
            this.radioStaticColor = new System.Windows.Forms.RadioButton();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.radioStaticMono = new System.Windows.Forms.RadioButton();
            this.radioRGBS = new System.Windows.Forms.RadioButton();
            this.txtRGBs = new System.Windows.Forms.TextBox();
            this.radioJitter = new System.Windows.Forms.RadioButton();
            this.txtJitter = new System.Windows.Forms.TextBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnChromaCol = new System.Windows.Forms.Button();
            this.checkChroma = new System.Windows.Forms.CheckBox();
            this.txtChromaSens = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPickColor = new System.Windows.Forms.Button();
            this.btnSolidColor = new System.Windows.Forms.Button();
            this.colorDialog2 = new System.Windows.Forms.ColorDialog();
            this.panelChromaColor = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.radioHSV = new System.Windows.Forms.RadioButton();
            this.txtHue = new System.Windows.Forms.TextBox();
            this.txtSat = new System.Windows.Forms.TextBox();
            this.txtLum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBarHue = new System.Windows.Forms.TrackBar();
            this.trackBarSat = new System.Windows.Forms.TrackBar();
            this.trackBarLum = new System.Windows.Forms.TrackBar();
            this.btnInv = new System.Windows.Forms.Button();
            this.btnmaskloadtest = new System.Windows.Forms.Button();
            this.txtAlpha = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnClearMask = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.checkFrameRange = new System.Windows.Forms.CheckBox();
            this.numMinFrame = new System.Windows.Forms.NumericUpDown();
            this.numMaxFrame = new System.Windows.Forms.NumericUpDown();
            this.btnFrameStart = new System.Windows.Forms.Button();
            this.btnFrameEnd = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelHSVPreview = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStep = new System.Windows.Forms.Button();
            this.checkEncodeVid = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkReuseProcessed = new System.Windows.Forms.CheckBox();
            this.btnPurge = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnShowRange = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.lblVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFrame)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSize
            // 
            this.lblSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(145, 466);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(87, 13);
            this.lblSize.TabIndex = 1;
            this.lblSize.Text = "No media loaded";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSel
            // 
            this.lblSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSel.AutoSize = true;
            this.lblSel.Location = new System.Drawing.Point(381, 466);
            this.lblSel.Name = "lblSel";
            this.lblSel.Size = new System.Drawing.Size(88, 13);
            this.lblSel.TabIndex = 5;
            this.lblSel.Text = "No area selected";
            this.lblSel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPxlSize
            // 
            this.txtPxlSize.Location = new System.Drawing.Point(146, 19);
            this.txtPxlSize.Name = "txtPxlSize";
            this.txtPxlSize.Size = new System.Drawing.Size(58, 20);
            this.txtPxlSize.TabIndex = 6;
            this.txtPxlSize.Text = "20";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(88, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Pixel Size";
            // 
            // btnCensor
            // 
            this.btnCensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCensor.Enabled = false;
            this.btnCensor.Location = new System.Drawing.Point(993, 649);
            this.btnCensor.Name = "btnCensor";
            this.btnCensor.Size = new System.Drawing.Size(75, 23);
            this.btnCensor.TabIndex = 8;
            this.btnCensor.Text = "Process";
            this.toolTip1.SetToolTip(this.btnCensor, "Apply the effect");
            this.btnCensor.UseVisualStyleBackColor = true;
            this.btnCensor.Click += new System.EventHandler(this.btnCensor_Click);
            // 
            // radioPixel
            // 
            this.radioPixel.AutoSize = true;
            this.radioPixel.Checked = true;
            this.radioPixel.Location = new System.Drawing.Point(10, 20);
            this.radioPixel.Name = "radioPixel";
            this.radioPixel.Size = new System.Drawing.Size(62, 17);
            this.radioPixel.TabIndex = 10;
            this.radioPixel.TabStop = true;
            this.radioPixel.Text = "Pixelate";
            this.radioPixel.UseVisualStyleBackColor = true;
            // 
            // radioSolid
            // 
            this.radioSolid.AutoSize = true;
            this.radioSolid.Location = new System.Drawing.Point(10, 67);
            this.radioSolid.Name = "radioSolid";
            this.radioSolid.Size = new System.Drawing.Size(74, 17);
            this.radioSolid.TabIndex = 11;
            this.radioSolid.Text = "Solid color";
            this.radioSolid.UseVisualStyleBackColor = true;
            // 
            // radioBlur
            // 
            this.radioBlur.AutoSize = true;
            this.radioBlur.Location = new System.Drawing.Point(10, 44);
            this.radioBlur.Name = "radioBlur";
            this.radioBlur.Size = new System.Drawing.Size(43, 17);
            this.radioBlur.TabIndex = 12;
            this.radioBlur.Text = "Blur";
            this.radioBlur.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Radius";
            // 
            // txtBlurRad
            // 
            this.txtBlurRad.Location = new System.Drawing.Point(146, 44);
            this.txtBlurRad.Name = "txtBlurRad";
            this.txtBlurRad.Size = new System.Drawing.Size(58, 20);
            this.txtBlurRad.TabIndex = 13;
            this.txtBlurRad.Text = "20";
            // 
            // btnPrev
            // 
            this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrev.Location = new System.Drawing.Point(934, 471);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(50, 23);
            this.btnPrev.TabIndex = 15;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(990, 471);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(50, 23);
            this.btnNext.TabIndex = 16;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // chkDispProcessed
            // 
            this.chkDispProcessed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDispProcessed.AutoSize = true;
            this.chkDispProcessed.Checked = true;
            this.chkDispProcessed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDispProcessed.Location = new System.Drawing.Point(751, 466);
            this.chkDispProcessed.Name = "chkDispProcessed";
            this.chkDispProcessed.Size = new System.Drawing.Size(128, 17);
            this.chkDispProcessed.TabIndex = 17;
            this.chkDispProcessed.Text = "Display processed file";
            this.toolTip1.SetToolTip(this.chkDispProcessed, "Show the processed file or not. Good for when you want to make many different edi" +
        "ts to one file without stacking effects.");
            this.chkDispProcessed.UseVisualStyleBackColor = true;
            // 
            // radioStaticColor
            // 
            this.radioStaticColor.AutoSize = true;
            this.radioStaticColor.Location = new System.Drawing.Point(10, 90);
            this.radioStaticColor.Name = "radioStaticColor";
            this.radioStaticColor.Size = new System.Drawing.Size(91, 17);
            this.radioStaticColor.TabIndex = 18;
            this.radioStaticColor.Text = "Colored Noise";
            this.toolTip1.SetToolTip(this.radioStaticColor, "RGB noise");
            this.radioStaticColor.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(901, 499);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(167, 144);
            this.richTextBox1.TabIndex = 19;
            this.richTextBox1.Text = "";
            // 
            // radioStaticMono
            // 
            this.radioStaticMono.AutoSize = true;
            this.radioStaticMono.Location = new System.Drawing.Point(10, 113);
            this.radioStaticMono.Name = "radioStaticMono";
            this.radioStaticMono.Size = new System.Drawing.Size(82, 17);
            this.radioStaticMono.TabIndex = 20;
            this.radioStaticMono.Text = "Static Noise";
            this.toolTip1.SetToolTip(this.radioStaticMono, "Black and white noise");
            this.radioStaticMono.UseVisualStyleBackColor = true;
            // 
            // radioRGBS
            // 
            this.radioRGBS.AutoSize = true;
            this.radioRGBS.Location = new System.Drawing.Point(10, 136);
            this.radioRGBS.Name = "radioRGBS";
            this.radioRGBS.Size = new System.Drawing.Size(70, 17);
            this.radioRGBS.TabIndex = 21;
            this.radioRGBS.Text = "RGB shift";
            this.radioRGBS.UseVisualStyleBackColor = true;
            // 
            // txtRGBs
            // 
            this.txtRGBs.Location = new System.Drawing.Point(146, 136);
            this.txtRGBs.Name = "txtRGBs";
            this.txtRGBs.Size = new System.Drawing.Size(58, 20);
            this.txtRGBs.TabIndex = 22;
            this.txtRGBs.Text = "20";
            // 
            // radioJitter
            // 
            this.radioJitter.AutoSize = true;
            this.radioJitter.Location = new System.Drawing.Point(10, 159);
            this.radioJitter.Name = "radioJitter";
            this.radioJitter.Size = new System.Drawing.Size(75, 17);
            this.radioJitter.TabIndex = 23;
            this.radioJitter.Text = "Jitter glitch";
            this.radioJitter.UseVisualStyleBackColor = true;
            // 
            // txtJitter
            // 
            this.txtJitter.Location = new System.Drawing.Point(146, 158);
            this.txtJitter.Name = "txtJitter";
            this.txtJitter.Size = new System.Drawing.Size(58, 20);
            this.txtJitter.TabIndex = 24;
            this.txtJitter.Text = "20";
            // 
            // btnChromaCol
            // 
            this.btnChromaCol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChromaCol.Location = new System.Drawing.Point(301, 115);
            this.btnChromaCol.Name = "btnChromaCol";
            this.btnChromaCol.Size = new System.Drawing.Size(52, 38);
            this.btnChromaCol.TabIndex = 26;
            this.btnChromaCol.Text = "Color picker";
            this.toolTip1.SetToolTip(this.btnChromaCol, "Pick the color from presets, or color codes");
            this.btnChromaCol.UseVisualStyleBackColor = true;
            this.btnChromaCol.Click += new System.EventHandler(this.btnChromaCol_Click);
            // 
            // checkChroma
            // 
            this.checkChroma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkChroma.AutoSize = true;
            this.checkChroma.Location = new System.Drawing.Point(220, 90);
            this.checkChroma.Name = "checkChroma";
            this.checkChroma.Size = new System.Drawing.Size(82, 17);
            this.checkChroma.TabIndex = 27;
            this.checkChroma.Text = "Chroma key";
            this.checkChroma.UseVisualStyleBackColor = true;
            // 
            // txtChromaSens
            // 
            this.txtChromaSens.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChromaSens.Location = new System.Drawing.Point(277, 159);
            this.txtChromaSens.Name = "txtChromaSens";
            this.txtChromaSens.Size = new System.Drawing.Size(76, 20);
            this.txtChromaSens.TabIndex = 28;
            this.txtChromaSens.Text = "10";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(217, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Sensitivity";
            // 
            // btnPickColor
            // 
            this.btnPickColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPickColor.Location = new System.Drawing.Point(220, 115);
            this.btnPickColor.Name = "btnPickColor";
            this.btnPickColor.Size = new System.Drawing.Size(75, 38);
            this.btnPickColor.TabIndex = 30;
            this.btnPickColor.Text = "Eyedropper";
            this.toolTip1.SetToolTip(this.btnPickColor, "Pick the color from the currently displayed media");
            this.btnPickColor.UseVisualStyleBackColor = true;
            this.btnPickColor.Click += new System.EventHandler(this.btnPickColor_Click);
            // 
            // btnSolidColor
            // 
            this.btnSolidColor.Location = new System.Drawing.Point(108, 67);
            this.btnSolidColor.Name = "btnSolidColor";
            this.btnSolidColor.Size = new System.Drawing.Size(96, 20);
            this.btnSolidColor.TabIndex = 31;
            this.btnSolidColor.Text = "Pick Color";
            this.toolTip1.SetToolTip(this.btnSolidColor, "Pick the color to fill the mask with");
            this.btnSolidColor.UseVisualStyleBackColor = true;
            this.btnSolidColor.Click += new System.EventHandler(this.btnSolidColor_Click);
            // 
            // colorDialog2
            // 
            this.colorDialog2.AnyColor = true;
            // 
            // panelChromaColor
            // 
            this.panelChromaColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelChromaColor.BackColor = System.Drawing.Color.Black;
            this.panelChromaColor.Location = new System.Drawing.Point(304, 87);
            this.panelChromaColor.Name = "panelChromaColor";
            this.panelChromaColor.Size = new System.Drawing.Size(49, 25);
            this.panelChromaColor.TabIndex = 32;
            this.panelChromaColor.Paint += new System.Windows.Forms.PaintEventHandler(this.panelChromaColor_Paint);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(901, 649);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(86, 23);
            this.btnClear.TabIndex = 33;
            this.btnClear.Text = "Clear Images";
            this.toolTip1.SetToolTip(this.btnClear, "Clear the image history and the current image");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // radioHSV
            // 
            this.radioHSV.AutoSize = true;
            this.radioHSV.Location = new System.Drawing.Point(219, 20);
            this.radioHSV.Name = "radioHSV";
            this.radioHSV.Size = new System.Drawing.Size(47, 17);
            this.radioHSV.TabIndex = 36;
            this.radioHSV.Text = "HSV";
            this.toolTip1.SetToolTip(this.radioHSV, "Tip: Use the eyedropper tool on the part you want to change, the changed colour i" +
        "s displayed here.");
            this.radioHSV.UseVisualStyleBackColor = true;
            // 
            // txtHue
            // 
            this.txtHue.Location = new System.Drawing.Point(323, 20);
            this.txtHue.Name = "txtHue";
            this.txtHue.Size = new System.Drawing.Size(58, 20);
            this.txtHue.TabIndex = 37;
            this.txtHue.Text = "100";
            this.txtHue.TextChanged += new System.EventHandler(this.txtHue_TextChanged);
            // 
            // txtSat
            // 
            this.txtSat.Location = new System.Drawing.Point(323, 68);
            this.txtSat.Name = "txtSat";
            this.txtSat.Size = new System.Drawing.Size(58, 20);
            this.txtSat.TabIndex = 38;
            this.txtSat.Text = "100";
            this.txtSat.TextChanged += new System.EventHandler(this.txtSat_TextChanged);
            // 
            // txtLum
            // 
            this.txtLum.Location = new System.Drawing.Point(322, 120);
            this.txtLum.Name = "txtLum";
            this.txtLum.Size = new System.Drawing.Size(58, 20);
            this.txtLum.TabIndex = 39;
            this.txtLum.Text = "100";
            this.txtLum.TextChanged += new System.EventHandler(this.txtLum_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(290, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Hue";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(266, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 41;
            this.label5.Text = "Saturation";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(261, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 42;
            this.label6.Text = "Luminance";
            // 
            // trackBarHue
            // 
            this.trackBarHue.Location = new System.Drawing.Point(387, 20);
            this.trackBarHue.Maximum = 200;
            this.trackBarHue.Name = "trackBarHue";
            this.trackBarHue.Size = new System.Drawing.Size(78, 45);
            this.trackBarHue.TabIndex = 43;
            this.trackBarHue.TickFrequency = 50;
            this.trackBarHue.Value = 100;
            this.trackBarHue.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBarSat
            // 
            this.trackBarSat.Location = new System.Drawing.Point(387, 70);
            this.trackBarSat.Maximum = 200;
            this.trackBarSat.Name = "trackBarSat";
            this.trackBarSat.Size = new System.Drawing.Size(78, 45);
            this.trackBarSat.TabIndex = 44;
            this.trackBarSat.TickFrequency = 50;
            this.trackBarSat.Value = 100;
            this.trackBarSat.Scroll += new System.EventHandler(this.trackBarSat_Scroll);
            // 
            // trackBarLum
            // 
            this.trackBarLum.Location = new System.Drawing.Point(386, 126);
            this.trackBarLum.Maximum = 200;
            this.trackBarLum.Name = "trackBarLum";
            this.trackBarLum.Size = new System.Drawing.Size(78, 45);
            this.trackBarLum.TabIndex = 45;
            this.trackBarLum.TickFrequency = 50;
            this.trackBarLum.Value = 100;
            this.trackBarLum.Scroll += new System.EventHandler(this.trackBarLum_Scroll);
            // 
            // btnInv
            // 
            this.btnInv.Location = new System.Drawing.Point(95, 24);
            this.btnInv.Name = "btnInv";
            this.btnInv.Size = new System.Drawing.Size(75, 23);
            this.btnInv.TabIndex = 46;
            this.btnInv.Text = "Invert mask";
            this.btnInv.UseVisualStyleBackColor = true;
            this.btnInv.Click += new System.EventHandler(this.btnInv_Click);
            // 
            // btnmaskloadtest
            // 
            this.btnmaskloadtest.Location = new System.Drawing.Point(17, 53);
            this.btnmaskloadtest.Name = "btnmaskloadtest";
            this.btnmaskloadtest.Size = new System.Drawing.Size(153, 23);
            this.btnmaskloadtest.TabIndex = 47;
            this.btnmaskloadtest.Text = "Use last mask";
            this.btnmaskloadtest.UseVisualStyleBackColor = true;
            this.btnmaskloadtest.Click += new System.EventHandler(this.btnmaskloadtest_Click);
            // 
            // txtAlpha
            // 
            this.txtAlpha.Location = new System.Drawing.Point(396, 164);
            this.txtAlpha.Name = "txtAlpha";
            this.txtAlpha.Size = new System.Drawing.Size(58, 20);
            this.txtAlpha.TabIndex = 48;
            this.txtAlpha.Text = "100";
            this.toolTip1.SetToolTip(this.txtAlpha, "Overlays the effect onto the original image if set to less than 100%");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(296, 166);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 49;
            this.label8.Text = "Effect Opacity %";
            this.toolTip1.SetToolTip(this.label8, "Overlays the effect onto the original image if set to less than 100%");
            // 
            // btnClearMask
            // 
            this.btnClearMask.Location = new System.Drawing.Point(17, 24);
            this.btnClearMask.Name = "btnClearMask";
            this.btnClearMask.Size = new System.Drawing.Size(75, 23);
            this.btnClearMask.TabIndex = 50;
            this.btnClearMask.Text = "Clear Mask";
            this.btnClearMask.UseVisualStyleBackColor = true;
            this.btnClearMask.Click += new System.EventHandler(this.btnClearMask_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFirst.Location = new System.Drawing.Point(901, 471);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(26, 23);
            this.btnFirst.TabIndex = 51;
            this.btnFirst.Text = "[<";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnLast
            // 
            this.btnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLast.Location = new System.Drawing.Point(1042, 471);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(26, 23);
            this.btnLast.TabIndex = 52;
            this.btnLast.Text = ">]";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // checkFrameRange
            // 
            this.checkFrameRange.AutoSize = true;
            this.checkFrameRange.Location = new System.Drawing.Point(21, 91);
            this.checkFrameRange.Name = "checkFrameRange";
            this.checkFrameRange.Size = new System.Drawing.Size(131, 17);
            this.checkFrameRange.TabIndex = 53;
            this.checkFrameRange.Text = "Apply effect to frames:";
            this.toolTip1.SetToolTip(this.checkFrameRange, "Only apply the effect to the frames in this range");
            this.checkFrameRange.UseVisualStyleBackColor = true;
            this.checkFrameRange.CheckedChanged += new System.EventHandler(this.checkFrameRange_CheckedChanged);
            // 
            // numMinFrame
            // 
            this.numMinFrame.Location = new System.Drawing.Point(21, 120);
            this.numMinFrame.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numMinFrame.Name = "numMinFrame";
            this.numMinFrame.Size = new System.Drawing.Size(50, 20);
            this.numMinFrame.TabIndex = 56;
            this.numMinFrame.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinFrame.ValueChanged += new System.EventHandler(this.numMinFrame_ValueChanged);
            // 
            // numMaxFrame
            // 
            this.numMaxFrame.Location = new System.Drawing.Point(21, 148);
            this.numMaxFrame.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numMaxFrame.Name = "numMaxFrame";
            this.numMaxFrame.Size = new System.Drawing.Size(50, 20);
            this.numMaxFrame.TabIndex = 57;
            this.numMaxFrame.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxFrame.ValueChanged += new System.EventHandler(this.numMaxFrame_ValueChanged);
            // 
            // btnFrameStart
            // 
            this.btnFrameStart.Location = new System.Drawing.Point(79, 120);
            this.btnFrameStart.Name = "btnFrameStart";
            this.btnFrameStart.Size = new System.Drawing.Size(75, 20);
            this.btnFrameStart.TabIndex = 59;
            this.btnFrameStart.Text = "Show start";
            this.toolTip1.SetToolTip(this.btnFrameStart, "Show the frame in the viewing window");
            this.btnFrameStart.UseVisualStyleBackColor = true;
            this.btnFrameStart.Click += new System.EventHandler(this.btnFrameStart_Click);
            // 
            // btnFrameEnd
            // 
            this.btnFrameEnd.Location = new System.Drawing.Point(79, 148);
            this.btnFrameEnd.Name = "btnFrameEnd";
            this.btnFrameEnd.Size = new System.Drawing.Size(75, 20);
            this.btnFrameEnd.TabIndex = 60;
            this.btnFrameEnd.Text = "Show end";
            this.toolTip1.SetToolTip(this.btnFrameEnd, "Show the frame in the viewing window");
            this.btnFrameEnd.UseVisualStyleBackColor = true;
            this.btnFrameEnd.Click += new System.EventHandler(this.btnFrameEnd_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.panelHSVPreview);
            this.groupBox1.Controls.Add(this.trackBarHue);
            this.groupBox1.Controls.Add(this.txtPxlSize);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioPixel);
            this.groupBox1.Controls.Add(this.radioSolid);
            this.groupBox1.Controls.Add(this.radioBlur);
            this.groupBox1.Controls.Add(this.txtBlurRad);
            this.groupBox1.Controls.Add(this.txtAlpha);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.radioStaticColor);
            this.groupBox1.Controls.Add(this.radioStaticMono);
            this.groupBox1.Controls.Add(this.radioRGBS);
            this.groupBox1.Controls.Add(this.trackBarLum);
            this.groupBox1.Controls.Add(this.txtRGBs);
            this.groupBox1.Controls.Add(this.trackBarSat);
            this.groupBox1.Controls.Add(this.radioJitter);
            this.groupBox1.Controls.Add(this.txtJitter);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnSolidColor);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.radioHSV);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtHue);
            this.groupBox1.Controls.Add(this.txtLum);
            this.groupBox1.Controls.Add(this.txtSat);
            this.groupBox1.Location = new System.Drawing.Point(16, 482);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(468, 190);
            this.groupBox1.TabIndex = 58;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Effects";
            // 
            // panelHSVPreview
            // 
            this.panelHSVPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelHSVPreview.BackColor = System.Drawing.Color.Black;
            this.panelHSVPreview.Location = new System.Drawing.Point(219, 40);
            this.panelHSVPreview.Name = "panelHSVPreview";
            this.panelHSVPreview.Size = new System.Drawing.Size(49, 25);
            this.panelHSVPreview.TabIndex = 33;
            this.toolTip1.SetToolTip(this.panelHSVPreview, "Preview the HSV effect changes. Use the chroma eyedropper tool to pick a color.");
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnStep);
            this.groupBox2.Controls.Add(this.btnFrameEnd);
            this.groupBox2.Controls.Add(this.btnInv);
            this.groupBox2.Controls.Add(this.btnmaskloadtest);
            this.groupBox2.Controls.Add(this.btnFrameStart);
            this.groupBox2.Controls.Add(this.btnClearMask);
            this.groupBox2.Controls.Add(this.numMaxFrame);
            this.groupBox2.Controls.Add(this.panelChromaColor);
            this.groupBox2.Controls.Add(this.checkFrameRange);
            this.groupBox2.Controls.Add(this.btnPickColor);
            this.groupBox2.Controls.Add(this.numMinFrame);
            this.groupBox2.Controls.Add(this.txtChromaSens);
            this.groupBox2.Controls.Add(this.checkChroma);
            this.groupBox2.Controls.Add(this.btnChromaCol);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(513, 482);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(366, 190);
            this.groupBox2.TabIndex = 61;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mask tools";
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(158, 120);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(41, 48);
            this.btnStep.TabIndex = 61;
            this.btnStep.Text = "Step";
            this.toolTip1.SetToolTip(this.btnStep, "Increments the frame range by the current difference between the positions. Usefu" +
        "l when you want to make lots of subsequent edits.");
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // checkEncodeVid
            // 
            this.checkEncodeVid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkEncodeVid.AutoSize = true;
            this.checkEncodeVid.Checked = true;
            this.checkEncodeVid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkEncodeVid.Location = new System.Drawing.Point(653, 466);
            this.checkEncodeVid.Name = "checkEncodeVid";
            this.checkEncodeVid.Size = new System.Drawing.Size(92, 17);
            this.checkEncodeVid.TabIndex = 62;
            this.checkEncodeVid.Text = "Encode video";
            this.toolTip1.SetToolTip(this.checkEncodeVid, "If unchecked, video frames will still be processed and stored, but the encoding s" +
        "tep is skipped. Saves time when making multiple edits to a video.");
            this.checkEncodeVid.UseVisualStyleBackColor = true;
            // 
            // checkReuseProcessed
            // 
            this.checkReuseProcessed.AutoSize = true;
            this.checkReuseProcessed.Checked = true;
            this.checkReuseProcessed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkReuseProcessed.Location = new System.Drawing.Point(15, 83);
            this.checkReuseProcessed.Name = "checkReuseProcessed";
            this.checkReuseProcessed.Size = new System.Drawing.Size(143, 17);
            this.checkReuseProcessed.TabIndex = 62;
            this.checkReuseProcessed.Text = "Resue processed frames";
            this.toolTip1.SetToolTip(this.checkReuseProcessed, "Reuse the frames we already extracted, rather than rextracting all the frames aga" +
        "in");
            this.checkReuseProcessed.UseVisualStyleBackColor = true;
            this.checkReuseProcessed.Click += new System.EventHandler(this.checkReuseProcessed_CheckedChanged);
            // 
            // btnPurge
            // 
            this.btnPurge.Location = new System.Drawing.Point(15, 107);
            this.btnPurge.Name = "btnPurge";
            this.btnPurge.Size = new System.Drawing.Size(125, 46);
            this.btnPurge.TabIndex = 63;
            this.btnPurge.Text = "Purge frame folders";
            this.toolTip1.SetToolTip(this.btnPurge, "Deletes any extracted or processed frame folders, in the same folder as the curre" +
        "ntly loaded file");
            this.btnPurge.UseVisualStyleBackColor = true;
            this.btnPurge.Click += new System.EventHandler(this.btnPurge_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnPurge);
            this.tabPage2.Controls.Add(this.checkReuseProcessed);
            this.tabPage2.Controls.Add(this.pictureBox2);
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Controls.Add(this.btnShowRange);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1052, 425);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Animation utilities";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Location = new System.Drawing.Point(618, 6);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(450, 407);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Location = new System.Drawing.Point(157, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(450, 407);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnShowRange
            // 
            this.btnShowRange.Location = new System.Drawing.Point(12, 21);
            this.btnShowRange.Name = "btnShowRange";
            this.btnShowRange.Size = new System.Drawing.Size(125, 55);
            this.btnShowRange.TabIndex = 0;
            this.btnShowRange.Text = "Show frames at range";
            this.btnShowRange.UseVisualStyleBackColor = true;
            this.btnShowRange.Click += new System.EventHandler(this.btnShowRange_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.webView21);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1052, 425);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(13, 20);
            this.webView21.Margin = new System.Windows.Forms.Padding(20);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(1026, 394);
            this.webView21.TabIndex = 4;
            this.webView21.ZoomFactor = 1D;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1060, 451);
            this.tabControl1.TabIndex = 35;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblVersion.Location = new System.Drawing.Point(1044, 9);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(28, 13);
            this.lblVersion.TabIndex = 63;
            this.lblVersion.Text = "v1.0";
            this.lblVersion.Click += new System.EventHandler(this.lblVersion_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 681);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.checkEncodeVid);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.chkDispProcessed);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnCensor);
            this.Controls.Add(this.lblSel);
            this.Controls.Add(this.lblSize);
            this.MinimumSize = new System.Drawing.Size(1100, 720);
            this.Name = "Form1";
            this.Text = "GifCensor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFrame)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label lblSel;
        private System.Windows.Forms.TextBox txtPxlSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCensor;
        private System.Windows.Forms.RadioButton radioPixel;
        private System.Windows.Forms.RadioButton radioSolid;
        private System.Windows.Forms.RadioButton radioBlur;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBlurRad;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.CheckBox chkDispProcessed;
        private System.Windows.Forms.RadioButton radioStaticColor;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RadioButton radioStaticMono;
        private System.Windows.Forms.RadioButton radioRGBS;
        private System.Windows.Forms.TextBox txtRGBs;
        private System.Windows.Forms.RadioButton radioJitter;
        private System.Windows.Forms.TextBox txtJitter;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btnChromaCol;
        private System.Windows.Forms.CheckBox checkChroma;
        private System.Windows.Forms.TextBox txtChromaSens;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPickColor;
        private System.Windows.Forms.Button btnSolidColor;
        private System.Windows.Forms.ColorDialog colorDialog2;
        private System.Windows.Forms.Panel panelChromaColor;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.RadioButton radioHSV;
        private System.Windows.Forms.TextBox txtHue;
        private System.Windows.Forms.TextBox txtSat;
        private System.Windows.Forms.TextBox txtLum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar trackBarHue;
        private System.Windows.Forms.TrackBar trackBarSat;
        private System.Windows.Forms.TrackBar trackBarLum;
        private System.Windows.Forms.Button btnInv;
        private System.Windows.Forms.Button btnmaskloadtest;
        private System.Windows.Forms.TextBox txtAlpha;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnClearMask;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.CheckBox checkFrameRange;
        private System.Windows.Forms.NumericUpDown numMinFrame;
        private System.Windows.Forms.NumericUpDown numMaxFrame;
        private System.Windows.Forms.Button btnFrameEnd;
        private System.Windows.Forms.Button btnFrameStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.CheckBox checkEncodeVid;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panelHSVPreview;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnPurge;
        private System.Windows.Forms.CheckBox checkReuseProcessed;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnShowRange;
        private System.Windows.Forms.TabPage tabPage1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label lblVersion;
    }
}

