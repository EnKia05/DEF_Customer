using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace DEF_Customer
{
    public partial class frmNotifications : MetroForm
    {
        // Central Database Connection String
        private string connectionString = @"Server=AIKENDAVE\SQLEXPRESS; Database=DEF_DeliveryDB; Integrated Security=True; TrustServerCertificate=True;";

        public frmNotifications()
        {
            InitializeComponent();
            // The color for the upper part of the Form
            this.Style = MetroFramework.MetroColorStyle.Pink;
        }

        /// <summary>
        /// Clears old dynamic notification cards and renders an informational tracking timeline.
        /// </summary>
        public void PopulateCustomerNotifications()
        {
            // ── SAFE CLEANUP ──
            List<Control> itemsToRemove = new List<Control>();
            foreach (Control ctrl in pnlNotificationPlaceholder.Controls)
            {
                if (ctrl is Panel && ctrl.Name == "DynamicNotifCard")
                {
                    itemsToRemove.Add(ctrl);
                }
                if (ctrl is Label && ctrl.Name == "lblEmptyStatus")
                {
                    itemsToRemove.Add(ctrl);
                }
            }
            foreach (Control ctrl in itemsToRemove)
            {
                pnlNotificationPlaceholder.Controls.Remove(ctrl);
                ctrl.Dispose();
            }

            // ── GLOBAL SESSION TRACKING SYNC ──
            int custID = frmLogIn.LoggedInCustID;

            // Simple safety check: if for some reason login state is 0, fallback for debug safety
            if (custID == 0) custID = 1002;

            int startY = 15;
            int cardSpacing = 12;
            // Locks width tightly inside the placeholder container, subtracting space for the scrollbar gutter
            int cardWidth = pnlNotificationPlaceholder.Width - 25;

            // Query references the clean notification log linked directly back to operational requests
            string query = @"
                SELECT n.notificationID, 
                       n.deliveryRequestID, 
                       n.notifTitle,
                       n.createdAt,
                       r.status AS DeliveryStatus,
                       r.itemName
                FROM CUSTOMER_NOTIFICATION n
                INNER JOIN DELIVERY_REQUEST r ON n.deliveryRequestID = r.deliveryRequestID
                WHERE n.custID = @custID
                ORDER BY n.createdAt DESC;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@custID", Convert.ToInt32(custID));

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        bool hasAny = false;

                        while (reader.Read())
                        {
                            hasAny = true;

                            string orderID = reader["deliveryRequestID"].ToString();
                            string dbTitle = reader["notifTitle"].ToString();
                            string currentStatus = reader["DeliveryStatus"].ToString();
                            string itemName = reader["itemName"].ToString();

                            // --- ADD THIS TRIGGER LAYER HERE ---
                            if (currentStatus.Trim() == "Completed")
                            {
                                // Convert string ID to int and pass it to your prompt layer
                                int numericOrderID = Convert.ToInt32(orderID);
                                PromptQuickFeedback(numericOrderID);
                            }
                            // -----------------------------------

                            // Formats timestamp cleanly for a standard log view
                            DateTime timeStamp = Convert.ToDateTime(reader["createdAt"]);
                            string displayTime = timeStamp.ToString("yyyy-MM-dd hh:mm tt");

                            // Generates industry standard logistics updates dynamically on the fly
                            string dynamicMessage = GetStandardLogisticsMessage(currentStatus, orderID, itemName);

                            // ── Card Container Layout ──
                            Panel card = new Panel
                            {
                                Name = "DynamicNotifCard",
                                Size = new Size(cardWidth, 85),
                                Location = new Point(10, startY),
                                BackColor = Color.White,
                                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
                            };

                            // ── Card Header Status Banner ──
                            Panel titleBar = new Panel
                            {
                                Location = new Point(0, 0),
                                Size = new Size(cardWidth, 28),
                                BackColor = Color.DeepPink
                            };

                            Label lblTitle = new Label
                            {
                                Text = $"🔔 {dbTitle} — Order #{orderID}",
                                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                                ForeColor = Color.White,
                                BackColor = Color.Transparent,
                                TextAlign = ContentAlignment.MiddleLeft,
                                Location = new Point(10, 0),
                                Size = new Size((cardWidth / 2) - 10, 28)
                            };

                            Label lblTime = new Label
                            {
                                Text = displayTime,
                                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                                ForeColor = Color.White,
                                BackColor = Color.Transparent,
                                TextAlign = ContentAlignment.MiddleRight,
                                Location = new Point(cardWidth / 2, 0),
                                Size = new Size((cardWidth / 2) - 10, 28)
                            };

                            titleBar.Controls.Add(lblTitle);
                            titleBar.Controls.Add(lblTime);

                            // ── Detailed Milestone Message Description ──
                            Label lblMessage = new Label
                            {
                                Text = dynamicMessage,
                                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                                ForeColor = Color.FromArgb(40, 40, 40),
                                Location = new Point(12, 36),
                                Size = new Size(cardWidth - 24, 40),
                                AutoEllipsis = true
                            };

                            // Assemble components into the card
                            card.Controls.Add(titleBar);
                            card.Controls.Add(lblMessage);

                            // Append complete card down into the targeted UI container panel
                            pnlNotificationPlaceholder.Controls.Add(card);
                            startY += card.Height + cardSpacing;
                        }

                        // Fallback UI view state empty logger
                        if (!hasAny)
                        {
                            Label lblEmpty = new Label
                            {
                                Name = "lblEmptyStatus",
                                Text = "✨ You don't have any delivery notifications updates yet.",
                                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                                ForeColor = Color.Gray,
                                AutoSize = true,
                                Location = new Point(20, 20)
                            };
                            pnlNotificationPlaceholder.Controls.Add(lblEmpty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load operational timeline logs:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Translates machine delivery states into user-friendly tracking logs (C# 7.3 Compatible).
        /// </summary>
        private string GetStandardLogisticsMessage(string status, string orderId, string itemName)
        {
            switch (status.Trim())
            {
                case "Pending":
                    return $"Order #{orderId} [{itemName}] has been successfully queued. Waiting for dispatch to match a courier.";

                case "Assigned":
                    return $"A courier has accepted your request for Order #{orderId}. Your items are currently being processed for pickup.";

                case "On the Way":
                    return $"Rider is now en route! Your package for Order #{orderId} is actively moving toward its destination.";

                case "Completed":
                    return $"Parcel delivered successfully! Order #{orderId} [{itemName}] has been marked as received. Thank you!";

                case "Cancelled":
                    return $"Order Update: Delivery request #{orderId} has been cancelled and will not proceed further.";

                default:
                    return $"Order #{orderId} status has transitioned to: {status}.";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateCustomerNotifications();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Re-instantiate the main dashboard home screen
            frmHome homeForm = new frmHome();
            homeForm.Show();

            // Close the notifications window safely
            this.Close();
        }

        private void frmNotifications_Load(object sender, EventArgs e)
        {
            // Ensure the container is ready for scrolling timelines
            pnlNotificationPlaceholder.AutoScroll = true;
            PopulateCustomerNotifications();
        }

        private void frmNotifications_Enter(object sender, EventArgs e)
        {
            PopulateCustomerNotifications();
        }

        private void PromptQuickFeedback(int orderID)
        {
            // ── 1. QUICK GUARD LAYER ──
            string checkQuery = "SELECT COUNT(1) FROM DELIVERY_FEEDBACK WHERE deliveryRequestID = @reqID;";

            using (SqlConnection checkConn = new SqlConnection(connectionString))
            using (SqlCommand checkCmd = new SqlCommand(checkQuery, checkConn))
            {
                checkCmd.Parameters.AddWithValue("@reqID", orderID);
                try
                {
                    checkConn.Open();
                    int alreadyExists = (int)checkCmd.ExecuteScalar();

                    if (alreadyExists > 0)
                    {
                        return; // Feedback already exists! Exit silently.
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error verifying feedback logs: {ex.Message}", "Database Check Failed");
                    return;
                }
            }

            // ── 2. SINGLE-PROMPT 1-5 STAR SELECTION LAYER ──
            int rating = 0;
            bool isValidRating = false;

            while (!isValidRating)
            {
                // Prompts the user to enter a specific number from 1 to 5
                string ratingInput = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Your Order #{orderID} has been successfully delivered! 🎉\n\n" +
                    "Please rate your delivery experience:\n" +
                    "5 = Excellent\n" +
                    "4 = Very Good\n" +
                    "3 = Average\n" +
                    "2 = Poor\n" +
                    "1 = Very Poor\n\n" +
                    "Enter a number (1-5):",
                    "Rate Your Delivery",
                    "5" // Defaults to 5 stars automatically for quick submission
                );

                // If the user clicks Cancel or leaves it empty, exit gracefully
                if (string.IsNullOrEmpty(ratingInput))
                {
                    return;
                }

                // Validate that the user typed an actual number between 1 and 5
                if (int.TryParse(ratingInput, out int numericScore) && numericScore >= 1 && numericScore <= 5)
                {
                    rating = numericScore;
                    isValidRating = true;
                }
                else
                {
                    MessageBox.Show("Invalid selection! Please enter a valid number between 1 and 5.",
                                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // ── 3. COMMENT CAPTURE LAYER ──
            string comment = "No comments provided.";
            string userInput = Microsoft.VisualBasic.Interaction.InputBox(
                $"You are logging a {rating}-Star rating.\n" +
                "Please share any additional comments or notes about this delivery (Optional):",
                "Add Feedback Remarks",
                ""
            ).Trim();

            if (!string.IsNullOrEmpty(userInput))
            {
                comment = userInput;
            }

            // ── 4. DATABASE SUBMISSION LAYER ──
            string query = @"INSERT INTO DELIVERY_FEEDBACK (deliveryRequestID, custID, rating, comments) 
                     VALUES (@reqID, @custID, @rating, @comments);";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@reqID", orderID);
                    command.Parameters.AddWithValue("@custID", frmLogIn.LoggedInCustID);
                    command.Parameters.AddWithValue("@rating", rating);
                    command.Parameters.AddWithValue("@comments", comment);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show($"Thank you! A {rating}-Star rating has been logged to our analytics dashboard.", "Feedback Saved");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to save feedback data: {ex.Message}", "SQL Insertion Error");
                    }
                }
            }
        }
    }
}