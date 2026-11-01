using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClinicManagement.Helpers;

namespace ClinicManagement
{
    public partial class DoctorDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Security check: Must be Doctor or Admin
            if (!SessionHelper.IsLoggedIn || (!SessionHelper.IsDoctor && !SessionHelper.IsAdmin))
            {
                Response.Redirect("~/Login.aspx?returnUrl=DoctorDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (SessionHelper.IsDoctor)
                    litDashboardHeader.Text = SessionHelper.FullName + " — Patient Queue";
                else
                    litDashboardHeader.Text = "Clinic Operations & Consultation Queue";

                LoadDoctorFilter();
                BindDoctorQueue();
            }
        }

        private void LoadDoctorFilter()
        {
            try
            {
                string query = "SELECT DoctorId, DoctorName, Specialization FROM Doctors ORDER BY DoctorName";
                DataTable dt = DBHelper.ExecuteDataTable(query);

                ddlFilterDoctor.Items.Clear();

                if (SessionHelper.IsAdmin)
                {
                    ddlFilterDoctor.Items.Add(new ListItem("-- All Doctors --", ""));
                }

                foreach (DataRow row in dt.Rows)
                {
                    string label = string.Format("{0} ({1})", row["DoctorName"], row["Specialization"]);
                    ddlFilterDoctor.Items.Add(new ListItem(label, row["DoctorId"].ToString()));
                }

                // If currently logged-in user is a doctor, default filter to their DoctorId
                if (SessionHelper.IsDoctor && SessionHelper.DoctorId > 0)
                {
                    string docIdStr = SessionHelper.DoctorId.ToString();
                    if (ddlFilterDoctor.Items.FindByValue(docIdStr) != null)
                    {
                        ddlFilterDoctor.SelectedValue = docIdStr;
                        // Doctors only review their own queue unless Admin
                        ddlFilterDoctor.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotice("Error loading doctor filter: " + ex.Message);
            }
        }

        protected void FilterChanged(object sender, EventArgs e)
        {
            BindDoctorQueue();
        }

        protected void btnResetFilters_Click(object sender, EventArgs e)
        {
            if (SessionHelper.IsAdmin)
            {
                ddlFilterDoctor.SelectedIndex = 0;
            }
            ddlFilterStatus.SelectedIndex = 0;
            txtFilterDate.Text = string.Empty;
            pnlPrescriptionBox.Visible = false;
            BindDoctorQueue();
        }

        private void BindDoctorQueue()
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
                                    P.FullName AS PatientName,
                                    P.Phone AS PatientPhone,
                                    D.DoctorName,
                                    D.Specialization
                                 FROM Appointments A
                                 INNER JOIN Users P ON A.PatientId = P.UserId
                                 INNER JOIN Doctors D ON A.DoctorId = D.DoctorId
                                 WHERE 1=1 ";

                List<SqlParameter> parameters = new List<SqlParameter>();

                // Filter by Doctor
                if (!string.IsNullOrEmpty(ddlFilterDoctor.SelectedValue))
                {
                    query += " AND A.DoctorId = @DocId ";
                    parameters.Add(new SqlParameter("@DocId", Convert.ToInt32(ddlFilterDoctor.SelectedValue)));
                }

                // Filter by Status
                if (!string.IsNullOrEmpty(ddlFilterStatus.SelectedValue))
                {
                    query += " AND A.Status = @Status ";
                    parameters.Add(new SqlParameter("@Status", ddlFilterStatus.SelectedValue));
                }

                // Filter by Date
                if (DateTime.TryParse(txtFilterDate.Text, out DateTime filterDate))
                {
                    query += " AND CAST(A.AppointmentDate AS DATE) = @AppDate ";
                    parameters.Add(new SqlParameter("@AppDate", filterDate.Date));
                }

                query += " ORDER BY A.AppointmentDate DESC, A.AppointmentId DESC";

                DataTable dt = DBHelper.ExecuteDataTable(query, parameters.ToArray());
                gvDoctorAppointments.DataSource = dt;
                gvDoctorAppointments.DataBind();

                // Compute KPI metrics
                int total = dt.Rows.Count;
                int todayCount = 0;
                int pendingCount = 0;
                int completedCount = 0;
                DateTime today = DateTime.Today;

                foreach (DataRow r in dt.Rows)
                {
                    string st = r["Status"]?.ToString() ?? "";
                    if (DateTime.TryParse(r["AppointmentDate"]?.ToString(), out DateTime appDate) && appDate.Date == today)
                    {
                        todayCount++;
                    }

                    if (st.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) || st.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        pendingCount++;
                    }
                    else if (st.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        completedCount++;
                    }
                }

                lblDocTotal.Text = total.ToString();
                lblDocToday.Text = todayCount.ToString();
                lblDocPending.Text = pendingCount.ToString();
                lblDocCompleted.Text = completedCount.ToString();
                lblRecordCount.Text = total + " patient record(s)";
            }
            catch (Exception ex)
            {
                ShowNotice("Error loading patient queue: " + ex.Message);
            }
        }

        protected void gvDoctorAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "OpenPrescription")
            {
                int appointmentId = Convert.ToInt32(e.CommandArgument);
                LoadAppointmentForPrescription(appointmentId);
            }
        }

        private void LoadAppointmentForPrescription(int appointmentId)
        {
            try
            {
                string query = @"SELECT 
                                    A.AppointmentId,
                                    A.Symptoms,
                                    A.DoctorNotes,
                                    P.FullName AS PatientName,
                                    P.Phone AS PatientPhone
                                 FROM Appointments A
                                 INNER JOIN Users P ON A.PatientId = P.UserId
                                 WHERE A.AppointmentId = @AppId";

                DataTable dt = DBHelper.ExecuteDataTable(query, new SqlParameter[] { new SqlParameter("@AppId", appointmentId) });

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    litEditingAppId.Text = appointmentId.ToString();
                    litEditPatientName.Text = Server.HtmlEncode(row["PatientName"].ToString());
                    litEditPatientPhone.Text = Server.HtmlEncode(row["PatientPhone"].ToString());
                    litEditSymptoms.Text = Server.HtmlEncode(row["Symptoms"].ToString());
                    txtDoctorNotes.Text = row["DoctorNotes"] != DBNull.Value ? row["DoctorNotes"].ToString() : string.Empty;

                    pnlPrescriptionBox.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ShowNotice("Error opening appointment: " + ex.Message);
            }
        }

        protected void btnSavePrescription_Click(object sender, EventArgs e)
        {
            if (int.TryParse(litEditingAppId.Text, out int appointmentId))
            {
                string notes = txtDoctorNotes.Text.Trim();

                try
                {
                    string updateQuery = @"UPDATE Appointments 
                                           SET DoctorNotes = @Notes, Status = 'Completed' 
                                           WHERE AppointmentId = @AppId";

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Notes", notes),
                        new SqlParameter("@AppId", appointmentId)
                    };

                    int affected = DBHelper.ExecuteNonQuery(updateQuery, parameters);
                    if (affected > 0)
                    {
                        ShowNotice(string.Format("Prescription and consultation recorded successfully for Appointment #{0} (Status marked as Completed).", appointmentId));
                        pnlPrescriptionBox.Visible = false;
                        BindDoctorQueue();
                    }
                }
                catch (Exception ex)
                {
                    ShowNotice("Error saving prescription: " + ex.Message);
                }
            }
        }

        protected void btnMarkCancelled_Click(object sender, EventArgs e)
        {
            if (int.TryParse(litEditingAppId.Text, out int appointmentId))
            {
                try
                {
                    string updateQuery = "UPDATE Appointments SET Status = 'Cancelled' WHERE AppointmentId = @AppId";
                    DBHelper.ExecuteNonQuery(updateQuery, new SqlParameter[] { new SqlParameter("@AppId", appointmentId) });

                    ShowNotice(string.Format("Appointment #{0} has been marked as Cancelled.", appointmentId));
                    pnlPrescriptionBox.Visible = false;
                    BindDoctorQueue();
                }
                catch (Exception ex)
                {
                    ShowNotice("Error cancelling appointment: " + ex.Message);
                }
            }
        }

        protected void btnClosePrescription_Click(object sender, EventArgs e)
        {
            pnlPrescriptionBox.Visible = false;
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
