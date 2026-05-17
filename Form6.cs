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
            this.Style = MetroFramework.MetroColorStyle.Pink; //
        }

        private void btnConfirmDetails_Click(object sender, EventArgs e) //
        {
            frmBookPayment newWindow = new frmBookPayment(); //
            newWindow.Show(); //
            this.Hide(); //
        }

        private void btnBack_Click(object sender, EventArgs e) //
        {
            frmBookDetails2 newWindow = new frmBookDetails2(); //
            newWindow.Show(); //
            this.Close(); //
        }

        private void frmBookingSummary_Load_1(object sender, EventArgs e)
        {
            // =================================================================
            // 1. POPULATE SENDER DETAILS (From Global Login Session)
            // =================================================================
            // Pulls directly from the static fields inside your login form structure
            txtSenderName.Text = frmLogIn.LoggedInCustName;
            txtSenderContact.Text = frmLogIn.LoggedInCustContact;

            // Matches the precise case-sensitive name of your form 1 storage slots
            txtPickUpAddress.Text = frmBookDetails1.TempPickupAddress; //

            // =================================================================
            // 2. POPULATE RECIPIENT DETAILS (From frmBookDetails1)
            // =================================================================
            txtRecipientName.Text = frmBookDetails1.TempRecipientName; //
            txtRecipientContact.Text = frmBookDetails1.TempRecipientContact; //
            txtDropOffAddress.Text = frmBookDetails1.TempDropoffAddress; //

            // =================================================================
            // 3. POPULATE PACKAGE INFORMATION (From frmBookDetails2)
            // =================================================================
            txtItemName.Text = frmBookDetails2.TempItemName; //
            txtItemDescription.Text = frmBookDetails2.TempItemDescription; //
            txtItemType.Text = frmBookDetails2.TempItemType; //

            // Populating your custom layout button strings cleanly
            txtPackageSize.Text = frmBookDetails2.TempPackageSize; //
            txtVehicleType.Text = frmBookDetails2.TempVehicleType; //
        }
    }
}