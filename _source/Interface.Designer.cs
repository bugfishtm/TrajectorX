namespace trajectorx
{
    partial class Interface
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interface));
            Panel_Top = new Panel();
            Label_Version = new Label();
            Image_Logo = new PictureBox();
            Label_Title = new Label();
            Panel_Body = new Panel();
            Panel_Right = new Panel();
            Panel_NoFocus = new Panel();
            groupBox4 = new GroupBox();
            button8 = new Button();
            button7 = new Button();
            LabelSettingsRotation = new Label();
            button5 = new Button();
            button4 = new Button();
            button6 = new Button();
            LabelSimScale = new Label();
            button3 = new Button();
            button2 = new Button();
            Label_CamSpeed = new Label();
            button1 = new Button();
            Label_CamZSpeed = new Label();
            groupBox3 = new GroupBox();
            Label_C = new Label();
            Label_Pi = new Label();
            Label_G = new Label();
            groupBox2 = new GroupBox();
            Label_CamX = new Label();
            Label_CamY = new Label();
            Label_CamZ = new Label();
            Label_CamRX = new Label();
            Label_CamRY = new Label();
            Label_CamRZ = new Label();
            Panel_Focus = new Panel();
            Label_F_Z = new Label();
            Label_F_Y = new Label();
            Label_F_X = new Label();
            groupBox1 = new GroupBox();
            Label_F_Mass = new Label();
            Label_F_Radius = new Label();
            Label_F_V_Z = new Label();
            Label_F_V_Y = new Label();
            Label_F_V_X = new Label();
            Button_FocusClose = new Button();
            Panel_Left = new Panel();
            Button_New = new Button();
            Button_Save = new Button();
            Button_Load = new Button();
            List_Sections = new ListBox();
            Panel_Content = new Panel();
            Panel_Settings = new Panel();
            richTextBox1 = new RichTextBox();
            Panel_Bodies = new Panel();
            List_Bodies = new ListView();
            Panel_Bottom = new Panel();
            labelTime = new Label();
            buttonTimeSlow = new Button();
            buttonTimeReset = new Button();
            buttonTimePause = new Button();
            buttonTimeStart = new Button();
            cb_rbar = new CheckBox();
            cb_lbar = new CheckBox();
            buttonTimeFaster = new Button();
            Panel_Background = new Panel();
            tooltip_frame = new ToolTip(components);
            btnMinimize = new Button();
            btnMaximize = new Button();
            btnClose = new Button();
            Panel_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Image_Logo).BeginInit();
            Panel_Body.SuspendLayout();
            Panel_Right.SuspendLayout();
            Panel_NoFocus.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            Panel_Focus.SuspendLayout();
            Panel_Left.SuspendLayout();
            Panel_Content.SuspendLayout();
            Panel_Settings.SuspendLayout();
            Panel_Bodies.SuspendLayout();
            Panel_Bottom.SuspendLayout();
            SuspendLayout();
            // 
            // Panel_Top
            // 
            Panel_Top.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Panel_Top.BackColor = Color.FromArgb(32, 32, 32);
            Panel_Top.Controls.Add(Label_Version);
            Panel_Top.Controls.Add(Image_Logo);
            Panel_Top.Controls.Add(Label_Title);
            Panel_Top.Location = new Point(0, 0);
            Panel_Top.Name = "Panel_Top";
            Panel_Top.Size = new Size(1169, 59);
            Panel_Top.TabIndex = 0;
            Panel_Top.MouseDown += CustomUI_Header_Panel_MouseDown;
            // 
            // Label_Version
            // 
            Label_Version.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label_Version.AutoSize = true;
            Label_Version.BackColor = Color.Transparent;
            Label_Version.Font = new Font("Segoe UI", 10F);
            Label_Version.ForeColor = Color.White;
            Label_Version.Location = new Point(1063, 34);
            Label_Version.Name = "Label_Version";
            Label_Version.Size = new Size(106, 23);
            Label_Version.TabIndex = 4;
            Label_Version.Text = "Version 1.0.1";
            // 
            // Image_Logo
            // 
            Image_Logo.BackgroundImage = (Image)resources.GetObject("Image_Logo.BackgroundImage");
            Image_Logo.BackgroundImageLayout = ImageLayout.Zoom;
            Image_Logo.Location = new Point(6, 6);
            Image_Logo.Name = "Image_Logo";
            Image_Logo.Padding = new Padding(5);
            Image_Logo.Size = new Size(66, 51);
            Image_Logo.TabIndex = 1;
            Image_Logo.TabStop = false;
            // 
            // Label_Title
            // 
            Label_Title.AutoSize = true;
            Label_Title.BackColor = Color.Transparent;
            Label_Title.Font = new Font("Segoe UI", 25.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label_Title.ForeColor = Color.White;
            Label_Title.Location = new Point(75, 0);
            Label_Title.Margin = new Padding(0);
            Label_Title.Name = "Label_Title";
            Label_Title.Size = new Size(209, 57);
            Label_Title.TabIndex = 3;
            Label_Title.Text = "TrajectorX";
            // 
            // Panel_Body
            // 
            Panel_Body.BackColor = Color.FromArgb(12, 12, 12);
            Panel_Body.Controls.Add(Panel_Right);
            Panel_Body.Controls.Add(Panel_Left);
            Panel_Body.Controls.Add(Panel_Content);
            Panel_Body.Controls.Add(Panel_Bottom);
            Panel_Body.Dock = DockStyle.Fill;
            Panel_Body.Location = new Point(0, 0);
            Panel_Body.Name = "Panel_Body";
            Panel_Body.Size = new Size(1169, 707);
            Panel_Body.TabIndex = 1;
            // 
            // Panel_Right
            // 
            Panel_Right.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            Panel_Right.BackColor = Color.FromArgb(24, 24, 24);
            Panel_Right.Controls.Add(Panel_NoFocus);
            Panel_Right.Controls.Add(Panel_Focus);
            Panel_Right.Location = new Point(956, 61);
            Panel_Right.Name = "Panel_Right";
            Panel_Right.Size = new Size(210, 620);
            Panel_Right.TabIndex = 10;
            // 
            // Panel_NoFocus
            // 
            Panel_NoFocus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Panel_NoFocus.BackColor = Color.FromArgb(12, 12, 12);
            Panel_NoFocus.Controls.Add(groupBox4);
            Panel_NoFocus.Controls.Add(groupBox3);
            Panel_NoFocus.Controls.Add(groupBox2);
            Panel_NoFocus.Location = new Point(0, 0);
            Panel_NoFocus.Name = "Panel_NoFocus";
            Panel_NoFocus.Size = new Size(210, 620);
            Panel_NoFocus.TabIndex = 0;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(button8);
            groupBox4.Controls.Add(button7);
            groupBox4.Controls.Add(LabelSettingsRotation);
            groupBox4.Controls.Add(button5);
            groupBox4.Controls.Add(button4);
            groupBox4.Controls.Add(button6);
            groupBox4.Controls.Add(LabelSimScale);
            groupBox4.Controls.Add(button3);
            groupBox4.Controls.Add(button2);
            groupBox4.Controls.Add(Label_CamSpeed);
            groupBox4.Controls.Add(button1);
            groupBox4.Controls.Add(Label_CamZSpeed);
            groupBox4.ForeColor = SystemColors.ButtonHighlight;
            groupBox4.Location = new Point(3, 267);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(204, 223);
            groupBox4.TabIndex = 14;
            groupBox4.TabStop = false;
            groupBox4.Text = "Settings";
            // 
            // button8
            // 
            button8.Font = new Font("Segoe UI", 7F);
            button8.ForeColor = SystemColors.ActiveCaption;
            button8.Location = new Point(65, 192);
            button8.Name = "button8";
            button8.Size = new Size(47, 23);
            button8.TabIndex = 22;
            button8.Text = "-";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button7
            // 
            button7.Font = new Font("Segoe UI", 7F);
            button7.ForeColor = SystemColors.ActiveCaption;
            button7.Location = new Point(12, 192);
            button7.Name = "button7";
            button7.Size = new Size(47, 23);
            button7.TabIndex = 21;
            button7.Text = "+";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // LabelSettingsRotation
            // 
            LabelSettingsRotation.AutoSize = true;
            LabelSettingsRotation.ForeColor = Color.FromArgb(31, 199, 255);
            LabelSettingsRotation.Location = new Point(12, 170);
            LabelSettingsRotation.Name = "LabelSettingsRotation";
            LabelSettingsRotation.Size = new Size(50, 20);
            LabelSettingsRotation.TabIndex = 20;
            LabelSettingsRotation.Text = "label1";
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 7F);
            button5.ForeColor = SystemColors.ActiveCaption;
            button5.Location = new Point(65, 144);
            button5.Name = "button5";
            button5.Size = new Size(47, 23);
            button5.TabIndex = 19;
            button5.Text = "-";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Font = new Font("Segoe UI", 7F);
            button4.ForeColor = SystemColors.ActiveCaption;
            button4.Location = new Point(65, 95);
            button4.Name = "button4";
            button4.Size = new Size(47, 23);
            button4.TabIndex = 19;
            button4.Text = "-";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button6
            // 
            button6.Font = new Font("Segoe UI", 7F);
            button6.ForeColor = SystemColors.ActiveCaption;
            button6.Location = new Point(12, 144);
            button6.Name = "button6";
            button6.Size = new Size(47, 23);
            button6.TabIndex = 16;
            button6.Text = "+";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // LabelSimScale
            // 
            LabelSimScale.AutoSize = true;
            LabelSimScale.ForeColor = Color.FromArgb(31, 199, 255);
            LabelSimScale.Location = new Point(12, 121);
            LabelSimScale.Name = "LabelSimScale";
            LabelSimScale.Size = new Size(50, 20);
            LabelSimScale.TabIndex = 6;
            LabelSimScale.Text = "label1";
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 7F);
            button3.ForeColor = SystemColors.ActiveCaption;
            button3.Location = new Point(65, 46);
            button3.Name = "button3";
            button3.Size = new Size(47, 23);
            button3.TabIndex = 18;
            button3.Text = "-";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 7F);
            button2.ForeColor = SystemColors.ActiveCaption;
            button2.Location = new Point(12, 95);
            button2.Name = "button2";
            button2.Size = new Size(47, 23);
            button2.TabIndex = 16;
            button2.Text = "+";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Label_CamSpeed
            // 
            Label_CamSpeed.AutoSize = true;
            Label_CamSpeed.ForeColor = Color.FromArgb(31, 199, 255);
            Label_CamSpeed.Location = new Point(12, 72);
            Label_CamSpeed.Name = "Label_CamSpeed";
            Label_CamSpeed.Size = new Size(50, 20);
            Label_CamSpeed.TabIndex = 6;
            Label_CamSpeed.Text = "label1";
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 7F);
            button1.ForeColor = SystemColors.ActiveCaption;
            button1.Location = new Point(12, 46);
            button1.Name = "button1";
            button1.Size = new Size(47, 23);
            button1.TabIndex = 17;
            button1.Text = "+";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // Label_CamZSpeed
            // 
            Label_CamZSpeed.AutoSize = true;
            Label_CamZSpeed.ForeColor = Color.FromArgb(31, 199, 255);
            Label_CamZSpeed.Location = new Point(12, 23);
            Label_CamZSpeed.Name = "Label_CamZSpeed";
            Label_CamZSpeed.Size = new Size(50, 20);
            Label_CamZSpeed.TabIndex = 7;
            Label_CamZSpeed.Text = "label1";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(Label_C);
            groupBox3.Controls.Add(Label_Pi);
            groupBox3.Controls.Add(Label_G);
            groupBox3.ForeColor = SystemColors.Window;
            groupBox3.Location = new Point(3, 166);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(204, 95);
            groupBox3.TabIndex = 13;
            groupBox3.TabStop = false;
            groupBox3.Text = "Mathematical";
            // 
            // Label_C
            // 
            Label_C.AutoSize = true;
            Label_C.ForeColor = Color.FromArgb(31, 199, 255);
            Label_C.Location = new Point(12, 63);
            Label_C.Name = "Label_C";
            Label_C.Size = new Size(50, 20);
            Label_C.TabIndex = 10;
            Label_C.Text = "label1";
            // 
            // Label_Pi
            // 
            Label_Pi.AutoSize = true;
            Label_Pi.ForeColor = Color.FromArgb(31, 199, 255);
            Label_Pi.Location = new Point(12, 24);
            Label_Pi.Name = "Label_Pi";
            Label_Pi.Size = new Size(50, 20);
            Label_Pi.TabIndex = 8;
            Label_Pi.Text = "label1";
            // 
            // Label_G
            // 
            Label_G.AutoSize = true;
            Label_G.ForeColor = Color.FromArgb(31, 199, 255);
            Label_G.Location = new Point(12, 43);
            Label_G.Name = "Label_G";
            Label_G.Size = new Size(50, 20);
            Label_G.TabIndex = 9;
            Label_G.Text = "label1";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(Label_CamX);
            groupBox2.Controls.Add(Label_CamY);
            groupBox2.Controls.Add(Label_CamZ);
            groupBox2.Controls.Add(Label_CamRX);
            groupBox2.Controls.Add(Label_CamRY);
            groupBox2.Controls.Add(Label_CamRZ);
            groupBox2.ForeColor = SystemColors.Control;
            groupBox2.Location = new Point(3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(204, 156);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "Camera Position";
            // 
            // Label_CamX
            // 
            Label_CamX.AutoSize = true;
            Label_CamX.ForeColor = Color.FromArgb(31, 199, 255);
            Label_CamX.Location = new Point(12, 23);
            Label_CamX.Name = "Label_CamX";
            Label_CamX.Size = new Size(50, 20);
            Label_CamX.TabIndex = 0;
            Label_CamX.Text = "label1";
            // 
            // Label_CamY
            // 
            Label_CamY.AutoSize = true;
            Label_CamY.ForeColor = Color.FromArgb(31, 199, 255);
            Label_CamY.Location = new Point(12, 43);
            Label_CamY.Name = "Label_CamY";
            Label_CamY.Size = new Size(50, 20);
            Label_CamY.TabIndex = 1;
            Label_CamY.Text = "label1";
            // 
            // Label_CamZ
            // 
            Label_CamZ.AutoSize = true;
            Label_CamZ.ForeColor = Color.FromArgb(31, 199, 255);
            Label_CamZ.Location = new Point(12, 63);
            Label_CamZ.Name = "Label_CamZ";
            Label_CamZ.Size = new Size(50, 20);
            Label_CamZ.TabIndex = 2;
            Label_CamZ.Text = "label1";
            // 
            // Label_CamRX
            // 
            Label_CamRX.AutoSize = true;
            Label_CamRX.ForeColor = Color.FromArgb(31, 199, 255);
            Label_CamRX.Location = new Point(11, 84);
            Label_CamRX.Name = "Label_CamRX";
            Label_CamRX.Size = new Size(50, 20);
            Label_CamRX.TabIndex = 4;
            Label_CamRX.Text = "label1";
            // 
            // Label_CamRY
            // 
            Label_CamRY.AutoSize = true;
            Label_CamRY.ForeColor = Color.FromArgb(31, 199, 255);
            Label_CamRY.Location = new Point(11, 104);
            Label_CamRY.Name = "Label_CamRY";
            Label_CamRY.Size = new Size(50, 20);
            Label_CamRY.TabIndex = 5;
            Label_CamRY.Text = "label1";
            // 
            // Label_CamRZ
            // 
            Label_CamRZ.AutoSize = true;
            Label_CamRZ.ForeColor = Color.FromArgb(31, 199, 255);
            Label_CamRZ.Location = new Point(11, 124);
            Label_CamRZ.Name = "Label_CamRZ";
            Label_CamRZ.Size = new Size(50, 20);
            Label_CamRZ.TabIndex = 3;
            Label_CamRZ.Text = "label1";
            // 
            // Panel_Focus
            // 
            Panel_Focus.Controls.Add(Label_F_Z);
            Panel_Focus.Controls.Add(Label_F_Y);
            Panel_Focus.Controls.Add(Label_F_X);
            Panel_Focus.Controls.Add(groupBox1);
            Panel_Focus.Controls.Add(Label_F_Mass);
            Panel_Focus.Controls.Add(Label_F_Radius);
            Panel_Focus.Controls.Add(Label_F_V_Z);
            Panel_Focus.Controls.Add(Label_F_V_Y);
            Panel_Focus.Controls.Add(Label_F_V_X);
            Panel_Focus.Controls.Add(Button_FocusClose);
            Panel_Focus.Location = new Point(0, 0);
            Panel_Focus.Name = "Panel_Focus";
            Panel_Focus.Size = new Size(210, 620);
            Panel_Focus.TabIndex = 12;
            // 
            // Label_F_Z
            // 
            Label_F_Z.AutoSize = true;
            Label_F_Z.ForeColor = SystemColors.ControlLightLight;
            Label_F_Z.Location = new Point(14, 67);
            Label_F_Z.Name = "Label_F_Z";
            Label_F_Z.Size = new Size(97, 20);
            Label_F_Z.TabIndex = 3;
            Label_F_Z.Text = "Position Z: 33";
            // 
            // Label_F_Y
            // 
            Label_F_Y.AutoSize = true;
            Label_F_Y.ForeColor = SystemColors.ControlLightLight;
            Label_F_Y.Location = new Point(10, 44);
            Label_F_Y.Name = "Label_F_Y";
            Label_F_Y.Size = new Size(96, 20);
            Label_F_Y.TabIndex = 2;
            Label_F_Y.Text = "Position Y: 33";
            // 
            // Label_F_X
            // 
            Label_F_X.AutoSize = true;
            Label_F_X.ForeColor = SystemColors.ControlLightLight;
            Label_F_X.Location = new Point(15, 18);
            Label_F_X.Name = "Label_F_X";
            Label_F_X.Size = new Size(69, 20);
            Label_F_X.TabIndex = 1;
            Label_F_X.Text = "Pos-X: 33";
            // 
            // groupBox1
            // 
            groupBox1.ForeColor = SystemColors.ButtonHighlight;
            groupBox1.Location = new Point(15, 303);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(163, 125);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "Camera";
            // 
            // Label_F_Mass
            // 
            Label_F_Mass.AutoSize = true;
            Label_F_Mass.ForeColor = SystemColors.ControlLightLight;
            Label_F_Mass.Location = new Point(15, 190);
            Label_F_Mass.Name = "Label_F_Mass";
            Label_F_Mass.Size = new Size(81, 20);
            Label_F_Mass.TabIndex = 7;
            Label_F_Mass.Text = "Mass: 400T";
            // 
            // Label_F_Radius
            // 
            Label_F_Radius.AutoSize = true;
            Label_F_Radius.ForeColor = SystemColors.ControlLightLight;
            Label_F_Radius.Location = new Point(15, 219);
            Label_F_Radius.Name = "Label_F_Radius";
            Label_F_Radius.Size = new Size(89, 20);
            Label_F_Radius.TabIndex = 8;
            Label_F_Radius.Text = "Radius: 50M";
            // 
            // Label_F_V_Z
            // 
            Label_F_V_Z.AutoSize = true;
            Label_F_V_Z.ForeColor = SystemColors.ControlLightLight;
            Label_F_V_Z.Location = new Point(15, 161);
            Label_F_V_Z.Name = "Label_F_V_Z";
            Label_F_V_Z.Size = new Size(97, 20);
            Label_F_V_Z.TabIndex = 6;
            Label_F_V_Z.Text = "Velocity Z: 33";
            // 
            // Label_F_V_Y
            // 
            Label_F_V_Y.AutoSize = true;
            Label_F_V_Y.ForeColor = SystemColors.ControlLightLight;
            Label_F_V_Y.Location = new Point(15, 130);
            Label_F_V_Y.Name = "Label_F_V_Y";
            Label_F_V_Y.Size = new Size(96, 20);
            Label_F_V_Y.TabIndex = 5;
            Label_F_V_Y.Text = "Velocity Y: 33";
            // 
            // Label_F_V_X
            // 
            Label_F_V_X.AutoSize = true;
            Label_F_V_X.ForeColor = SystemColors.ControlLightLight;
            Label_F_V_X.Location = new Point(7, 90);
            Label_F_V_X.Name = "Label_F_V_X";
            Label_F_V_X.Size = new Size(97, 20);
            Label_F_V_X.TabIndex = 4;
            Label_F_V_X.Text = "Velocity X: 33";
            // 
            // Button_FocusClose
            // 
            Button_FocusClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Button_FocusClose.Location = new Point(107, 586);
            Button_FocusClose.Name = "Button_FocusClose";
            Button_FocusClose.Size = new Size(94, 29);
            Button_FocusClose.TabIndex = 0;
            Button_FocusClose.Text = "Close";
            Button_FocusClose.UseVisualStyleBackColor = true;
            Button_FocusClose.Click += Button_FocusClose_Click;
            // 
            // Panel_Left
            // 
            Panel_Left.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Panel_Left.BackColor = Color.FromArgb(32, 32, 32);
            Panel_Left.Controls.Add(Button_New);
            Panel_Left.Controls.Add(Button_Save);
            Panel_Left.Controls.Add(Button_Load);
            Panel_Left.Controls.Add(List_Sections);
            Panel_Left.Location = new Point(0, 61);
            Panel_Left.Margin = new Padding(0);
            Panel_Left.Name = "Panel_Left";
            Panel_Left.Size = new Size(210, 620);
            Panel_Left.TabIndex = 9;
            // 
            // Button_New
            // 
            Button_New.Location = new Point(138, 9);
            Button_New.Name = "Button_New";
            Button_New.Size = new Size(57, 29);
            Button_New.TabIndex = 4;
            Button_New.Text = "New";
            Button_New.UseVisualStyleBackColor = true;
            Button_New.Click += Button_New_Click;
            // 
            // Button_Save
            // 
            Button_Save.Location = new Point(75, 9);
            Button_Save.Name = "Button_Save";
            Button_Save.Size = new Size(57, 29);
            Button_Save.TabIndex = 3;
            Button_Save.Text = "Save";
            Button_Save.UseVisualStyleBackColor = true;
            Button_Save.Click += Button_Save_Click;
            // 
            // Button_Load
            // 
            Button_Load.Location = new Point(12, 9);
            Button_Load.Name = "Button_Load";
            Button_Load.Size = new Size(57, 29);
            Button_Load.TabIndex = 2;
            Button_Load.Text = "Load";
            Button_Load.UseVisualStyleBackColor = true;
            Button_Load.Click += Button_Load_Click;
            // 
            // List_Sections
            // 
            List_Sections.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            List_Sections.BackColor = Color.FromArgb(32, 32, 32);
            List_Sections.BorderStyle = BorderStyle.None;
            List_Sections.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            List_Sections.ForeColor = SystemColors.ControlLightLight;
            List_Sections.FormattingEnabled = true;
            List_Sections.ItemHeight = 28;
            List_Sections.Location = new Point(0, 56);
            List_Sections.Margin = new Padding(10, 3, 3, 3);
            List_Sections.Name = "List_Sections";
            List_Sections.Size = new Size(210, 560);
            List_Sections.TabIndex = 1;
            // 
            // Panel_Content
            // 
            Panel_Content.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel_Content.BackColor = Color.RosyBrown;
            Panel_Content.Controls.Add(Panel_Settings);
            Panel_Content.Controls.Add(Panel_Bodies);
            Panel_Content.Location = new Point(216, 65);
            Panel_Content.Name = "Panel_Content";
            Panel_Content.Size = new Size(734, 611);
            Panel_Content.TabIndex = 9;
            // 
            // Panel_Settings
            // 
            Panel_Settings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel_Settings.BackColor = Color.FromArgb(12, 12, 12);
            Panel_Settings.Controls.Add(richTextBox1);
            Panel_Settings.Location = new Point(0, 0);
            Panel_Settings.Name = "Panel_Settings";
            Panel_Settings.Size = new Size(734, 611);
            Panel_Settings.TabIndex = 1;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.FromArgb(32, 32, 32);
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.ForeColor = SystemColors.Window;
            richTextBox1.Location = new Point(104, 316);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ScrollBars = RichTextBoxScrollBars.None;
            richTextBox1.Size = new Size(195, 181);
            richTextBox1.TabIndex = 11;
            richTextBox1.Text = "WASD - Move with Camera\nMouse Wheel Up, Down - Scroll/Zoom\nAlt + Mouse Move - Rotate Camera";
            // 
            // Panel_Bodies
            // 
            Panel_Bodies.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel_Bodies.Controls.Add(List_Bodies);
            Panel_Bodies.Location = new Point(0, 0);
            Panel_Bodies.Name = "Panel_Bodies";
            Panel_Bodies.Size = new Size(734, 611);
            Panel_Bodies.TabIndex = 0;
            // 
            // List_Bodies
            // 
            List_Bodies.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            List_Bodies.BorderStyle = BorderStyle.None;
            List_Bodies.GridLines = true;
            List_Bodies.Location = new Point(0, 0);
            List_Bodies.Name = "List_Bodies";
            List_Bodies.Size = new Size(734, 611);
            List_Bodies.TabIndex = 0;
            List_Bodies.UseCompatibleStateImageBehavior = false;
            List_Bodies.View = View.List;
            // 
            // Panel_Bottom
            // 
            Panel_Bottom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel_Bottom.BackColor = Color.FromArgb(32, 32, 32);
            Panel_Bottom.Controls.Add(labelTime);
            Panel_Bottom.Controls.Add(buttonTimeSlow);
            Panel_Bottom.Controls.Add(buttonTimeReset);
            Panel_Bottom.Controls.Add(buttonTimePause);
            Panel_Bottom.Controls.Add(buttonTimeStart);
            Panel_Bottom.Controls.Add(cb_rbar);
            Panel_Bottom.Controls.Add(cb_lbar);
            Panel_Bottom.Controls.Add(buttonTimeFaster);
            Panel_Bottom.Location = new Point(0, 682);
            Panel_Bottom.Name = "Panel_Bottom";
            Panel_Bottom.Size = new Size(1169, 25);
            Panel_Bottom.TabIndex = 12;
            // 
            // labelTime
            // 
            labelTime.AutoSize = true;
            labelTime.ForeColor = SystemColors.ButtonFace;
            labelTime.Location = new Point(878, 1);
            labelTime.Name = "labelTime";
            labelTime.Size = new Size(50, 20);
            labelTime.TabIndex = 14;
            labelTime.Text = "label1";
            // 
            // buttonTimeSlow
            // 
            buttonTimeSlow.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonTimeSlow.Font = new Font("Segoe UI", 7F);
            buttonTimeSlow.Location = new Point(934, 1);
            buttonTimeSlow.Name = "buttonTimeSlow";
            buttonTimeSlow.Size = new Size(26, 23);
            buttonTimeSlow.TabIndex = 10;
            buttonTimeSlow.Text = "-";
            buttonTimeSlow.UseVisualStyleBackColor = true;
            buttonTimeSlow.Click += buttonTimeSlow_Click;
            // 
            // buttonTimeReset
            // 
            buttonTimeReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonTimeReset.Font = new Font("Segoe UI", 7F);
            buttonTimeReset.Location = new Point(966, 1);
            buttonTimeReset.Name = "buttonTimeReset";
            buttonTimeReset.Size = new Size(52, 23);
            buttonTimeReset.TabIndex = 13;
            buttonTimeReset.Text = "Reset";
            buttonTimeReset.UseVisualStyleBackColor = true;
            // 
            // buttonTimePause
            // 
            buttonTimePause.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonTimePause.Font = new Font("Segoe UI", 7F);
            buttonTimePause.Location = new Point(1024, 1);
            buttonTimePause.Name = "buttonTimePause";
            buttonTimePause.Size = new Size(52, 23);
            buttonTimePause.TabIndex = 11;
            buttonTimePause.Text = "Pause";
            buttonTimePause.UseVisualStyleBackColor = true;
            // 
            // buttonTimeStart
            // 
            buttonTimeStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonTimeStart.Font = new Font("Segoe UI", 7F);
            buttonTimeStart.Location = new Point(1082, 1);
            buttonTimeStart.Name = "buttonTimeStart";
            buttonTimeStart.Size = new Size(52, 23);
            buttonTimeStart.TabIndex = 12;
            buttonTimeStart.Text = "Start";
            buttonTimeStart.UseVisualStyleBackColor = true;
            // 
            // cb_rbar
            // 
            cb_rbar.AutoSize = true;
            cb_rbar.Checked = true;
            cb_rbar.CheckState = CheckState.Checked;
            cb_rbar.ForeColor = SystemColors.ButtonHighlight;
            cb_rbar.Location = new Point(120, 0);
            cb_rbar.Name = "cb_rbar";
            cb_rbar.Size = new Size(121, 24);
            cb_rbar.TabIndex = 1;
            cb_rbar.Text = "Right Sidebar";
            cb_rbar.UseVisualStyleBackColor = true;
            cb_rbar.CheckedChanged += cb_rbar_CheckedChanged;
            // 
            // cb_lbar
            // 
            cb_lbar.AutoSize = true;
            cb_lbar.Checked = true;
            cb_lbar.CheckState = CheckState.Checked;
            cb_lbar.ForeColor = SystemColors.ButtonHighlight;
            cb_lbar.Location = new Point(3, 0);
            cb_lbar.Name = "cb_lbar";
            cb_lbar.Size = new Size(111, 24);
            cb_lbar.TabIndex = 0;
            cb_lbar.Text = "Left Sidebar";
            cb_lbar.UseVisualStyleBackColor = true;
            cb_lbar.CheckedChanged += cb_lbar_CheckedChanged;
            // 
            // buttonTimeFaster
            // 
            buttonTimeFaster.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonTimeFaster.Font = new Font("Segoe UI", 7F);
            buttonTimeFaster.Location = new Point(1140, 1);
            buttonTimeFaster.Name = "buttonTimeFaster";
            buttonTimeFaster.Size = new Size(26, 23);
            buttonTimeFaster.TabIndex = 9;
            buttonTimeFaster.Text = "+";
            buttonTimeFaster.UseVisualStyleBackColor = true;
            buttonTimeFaster.Click += button1_Click;
            // 
            // Panel_Background
            // 
            Panel_Background.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel_Background.BackColor = Color.FromArgb(24, 24, 24);
            Panel_Background.Location = new Point(0, 0);
            Panel_Background.Name = "Panel_Background";
            Panel_Background.Size = new Size(1169, 707);
            Panel_Background.TabIndex = 11;
            // 
            // btnMinimize
            // 
            btnMinimize.Location = new Point(0, 0);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(75, 23);
            btnMinimize.TabIndex = 0;
            // 
            // btnMaximize
            // 
            btnMaximize.Location = new Point(0, 0);
            btnMaximize.Name = "btnMaximize";
            btnMaximize.Size = new Size(75, 23);
            btnMaximize.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(0, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 0;
            // 
            // Interface
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(1169, 707);
            Controls.Add(Panel_Top);
            Controls.Add(Panel_Body);
            Controls.Add(Panel_Background);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Interface";
            Text = "Form1";
            Panel_Top.ResumeLayout(false);
            Panel_Top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Image_Logo).EndInit();
            Panel_Body.ResumeLayout(false);
            Panel_Right.ResumeLayout(false);
            Panel_NoFocus.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            Panel_Focus.ResumeLayout(false);
            Panel_Focus.PerformLayout();
            Panel_Left.ResumeLayout(false);
            Panel_Content.ResumeLayout(false);
            Panel_Settings.ResumeLayout(false);
            Panel_Bodies.ResumeLayout(false);
            Panel_Bottom.ResumeLayout(false);
            Panel_Bottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel Panel_Top;
        private Panel Panel_Body;
        private PictureBox Image_Logo;
        private Label Label_Title;
        private ListBox List_Sections;
        private Label label4;
        private Panel Panel_Content;
        private Panel Panel_Left;
        private Label Label_Version;
        private Panel Panel_Background;
        private Panel Panel_Bottom;
        private Panel Panel_Right;
        private Button Button_Load;
        private Button Button_Save;
        private Button Button_New;
        private ToolTip tooltip_frame;
        private CheckBox cb_rbar;
        private CheckBox cb_lbar;
        private Panel Panel_NoFocus;
        private Label Label_CamZ;
        private Label Label_CamY;
        private Label Label_CamX;
        private Label Label_CamRY;
        private Label Label_CamRX;
        private Label Label_CamRZ;
        private Label Label_CamZSpeed;
        private Label Label_CamSpeed;
        private Label Label_C;
        private Label Label_G;
        private Label Label_Pi;
        private RichTextBox richTextBox1;
        private Panel Panel_Bodies;
        private ListView List_Bodies;
        private Panel Panel_Focus;
        private Button Button_FocusClose;
        private Label Label_F_X;
        private Label Label_F_V_Z;
        private Label Label_F_V_Y;
        private Label Label_F_Y;
        private Label Label_F_Z;
        private Label Label_F_V_X;
        private Label Label_F_Mass;
        private Label Label_F_Radius;
        private Button buttonTimePause;
        private Button buttonTimeSlow;
        private Button buttonTimeFaster;
        private Button buttonTimeStart;
        private Button buttonTimeReset;
        private Panel Panel_Settings;
        private Label labelTime;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Button button2;
        private Button button4;
        private Button button3;
        private Button button1;
        private Button button5;
        private Label LabelSimScale;
        private Button button6;
        private Button button8;
        private Button button7;
        private Label LabelSettingsRotation;
    }
}
