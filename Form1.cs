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
            string query = @"SELECT COUNT(1) FROM CUSTOMER 
                             WHERE custEmail = @custEmail AND custPassword = @custPassword;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Securely bind the user inputs to your parameters
                    command.Parameters.AddWithValue("@custEmail", email);
                    command.Parameters.AddWithValue("@custPassword", password);

                    try
                    {
                        connection.Open();

                        // ExecuteScalar returns the first column of the first row (the count)
                        int userExists = Convert.ToInt32(command.ExecuteScalar());

                        if (userExists == 1)
                        {
                            // Success! Credentials match perfectly
                            MessageBox.Show("Login successful! Welcome to DEF Delivery Services. 🚀",
                                            "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Open the next form (assuming it's named frmHome)
                            frmHome homePage = new frmHome();
                            homePage.Show();

                            // Hide or Close this login screen
                            this.Hide();
                        }
                        else
                        {
                            // Security Best Practice: Don't specify if the email or password was the wrong one
                            MessageBox.Show("Invalid Email or Password. Please try again.",
                                            "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            txtPassword.Clear();
                            txtPassword.Focus();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occurred: {ex.Message}",
                                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
