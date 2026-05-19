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
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace DEF_Customer
{
    public partial class frmSignUp : MetroForm
    {
        // Place your precise connection string here at the class level
        private string connectionString = @"Server=AIKENDAVE\SQLEXPRESS; Database=DEF_DeliveryDB; Integrated Security=True; TrustServerCertificate=True;";

        private void ClearFormFields()
        {
            txtCustContact.Clear();
            txtCustEmail.Clear();
            txtCustPassword.Clear();
            txtCustFullName.Clear();
            // Puts cursor back to the top box automatically

            // Reset the checkboxes to unchecked state
            chkTermsOfService.Checked = false;
            chkPrivacyPolicy.Checked = false;
        }

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

        public frmSignUp()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.White;
        }

        //Gradient Text Effect for the Label
        private void label1_Paint(object sender, PaintEventArgs e)
        {
            // 1. Clear the default text so we can draw our own
            e.Graphics.Clear(label1.BackColor);

            // 2. Setup the Gradient (Left to Right)
            using (LinearGradientBrush brush = new LinearGradientBrush(
                label1.ClientRectangle,
                Color.DeepPink,           // Start Color
                Color.Black,       // End Color
                0f))                  // Angle (0 = horizontal)
            {
                // 3. Create the text path
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddString(
                        label1.Text,
                        label1.Font.FontFamily,
                        (int)label1.Font.Style,
                        e.Graphics.DpiY * label1.Font.SizeInPoints / 72,
                        label1.ClientRectangle,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // 4. Fill the text with the gradient
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private void frmSignUp_Load(object sender, EventArgs e)
        {
            panel1.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 25, 25));
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            // 1. Collect inputs and trim trailing or leading blank spaces
            string fullName = txtCustFullName.Text.Trim();
            string contact = txtCustContact.Text.Trim();
            string email = txtCustEmail.Text.Trim();
            string password = txtCustPassword.Text.Trim();

            // 2. Input Validation: Prevent submission if any text boxes are completely empty
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(contact) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("All registration fields are required! Please complete the form.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── COMPLIANCE CHECKBOX VALIDATION LAYER ──
            // Prevent progression unless both agreement checkboxes are ticked
            if (!chkTermsOfService.Checked || !chkPrivacyPolicy.Checked)
            {
                MessageBox.Show("You must read and agree to both the Terms of Service and Privacy Policy before creating an account.",
                                "Policy Agreement Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Prompt Confirmation: Show the user their data before saving
            string confirmationPrompt = $"Are you sure you want to register with these details?\n\n" +
                                         $"Name: {fullName}\n" +
                                         $"Contact: {contact}\n" +
                                         $"Email: {email}";

            DialogResult result = MessageBox.Show(confirmationPrompt, "Confirm Registration",
                                                  MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            // If the user clicks Cancel instead of OK, halt the transaction completely
            if (result != DialogResult.OK)
            {
                return;
            }

            // 4. SQL Execution Layer
            string query = @"INSERT INTO CUSTOMER (custFullName, custcontact, custEmail, custPassword) 
                             VALUES (@custFullName, @custcontact, @custEmail, @custPassword);";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@custFullName", fullName);
                    command.Parameters.AddWithValue("@custcontact", contact);
                    command.Parameters.AddWithValue("@custEmail", email);
                    command.Parameters.AddWithValue("@custPassword", password);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Account successfully created and stored! 🎉",
                                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            ClearFormFields();

                            frmLogIn loginForm = new frmLogIn();
                            loginForm.Show();
                            this.Close();
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            MessageBox.Show("This email address is already registered to a different account.",
                                            "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show($"A database error occurred: {ex.Message}",
                                            "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void lnkTermsofService_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string tosText = "DEF Delivery Service — Terms of Service\n\n" +
                             "1. Acceptance of Terms: By creating an account, you agree to be bound by these local logistics terms.\n" +
                             "2. User Conduct: Users must provide accurate profile details and drop-off addresses.\n" +
                             "3. Booking Limitations: Prohibited items, hazardous materials, and illegal goods will not be handled by our delivery couriers.\n" +
                             "4. Payment Policies: Cash on Delivery must be settled immediately upon arrival. Online Payments are validated using reference matching rules.";

            MessageBox.Show(tosText, "Terms of Service", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lnlPrivacyPolicy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string privacyText = "DEF Delivery Service — Privacy Policy\n\n" +
                                 "1. Information Collection: We safely store your full name, mobile number, and email credentials to handle package fulfillment.\n" +
                                 "2. Data Usage: Address and packaging attributes are strictly shared with matched dispatch couriers for delivery routing.\n" +
                                 "3. Security Protections: Session verification controls guard account credentials securely within our relational database space.";

            MessageBox.Show(privacyText, "Privacy Policy", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
