using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MetroFramework.Forms;

namespace DEF_Customer
{
    public partial class frmProfile : MetroForm
    {
        // Database connection string config targeting your local instance
        private readonly string connectionString = @"Server=AIKENDAVE\SQLEXPRESS; Database=DEF_DeliveryDB; Integrated Security=True; TrustServerCertificate=True;";

        // Simulating the globally logged-in customer's ID (Replace with your global session variable, e.g., Program.CurrentCustomerID)
        private readonly int loggedInCustomerID = 1001;

        public frmProfile()
        {
            InitializeComponent();
            // The color for the upper part of the Form
            this.Style = MetroFramework.MetroColorStyle.Pink;
        }

        /// <summary>
        /// Master method to trigger all customer profile data hydration concurrently.
        /// </summary>
        public async Task RefreshProfileDashboardAsync()
        {
            await LoadCustomerAccountDetailsAsync();
            await LoadMetricCardsAsync();
            await LoadDeliveryHistoryAsync();
        }

        /// <summary>
        /// Fetches customer personal information and fills the text controls securely.
        /// </summary>
        private async Task LoadCustomerAccountDetailsAsync()
        {
            // Updated with your exact database schema column names: custFullName, custEmail, custcontact, custPassword
            string query = "SELECT custFullName, custEmail, custcontact, custPassword FROM CUSTOMER WHERE custID = @CustID;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustID", loggedInCustomerID);

                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            txtFullName.Text = reader["custFullName"].ToString();
                            txtEmail.Text = reader["custEmail"].ToString(); // Fixed column mapping
                            txtContact.Text = reader["custcontact"].ToString(); // Fixed column mapping

                            // Mask password for security layout compliance
                            txtPassword.Text = reader["custPassword"].ToString(); // Fixed column mapping
                            txtPassword.UseSystemPasswordChar = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading account details: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Calculates spending aggregates and lifetime order volume totals.
        /// </summary>
        private async Task LoadMetricCardsAsync()
        {
            // FIX: Joined DELIVERY_REQUEST with PAYMENT to access the 'totalFee' column securely
            string querySpend = @"
        SELECT ISNULL(SUM(P.totalFee), 0) 
        FROM DELIVERY_REQUEST DR
        INNER JOIN PAYMENT P ON DR.deliveryRequestID = P.deliveryRequestID
        WHERE DR.custID = @CustID AND DR.status = 'Completed';";

            string queryOrders = "SELECT COUNT(*) FROM DELIVERY_REQUEST WHERE custID = @CustID;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // 1. Calculate and populate Total Spending
                    using (SqlCommand cmdSpend = new SqlCommand(querySpend, connection))
                    {
                        cmdSpend.Parameters.AddWithValue("@CustID", loggedInCustomerID);
                        decimal totalSpend = Convert.ToDecimal(await cmdSpend.ExecuteScalarAsync());
                        lblTotalSpendingCount.Text = totalSpend == 0 ? "₱0.00" : $"₱{totalSpend:N2}";
                    }

                    // 2. Calculate and populate Total Orders
                    using (SqlCommand cmdOrders = new SqlCommand(queryOrders, connection))
                    {
                        cmdOrders.Parameters.AddWithValue("@CustID", loggedInCustomerID);
                        int totalOrders = (int)await cmdOrders.ExecuteScalarAsync();
                        lblTotalOrdersCount.Text = totalOrders.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading profile metrics: {ex.Message}", "Metrics Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Hydrates the Delivery History DataGridView with records belonging to this client.
        /// </summary>
        private async Task LoadDeliveryHistoryAsync()
        {
            if (dgvDeliveryHistory == null) return;

            string queryHistory = @"
            SELECT 
                deliveryRequestID AS [Delivery ID],
                status AS [Status],
                createdAt AS [Date]
            FROM DELIVERY_REQUEST 
            WHERE custID = @CustID
            ORDER BY createdAt DESC;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(queryHistory, connection))
            {
                command.Parameters.AddWithValue("@CustID", loggedInCustomerID);

                try
                {
                    DataTable dataTable = new DataTable();

                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }

                    dgvDeliveryHistory.AutoGenerateColumns = false;
                    dgvDeliveryHistory.Columns.Clear();

                    // Adding and binding grid column views explicitly
                    dgvDeliveryHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Delivery ID", Name = "Delivery ID", HeaderText = "Delivery ID", Width = 110 });
                    dgvDeliveryHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", Name = "Status", HeaderText = "Status", Width = 120 });
                    dgvDeliveryHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Date", Name = "Date", HeaderText = "Order Date", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

                    dgvDeliveryHistory.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading history log: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #region Navigation and Authentication Controls

        /// <summary>
        /// Sign out action sequence handling clean-up operations.
        /// </summary>


        #endregion

        // Added the 'async' modifier here so the 'await' keyword can be used inside
        private async void frmProfile_Load(object sender, EventArgs e)
        {
            await RefreshProfileDashboardAsync();
        }

        private void btnNotifications_Click(object sender, EventArgs e)
        {
            frmNotifications notificationsForm = new frmNotifications();
            notificationsForm.Show();
            this.Close(); // Closes current profile screen context
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmHome homeForm = new frmHome();
            homeForm.Show();
            this.Close();
        }

        private void btnSignOut_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show("Are you sure you want to sign out?", "Confirm Log Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                // Re-open authentication portal or flush runtime context before hard exit closure
                Application.Exit();
            }
        }
    }
}