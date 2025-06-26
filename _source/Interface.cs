using System.Data;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Security.Policy;
using System.Media;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using trajectorx.Properties;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.GLControl;
using OpenTK.Mathematics;
using System.Drawing;
using System.Xml.Linq;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using trajectorx.Library.Struct;

////////////////////////////////////////////////////////////
/// Main Namespace and Form
////////////////////////////////////////////////////////////
namespace trajectorx
{
    public partial class Interface : Form
    {
        ////////////////////////////////////////////////////////////
        /// Resizing/Moving (No Save, Not Related to Purpose)
        ////////////////////////////////////////////////////////////
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private Point offset;
        private int previousheight;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        // Debug Console
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        // Form Styling
        private int borderWidth = 5;
        private Color borderColor = Color.FromArgb(0x32, 0x32, 0x32);
        private Color buttonColor = Color.FromArgb(0xFF, 0xFF, 0xFF);
        private Color buttonTextColor = Color.FromArgb(0x00, 0x00, 0x00);
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Button btnClose;

        ////////////////////////////////////////////////////////////
        /// Debug Constants
        ////////////////////////////////////////////////////////////
        const bool SF_DEBUG_CR = true;
        const int SF_MINIMUM_WIDTH = 800;
        const int SF_MINIMUM_HEIGHT = 600;

        ////////////////////////////////////////////////////////////
        /// Simulation Elements (No Save)
        ////////////////////////////////////////////////////////////
        private GLControl glControl;
        private int vao;
        private int vbo;
        private int ebo;
        int shaderProgram;
        private bool isAltDown = false;
        private Point lastMousePos;
        SphereMesh sphere = null;
        private int currentFocusIndex = -1;

        ////////////////////////////////////////////////////////////
        /// Camera Elements (No Save)
        ////////////////////////////////////////////////////////////
        private OpenTK.Mathematics.Vector3 cameraPosition = new OpenTK.Mathematics.Vector3(0, 0, 0);
        private OpenTK.Mathematics.Vector3 cameraTarget = new OpenTK.Mathematics.Vector3(0, 0, 0);
        private OpenTK.Mathematics.Vector3 cameraUp = OpenTK.Mathematics.Vector3.UnitY;
        private float cameraSpeed = 1.0f;
        private float zoomSpeed = 1.0f;
        private float rotationSpeed = 0.005f;

        ////////////////////////////////////////////////////////////
        /// Simulation Elements (No Save)
        ////////////////////////////////////////////////////////////
        double const_g = 6.67430e-11;
        double const_pi = 3.14159265358979323846;
        double const_c = 3e8;
        double const_simtime = 0.0;
        double const_simtimemultiplier = 1.0;
        double simScale = 1.0;

        ////////////////////////////////////////////////////////////
        /// Software Version (Save)
        ////////////////////////////////////////////////////////////
        const string SF_VERSION_CR = "1.0.1";

        ////////////////////////////////////////////////////////////
        /// Body Elements (Save)
        ////////////////////////////////////////////////////////////
        List<GravityBody> bodies = new List<GravityBody>();

        ////////////////////////////////////////////////////////////
        /// Settings Elements (Save)
        ////////////////////////////////////////////////////////////
        Dictionary<string, string> settings = new Dictionary<string, string>();

        ////////////////////////////////////////////////////////////
        /// Interface Function
        ////////////////////////////////////////////////////////////
        public Interface()
        {
            ////////////////////////////////////////////////////////////
            /// Form Settings
            ////////////////////////////////////////////////////////////
            InitializeComponent();
            this.Text = "TrajectorX";
            Label_Version.Text = "Version: " + SF_VERSION_CR;
            this.Icon = new Icon(new MemoryStream(Properties.Resources.favicon));
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderWidth);
            this.Padding = new Padding(5);
            this.Paint += new PaintEventHandler(CustomUI_Interface_Paint);
            this.Resize += CustomUI_Interface_Resize;
            this.FormClosing += CustomUI_FormClosing;

            ////////////////////////////////////////////////////////////
            /// CustomUI Buttons
            ////////////////////////////////////////////////////////////
            btnMinimize = new System.Windows.Forms.Button
            {
                Text = "_",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 100, 0),
                BackColor = buttonColor,
                FlatStyle = FlatStyle.Flat
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += CustomUI_BtnMinimize_Click;
            btnMinimize.ForeColor = buttonTextColor;
            btnMinimize.BringToFront();
            tooltip_frame.SetToolTip(btnMinimize, "Minimize");
            btnMaximize = new System.Windows.Forms.Button
            {
                Text = "O",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 70, 0),
                BackColor = buttonColor,
                FlatStyle = FlatStyle.Flat
            };
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.Click += CustomUI_BtnMaximize_Click;
            btnMaximize.ForeColor = buttonTextColor;
            btnMaximize.BringToFront();
            tooltip_frame.SetToolTip(btnMaximize, "Maximize");
            btnClose = new System.Windows.Forms.Button
            {
                Text = "X",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 40, 0),
                BackColor = buttonColor,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += CustomUI_BtnClose_Click;
            btnClose.ForeColor = buttonTextColor;
            btnClose.BringToFront();
            tooltip_frame.SetToolTip(btnClose, "Close");
            Panel_Top.Controls.Add(btnMaximize);
            Panel_Top.Controls.Add(btnMinimize);
            Panel_Top.Controls.Add(btnClose);

            ////////////////////////////////////////////////////////////
            /// Debug Console if Debugging Constant is activated
            ////////////////////////////////////////////////////////////
            if (SF_DEBUG_CR)
            {
                AllocConsole();
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Debugging Mode for TrajectorX!");
                Console.WriteLine("Whoop whoop!");
                Console.WriteLine("-----------------------------------------------");
            }

            ////////////////////////////////////////////////////////////
            // Additional Event Handlers
            ////////////////////////////////////////////////////////////
            List_Sections.MouseClick += List_Sections_MouseClick;
            List_Bodies.MouseClick += List_Bodies_MouseClick;

            ////////////////////////////////////////////////////////////
            // Add Section Listing Elements
            ////////////////////////////////////////////////////////////
            List_Sections.Items.Add("Simulation");
            List_Sections.Items.Add("Bodies");
            List_Sections.Items.Add("About");
            List_Sections.SelectedIndex = 0;

            ////////////////////////////////////////////////////////////
            // Pre-Styling
            ////////////////////////////////////////////////////////////
            Panel_Settings.Hide();
            Panel_Bodies.Hide();
            Panel_Focus.Hide();

            ////////////////////////////////////////////////////////////
            // Restore and Reinit Settings
            ////////////////////////////////////////////////////////////
            settings_restore();
            settings_reinit();

            ////////////////////////////////////////////////////////////
            // Add Initial Solar System
            ////////////////////////////////////////////////////////////
            var body_data = new List<BodyInit> {
                new BodyInit { name = "Sun", mass = 1.98847e30, x = 0, y = 0, z = 0, vx = 0, vy = 0, vz = 0, radius = 6.957e8, color = Color.Yellow, angularSpeed = 2 * Math.PI / (24.47 * 86400), rotationAngle = 0 },
                new BodyInit { name = "Mercury", mass = 3.3011e23, x = 5.7909227e10, y = 0, z = 0, vx = 0, vy = 4.787e4, vz = 0, radius = 2.4397e6, color = Color.Gray, angularSpeed = 2 * Math.PI / (58.646 * 86400), rotationAngle = 0 },
                new BodyInit { name = "Venus", mass = 4.8675e24, x = 1.082094e11, y = 0, z = 0, vx = 0, vy = 3.502e4, vz = 0, radius = 6.0518e6, color = Color.Orange, angularSpeed = 2 * Math.PI / (243.025 * 86400), rotationAngle = 0 },
                new BodyInit { name = "Earth", mass = 5.97219e24, x = 1.495978707e11, y = 0, z = 0, vx = 0, vy = 2.9785e4, vz = 0, radius = 6.371e6, color = Color.Blue, angularSpeed = 2 * Math.PI / (23.9344696 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Moon", mass = 7.342e22, x = 1.495978707e11 + 3.844e8, y = 0, z = 0, vx = 0, vy = 2.9785e4 + 1.022e3, vz = 0, radius = 1.7374e6, color = Color.LightGray, angularSpeed = 2 * Math.PI / (655.728 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Mars", mass = 6.4171e23, x = 2.2794382e11, y = 0, z = 0, vx = 0, vy = 2.4077e4, vz = 0, radius = 3.3895e6, color = Color.Red, angularSpeed = 2 * Math.PI / (24.6229 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Jupiter", mass = 1.8982e27, x = 7.785701e11, y = 0, z = 0, vx = 0, vy = 1.306e4, vz = 0, radius = 6.9911e7, color = Color.Orange, angularSpeed = 2 * Math.PI / (9.92496 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Saturn", mass = 5.6834e26, x = 1.43353e12, y = 0, z = 0, vx = 0, vy = 9.672e3, vz = 0, radius = 5.8232e7, color = Color.Gold, angularSpeed = 2 * Math.PI / (10.656 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Uranus", mass = 8.6810e25, x = 2.87246e12, y = 0, z = 0, vx = 0, vy = 6.803e3, vz = 0, radius = 2.5362e7, color = Color.LightBlue, angularSpeed = 2 * Math.PI / (17.24 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Neptune", mass = 1.02413e26, x = 4.49506e12, y = 0, z = 0, vx = 0, vy = 5.434e3, vz = 0, radius = 2.4622e7, color = Color.Blue, angularSpeed = 2 * Math.PI / (16.11 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Pluto", mass = 1.303e22, x = 5.90638e12, y = 0, z = 0, vx = 0, vy = 4.743e3, vz = 0, radius = 1.1883e6, color = Color.LightGray, angularSpeed = 2 * Math.PI / (153.2928 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Io", mass = 8.931938e22, x = 7.785701e11 + 4.217e8, y = 0, z = 0, vx = 0, vy = 1.306e4 + 1.7319e4, vz = 0, radius = 1.8216e6, color = Color.Yellow, angularSpeed = 2 * Math.PI / (42.459306 * 3600), rotationAngle = 0 },
                new BodyInit { name = "Europa", mass = 4.799844e22, x = 7.785701e11 + 6.709e8, y = 0, z = 0, vx = 0, vy = 1.306e4 + 1.3704e4, vz = 0, radius = 1.5608e6, color = Color.White, angularSpeed = 2 * Math.PI / (85.228 * 3600), rotationAngle = 0 }
            };

            ////////////////////////////////////////////////////////////
            // Add Initial System to bodies Array
            ////////////////////////////////////////////////////////////
            if (SF_DEBUG_CR)
            {
                Console.WriteLine("Adding initial body objects");
                Console.WriteLine("-----------------------------------------------");
            }
            foreach (var b in body_data)
            {
                bodies.Add(new GravityBody(
                    b.name, b.mass,
                    new GravityBodyVector(b.x, b.y, b.z),
                    new GravityBodyVector(b.vx, b.vy, b.vz),
                    b.radius,
                    new GravityBodyVector(b.rot_ax_x, b.rot_ax_y, b.rot_ax_z),
                    b.angularSpeed,
                    b.rotationAngle
                ));
                if (SF_DEBUG_CR)
                {
                    Console.WriteLine("+++ New Object added");
                    Console.WriteLine(string.Join(", ", b.GetType().GetFields().Select(f => $"{f.Name}={f.GetValue(b)}")));
                }
            }

            ////////////////////////////////////////////////////////////
            // Update Bodies List
            ////////////////////////////////////////////////////////////
            List_Bodies_Update();

            ////////////////////////////////////////////////////////////
            // Load new Control for OpenGL
            ////////////////////////////////////////////////////////////
            glControl = new GLControl();
            glControl.Dock = DockStyle.Fill;
            glControl.Load += OGL_GlControl_Load;
            glControl.Paint += GlControl_Paint;
            glControl.Resize += OGL_GlControl_Resize;
            glControl.MouseWheel += GlControl_MouseWheel;
            this.KeyPreview = true;
            this.KeyDown += Interface_KeyDown;
            this.KeyUp += Interface_KeyUp;
            glControl.MouseMove += GlControl_MouseMove;

            ////////////////////////////////////////////////////////////
            // AddOpenGL Control to Content Simulation Panel
            ////////////////////////////////////////////////////////////
            Panel_Content.Controls.Add(glControl);

            ////////////////////////////////////////////////////////////
            // Reinitialize the Labels
            ////////////////////////////////////////////////////////////
            label_reinit();
        }

















        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            glControl.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.Enable(EnableCap.DepthTest);

            Matrix4 view = Matrix4.LookAt(cameraPosition, cameraTarget, cameraUp);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)glControl.Width / glControl.Height,
                0.1f,
                1e8f);

            GL.UseProgram(shaderProgram);

            // Set view and projection matrices
            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            GL.BindVertexArray(vao);

            // Set color and opacity uniform
            OGL_SetColorAndOpacity(shaderProgram, 1.0f, 0.0f, 0.0f, 0.5f); // Red color, 50% opacity

            foreach (var b in bodies)
            {
                double SCALE = simScale;

                float scaledX = (float)(b.Position.X * SCALE);
                float scaledY = (float)(b.Position.Y * SCALE);
                float scaledZ = (float)(b.Position.Z * SCALE);
                float scaledRadius = (float)(b.Radius * SCALE);

                // Create the model matrix
                Matrix4 model = Matrix4.CreateScale(scaledRadius) *
                                Matrix4.CreateTranslation(scaledX, scaledY, scaledZ);

                int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
                GL.UniformMatrix4(modelLoc, false, ref model);

                int indexCount = sphere.Indices.Length;
                GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);

            }

            GL.BindVertexArray(0);
            GL.UseProgram(0);






            // Save the current projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, glControl.Width, glControl.Height, 0, -1, 1); // Origin at top-left

            // Save the current modelview matrix
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            // Disable depth test for overlay (so it always draws on top)
            GL.Disable(EnableCap.DepthTest);
            foreach (var b in bodies)
            {
                // Project to screen coordinates (manual matrix-vector multiplication)
                OpenTK.Mathematics.Vector4 worldPos = new OpenTK.Mathematics.Vector4(
                    (float)(b.Position.X * simScale),
                    (float)(b.Position.Y * simScale),
                    (float)(b.Position.Z * simScale),
                    1.0f);

                Matrix4 model = Matrix4.CreateScale((float)(b.Radius * simScale)) *
                                Matrix4.CreateTranslation(worldPos.X, worldPos.Y, worldPos.Z);
                Matrix4 mvp = model * view * projection;

                // Manual matrix-vector multiplication (no Vector4.Transform)
                OpenTK.Mathematics.Vector4 clipSpace = new OpenTK.Mathematics.Vector4(
                    worldPos.X * mvp.M11 + worldPos.Y * mvp.M21 + worldPos.Z * mvp.M31 + worldPos.W * mvp.M41,
                    worldPos.X * mvp.M12 + worldPos.Y * mvp.M22 + worldPos.Z * mvp.M32 + worldPos.W * mvp.M42,
                    worldPos.X * mvp.M13 + worldPos.Y * mvp.M23 + worldPos.Z * mvp.M33 + worldPos.W * mvp.M43,
                    worldPos.X * mvp.M14 + worldPos.Y * mvp.M24 + worldPos.Z * mvp.M34 + worldPos.W * mvp.M44
                );

                if (clipSpace.W > 0)
                {
                    clipSpace.X /= clipSpace.W;
                    clipSpace.Y /= clipSpace.W;
                    int screenX = (int)((clipSpace.X * 0.5f + 0.5f) * glControl.Width);
                    int screenY = (int)((-clipSpace.Y * 0.5f + 0.5f) * glControl.Height);

                    // Draw a fixed-size symbol (here, a square, but you can use a texture or sprite)
                    int iconSize = 16; // pixels
                    if (screenX >= 0 && screenX < glControl.Width && screenY >= 0 && screenY < glControl.Height)
                    {
                        GL.Begin(PrimitiveType.Quads);
                        GL.Color3(1.0f, 0.0f, 0.0f); // Red color
                        GL.Vertex2(screenX - iconSize / 2, screenY - iconSize / 2);
                        GL.Vertex2(screenX + iconSize / 2, screenY - iconSize / 2);
                        GL.Vertex2(screenX + iconSize / 2, screenY + iconSize / 2);
                        GL.Vertex2(screenX - iconSize / 2, screenY + iconSize / 2);
                        GL.End();
                    }
                }
            }
            GL.Enable(EnableCap.DepthTest); // Re-enable if needed for future 3D

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();



            glControl.SwapBuffers();
        }













        private void Button_Load_Click(object sender, EventArgs e)
        {

        }
        private void Button_Save_Click(object sender, EventArgs e)
        {

        }








        public void label_reinit()
        {
            if (SF_DEBUG_CR)
            {
                Console.WriteLine("Calling label_reinit()");
                Console.WriteLine("-----------------------------------------------");
            }
            Label_Pi.Text = "PI: " + const_pi.ToString();
            Label_G.Text = "G: " + const_g.ToString();
            Label_C.Text = "C: " + const_c.ToString();
            Label_CamX.Text = "Pos-X: " + cameraPosition.X.ToString();
            Label_CamY.Text = "Pos-Y: " + cameraPosition.Y.ToString();
            Label_CamZ.Text = "Pos-Z: " + cameraPosition.Z.ToString();
            Label_CamRX.Text = "Target-X: " + cameraTarget.X.ToString();
            Label_CamRY.Text = "Target-Y: " + cameraTarget.Y.ToString();
            Label_CamRZ.Text = "Target-Z: " + cameraTarget.Z.ToString();
            Label_CamSpeed.Text = "Cam-Speed: " + cameraSpeed.ToString();
            Label_CamZSpeed.Text = "Cam-Zoom-Speed: " + zoomSpeed.ToString();
            LabelSimScale.Text = "Sim Scale: " + simScale.ToString();
            LabelSettingsRotation.Text = "Rotation Speed: " + rotationSpeed.ToString();

            if (currentFocusIndex > -1)
            {

            }
        }

































        //////////////////////////////////////////////////////////////////////////////////
        /// Update the Bodies List
        //////////////////////////////////////////////////////////////////////////////////
        private void List_Bodies_Update()
        {
            List_Bodies.Clear();
            List_Bodies.Items.Clear();
            List_Bodies.Columns.Clear();
            List_Bodies.View = View.Details;
            List_Bodies.FullRowSelect = true;
            List_Bodies.Columns.Add("name");
            List_Bodies.Columns.Add("pos-x");
            List_Bodies.Columns.Add("pos-y");
            List_Bodies.Columns.Add("pos-z");
            List_Bodies.Columns.Add("vel_x");
            List_Bodies.Columns.Add("vel_y");
            List_Bodies.Columns.Add("vel_z");
            List_Bodies.Columns.Add("mass");
            List_Bodies.Columns.Add("radius");

            //bodies.Clear();
            foreach (var b in bodies)
            {
                var item = new ListViewItem(b.Name);
                item.SubItems.Add(b.Position.X.ToString());
                item.SubItems.Add(b.Position.Y.ToString());
                item.SubItems.Add(b.Position.Z.ToString());
                item.SubItems.Add(b.Velocity.X.ToString());
                item.SubItems.Add(b.Velocity.Y.ToString());
                item.SubItems.Add(b.Velocity.Z.ToString());
                item.SubItems.Add(b.Mass.ToString());
                item.SubItems.Add(b.Radius.ToString());
                List_Bodies.Items.Add(item);
            }
        }






















        private void button7_Click(object sender, EventArgs e)
        {
            rotationSpeed = rotationSpeed * 2;
            label_reinit();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            rotationSpeed = rotationSpeed / 2;
            label_reinit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            simScale = simScale * 2;
            label_reinit();
            glControl.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            simScale = simScale / 2;
            label_reinit();
            glControl.Invalidate();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            zoomSpeed = zoomSpeed * 2;
            label_reinit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            zoomSpeed = zoomSpeed / 2;
            label_reinit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cameraSpeed = cameraSpeed * 2;
            label_reinit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cameraSpeed = cameraSpeed / 2;
            label_reinit();
        }

        ////////////////////////////////////////////////////////////
        // Time Slower / Faster Buttons
        ////////////////////////////////////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            if (const_simtimemultiplier < 1000000)
            {
                const_simtimemultiplier = const_simtimemultiplier * 2;
            }
            label_reinit();
        }

        ////////////////////////////////////////////////////////////
        // Time Slower / Faster Buttons
        ////////////////////////////////////////////////////////////
        private void buttonTimeSlow_Click(object sender, EventArgs e)
        {
            if (const_simtimemultiplier > 1)
            {
                const_simtimemultiplier = const_simtimemultiplier / 2;
            }
            label_reinit();
        }

        ////////////////////////////////////////////////////////////
        // Create new Template Button
        ////////////////////////////////////////////////////////////
        private void Button_New_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "A new template will be created. All current data will be deleted if not saved. Continue?",
                "Caution: Unsaved Data Will Be Lost",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.OK)
                return;

            remove_focus();
            bodies.Clear();
            settings_restore();
            settings_reinit();
            List_Bodies_Update();
            label_reinit();
            glControl.Invalidate();
        }

        ////////////////////////////////////////////////////////////
        /// Close Focus Window Click
        ////////////////////////////////////////////////////////////
        private void Button_FocusClose_Click(object sender, EventArgs e)
        {
            remove_focus();
        }

        ////////////////////////////////////////////////////////////
        /// Set Focus Function at Mouseclick on Left Sidebar Item
        ////////////////////////////////////////////////////////////
        private void List_Bodies_MouseClick(object sender, MouseEventArgs e)
        {
            var info = List_Bodies.HitTest(e.Location);
            if (info.Item != null)
            {
                int index = info.Item.Index;
                var item = info.Item;
                change_focus(index, item);
            }
        }

        ////////////////////////////////////////////////////////////
        // Initialy Restore Settings
        ////////////////////////////////////////////////////////////
        public void settings_restore()
        {
            if (SF_DEBUG_CR)
            {
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Calling settings_restore()");
                Console.WriteLine("-----------------------------------------------");
            }
            settings.Clear();
            settings_string("const_pi", "3.14159265358979323846");
            settings_string("const_g", "6.67430e-11");
            settings_string("const_c", "3e8");
            settings_vector("cameraTarget", new GravityBodyVector(0, 0, 0));
            settings_vector("cameraPosition", new GravityBodyVector(0, 0, 0));
            settings_string("cameraSpeed", "1.0f");
            settings_string("zoomSpeed", "1.0f");
            settings_string("rotationSpeed", "0.005f");
            settings_string("simScale", "1.0f");
        }

        ////////////////////////////////////////////////////////////
        // Reinit Settings with Actual Variable Values
        ////////////////////////////////////////////////////////////
        public void settings_reinit()
        {
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Calling settings_reinit()");
            Console.WriteLine("-----------------------------------------------");

            float temp_f;
            double temp_d;
            OpenTK.Mathematics.Vector3 temp_v;

            TryParseDouble(settings_string("const_pi"), out temp_d);
            const_pi = temp_d;

            TryParseDouble(settings_string("const_g"), out temp_d);
            const_g = temp_d;

            TryParseDouble(settings_string("const_c"), out temp_d);
            const_c = temp_d;

            temp_v = settings_vector3("cameraTarget") ?? new OpenTK.Mathematics.Vector3(0, 0, 0);
            cameraTarget = temp_v;

            temp_v = settings_vector3("cameraPosition") ?? new OpenTK.Mathematics.Vector3(0, 0, 0);
            cameraPosition = temp_v;

            TryParseFloat(settings_string("cameraSpeed"), out temp_f);
            cameraSpeed = temp_f;

            TryParseFloat(settings_string("zoomSpeed"), out temp_f);
            zoomSpeed = temp_f;

            TryParseFloat(settings_string("rotationSpeed"), out temp_f);
            rotationSpeed = temp_f;

            TryParseFloat(settings_string("simScale"), out temp_f);
            simScale = temp_f;
        }

        ////////////////////////////////////////////////////////////
        // Struct for Body Variables
        ////////////////////////////////////////////////////////////
        struct BodyInit
        {
            public string name;
            public double mass;
            public double x, y, z;
            public double vx, vy, vz;
            public double radius;
            public double rot_ax_x, rot_ax_y, rot_ax_z;
            public double angularSpeed;
            public double rotationAngle;
            public Color color;
        };

        ////////////////////////////////////////////////////////////
        /// Set Focus Function at Mouseclick on Left Sidebar Item
        ////////////////////////////////////////////////////////////
        private void List_Sections_MouseClick(object sender, MouseEventArgs e)
        {
            int index = List_Sections.IndexFromPoint(e.Location);
            switch (index)
            {
                case 0:
                    glControl.BringToFront();
                    Panel_Bodies.Hide();
                    Panel_Settings.Hide();
                    break;
                case 1:
                    glControl.SendToBack();
                    Panel_Bodies.Show();
                    Panel_Bodies.BringToFront();
                    Panel_Settings.Hide();
                    List_Bodies_Update();
                    break;
                case 2:
                    glControl.SendToBack();
                    Panel_Bodies.Hide();
                    Panel_Settings.Show();
                    Panel_Settings.BringToFront();
                    break;
            }
            ;
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// OpenGL Additional Functions
        //////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        // Function to set the color and opacity uniforms
        ////////////////////////////////////////////////////////////
        private void OGL_SetColorAndOpacity(int shaderProgram, float r, float g, float b, float opacity)
        {
            int objectColorLoc = GL.GetUniformLocation(shaderProgram, "objectColor");
            int opacityLoc = GL.GetUniformLocation(shaderProgram, "opacity");

            // Ensure the locations are valid (greater than -1)
            if (objectColorLoc != -1 && opacityLoc != -1)
            {
                GL.UseProgram(shaderProgram);
                GL.Uniform3(objectColorLoc, r, g, b);  // Set RGB color
                GL.Uniform1(opacityLoc, opacity);       // Set opacity
            }
        }

        ////////////////////////////////////////////////////////////
        /// Load GL Control
        ////////////////////////////////////////////////////////////
        private void OGL_GlControl_Load(object sender, EventArgs e)
        {
            // 1. Create sphere mesh (replace with your mesh generator)
            sphere = new SphereMesh(1.0f, 36, 18);

            // 2. Create buffers/VAO
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sphere.Vertices.Length * sizeof(float), sphere.Vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sphere.Indices.Length * sizeof(uint), sphere.Indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            // 3. Load and compile shaders (see previous answers for CreateShaderProgram)
            shaderProgram = OGL_CreateShaderProgram("Shaders/shader.vert", "Shaders/shader.frag");
        }

        ////////////////////////////////////////////////////////////
        /// GL Resize Window Control
        ////////////////////////////////////////////////////////////
        private void OGL_GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        ////////////////////////////////////////////////////////////
        /// Load Shaders
        ////////////////////////////////////////////////////////////
        int OGL_CreateShaderProgram(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        ////////////////////////////////////////////////////////////
        /// DrawaTextLabel
        ////////////////////////////////////////////////////////////
        void DrawTextLabel(string text, int screenX, int screenY, Font font, Color color)
        {
            using (var bmp = CreateTextBitmap(text, font, color))
            {
                int texId = LoadTextureFromBitmap(bmp);

                // Set up orthographic projection for 2D overlay
                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.Ortho(0, glControl.Width, glControl.Height, 0, -1, 1);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                GL.LoadIdentity();

                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, texId);

                // Draw quad (bottom-left origin)
                int w = bmp.Width, h = bmp.Height;
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 1); GL.Vertex2(screenX, screenY + h);
                GL.TexCoord2(1, 1); GL.Vertex2(screenX + w, screenY + h);
                GL.TexCoord2(1, 0); GL.Vertex2(screenX + w, screenY);
                GL.TexCoord2(0, 0); GL.Vertex2(screenX, screenY);
                GL.End();

                GL.Disable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                // Restore matrices
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Modelview);

                GL.DeleteTexture(texId);
            }
        }

        ////////////////////////////////////////////////////////////
        /// Load Texture from Bitmap
        ////////////////////////////////////////////////////////////
        private int LoadTextureFromBitmap(Bitmap bmp)
        {
            int tex;
            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return tex;
        }

        ////////////////////////////////////////////////////////////
        /// CreateTextBitmap
        ////////////////////////////////////////////////////////////
        private Bitmap CreateTextBitmap(string text, Font font, Color color)
        {
            // Measure string size
            SizeF textSize;
            using (var tmpBmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(tmpBmp))
                textSize = g.MeasureString(text, font);

            // Create bitmap
            var bmp = new Bitmap((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (var brush = new SolidBrush(color))
                    g.DrawString(text, font, brush, 0, 0);
            }
            return bmp;
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Change and Remove Focus Functions
        //////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Change the Focus Function
        ////////////////////////////////////////////////////////////
        public void change_focus(int objectindex, Object objectitem)
        {
            Panel_Focus.Show();
            Panel_NoFocus.Hide();
            currentFocusIndex = objectindex;
            label_reinit();
        }

        ////////////////////////////////////////////////////////////
        /// Remove a Planets Focus
        ////////////////////////////////////////////////////////////
        public void remove_focus()
        {
            currentFocusIndex = -1;
            Panel_Focus.Hide();
            Panel_NoFocus.Show();
            label_reinit();
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// OpenGL Controls
        //////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Camera Movement
        ////////////////////////////////////////////////////////////
        private void Interface_KeyDown(object sender, KeyEventArgs e)
        {
            isAltDown = e.Alt;
            OpenTK.Mathematics.Vector3 forward;
            var direction = cameraTarget - cameraPosition;
            if (direction.LengthSquared > 0.00001f) // Use a small epsilon to avoid floating point issues
                forward = OpenTK.Mathematics.Vector3.Normalize(direction);
            else
                forward = OpenTK.Mathematics.Vector3.UnitZ; // Or any default direction
            OpenTK.Mathematics.Vector3 right = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(forward, OpenTK.Mathematics.Vector3.UnitY));

            switch (e.KeyCode)
            {
                case Keys.W:
                    cameraPosition += OpenTK.Mathematics.Vector3.UnitY * cameraSpeed;
                    cameraTarget += OpenTK.Mathematics.Vector3.UnitY * cameraSpeed;
                    label_reinit();
                    break;
                case Keys.S:
                    cameraPosition -= OpenTK.Mathematics.Vector3.UnitY * cameraSpeed;
                    cameraTarget -= OpenTK.Mathematics.Vector3.UnitY * cameraSpeed;
                    label_reinit();
                    break;
                case Keys.A:
                    cameraPosition -= right * cameraSpeed;
                    cameraTarget -= right * cameraSpeed;
                    label_reinit();
                    break;
                case Keys.D:
                    cameraPosition += right * cameraSpeed;
                    cameraTarget += right * cameraSpeed;
                    label_reinit();
                    break;
            }
            glControl.Invalidate();
        }

        ////////////////////////////////////////////////////////////
        // If alt release no rotation switch per mouse anymore
        ////////////////////////////////////////////////////////////
        private void Interface_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu) // Alt key up
                isAltDown = false;

            label_reinit();
            glControl.Invalidate();
        }

        ////////////////////////////////////////////////////////////
        // Zoom on Mousewheel
        ////////////////////////////////////////////////////////////
        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            var direction = cameraTarget - cameraPosition;
            OpenTK.Mathematics.Vector3 forward;
            if (direction.LengthSquared > 0.00001f) // Use a small epsilon to avoid floating point issues
                forward = OpenTK.Mathematics.Vector3.Normalize(direction);
            else
                forward = OpenTK.Mathematics.Vector3.UnitZ; // Or any default direction

            cameraPosition += forward * zoomSpeed * (e.Delta > 0 ? 1f : -1f);
            label_reinit();
            glControl.Invalidate();
        }

        ////////////////////////////////////////////////////////////
        // Rotate Screen on Mouse Move
        ////////////////////////////////////////////////////////////
        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isAltDown || e.Button != MouseButtons.Left)
            {
                lastMousePos = e.Location;
                return;
            }

            int deltaX = e.X - lastMousePos.X;
            int deltaY = e.Y - lastMousePos.Y;

            lastMousePos = e.Location;

            // Vector from camera position to target
            OpenTK.Mathematics.Vector3 direction = cameraTarget - cameraPosition;
            float radius = direction.Length;
            direction = OpenTK.Mathematics.Vector3.Normalize(direction);

            // Convert direction to spherical coordinates
            float yaw = (float)Math.Atan2(direction.Z, direction.X);
            float pitch = (float)Math.Asin(direction.Y);

            // Update angles based on mouse movement and rotation speed
            yaw -= deltaX * rotationSpeed;
            pitch -= deltaY * rotationSpeed;

            // Clamp pitch so camera doesn't flip upside down
            // pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);

            // Convert spherical back to Cartesian coordinates
            direction.X = (float)(Math.Cos(pitch) * Math.Cos(yaw));
            direction.Y = (float)(Math.Sin(pitch));
            direction.Z = (float)(Math.Cos(pitch) * Math.Sin(yaw));

            // Update cameraTarget keeping radius
            cameraTarget = cameraPosition + direction * radius;

            label_reinit();
            glControl.Invalidate();
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Parse Helper Functions
        //////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Try Parse Double
        ////////////////////////////////////////////////////////////
        public static bool TryParseDouble(string input, out double value)
        {
            value = 0.0;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Remove trailing 'd' or 'D' or 'f' or 'F' if any
            input = input.Trim().TrimEnd('d', 'D', 'f', 'F');

            return double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value);
        }

        ////////////////////////////////////////////////////////////
        /// Try Parse Float
        ////////////////////////////////////////////////////////////
        public static bool TryParseFloat(string input, out float value)
        {
            value = 0f;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Remove trailing 'f' or 'F'
            input = input.Trim().TrimEnd('f', 'F');

            // Parse using invariant culture to handle decimals and scientific notation correctly
            return float.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value);
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Get / Set Settings Functions
        //////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Set Settings or Get Settings - String
        ////////////////////////////////////////////////////////////
        public string settings_string(string key, string stringinput = null)
        {
            if (settings.ContainsKey(key))
            {
                if (stringinput == null)
                {
                    return settings[key];
                }
                else
                {
                    settings.Remove(key);
                    settings.Add(key, stringinput);
                    return settings[key];
                }
            }
            else
            {
                if (stringinput == null)
                {
                    return null;
                }
                else
                {
                    settings.Add(key, stringinput);
                    return settings[key];
                }
            }
        }

        ////////////////////////////////////////////////////////////
        /// Set Settings or Get Settings - Int
        ////////////////////////////////////////////////////////////
        public int? settings_int(string key, int? intInput = null)
        {
            if (settings.ContainsKey(key))
            {
                if (intInput == null)
                {
                    // Try parsing existing string value to int
                    if (int.TryParse(settings[key], out int result))
                        return result;
                    else
                        return null; // or throw?
                }
                else
                {
                    settings[key] = intInput.Value.ToString();
                    return intInput;
                }
            }
            else
            {
                if (intInput == null)
                {
                    return null;
                }
                else
                {
                    settings.Add(key, intInput.Value.ToString());
                    return intInput;
                }
            }
        }

        ////////////////////////////////////////////////////////////
        /// Set Settings or Get Settings - Vector
        ////////////////////////////////////////////////////////////
        public GravityBodyVector? settings_vector(string key, GravityBodyVector? vectorInput = null)
        {
            if (settings.ContainsKey(key))
            {
                if (vectorInput == null)
                {
                    var parts = settings[key].Split(';');
                    if (parts.Length == 3 &&
                        double.TryParse(parts[0], out double x) &&
                        double.TryParse(parts[1], out double y) &&
                        double.TryParse(parts[2], out double z))
                    {
                        return new GravityBodyVector(x, y, z);
                    }
                    else
                    {
                        return null; // or throw?
                    }
                }
                else
                {
                    string serialized = $"{vectorInput.Value.X};{vectorInput.Value.Y};{vectorInput.Value.Z}";
                    settings[key] = serialized;
                    return vectorInput;
                }
            }
            else
            {
                if (vectorInput == null)
                {
                    return null;
                }
                else
                {
                    string serialized = $"{vectorInput.Value.X};{vectorInput.Value.Y};{vectorInput.Value.Z}";
                    settings.Add(key, serialized);
                    return vectorInput;
                }
            }
        }

        ////////////////////////////////////////////////////////////
        /// Set Settings or Get Settings - Vector3
        ////////////////////////////////////////////////////////////
        public OpenTK.Mathematics.Vector3? settings_vector3(string key, OpenTK.Mathematics.Vector3? vectorInput = null)
        {
            if (settings.ContainsKey(key))
            {
                if (vectorInput == null)
                {
                    var parts = settings[key].Split(';');
                    if (parts.Length == 3 &&
                        float.TryParse(parts[0], out float x) &&
                        float.TryParse(parts[1], out float y) &&
                        float.TryParse(parts[2], out float z))
                    {
                        return new OpenTK.Mathematics.Vector3(x, y, z);
                    }
                    else
                    {
                        return null; // or throw exception if invalid format
                    }
                }
                else
                {
                    string serialized = $"{vectorInput.Value.X};{vectorInput.Value.Y};{vectorInput.Value.Z}";
                    settings[key] = serialized;
                    return vectorInput;
                }
            }
            else
            {
                if (vectorInput == null)
                {
                    return null;
                }
                else
                {
                    string serialized = $"{vectorInput.Value.X};{vectorInput.Value.Y};{vectorInput.Value.Z}";
                    settings.Add(key, serialized);
                    return vectorInput;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Bottom Buttons to show/hide Left/Right Sidebar
        //////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Show/Hide Left Side Bar
        ////////////////////////////////////////////////////////////
        private void cb_lbar_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_lbar.Checked)
            {
                Panel_Left.Enabled = true;
                Panel_Left.Visible = true;
                Panel_Content.Location = new Point(Panel_Content.Location.X + 210, Panel_Content.Location.Y);
                Panel_Content.Size = new Size(Panel_Content.Width + 210, Panel_Content.Height);
            }
            else
            {
                Panel_Left.Enabled = false;
                Panel_Left.Visible = false;
                Panel_Content.Location = new Point(Panel_Content.Location.X - 210, Panel_Content.Location.Y);
                Panel_Content.Size = new Size(Panel_Content.Width + 210, Panel_Content.Height);
            }
        }

        ////////////////////////////////////////////////////////////
        /// Show/Hide Right Side Bar
        ////////////////////////////////////////////////////////////
        private void cb_rbar_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_rbar.Checked)
            {
                Panel_Right.Enabled = true;
                Panel_Right.Visible = true;
                Panel_Content.Location = new Point(Panel_Content.Location.X, Panel_Content.Location.Y);
                Panel_Content.Size = new Size(Panel_Content.Width - 210, Panel_Content.Height);
            }
            else
            {
                Panel_Right.Enabled = false;
                Panel_Right.Visible = false;
                Panel_Content.Location = new Point(Panel_Content.Location.X, Panel_Content.Location.Y);
                Panel_Content.Size = new Size(Panel_Content.Width + 210, Panel_Content.Height);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Custom UI Functionalities
        //////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Close Window to Tray or Close Completely
        ////////////////////////////////////////////////////////////
        private void CustomUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("You have unsaved changes. Are you sure you want to exit? Any unsaved data will be lost.", "Confirm Termination", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result != DialogResult.OK)
            {
                e.Cancel = true;
            }
            else
            {
                if (glControl != null && glControl.Context != null)
                {
                    glControl.MakeCurrent();

                    // Delete OpenGL resources if they exist
                    if (vao != 0)
                        GL.DeleteVertexArray(vao);
                    if (vbo != 0)
                        GL.DeleteBuffer(vbo);
                    if (ebo != 0)
                        GL.DeleteBuffer(ebo);
                    if (shaderProgram != 0)
                        GL.DeleteProgram(shaderProgram);

                    // If you have textures or other resources, delete them here as well
                    glControl.Dispose();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Exit Function
        //////////////////////////////////////////////////////////////////////////////////
        private void CustomUI_app_exit(bool header_press)
        {
            Application.Exit();
        }

        ////////////////////////////////////////////////////////////
        /// Drag Window by Holding on Header
        ////////////////////////////////////////////////////////////
        private void CustomUI_Header_Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.WindowState != FormWindowState.Maximized)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        ////////////////////////////////////////////////////////////
        /// Maximize Button Click Functionality
        ////////////////////////////////////////////////////////////
        private void CustomUI_BtnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Height = previousheight;
            }
            else
            {
                previousheight = this.Height;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        ////////////////////////////////////////////////////////////
        /// Allow for resizing by overriding WndProc
        ////////////////////////////////////////////////////////////
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int WM_GETMINMAXINFO = 0x24;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    if (this.WindowState == FormWindowState.Maximized) { return; }
                    base.WndProc(ref m);

                    Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                    if (pos.X < borderWidth && pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOPLEFT;
                    }
                    else if (pos.X > Width - borderWidth && pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOPRIGHT;
                    }
                    else if (pos.X < borderWidth && pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOMLEFT;
                    }
                    else if (pos.X > Width - borderWidth && pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOMRIGHT;
                    }
                    else if (pos.X < borderWidth)
                    {
                        m.Result = (IntPtr)HTLEFT;
                    }
                    else if (pos.X > Width - borderWidth)
                    {
                        m.Result = (IntPtr)HTRIGHT;
                    }
                    else if (pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOP;
                    }
                    else if (pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOM;
                    }
                    else
                    {
                        m.Result = (IntPtr)HTCLIENT;
                    }
                    CustomUI_interface_reinit();
                    return;

                case WM_GETMINMAXINFO:
                    if (this.WindowState == FormWindowState.Maximized) { return; }
                    CustomUI_MINMAXINFO minMaxInfo = (CustomUI_MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(CustomUI_MINMAXINFO));
                    minMaxInfo.ptMinTrackSize.X = SF_MINIMUM_WIDTH; // Minimum width
                    minMaxInfo.ptMinTrackSize.Y = SF_MINIMUM_HEIGHT; // Minimum height
                    Marshal.StructureToPtr(minMaxInfo, m.LParam, true);
                    CustomUI_interface_reinit();
                    break;
            }
            base.WndProc(ref m);
        }

        ////////////////////////////////////////////////////////////
        /// Extra Function for Minimum Resizing in Width and Height to not make the Window Disappear
        ////////////////////////////////////////////////////////////
        [StructLayout(LayoutKind.Sequential)]
        public struct CustomUI_MINMAXINFO
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }

        ////////////////////////////////////////////////////////////
        /// Interface Paint Functionality
        ////////////////////////////////////////////////////////////
        private void CustomUI_Interface_Paint(object sender, PaintEventArgs e)
        {
            // Draw the custom border
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(borderPen, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1));
            }
        }

        ////////////////////////////////////////////////////////////
        /// Update button locations on resize
        ////////////////////////////////////////////////////////////
        private void CustomUI_Interface_Resize(object sender, EventArgs e)
        {
            btnMinimize.Location = new Point(this.Width - 100, 0);
            btnMaximize.Location = new Point(this.Width - 70, 0);
            btnClose.Location = new Point(this.Width - 40, 0);
        }

        ////////////////////////////////////////////////////////////
        /// Close Window to Tray or Close Completely
        ////////////////////////////////////////////////////////////
        private void CustomUI_BtnClose_Click(object sender, EventArgs e)
        {
            CustomUI_app_exit(true);
        }

        ////////////////////////////////////////////////////////////
        /// Minimize Button Click to Minimize Current Form
        ////////////////////////////////////////////////////////////
        private void CustomUI_BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            CustomUI_interface_reinit();
        }

        ////////////////////////////////////////////////////////////
        /// ReInitialize Border and Buttons
        ////////////////////////////////////////////////////////////
        private void CustomUI_interface_reinit()
        {
            btnClose.Location = new Point(this.Width - 40, 0);
            btnMaximize.Location = new Point(this.Width - 70, 0);
            btnMinimize.Location = new Point(this.Width - 100, 0);
            btnClose.BringToFront();
            btnMaximize.BringToFront();
            btnMinimize.BringToFront();
        }
    }
}
