using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DEF_Customer
{
    public partial class frmBookDetails : MetroForm
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

        public frmBookDetails()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.Pink;
        }

        private void frmBookDetails_Load(object sender, EventArgs e)
        {
            pnlSender.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlSender.Width, pnlSender.Height, 25, 25));
            pnlRecipient.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlRecipient.Width, pnlRecipient.Height, 25, 25));
        }

        private void lblSender_Paint(object sender, PaintEventArgs e)
        {
            // 1. Clear the default text so we can draw our own
            e.Graphics.Clear(lblSender.BackColor);

            // 2. Setup the Gradient (Left to Right)
            using (LinearGradientBrush brush = new LinearGradientBrush(
                lblSender.ClientRectangle,
                Color.DeepPink,           // Start Color
                Color.Black,       // End Color
                0f))                  // Angle (0 = horizontal)
            {
                // 3. Create the text path
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddString(
                        lblSender.Text,
                        lblSender.Font.FontFamily,
                        (int)lblSender.Font.Style,
                        e.Graphics.DpiY * lblSender.Font.SizeInPoints / 72,
                        lblSender.ClientRectangle,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // 4. Fill the text with the gradient
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private void lblRecipient_Paint(object sender, PaintEventArgs e)
        {
            // 1. Clear the default text so we can draw our own
            e.Graphics.Clear(lblRecipient.BackColor);

            // 2. Setup the Gradient (Left to Right)
            using (LinearGradientBrush brush = new LinearGradientBrush(
                lblRecipient.ClientRectangle,
                Color.DeepPink,           // Start Color
                Color.Black,       // End Color
                0f))                  // Angle (0 = horizontal)
            {
                // 3. Create the text path
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddString(
                        lblRecipient.Text,
                        lblRecipient.Font.FontFamily,
                        (int)lblRecipient.Font.Style,
                        e.Graphics.DpiY * lblRecipient.Font.SizeInPoints / 72,
                        lblRecipient.ClientRectangle,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // 4. Fill the text with the gradient
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }
            }
        }
    }
}
