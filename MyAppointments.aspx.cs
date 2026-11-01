using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClinicManagement.Helpers;

namespace ClinicManagement
{
    public partial class MyAppointments : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.IsLoggedIn)
            {
                Response.Redirect("~/Login.aspx?returnUrl=MyAppointments.aspx");
                return;
            }

            if (!IsPostBack)
            {
                litPatientName.Text = Server.HtmlEncode(SessionHelper.FullName);
                BindAppointments();
            }
        }

        private void BindAppointments()
        {
            try
            {
                string query = @"SELECT 
                                    A.AppointmentId,
                                    A.AppointmentDate,
                                    A.TimeSlot,
                                    A.Symptoms,
                                    A.Status,
                                    A.DoctorNotes,
                                    D.DoctorName,
                                    D.Specialization
                                 FROM Appointments A
                                 INNER JOIN Doctors D ON A.DoctorId = D.DoctorId
                                 WHERE A.PatientId = @PatientId
                                 ORDER BY A.AppointmentDate DESC, A.AppointmentId DESC";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@PatientId", SessionHelper.UserId)
                };

                DataTable dt = DBHelper.ExecuteDataTable(query, parameters);
                gvAppointments.DataSource = dt;
                gvAppointments.DataBind();

                int total = dt.Rows.Count;
                int upcoming = 0;
                int completed = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string status = row["Status"]?.ToString() ?? "";
                    if (status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) ||
                        status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        upcoming++;
                    }
                    else if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        completed++;
                    }
                }

                lblTotalVisits.Text = total.ToString();
                lblUpcomingCount.Text = upcoming.ToString();
                lblCompletedCount.Text = completed.ToString();
                lblRecordCount.Text = total + " record(s)";
            }
            catch (Exception ex)
            {
                ShowNotice("Error retrieving appointments: " + ex.Message);
            }
        }

        protected void gvAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelAppointment")
            {
                int appointmentId = Convert.ToInt32(e.CommandArgument);

                try
                {
                    string query = @"UPDATE Appointments 
                                     SET Status = 'Cancelled' 
                                     WHERE AppointmentId = @AppId AND PatientId = @PatientId AND Status IN ('Pending', 'Confirmed')";

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@AppId", appointmentId),
                        new SqlParameter("@PatientId", SessionHelper.UserId)
                    };

                    int affected = DBHelper.ExecuteNonQuery(query, parameters);
                    if (affected > 0)
                    {
                        ShowNotice(string.Format("Appointment #{0} has been successfully cancelled.", appointmentId));
                        BindAppointments();
                    }
                    else
                    {
                        ShowNotice("Could not cancel the appointment. It may already be completed or cancelled.");
                    }
                }
                catch (Exception ex)
                {
                    ShowNotice("Error cancelling appointment: " + ex.Message);
                }
            }
        }

        protected string GetStatusBadgeClass(string status)
        {
            switch (status)
            {
                case "Confirmed":
                    return "badge-confirmed";
                case "Completed":
                    return "badge-completed";
                case "Cancelled":
                    return "badge-cancelled";
                case "Pending":
                default:
                    return "badge-pending";
            }
        }

        private void ShowNotice(string message)
        {
            pnlNotice.Visible = true;
            litNoticeMsg.Text = Server.HtmlEncode(message);
        }
    }
}
