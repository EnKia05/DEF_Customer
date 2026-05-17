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

namespace DEF_Customer
{
    public partial class frmBookDetails2 : MetroForm
    {
        public frmBookDetails2()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.Pink;
        }

        private void btnBookDelivery2_Click(object sender, EventArgs e)
        {
            frmBookingSummary newWindow = new frmBookingSummary();
            newWindow.Show();
            this.Hide();
        }

        private void btnBookDelivery2Back_Click(object sender, EventArgs e)
        {
            frmBookDetails1 newWindow = new frmBookDetails1();
            newWindow.Show();
            this.Hide();
        }
    }
}
