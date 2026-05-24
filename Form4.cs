using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DEF_Customer
{
    public partial class frmBookDetails1 : MetroForm
    {
        // Connection string pointing to your local SQLEXPRESS instance
        private string connectionString = @"Server=AIKENDAVE\SQLEXPRESS; Database=DEF_DeliveryDB; Integrated Security=True; TrustServerCertificate=True;";

        // --- 4 TEMPORARY SLOTS ---
        public static string TempPickupAddress = "";
        public static string TempRecipientName = "";
        public static string TempRecipientContact = "";
        public static string TempDropoffAddress = "";

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

        public frmBookDetails1()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.Pink;
        }

        private void frmBookDetails_Load(object sender, EventArgs e)
        {
            pnlSender.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlSender.Width, pnlSender.Height, 25, 25));
            pnlRecipient.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlRecipient.Width, pnlRecipient.Height, 25, 25));

            // Query to fetch fields based on the active session token
            string query = "SELECT custFullName, custcontact FROM CUSTOMER WHERE custID = @custID;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Securely grab the ID stored during the login stage
                    command.Parameters.AddWithValue("@custID", frmLogIn.LoggedInCustID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Assign the database records straight into your text elements!
                                txtFullName.Text = reader["custFullName"].ToString();
                                txtContact.Text = reader["custcontact"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to pull customer profile details: {ex.Message}",
                                        "Database Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            txtPickUp.Text = TempPickupAddress;
            txtRecipientName.Text = TempRecipientName;
            txtRecipientContact.Text = TempRecipientContact;
            txtDropOff.Text = TempDropoffAddress;
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

        private void btnBookDelivery1_Click(object sender, EventArgs e)
        {
            // 1. Validation check
            if (string.IsNullOrEmpty(txtPickUp.Text.Trim()) ||
                string.IsNullOrEmpty(txtRecipientName.Text.Trim()) ||
                string.IsNullOrEmpty(txtRecipientContact.Text.Trim()) ||
                string.IsNullOrEmpty(txtDropOff.Text.Trim()))
            {
                MessageBox.Show("Please complete all input fields before moving forward.",
                                "Incomplete Form", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. --- SAVE COPIES TO TEMPORARY MEMORY BEFORE LEAVING ---
            TempPickupAddress = txtPickUp.Text.Trim();
            TempRecipientName = txtRecipientName.Text.Trim();
            TempRecipientContact = txtRecipientContact.Text.Trim();
            TempDropoffAddress = txtDropOff.Text.Trim();

            // 3. Move cleanly to form 2
            frmBookDetails2 nextForm = new frmBookDetails2();
            nextForm.Show();
            this.Hide();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            // Reset the temporary booking memory slots so they are clean for the next booking
            TempPickupAddress = "";
            TempRecipientName = "";
            TempRecipientContact = "";
            TempDropoffAddress = "";

            // 2. Wipe out temporary memory slots from Form 2
            frmBookDetails2.TempItemName = "";
            frmBookDetails2.TempItemDescription = "";
            frmBookDetails2.TempItemType = "";
            frmBookDetails2.TempPackageSize = "";
            frmBookDetails2.TempVehicleType = "";

            // Show the Home form and close this one
            frmHome homeForm = new frmHome();
            homeForm.Show();
            this.Close(); // Use .Close() instead of .Hide() to properly dispose of this wizard instance
        }
    }
}
