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
        public static int LoggedInCustID { get; set; }

        public frmLogIn()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.White;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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

            // 3. SQL Query: Count how many matching records exist
            string query = "SELECT custID FROM CUSTOMER WHERE custEmail = @custEmail AND custPassword = @custPassword;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@custEmail", txtEmail.Text.Trim());
                    command.Parameters.AddWithValue("@custPassword", txtPassword.Text.Trim());

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null) // If a match is found
                        {
                            // Save the ID to our memory slot
                            LoggedInCustID = Convert.ToInt32(result);

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
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }
    }
}
