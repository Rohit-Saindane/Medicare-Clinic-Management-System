using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClinicManagement.Helpers;

namespace ClinicManagement
{
    public partial class BookAppointment : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Security check: Must be a logged-in user
            if (!SessionHelper.IsLoggedIn)
            {
                Response.Redirect("~/Login.aspx?returnUrl=BookAppointment.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Default date to tomorrow
                txtAppointmentDate.Text = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

                LoadSpecializations();

                // If doctor passed via QueryString from the homepage card:
                if (!string.IsNullOrEmpty(Request.QueryString["docId"]))
                {
                    SelectDoctorFromQuery(Request.QueryString["docId"]);
                }
                else
                {
                    LoadDoctors(null);
                }
            }
        }

        private void LoadSpecializations()
        {
            try
            {
                string query = "SELECT DISTINCT Specialization FROM Doctors ORDER BY Specialization";
                DataTable dt = DBHelper.ExecuteDataTable(query);

                ddlSpecialization.Items.Clear();
                ddlSpecialization.Items.Add(new ListItem("-- All Specializations --", ""));

                foreach (DataRow row in dt.Rows)
                {
                    ddlSpecialization.Items.Add(new ListItem(row["Specialization"].ToString(), row["Specialization"].ToString()));
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading specialties: " + ex.Message);
            }
        }

        private void LoadDoctors(string specialization)
        {
            try
            {
                string query = "SELECT DoctorId, DoctorName, Specialization FROM Doctors ";
                SqlParameter[] parameters = null;

                if (!string.IsNullOrEmpty(specialization))
                {
                    query += "WHERE Specialization = @Spec ";
                    parameters = new SqlParameter[] { new SqlParameter("@Spec", specialization) };
                }

                query += "ORDER BY DoctorName";

                DataTable dt = DBHelper.ExecuteDataTable(query, parameters);

                ddlDoctor.Items.Clear();
                ddlDoctor.Items.Add(new ListItem("-- Select Doctor --", ""));

                foreach (DataRow row in dt.Rows)
                {
                    string label = string.Format("{0} ({1})", row["DoctorName"], row["Specialization"]);
                    ddlDoctor.Items.Add(new ListItem(label, row["DoctorId"].ToString()));
                }

                pnlDoctorDetails.Visible = false;
            }
            catch (Exception ex)
            {
                ShowError("Error loading doctors: " + ex.Message);
            }
        }

        private void SelectDoctorFromQuery(string docId)
        {
            try
            {
                string query = "SELECT DoctorId, Specialization FROM Doctors WHERE DoctorId = @DocId";
                DataTable dt = DBHelper.ExecuteDataTable(query, new SqlParameter[] { new SqlParameter("@DocId", docId) });

                if (dt.Rows.Count > 0)
                {
                    string spec = dt.Rows[0]["Specialization"].ToString();
                    if (ddlSpecialization.Items.FindByValue(spec) != null)
                    {
                        ddlSpecialization.SelectedValue = spec;
                    }
                    LoadDoctors(spec);

                    if (ddlDoctor.Items.FindByValue(docId) != null)
                    {
                        ddlDoctor.SelectedValue = docId;
                        UpdateDoctorPreview(Convert.ToInt32(docId));
                    }
                }
            }
            catch
            {
                LoadDoctors(null);
            }
        }

        protected void ddlSpecialization_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDoctors(ddlSpecialization.SelectedValue);
        }

        protected void ddlDoctor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(ddlDoctor.SelectedValue, out int docId))
            {
                UpdateDoctorPreview(docId);
            }
            else
            {
                pnlDoctorDetails.Visible = false;
            }
        }

        private void UpdateDoctorPreview(int docId)
        {
            try
            {
                // Disconnected Architecture / Parameterized Query
                string query = "SELECT Qualification, ExperienceYears, RoomNumber, ConsultationFee, AvailableDays FROM Doctors WHERE DoctorId = @DocId";
                DataTable dt = DBHelper.ExecuteDataTable(query, new SqlParameter[] { new SqlParameter("@DocId", docId) });

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    litDocDetails.Text = string.Format("{0} ({1} yrs experience)", row["Qualification"], row["ExperienceYears"]);
                    litDocRoom.Text = row["RoomNumber"].ToString();
                    litDocFee.Text = "₹" + Convert.ToDecimal(row["ConsultationFee"]).ToString("N0");
                    litDocDays.Text = row["AvailableDays"].ToString();
                    pnlDoctorDetails.Visible = true;
                }
            }
            catch
            {
                pnlDoctorDetails.Visible = false;
            }
        }

        protected void btnConfirmBooking_Click(object sender, EventArgs e)
        {
            pnlError.Visible = false;
            pnlSuccess.Visible = false;

            if (!int.TryParse(ddlDoctor.SelectedValue, out int docId))
            {
                ShowError("Please select a doctor.");
                return;
            }

            if (!DateTime.TryParse(txtAppointmentDate.Text, out DateTime appDate))
            {
                ShowError("Please provide a valid appointment date.");
                return;
            }

            if (appDate.Date < DateTime.Today)
            {
                ShowError("Appointment date cannot be in the past. Please choose today or a future date.");
                return;
            }

            string timeSlot = ddlTimeSlot.SelectedValue;
            if (string.IsNullOrEmpty(timeSlot))
            {
                ShowError("Please choose a time slot.");
                return;
            }

            string symptoms = txtSymptoms.Text.Trim();

            try
            {
                // Conflict Validation:
                // Check if this doctor is already booked for this specific date and time slot
                string conflictQuery = @"SELECT COUNT(*) 
                                        FROM Appointments 
                                        WHERE DoctorId = @DocId 
                                          AND AppointmentDate = @AppDate 
                                          AND TimeSlot = @TimeSlot 
                                          AND Status NOT IN ('Cancelled')";

                SqlParameter[] conflictParams = new SqlParameter[]
                {
                    new SqlParameter("@DocId", docId),
                    new SqlParameter("@AppDate", appDate.Date),
                    new SqlParameter("@TimeSlot", timeSlot)
                };

                int conflictCount = Convert.ToInt32(DBHelper.ExecuteScalar(conflictQuery, conflictParams));
                if (conflictCount > 0)
                {
                    ShowError(string.Format("Doctor is already booked for '{0}' on {1:dd-MMM-yyyy}. Please select another time slot or date.", timeSlot, appDate));
                    return;
                }

                // Insert the new appointment record
                string insertQuery = @"INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, TimeSlot, Symptoms, Status)
                                       VALUES (@PatientId, @DocId, @AppDate, @TimeSlot, @Symptoms, 'Confirmed')";

                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@PatientId", SessionHelper.UserId),
                    new SqlParameter("@DocId", docId),
                    new SqlParameter("@AppDate", appDate.Date),
                    new SqlParameter("@TimeSlot", timeSlot),
                    new SqlParameter("@Symptoms", symptoms)
                };

                int rowsAffected = DBHelper.ExecuteNonQuery(insertQuery, insertParams);
                if (rowsAffected > 0)
                {
                    pnlSuccess.Visible = true;
                    litSuccessMsg.Text = string.Format("Your appointment has been confirmed for <strong>{0:dd-MMM-yyyy}</strong> at <strong>{1}</strong>!", appDate, timeSlot);
                    txtSymptoms.Text = string.Empty;
                    ddlTimeSlot.SelectedIndex = 0;
                }
                else
                {
                    ShowError("Unable to save appointment. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Database error during booking: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            litErrorMsg.Text = Server.HtmlEncode(message);
        }
    }
}

