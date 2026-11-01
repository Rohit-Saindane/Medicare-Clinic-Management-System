using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClinicManagement.Helpers;

namespace ClinicManagement
{
    public partial class ManageDoctors : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.IsLoggedIn)
            {
                Response.Redirect("~/Login.aspx?returnUrl=ManageDoctors.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Only Admin can add or delete doctors
                phAddDoctorBtn.Visible = SessionHelper.IsAdmin;
                BindDoctors();
            }
        }

        private void BindDoctors()
        {
            try
            {
                string query = @"SELECT 
                                    D.DoctorId, 
                                    D.UserId, 
                                    D.DoctorName, 
                                    D.Specialization, 
                                    D.Qualification, 
                                    D.ExperienceYears, 
                                    D.ConsultationFee, 
                                    D.RoomNumber, 
                                    D.AvailableDays, 
                                    U.Email
                                 FROM Doctors D 
                                 INNER JOIN Users U ON D.UserId = U.UserId 
                                 ORDER BY D.Specialization, D.DoctorName";

                DataTable dt = DBHelper.ExecuteDataTable(query);
                gvDoctors.DataSource = dt;
                gvDoctors.DataBind();

                lblDoctorCount.Text = dt.Rows.Count + " doctor(s)";
            }
            catch (Exception ex)
            {
                ShowNotice("Error retrieving doctors list: " + ex.Message);
            }
        }

        protected void btnSaveDoctor_Click(object sender, EventArgs e)
        {
            if (!SessionHelper.IsAdmin)
            {
                ShowNotice("Only Administrator has permission to onboard new doctors.");
                return;
            }

            string name = txtNewDocName.Text.Trim();
            string email = txtNewDocEmail.Text.Trim();
            string pass = txtNewDocPassword.Text.Trim();
            string spec = txtNewDocSpec.Text.Trim();
            string qual = txtNewDocQual.Text.Trim();
            int exp = int.TryParse(txtNewDocExp.Text.Trim(), out int expVal) ? expVal : 1;
            decimal fee = decimal.TryParse(txtNewDocFee.Text.Trim(), out decimal feeVal) ? feeVal : 500;
            string room = txtNewDocRoom.Text.Trim();
            string days = txtNewDocDays.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(spec))
            {
                ShowNotice("Please fill in all required fields (Name, Email, Password, Specialty).");
                return;
            }

            try
            {
                // 1. Check if email already registered
                int count = Convert.ToInt32(DBHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Users WHERE Email = @Email",
                    new SqlParameter[] { new SqlParameter("@Email", email) }
                ));

                if (count > 0)
                {
                    ShowNotice("An account with this email address already exists.");
                    return;
                }

                // 2. Insert User record with Role = 'Doctor' and retrieve new UserId
                string insertUserQuery = @"INSERT INTO Users (FullName, Email, Password, Phone, Role)
                                           VALUES (@FullName, @Email, @Password, 'N/A', 'Doctor');
                                           SELECT SCOPE_IDENTITY();";

                SqlParameter[] userParams = new SqlParameter[]
                {
                    new SqlParameter("@FullName", name),
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Password", pass)
                };

                object newUserIdObj = DBHelper.ExecuteScalar(insertUserQuery, userParams);
                if (newUserIdObj != null && int.TryParse(newUserIdObj.ToString(), out int newUserId))
                {
                    // 3. Insert Doctor profile linked by foreign key
                    string insertDocQuery = @"INSERT INTO Doctors (UserId, DoctorName, Specialization, Qualification, ExperienceYears, ConsultationFee, RoomNumber, AvailableDays)
                                              VALUES (@UserId, @DoctorName, @Spec, @Qual, @Exp, @Fee, @Room, @Days)";

                    SqlParameter[] docParams = new SqlParameter[]
                    {
                        new SqlParameter("@UserId", newUserId),
                        new SqlParameter("@DoctorName", name),
                        new SqlParameter("@Spec", spec),
                        new SqlParameter("@Qual", qual),
                        new SqlParameter("@Exp", exp),
                        new SqlParameter("@Fee", fee),
                        new SqlParameter("@Room", string.IsNullOrEmpty(room) ? "Room 101" : room),
                        new SqlParameter("@Days", string.IsNullOrEmpty(days) ? "Mon - Sat" : days)
                    };

                    DBHelper.ExecuteNonQuery(insertDocQuery, docParams);

                    ShowNotice(string.Format("Doctor '{0}' registered successfully!", name));
                    ClearAddForm();
                    BindDoctors();
                }
            }
            catch (Exception ex)
            {
                ShowNotice("Database error while registering doctor: " + ex.Message);
            }
        }

        protected void gvDoctors_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteDoctor")
            {
                if (!SessionHelper.IsAdmin)
                {
                    ShowNotice("Only Administrator has permission to delete doctors.");
                    return;
                }

                int userId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    // Deleting the User cascades to Doctors table due to ON DELETE CASCADE
                    string deleteQuery = "DELETE FROM Users WHERE UserId = @UserId AND Role = 'Doctor'";
                    int affected = DBHelper.ExecuteNonQuery(deleteQuery, new SqlParameter[] { new SqlParameter("@UserId", userId) });

                    if (affected > 0)
                    {
                        ShowNotice("Doctor profile deleted successfully.");
                        BindDoctors();
                    }
                }
                catch (Exception ex)
                {
                    ShowNotice("Error deleting doctor: " + ex.Message);
                }
            }
        }

        private void ClearAddForm()
        {
            txtNewDocName.Text = string.Empty;
            txtNewDocEmail.Text = string.Empty;
            txtNewDocPassword.Text = string.Empty;
            txtNewDocSpec.Text = string.Empty;
            txtNewDocQual.Text = string.Empty;
            txtNewDocRoom.Text = string.Empty;
            txtNewDocDays.Text = string.Empty;
        }

        private void ShowNotice(string message)
        {
            pnlNotice.Visible = true;
            litNoticeMsg.Text = Server.HtmlEncode(message);
        }
    }
}

