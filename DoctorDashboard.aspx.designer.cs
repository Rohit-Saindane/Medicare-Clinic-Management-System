namespace ClinicManagement
{
    public partial class DoctorDashboard
    {
        protected global::System.Web.UI.WebControls.Literal litDashboardHeader;
        protected global::System.Web.UI.WebControls.HyperLink hlManageDoctors;
        protected global::System.Web.UI.WebControls.Label lblDocToday;
        protected global::System.Web.UI.WebControls.Label lblDocPending;
        protected global::System.Web.UI.WebControls.Label lblDocCompleted;
        protected global::System.Web.UI.WebControls.Label lblDocTotal;
        protected global::System.Web.UI.WebControls.Panel pnlNotice;
        protected global::System.Web.UI.WebControls.Literal litNoticeMsg;
        protected global::System.Web.UI.WebControls.DropDownList ddlFilterDoctor;
        protected global::System.Web.UI.WebControls.DropDownList ddlFilterStatus;
        protected global::System.Web.UI.WebControls.TextBox txtFilterDate;
        protected global::System.Web.UI.WebControls.Button btnResetFilters;
        protected global::System.Web.UI.WebControls.Panel pnlPrescriptionBox;
        protected global::System.Web.UI.WebControls.Literal litEditingAppId;
        protected global::System.Web.UI.WebControls.Button btnClosePrescription;
        protected global::System.Web.UI.WebControls.Literal litEditPatientName;
        protected global::System.Web.UI.WebControls.Literal litEditPatientPhone;
        protected global::System.Web.UI.WebControls.Literal litEditSymptoms;
        protected global::System.Web.UI.WebControls.TextBox txtDoctorNotes;
        protected global::System.Web.UI.WebControls.Button btnSavePrescription;
        protected global::System.Web.UI.WebControls.Button btnMarkCancelled;
        protected global::System.Web.UI.WebControls.Label lblRecordCount;
        protected global::System.Web.UI.WebControls.GridView gvDoctorAppointments;
    }
}
