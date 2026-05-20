using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DEF_Customer
{
    public partial class frmLogIn : MetroForm
    {
        // Your precise connection string matching your environment
        private string connectionString = @"Server=AIKENDAVE\SQLEXPRESS; Database=DEF_DeliveryDB; Integrated Security=True; TrustServerCertificate=True;";

        // --- CLEANED GLOBAL SESSION SLOTS ---
        public static int LoggedInCustID;
        public static string LoggedInCustName = "";
        public static string LoggedInCustContact = "";

        public frmLogIn()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.White;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Forces the window context to become active immediately on screen
            this.Activate();

            // Directly forces the input cursor focus onto your Email field
            txtEmail.Focus();
        }

        private void lnkSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmSignUp newWindow = new frmSignUp();
            newWindow.Show();
            this.Hide();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            // 1. Gather values and remove any accidental leading/trailing spaces
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 2. Simple Validation Check
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both your Email and Password.",
                                "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. UPDATED SQL QUERY: Retrieve all required session fields from CUSTOMER
            string query = "SELECT custID, custFullName, custcontact FROM CUSTOMER WHERE custEmail = @custEmail AND custPassword = @custPassword;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@custEmail", email);
                    command.Parameters.AddWithValue("@custPassword", password);

                    try
                    {
                        connection.Open();

                        // Using SqlDataReader instead of ExecuteScalar to fetch multiple data columns seamlessly
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // If a matching record is found
                            {
                                // Save the pulled database records into our global application memory slots
                                LoggedInCustID = Convert.ToInt32(reader["custID"]);
                                LoggedInCustName = reader["custFullName"].ToString();
                                LoggedInCustContact = reader["custcontact"].ToString();

                                MessageBox.Show("Login successful!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Proceed cleanly to your Home form
                                frmHome homeForm = new frmHome();
                                homeForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid Email or Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }
    }
}