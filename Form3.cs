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
        // ── CUSTOMER SESSION PROPERTY ──
        // This holds the logged-in user's ID so sub-forms can access it
        public string CustomerID { get; set; } = "1"; // Default fallback to ID '1' for easy testing

        // Rounded Button Effect for the Form
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nL, int nT, int nR, int nB, int nW, int nH);

        public frmHome()
        {
            InitializeComponent();
            // The color for the upper part of the Form.
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
            this.Close();
        }

        // ── NOTIFICATION LINK CLICK EVENT ──
        private void btnNotifications_Click(object sender, EventArgs e)
        {
            // Instantiate the notifications sub-window
            frmNotifications notifWindow = new frmNotifications();

            // CRITICAL STEP: Set this form as the owner/parent container.
            // This ensures that 'this.FindForm() as frmHome' inside your 
            // frmNotifications code will successfully find this form and grab the CustomerID!
            notifWindow.Owner = this;

            // Display the notifications window modally
            // This keeps the user focused on their notifications until they click exit
            notifWindow.ShowDialog();
        }
    }
}