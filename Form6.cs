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
    public partial class frmBookingSummary : MetroForm
    {
        public frmBookingSummary()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.Pink;
        }

        private void btnConfirmDetails_Click(object sender, EventArgs e)
        {
            frmBookPayment newWindow = new frmBookPayment();
            newWindow.Show();
            this.Hide();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmBookDetails2 newWindow = new frmBookDetails2();
            newWindow.Show();
            this.Hide();
        }
    }
}
