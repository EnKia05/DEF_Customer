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
using System.Runtime.InteropServices;

namespace DEF_Customer
{
    public partial class frmHome : MetroForm
    {
        //Rounded Button Effect for the Form
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nL, int nT, int nR, int nB, int nW, int nH);

        public frmHome()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.Pink;
            // Apply to your button (button1). The '30, 30' is the roundness.
            btnBookDelivery.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnBookDelivery.Width, btnBookDelivery.Height, 20, 20));
        }

        private void frmHome_Load(object sender, EventArgs e)
        {

        }

        private void btnBookDelivery_Click(object sender, EventArgs e)
        {
            frmBookDetails1 newWindow = new frmBookDetails1();
            newWindow.Show();
            this.Hide();
        }
    }
}
