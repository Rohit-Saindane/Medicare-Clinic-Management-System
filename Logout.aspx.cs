using System;
using System.Web.UI;
using ClinicManagement.Helpers;

namespace ClinicManagement
{
    public partial class Logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionHelper.Clear();
            Response.Redirect("~/Login.aspx");
        }
    }
}

