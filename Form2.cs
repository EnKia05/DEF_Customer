using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace DEF_Customer
{
    public partial class frmSignUp : MetroForm
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
            (
                int nLeftRect,     // x-coordinate of upper-left corner
                int nTopRect,      // y-coordinate of upper-left corner
                int nRightRect,    // x-coordinate of lower-right corner
                int nBottomRect,   // y-coordinate of lower-right corner
                int nWidthEllipse, // width of ellipse (the roundness)
                int nHeightEllipse // height of ellipse (the roundness)
            );

        public frmSignUp()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.White;
        }

        //Gradient Text Effect for the Label
        private void label1_Paint(object sender, PaintEventArgs e)
        {
            // 1. Clear the default text so we can draw our own
            e.Graphics.Clear(label1.BackColor);

            // 2. Setup the Gradient (Left to Right)
            using (LinearGradientBrush brush = new LinearGradientBrush(
                label1.ClientRectangle,
                Color.DeepPink,           // Start Color
                Color.Black,       // End Color
                0f))                  // Angle (0 = horizontal)
            {
                // 3. Create the text path
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddString(
                        label1.Text,
                        label1.Font.FontFamily,
                        (int)label1.Font.Style,
                        e.Graphics.DpiY * label1.Font.SizeInPoints / 72,
                        label1.ClientRectangle,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // 4. Fill the text with the gradient
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private void frmSignUp_Load(object sender, EventArgs e)
        {
            panel1.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 25, 25));
        }
    }
}
