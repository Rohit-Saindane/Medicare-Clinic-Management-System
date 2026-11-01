namespace ClinicManagement
{
    public partial class BookAppointment
    {
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Literal litSuccessMsg;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litErrorMsg;
        protected global::System.Web.UI.WebControls.DropDownList ddlSpecialization;
        protected global::System.Web.UI.WebControls.DropDownList ddlDoctor;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvDoctor;
        protected global::System.Web.UI.WebControls.Panel pnlDoctorDetails;
        protected global::System.Web.UI.WebControls.Literal litDocDetails;
        protected global::System.Web.UI.WebControls.Literal litDocRoom;
        protected global::System.Web.UI.WebControls.Literal litDocFee;
        protected global::System.Web.UI.WebControls.Literal litDocDays;
        protected global::System.Web.UI.WebControls.TextBox txtAppointmentDate;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvDate;
        protected global::System.Web.UI.WebControls.DropDownList ddlTimeSlot;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvTimeSlot;
        protected global::System.Web.UI.WebControls.TextBox txtSymptoms;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvSymptoms;
        protected global::System.Web.UI.WebControls.Button btnConfirmBooking;
    }
}

