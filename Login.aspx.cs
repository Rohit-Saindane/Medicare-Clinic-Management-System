using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using ClinicManagement.Helpers;

namespace ClinicManagement
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If already logged in, redirect to respective dashboard
                if (SessionHelper.IsLoggedIn)
                {
                    RedirectByRole();
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowAlert("Please enter both email and password.");
                return;
            }

            try
            {
                // ADO.NET Connected Architecture demonstration:
                // Using SqlDataReader with parameterized query to avoid SQL Injection vulnerability
                string query = @"SELECT UserId, FullName, Email, Role 
                                 FROM Users 
                                 WHERE Email = @Email AND Password = @Password";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Password", password)
                };

                using (IDataReader reader = DBHelper.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        // Set Session state
                        SessionHelper.UserId = Convert.ToInt32(reader["UserId"]);
                        SessionHelper.FullName = reader["FullName"].ToString();
                        SessionHelper.Email = reader["Email"].ToString();
                        SessionHelper.Role = reader["Role"].ToString();

                        // If user is a Doctor, fetch their DoctorId for appointment filtering
                        if (SessionHelper.IsDoctor)
                        {
                            reader.Close(); // Close existing reader before executing another scalar query on the same connection
                            object docIdObj = DBHelper.ExecuteScalar(
                                "SELECT DoctorId FROM Doctors WHERE UserId = @UserId",
                                new SqlParameter[] { new SqlParameter("@UserId", SessionHelper.UserId) }
                            );
                            if (docIdObj != null)
                            {
                                SessionHelper.DoctorId = Convert.ToInt32(docIdObj);
                            }
                        }

                        RedirectByRole();
                        return;
                    }
                    else
                    {
                        ShowAlert("Invalid email or password. Please verify and try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Database connection error: " + ex.Message);
            }
        }

        private void RedirectByRole()
        {
            if (SessionHelper.IsPatient)
            {
                Response.Redirect("~/MyAppointments.aspx");
            }
            else if (SessionHelper.IsDoctor || SessionHelper.IsAdmin)
            {
                Response.Redirect("~/DoctorDashboard.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        private void ShowAlert(string message)
        {
            pnlAlert.Visible = true;
            litAlertMsg.Text = Server.HtmlEncode(message);
        }
    }
}

