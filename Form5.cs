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
        // --- ADD THESE 5 TEMPORARY GLOBAL SLOTS ---
        public static string TempItemName = "";
        public static string TempItemDescription = "";
        public static string TempItemType = "";
        public static string TempPackageSize = "";  // Will store "S", "M", "L", or "XL"
        public static string TempVehicleType = "";  // Will store "Bicycle", "Motorcycle", "Car", or "Truck"

        public frmBookDetails2()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.Pink;
        }

        private void frmBookDetails2_Load(object sender, EventArgs e)
        {
            // 1. Setup default structural items
            cmbItemType.Items.Clear(); //
            cmbItemType.Items.Add("Documents"); //
            cmbItemType.Items.Add("Food & Beverages"); //
            cmbItemType.Items.Add("Parcels / Packages"); //
            cmbItemType.Items.Add("Equipment / Electronics"); //
            cmbItemType.Items.Add("Others"); //
            cmbItemType.SelectedIndex = -1; //

            // 2. RESTORE STANDARD CONTROL INPUTS
            txtItemName.Text = TempItemName;
            txtItemDescription.Text = TempItemDescription;

            if (!string.IsNullOrEmpty(TempItemType))
            {
                cmbItemType.SelectedItem = TempItemType;
            }

            // 3. RESTORE BUTTON GROUP SELECTIONS BY TRIGGERING PERTINENT CLICK EVENTS
            if (TempPackageSize == "S") btnSizeS_Click(btnSizeS, EventArgs.Empty);
            else if (TempPackageSize == "M") btnSizeM_Click(btnSizeM, EventArgs.Empty);
            else if (TempPackageSize == "L") btnSizeL_Click(btnSizeL, EventArgs.Empty);
            else if (TempPackageSize == "XL") btnSizeXL_Click(btnSizeXL, EventArgs.Empty);

            if (TempVehicleType == "Bicycle") btnBicycle_Click(btnBicycle, EventArgs.Empty);
            else if (TempVehicleType == "Motorcycle") btnMotorcycle_Click(btnMotorcycle, EventArgs.Empty);
            else if (TempVehicleType == "Car") btnCar_Click(btnCar, EventArgs.Empty);
            else if (TempVehicleType == "Truck") btnTruck_Click(btnTruck, EventArgs.Empty);
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

        private void btnNext_Click(object sender, EventArgs e)
        {
            // 1. Strict Input Validation
            if (string.IsNullOrEmpty(txtItemName.Text.Trim()) ||
                string.IsNullOrEmpty(txtItemDescription.Text.Trim()) ||
                cmbItemType.SelectedIndex == -1 ||
                pnlPackageSize.Tag == null ||
                pnlVehicleType.Tag == null)
            {
                MessageBox.Show("Please fill all textboxes, select an item type, a package size, and a vehicle type before proceeding.",
                                "Incomplete Form", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. SAVE SELECTIONS TO TEMPORARY RAM SLOTS BEFORE SHIFTING FORMS
            TempItemName = txtItemName.Text.Trim();
            TempItemDescription = txtItemDescription.Text.Trim();
            TempItemType = cmbItemType.SelectedItem.ToString();
            TempPackageSize = pnlPackageSize.Tag.ToString();
            TempVehicleType = pnlVehicleType.Tag.ToString();

            // 3. Clear window transitions cleanly
            frmBookingSummary newWindow = new frmBookingSummary(); //
            newWindow.Show(); //
            this.Hide(); //
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            // SAVE SELECTIONS EVEN WHEN GOING BACK so they are waiting when the user returns here!
            TempItemName = txtItemName.Text.Trim();
            TempItemDescription = txtItemDescription.Text.Trim();
            TempItemType = cmbItemType.SelectedItem != null ? cmbItemType.SelectedItem.ToString() : "";
            TempPackageSize = pnlPackageSize.Tag != null ? pnlPackageSize.Tag.ToString() : "";
            TempVehicleType = pnlVehicleType.Tag != null ? pnlVehicleType.Tag.ToString() : "";

            frmBookDetails1 newWindow = new frmBookDetails1(); //
            newWindow.Show(); //
            this.Close(); // Changed to Close() to release form resources
        }
    }
}
