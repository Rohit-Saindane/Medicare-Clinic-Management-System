using System;
using System.Data.SqlClient;
using System.Web.UI;
using ClinicManagement.Helpers;

namespace ClinicManagement
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (SessionHelper.IsLoggedIn)
                {
                    Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowAlert("All fields are required.");
                return;
            }

            try
            {
                // 1. Check if email already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                SqlParameter[] checkParams = new SqlParameter[]
                {
                    new SqlParameter("@Email", email)
                };

                int existingCount = Convert.ToInt32(DBHelper.ExecuteScalar(checkQuery, checkParams));
                if (existingCount > 0)
                {
                    ShowAlert("An account with this email address already exists. Please sign in.");
                    return;
                }

                // 2. Insert new Patient user record and return the generated UserId using SCOPE_IDENTITY()
                string insertQuery = @"INSERT INTO Users (FullName, Email, Password, Phone, Role)
                                       VALUES (@FullName, @Email, @Password, @Phone, 'Patient');
                                       SELECT SCOPE_IDENTITY();";

                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@FullName", fullName),
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Password", password),
                    new SqlParameter("@Phone", phone)
                };

                object newId = DBHelper.ExecuteScalar(insertQuery, insertParams);
                if (newId != null && int.TryParse(newId.ToString(), out int userId))
                {
                    // Auto login the newly registered patient
                    SessionHelper.UserId = userId;
                    SessionHelper.FullName = fullName;
                    SessionHelper.Email = email;
                    SessionHelper.Role = "Patient";

                    Response.Redirect("~/BookAppointment.aspx?registered=1");
                }
                else
                {
                    ShowAlert("Failed to complete registration. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Database error during registration: " + ex.Message);
            }
        }

        private void ShowAlert(string message)
        {
            pnlAlert.Visible = true;
            litAlertMsg.Text = Server.HtmlEncode(message);
        }
    }
}

