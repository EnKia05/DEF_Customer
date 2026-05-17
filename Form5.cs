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

        private void frmBookDetails2_Load(object sender, EventArgs e)
        {
            // 1. Clear any default values to avoid duplicates
            cmbItemType.Items.Clear();
            // 2. Add your predefined campus delivery item types
            cmbItemType.Items.Add("Documents");
            cmbItemType.Items.Add("Food & Beverages");
            cmbItemType.Items.Add("Parcels / Packages");
            cmbItemType.Items.Add("Equipment / Electronics");
            cmbItemType.Items.Add("Others");
            // 3. Optional: Set a default selection so it isn't blank
            cmbItemType.SelectedIndex = 0; // Automatically selects "Documents"
        }

        private void ForceSingleButtonSelection(Button selectedButton, Button[] buttonGroup, Panel targetPanel)
        {
            // 1. Loop through your explicit group array and turn them all gray
            foreach (Button btn in buttonGroup)
            {
                btn.BackColor = Color.White;
                btn.ForeColor = Color.HotPink;
            }

            // 2. Light up the clicked button to your signature Pink
            selectedButton.BackColor = Color.DeepPink;
            selectedButton.ForeColor = Color.White;

            // 3. Save the exact string text directly to the master section container Tag
            targetPanel.Tag = selectedButton.Text;
        }

        private void btnSizeS_Click(object sender, EventArgs e)
        {
            Button[] sizeGroup = { btnSizeS, btnSizeM, btnSizeL, btnSizeXL };
            ForceSingleButtonSelection((Button)sender, sizeGroup, pnlPackageSize);
        }

        private void btnSizeM_Click(object sender, EventArgs e)
        {
            Button[] sizeGroup = { btnSizeS, btnSizeM, btnSizeL, btnSizeXL };
            ForceSingleButtonSelection((Button)sender, sizeGroup, pnlPackageSize);
        }

        private void btnSizeL_Click(object sender, EventArgs e)
        {
            Button[] sizeGroup = { btnSizeS, btnSizeM, btnSizeL, btnSizeXL };
            ForceSingleButtonSelection((Button)sender, sizeGroup, pnlPackageSize);
        }

        private void btnSizeXL_Click(object sender, EventArgs e)
        {
            Button[] sizeGroup = { btnSizeS, btnSizeM, btnSizeL, btnSizeXL };
            ForceSingleButtonSelection((Button)sender, sizeGroup, pnlPackageSize);
        }

        private void btnBicycle_Click(object sender, EventArgs e)
        {
            Button[] vehicleGroup = { btnBicycle, btnMotorcycle, btnCar, btnTruck };
            ForceSingleButtonSelection((Button)sender, vehicleGroup, pnlVehicleType);
        }

        private void btnMotorcycle_Click(object sender, EventArgs e)
        {
            Button[] vehicleGroup = { btnBicycle, btnMotorcycle, btnCar, btnTruck };
            ForceSingleButtonSelection((Button)sender, vehicleGroup, pnlVehicleType);
        }

        private void btnCar_Click(object sender, EventArgs e)
        {
            Button[] vehicleGroup = { btnBicycle, btnMotorcycle, btnCar, btnTruck };
            ForceSingleButtonSelection((Button)sender, vehicleGroup, pnlVehicleType);
        }

        private void btnTruck_Click(object sender, EventArgs e)
        {
            Button[] vehicleGroup = { btnBicycle, btnMotorcycle, btnCar, btnTruck };
            ForceSingleButtonSelection((Button)sender, vehicleGroup, pnlVehicleType);
        }
    }
}
