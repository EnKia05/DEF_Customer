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
using MetroFramework.Forms;

namespace DEF_Customer
{
    public partial class frmBookPayment : MetroForm
    {
        // Connection string matching your server environment
        private string connectionString = @"Server=AIKENDAVE\SQLEXPRESS; Database=DEF_DeliveryDB; Integrated Security=True; TrustServerCertificate=True;";

        // Financial Variables
        private decimal flatRate = 50.00m;
        private decimal vehicleSurcharge = 0.00m;
        private decimal totalFee = 0.00m;

        public frmBookPayment()
        {
            InitializeComponent();
            //The color for the upper part of the Form.
            this.Style = MetroFramework.MetroColorStyle.Pink; //
        }


        private void btnSubmitOrder_Click(object sender, EventArgs e) //
        {
            // 1. INPUT VALIDATION
            if (!rdoOnlinePayment.Checked && !rdoCOD.Checked)
            {
                MessageBox.Show("Please select a payment option before submitting.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string paymentMethod = "";
            string paymentStatus = "";
            string gcashRefNo = "NULL";

            if (rdoOnlinePayment.Checked)
            {
                // Simple check to make sure they didn't leave reference fields blank
                if (string.IsNullOrEmpty(txtCellphoneNumber.Text.Trim()) || string.IsNullOrEmpty(txtReferenceNumber.Text.Trim()))
                {
                    MessageBox.Show("Please fill out your GCash details and reference number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                paymentMethod = "Online Payment (GCash)";
                paymentStatus = "Paid";
                gcashRefNo = txtReferenceNumber.Text.Trim();
            }
            else if (rdoCOD.Checked)
            {
                paymentMethod = "Cash on Delivery";
                paymentStatus = "Pending"; // Stays pending until package arrives at drop-off location
            }

            // 2. TRANSACTION EXECUTION ENGINE
            // Query 1: Save Delivery Request data and instantly output the generated identity ID scope number
            string deliveryQuery = @"INSERT INTO DELIVERY_REQUEST 
                (custID, pickupLocation, receiverName, ReceiverContact, dropOffLocation, itemName, itemDescription, itemType, packageSize, vehicleType) 
                VALUES (@custID, @pickup, @recName, @recContact, @dropoff, @itemName, @itemDesc, @itemType, @pkgSize, @vehType);
                SELECT SCOPE_IDENTITY();"; // This captures the deliveryRequestID that SQL server just generated!

            // Query 2: Save Payment log entry using the extracted tracking token
            string paymentQuery = @"INSERT INTO PAYMENT 
                (deliveryRequestID, flatRate, vehicleSurcharge, totalFee, paymentMethod, paymentStatus, gcashRefNo) 
                VALUES (@reqID, @flat, @surcharge, @total, @method, @status, @refNo);";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    int newRequestID = 0;

                    // Execute Phase 1: Save Delivery
                    using (SqlCommand command = new SqlCommand(deliveryQuery, connection))
                    {
                        command.Parameters.AddWithValue("@custID", frmLogIn.LoggedInCustID);
                        command.Parameters.AddWithValue("@pickup", frmBookDetails1.TempPickupAddress);
                        command.Parameters.AddWithValue("@recName", frmBookDetails1.TempRecipientName);
                        command.Parameters.AddWithValue("@recContact", frmBookDetails1.TempRecipientContact);
                        command.Parameters.AddWithValue("@dropoff", frmBookDetails1.TempDropoffAddress);
                        command.Parameters.AddWithValue("@itemName", frmBookDetails2.TempItemName);
                        command.Parameters.AddWithValue("@itemDesc", string.IsNullOrEmpty(frmBookDetails2.TempItemDescription) ? (object)DBNull.Value : frmBookDetails2.TempItemDescription);
                        command.Parameters.AddWithValue("@itemType", frmBookDetails2.TempItemType);
                        command.Parameters.AddWithValue("@pkgSize", frmBookDetails2.TempPackageSize);
                        command.Parameters.AddWithValue("@vehType", frmBookDetails2.TempVehicleType);

                        // Execute and convert object result into our tracking identifier integer variable
                        newRequestID = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Execute Phase 2: Save Financials
                    using (SqlCommand command = new SqlCommand(paymentQuery, connection))
                    {
                        command.Parameters.AddWithValue("@reqID", newRequestID);
                        command.Parameters.AddWithValue("@flat", flatRate);
                        command.Parameters.AddWithValue("@surcharge", vehicleSurcharge);
                        command.Parameters.AddWithValue("@total", totalFee);
                        command.Parameters.AddWithValue("@method", paymentMethod);
                        command.Parameters.AddWithValue("@status", paymentStatus);
                        command.Parameters.AddWithValue("@refNo", rdoOnlinePayment.Checked ? (object)gcashRefNo : DBNull.Value);

                        command.ExecuteNonQuery();
                    }

                    // Success Feedback Message Box
                    MessageBox.Show($"Order placed successfully!\nYour Booking ID is #{newRequestID}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 3. NAVIGATION FORWARD
                    frmNotifications newWindow = new frmNotifications(); //
                    newWindow.Show(); //
                    this.Hide(); //
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database Error encountered: {ex.Message}", "Transaction Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void frmBookPayment_Load_1(object sender, EventArgs e)
        {
            // 1. Calculate vehicle surcharge based on the choice from Form 2
            string chosenVehicle = frmBookDetails2.TempVehicleType; //

            if (chosenVehicle == "Bicycle") vehicleSurcharge = 10.00m;
            else if (chosenVehicle == "Motorcycle") vehicleSurcharge = 30.00m;
            else if (chosenVehicle == "Car") vehicleSurcharge = 70.00m;
            else if (chosenVehicle == "Truck") vehicleSurcharge = 150.00m;

            // 2. Sum the final pricing math
            totalFee = flatRate + vehicleSurcharge;

            // 3. Display the calculation dynamically on your UI label
            lblTotalFee.Text = "₱ " + totalFee.ToString("0.00");

            // 4. Set container visibility defaults (Keeps layouts clean until a choice is selected)
            pnlGCash.Visible = false;
            pnlCOD.Visible = false;
        }

        private void rdoOnlinePayment_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rdoOnlinePayment.Checked)
            {
                pnlGCash.Visible = true;  // Displays QR, cellphone number, and reference boxes
                pnlCOD.Visible = false;   // Hides standard COD placeholder text box
            }
        }

        private void rdoCOD_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rdoCOD.Checked)
            {
                pnlCOD.Visible = true;    // Displays instructions for preparing exact change
                pnlGCash.Visible = false; // Hides GCash inputs
            }
        }
    }
}