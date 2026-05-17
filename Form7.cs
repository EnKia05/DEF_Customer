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
    public partial class frmBookPayment : MetroForm
    {
        public frmBookPayment()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.Pink;
        }

        private void btnSubmitOrder_Click(object sender, EventArgs e)
        {
            frmNotifications newWindow = new frmNotifications();
            newWindow.Show();
            this.Hide();
        }
    }
}
