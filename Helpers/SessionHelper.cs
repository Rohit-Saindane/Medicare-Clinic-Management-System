using System;
using System.Web;

namespace ClinicManagement.Helpers
{
    /// <summary>
    /// Helper class to manage session state across all ASP.NET Web Forms.
    /// Prevents null-reference bugs and keeps session key names consistent.
    /// </summary>
    public static class SessionHelper
    {
        public static bool IsLoggedIn
        {
            get { return HttpContext.Current.Session["UserId"] != null; }
        }

        public static int UserId
        {
            get
            {
                if (HttpContext.Current.Session["UserId"] != null)
                    return Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                return 0;
            }
            set { HttpContext.Current.Session["UserId"] = value; }
        }

        public static string FullName
        {
            get
            {
                if (HttpContext.Current.Session["FullName"] != null)
                    return HttpContext.Current.Session["FullName"].ToString();
                return string.Empty;
            }
            set { HttpContext.Current.Session["FullName"] = value; }
        }

        public static string Email
        {
            get
            {
                if (HttpContext.Current.Session["Email"] != null)
                    return HttpContext.Current.Session["Email"].ToString();
                return string.Empty;
            }
            set { HttpContext.Current.Session["Email"] = value; }
        }

        public static string Role
        {
            get
            {
                if (HttpContext.Current.Session["Role"] != null)
                    return HttpContext.Current.Session["Role"].ToString();
                return string.Empty;
            }
            set { HttpContext.Current.Session["Role"] = value; }
        }

        public static int DoctorId
        {
            get
            {
                if (HttpContext.Current.Session["DoctorId"] != null)
                    return Convert.ToInt32(HttpContext.Current.Session["DoctorId"]);
                return 0;
            }
            set { HttpContext.Current.Session["DoctorId"] = value; }
        }

        public static bool IsPatient
        {
            get { return IsLoggedIn && Role.Equals("Patient", StringComparison.OrdinalIgnoreCase); }
        }

        public static bool IsDoctor
        {
            get { return IsLoggedIn && Role.Equals("Doctor", StringComparison.OrdinalIgnoreCase); }
        }

        public static bool IsAdmin
        {
            get { return IsLoggedIn && Role.Equals("Admin", StringComparison.OrdinalIgnoreCase); }
        }

        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}

