using System;
using System.Web.UI;
using ClinicManagement.Helpers;

namespace ClinicManagement
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ConfigureNavigation();
            }
        }

        private void ConfigureNavigation()
        {
            if (SessionHelper.IsLoggedIn)
            {
                phGuestNav.Visible = false;
                phUserNav.Visible = true;
                litUserName.Text = Server.HtmlEncode(SessionHelper.FullName);
                litUserRole.Text = Server.HtmlEncode(SessionHelper.Role);

                if (SessionHelper.IsPatient)
                {
                    phPatientNav.Visible = true;
                    phDoctorNav.Visible = false;
                }
                else if (SessionHelper.IsDoctor || SessionHelper.IsAdmin)
                {
                    phPatientNav.Visible = false;
                    phDoctorNav.Visible = true;
                }
            }
            else
            {
                phGuestNav.Visible = true;
                phUserNav.Visible = false;
                phPatientNav.Visible = false;
                phDoctorNav.Visible = false;
            }
        }
    }
}

