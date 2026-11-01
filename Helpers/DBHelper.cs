using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace ClinicManagement.Helpers
{
    /// <summary>
    /// Dual-Engine ADO.NET Database Helper (C# 5 compatible for ASP.NET legacy runtime build provider).
    /// Primary: SQL Server.
    /// Fallback: ADO.NET Disconnected XML/DataTable Store (App_Data/ClinicDB.xml).
    /// </summary>
    public static class DBHelper
    {
        private static bool? _isSqlServerAvailable = null;
        private static readonly object _lock = new object();
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ClinicDB"] != null 
            ? ConfigurationManager.ConnectionStrings["ClinicDB"].ConnectionString 
            : "Data Source=.;Initial Catalog=ClinicDB;Integrated Security=True;Connect Timeout=2";

        private static string XmlFilePath
        {
            get
            {
                string appData = HttpContext.Current != null
                    ? HttpContext.Current.Server.MapPath("~/App_Data")
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

                if (!Directory.Exists(appData))
                {
                    Directory.CreateDirectory(appData);
                }
                return Path.Combine(appData, "ClinicDB.xml");
            }
        }

        public static bool CheckSqlServer()
        {
            if (_isSqlServerAvailable.HasValue)
                return _isSqlServerAvailable.Value;

            lock (_lock)
            {
                if (_isSqlServerAvailable.HasValue)
                    return _isSqlServerAvailable.Value;

                try
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
                    builder.ConnectTimeout = 2; // 2-second timeout to check fast

                    using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                    {
                        conn.Open();
                        _isSqlServerAvailable = true;
                        return true;
                    }
                }
                catch
                {
                    _isSqlServerAvailable = false;
                    return false;
                }
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        #region ExecuteNonQuery
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            if (CheckSqlServer())
            {
                try
                {
                    using (SqlConnection conn = GetConnection())
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            if (parameters != null) cmd.Parameters.AddRange(parameters);
                            conn.Open();
                            return cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    _isSqlServerAvailable = false;
                }
            }

            return ExecuteXmlNonQuery(query, parameters);
        }
        #endregion

        #region ExecuteScalar
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            if (CheckSqlServer())
            {
                try
                {
                    using (SqlConnection conn = GetConnection())
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            if (parameters != null) cmd.Parameters.AddRange(parameters);
                            conn.Open();
                            return cmd.ExecuteScalar();
                        }
                    }
                }
                catch
                {
                    _isSqlServerAvailable = false;
                }
            }

            return ExecuteXmlScalar(query, parameters);
        }
        #endregion

        #region ExecuteDataTable
        public static DataTable ExecuteDataTable(string query, SqlParameter[] parameters = null)
        {
            if (CheckSqlServer())
            {
                try
                {
                    using (SqlConnection conn = GetConnection())
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            if (parameters != null) cmd.Parameters.AddRange(parameters);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                return dt;
                            }
                        }
                    }
                }
                catch
                {
                    _isSqlServerAvailable = false;
                }
            }

            return ExecuteXmlDataTable(query, parameters);
        }
        #endregion

        #region ExecuteReader
        public static IDataReader ExecuteReader(string query, SqlParameter[] parameters = null)
        {
            if (CheckSqlServer())
            {
                try
                {
                    SqlConnection conn = GetConnection();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch
                {
                    _isSqlServerAvailable = false;
                }
            }

            DataTable dt = ExecuteXmlDataTable(query, parameters);
            return dt.CreateDataReader();
        }
        #endregion

        #region XML/DataTable Storage Engine Implementation
        private static DataSet LoadDataSet()
        {
            DataSet ds = new DataSet("ClinicData");
            string path = XmlFilePath;

            if (File.Exists(path))
            {
                ds.ReadXml(path);
            }
            else
            {
                InitializeDefaultXmlData(ds, path);
            }
            return ds;
        }

        private static void SaveDataSet(DataSet ds)
        {
            lock (_lock)
            {
                ds.WriteXml(XmlFilePath, XmlWriteMode.WriteSchema);
            }
        }

        private static void InitializeDefaultXmlData(DataSet ds, string path)
        {
            DataTable dtUsers = ds.Tables.Add("Users");
            dtUsers.Columns.Add("UserId", typeof(int));
            dtUsers.Columns.Add("FullName", typeof(string));
            dtUsers.Columns.Add("Email", typeof(string));
            dtUsers.Columns.Add("Password", typeof(string));
            dtUsers.Columns.Add("Phone", typeof(string));
            dtUsers.Columns.Add("Role", typeof(string));
            dtUsers.Columns.Add("CreatedAt", typeof(DateTime));

            DataTable dtDocs = ds.Tables.Add("Doctors");
            dtDocs.Columns.Add("DoctorId", typeof(int));
            dtDocs.Columns.Add("UserId", typeof(int));
            dtDocs.Columns.Add("DoctorName", typeof(string));
            dtDocs.Columns.Add("Specialization", typeof(string));
            dtDocs.Columns.Add("Qualification", typeof(string));
            dtDocs.Columns.Add("ExperienceYears", typeof(int));
            dtDocs.Columns.Add("ConsultationFee", typeof(decimal));
            dtDocs.Columns.Add("RoomNumber", typeof(string));
            dtDocs.Columns.Add("AvailableDays", typeof(string));
            dtDocs.Columns.Add("PatientCount", typeof(string));
            dtDocs.Columns.Add("Rating", typeof(string));

            DataTable dtApp = ds.Tables.Add("Appointments");
            dtApp.Columns.Add("AppointmentId", typeof(int));
            dtApp.Columns.Add("PatientId", typeof(int));
            dtApp.Columns.Add("DoctorId", typeof(int));
            dtApp.Columns.Add("AppointmentDate", typeof(DateTime));
            dtApp.Columns.Add("TimeSlot", typeof(string));
            dtApp.Columns.Add("Symptoms", typeof(string));
            dtApp.Columns.Add("Status", typeof(string));
            dtApp.Columns.Add("DoctorNotes", typeof(string));
            dtApp.Columns.Add("CreatedAt", typeof(DateTime));

            // Seed Users
            dtUsers.Rows.Add(1, "System Administrator", "admin@clinic.com", "Admin@123", "9876543210", "Admin", DateTime.Now);
            dtUsers.Rows.Add(2, "Dr. Rajesh Sharma", "dr.sharma@clinic.com", "Doctor@123", "9876543211", "Doctor", DateTime.Now);
            dtUsers.Rows.Add(3, "Dr. Priya Nair", "dr.priya@clinic.com", "Doctor@123", "9876543212", "Doctor", DateTime.Now);
            dtUsers.Rows.Add(4, "Dr. Rahul Verma", "dr.rahul@clinic.com", "Doctor@123", "9876543213", "Doctor", DateTime.Now);
            dtUsers.Rows.Add(5, "Amit Patel", "amit@example.com", "Patient@123", "9876543220", "Patient", DateTime.Now);
            dtUsers.Rows.Add(6, "Neha Singh", "neha@example.com", "Patient@123", "9876543221", "Patient", DateTime.Now);

            // Seed Doctors
            dtDocs.Rows.Add(1, 2, "Dr. Rajesh Sharma", "Cardiology", "MBBS, MD, DM (Cardiology) - AIIMS Gold Medalist", 14, 800, "Chamber 101", "Mon, Wed, Fri (10:00 AM - 04:00 PM)", "1,850+", "4.9");
            dtDocs.Rows.Add(2, 3, "Dr. Priya Nair", "Dermatology", "MBBS, MD (Dermatology & Cosmetology)", 9, 600, "Chamber 204", "Tue, Thu, Sat (11:00 AM - 05:00 PM)", "1,420+", "4.8");
            dtDocs.Rows.Add(3, 4, "Dr. Rahul Verma", "General Medicine", "MBBS, MD (Internal Medicine), FACP", 11, 500, "Chamber 105", "Mon - Sat (09:00 AM - 02:00 PM)", "2,100+", "4.9");

            // Seed Appointments
            dtApp.Rows.Add(1, 5, 1, DateTime.Today.AddDays(1), "10:00 AM - 10:30 AM", "Mild chest tightness during exercise, occasional fatigue", "Confirmed", "Advised resting ECG and routine lipid profile checkup.", DateTime.Now);
            dtApp.Rows.Add(2, 6, 2, DateTime.Today.AddDays(2), "11:30 AM - 12:00 PM", "Dry skin patches and mild allergy on forearm", "Pending", "", DateTime.Now);
            dtApp.Rows.Add(3, 5, 3, DateTime.Today, "09:30 AM - 10:00 AM", "High fever, sore throat and joint aches for 2 days", "Completed", "Diagnosed acute seasonal viral fever. Prescribed Paracetamol 650mg TDS, warm saline gargles, and 3 days complete bed rest.", DateTime.Now);

            ds.WriteXml(path, XmlWriteMode.WriteSchema);
        }

        private static Dictionary<string, object> ParamsToDict(SqlParameter[] parameters)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (parameters != null)
            {
                foreach (SqlParameter p in parameters)
                {
                    dict[p.ParameterName.TrimStart('@')] = p.Value;
                }
            }
            return dict;
        }

        private static object ExecuteXmlScalar(string query, SqlParameter[] parameters)
        {
            DataSet ds = LoadDataSet();
            Dictionary<string, object> p = ParamsToDict(parameters);

            if (query.IndexOf("INSERT INTO Users", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ExecuteXmlNonQuery(query, parameters);
            }

            if (query.IndexOf("COUNT(*)", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (query.IndexOf("Doctors", StringComparison.OrdinalIgnoreCase) >= 0 && query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    return ds.Tables["Doctors"] != null ? ds.Tables["Doctors"].Rows.Count : 0;
                }
                if (query.IndexOf("Role = 'Patient'", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return ds.Tables["Users"] != null ? ds.Tables["Users"].Select("Role = 'Patient'").Length : 0;
                }
                if (query.IndexOf("Status = 'Completed'", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return ds.Tables["Appointments"] != null ? ds.Tables["Appointments"].Select("Status = 'Completed'").Length : 0;
                }
                if (query.IndexOf("AppointmentDate", StringComparison.OrdinalIgnoreCase) >= 0 && query.IndexOf("GETDATE()", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (ds.Tables["Appointments"] == null) return 0;
                    int count = 0;
                    DateTime today = DateTime.Today;
                    foreach (DataRow r in ds.Tables["Appointments"].Rows)
                    {
                        DateTime d;
                        if (DateTime.TryParse(r["AppointmentDate"].ToString(), out d) && d.Date == today)
                            count++;
                    }
                    return count;
                }
                if (query.IndexOf("Users", StringComparison.OrdinalIgnoreCase) >= 0 && p.ContainsKey("Email"))
                {
                    string email = (p["Email"] != null) ? p["Email"].ToString() : "";
                    if (ds.Tables["Users"] == null) return 0;
                    DataRow[] rows = ds.Tables["Users"].Select(string.Format("Email = '{0}'", email.Replace("'", "''")));
                    return rows.Length;
                }
                if (query.IndexOf("Appointments", StringComparison.OrdinalIgnoreCase) >= 0 && p.ContainsKey("DocId") && p.ContainsKey("TimeSlot"))
                {
                    if (ds.Tables["Appointments"] == null) return 0;
                    int docId = Convert.ToInt32(p["DocId"]);
                    string slot = p["TimeSlot"].ToString();
                    DateTime date = Convert.ToDateTime(p["AppDate"]).Date;

                    int count = 0;
                    foreach (DataRow r in ds.Tables["Appointments"].Rows)
                    {
                        if (Convert.ToInt32(r["DoctorId"]) == docId &&
                            Convert.ToDateTime(r["AppointmentDate"]).Date == date &&
                            r["TimeSlot"].ToString() == slot &&
                            r["Status"].ToString() != "Cancelled")
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }

            if (query.IndexOf("SELECT DoctorId FROM Doctors WHERE UserId", StringComparison.OrdinalIgnoreCase) >= 0 && p.ContainsKey("UserId"))
            {
                int uId = Convert.ToInt32(p["UserId"]);
                if (ds.Tables["Doctors"] != null)
                {
                    foreach (DataRow r in ds.Tables["Doctors"].Rows)
                    {
                        if (Convert.ToInt32(r["UserId"]) == uId)
                            return r["DoctorId"];
                    }
                }
                return null;
            }

            return 0;
        }

        private static int ExecuteXmlNonQuery(string query, SqlParameter[] parameters)
        {
            DataSet ds = LoadDataSet();
            Dictionary<string, object> p = ParamsToDict(parameters);

            // 1. Insert into Users
            if (query.IndexOf("INSERT INTO Users", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable dt = ds.Tables["Users"];
                int newId = 1;
                foreach (DataRow r in dt.Rows)
                {
                    int id = Convert.ToInt32(r["UserId"]);
                    if (id >= newId) newId = id + 1;
                }
                string role = query.IndexOf("'Doctor'", StringComparison.OrdinalIgnoreCase) >= 0 ? "Doctor" : "Patient";
                dt.Rows.Add(newId, p["FullName"], p["Email"], p["Password"], p.ContainsKey("Phone") ? p["Phone"] : "N/A", role, DateTime.Now);
                SaveDataSet(ds);
                return newId;
            }

            // 2. Insert into Doctors
            if (query.IndexOf("INSERT INTO Doctors", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable dt = ds.Tables["Doctors"];
                int newId = 1;
                foreach (DataRow r in dt.Rows)
                {
                    int id = Convert.ToInt32(r["DoctorId"]);
                    if (id >= newId) newId = id + 1;
                }
                dt.Rows.Add(newId, p["UserId"], p["DoctorName"], p["Spec"], p["Qual"], p["Exp"], p["Fee"], p["Room"], p["Days"], "0", "5.0");
                SaveDataSet(ds);
                return 1;
            }

            // 3. Insert into Appointments
            if (query.IndexOf("INSERT INTO Appointments", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable dt = ds.Tables["Appointments"];
                int newId = 1;
                foreach (DataRow r in dt.Rows)
                {
                    int id = Convert.ToInt32(r["AppointmentId"]);
                    if (id >= newId) newId = id + 1;
                }
                dt.Rows.Add(newId, p["PatientId"], p["DocId"], Convert.ToDateTime(p["AppDate"]), p["TimeSlot"], p["Symptoms"], "Confirmed", "", DateTime.Now);
                SaveDataSet(ds);
                return 1;
            }

            // 4. Update Appointments (Prescription / Completed / Cancelled)
            if (query.IndexOf("UPDATE Appointments", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable dt = ds.Tables["Appointments"];
                int appId = p.ContainsKey("AppId") ? Convert.ToInt32(p["AppId"]) : 0;
                int affected = 0;

                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToInt32(r["AppointmentId"]) == appId)
                    {
                        if (p.ContainsKey("Notes"))
                        {
                            r["DoctorNotes"] = p["Notes"];
                            r["Status"] = "Completed";
                        }
                        else if (query.IndexOf("Status = 'Cancelled'", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            r["Status"] = "Cancelled";
                        }
                        affected++;
                    }
                }
                if (affected > 0) SaveDataSet(ds);
                return affected;
            }

            // 5. Delete Doctor / User
            if (query.IndexOf("DELETE FROM Users", StringComparison.OrdinalIgnoreCase) >= 0 && p.ContainsKey("UserId"))
            {
                int uId = Convert.ToInt32(p["UserId"]);
                DataTable dtUsers = ds.Tables["Users"];
                DataTable dtDocs = ds.Tables["Doctors"];

                for (int i = dtDocs.Rows.Count - 1; i >= 0; i--)
                {
                    if (Convert.ToInt32(dtDocs.Rows[i]["UserId"]) == uId) dtDocs.Rows.RemoveAt(i);
                }
                for (int i = dtUsers.Rows.Count - 1; i >= 0; i--)
                {
                    if (Convert.ToInt32(dtUsers.Rows[i]["UserId"]) == uId) dtUsers.Rows.RemoveAt(i);
                }
                SaveDataSet(ds);
                return 1;
            }

            return 0;
        }

        private static DataTable ExecuteXmlDataTable(string query, SqlParameter[] parameters)
        {
            DataSet ds = LoadDataSet();
            Dictionary<string, object> p = ParamsToDict(parameters);

            // 1. Doctors List
            if (query.IndexOf("FROM Doctors", StringComparison.OrdinalIgnoreCase) >= 0 && query.IndexOf("INNER JOIN", StringComparison.OrdinalIgnoreCase) < 0)
            {
                DataTable dt = ds.Tables["Doctors"].Copy();
                if (p.ContainsKey("DocId"))
                {
                    int docId = Convert.ToInt32(p["DocId"]);
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToInt32(dt.Rows[i]["DoctorId"]) != docId) dt.Rows.RemoveAt(i);
                    }
                }
                else if (p.ContainsKey("Spec") && p["Spec"] != null && !string.IsNullOrEmpty(p["Spec"].ToString()))
                {
                    string spec = p["Spec"].ToString();
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        if (!dt.Rows[i]["Specialization"].ToString().Equals(spec, StringComparison.OrdinalIgnoreCase))
                            dt.Rows.RemoveAt(i);
                    }
                }
                return dt;
            }

            // 2. Distinct Specializations
            if (query.IndexOf("SELECT DISTINCT Specialization FROM Doctors", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable result = new DataTable();
                result.Columns.Add("Specialization", typeof(string));
                HashSet<string> specs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow r in ds.Tables["Doctors"].Rows)
                {
                    string s = r["Specialization"].ToString();
                    if (!specs.Contains(s))
                    {
                        specs.Add(s);
                        result.Rows.Add(s);
                    }
                }
                return result;
            }

            // 3. User Authentication Query
            if (query.IndexOf("FROM Users", StringComparison.OrdinalIgnoreCase) >= 0 && p.ContainsKey("Email") && p.ContainsKey("Password"))
            {
                string email = (p["Email"] != null) ? p["Email"].ToString() : "";
                string pass = (p["Password"] != null) ? p["Password"].ToString() : "";
                DataTable result = ds.Tables["Users"].Clone();

                foreach (DataRow r in ds.Tables["Users"].Rows)
                {
                    if (r["Email"].ToString().Equals(email, StringComparison.OrdinalIgnoreCase) &&
                        r["Password"].ToString() == pass)
                    {
                        result.ImportRow(r);
                    }
                }
                return result;
            }

            // 4. Appointments Join
            if (query.IndexOf("FROM Appointments", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("AppointmentId", typeof(int));
                dt.Columns.Add("PatientId", typeof(int));
                dt.Columns.Add("DoctorId", typeof(int));
                dt.Columns.Add("PatientName", typeof(string));
                dt.Columns.Add("PatientPhone", typeof(string));
                dt.Columns.Add("DoctorName", typeof(string));
                dt.Columns.Add("Specialization", typeof(string));
                dt.Columns.Add("AppointmentDate", typeof(DateTime));
                dt.Columns.Add("TimeSlot", typeof(string));
                dt.Columns.Add("Symptoms", typeof(string));
                dt.Columns.Add("Status", typeof(string));
                dt.Columns.Add("DoctorNotes", typeof(string));

                DataTable dtApp = ds.Tables["Appointments"];
                DataTable dtUsers = ds.Tables["Users"];
                DataTable dtDocs = ds.Tables["Doctors"];

                if (dtApp != null)
                {
                    foreach (DataRow app in dtApp.Rows)
                    {
                        int appId = Convert.ToInt32(app["AppointmentId"]);
                        int patId = Convert.ToInt32(app["PatientId"]);
                        int docId = Convert.ToInt32(app["DoctorId"]);

                        if (p.ContainsKey("AppId") && appId != Convert.ToInt32(p["AppId"]))
                            continue;

                        if (p.ContainsKey("PatientId") && patId != Convert.ToInt32(p["PatientId"]))
                            continue;

                        if (p.ContainsKey("DocId") && docId != Convert.ToInt32(p["DocId"]))
                            continue;

                        if (p.ContainsKey("Status") && p["Status"] != null && !string.IsNullOrEmpty(p["Status"].ToString()))
                        {
                            if (!app["Status"].ToString().Equals(p["Status"].ToString(), StringComparison.OrdinalIgnoreCase))
                                continue;
                        }

                        if (p.ContainsKey("AppDate"))
                        {
                            DateTime fDate = Convert.ToDateTime(p["AppDate"]).Date;
                            if (Convert.ToDateTime(app["AppointmentDate"]).Date != fDate)
                                continue;
                        }

                        string patName = "Unknown";
                        string patPhone = "";
                        DataRow[] patRows = dtUsers.Select(string.Format("UserId = {0}", patId));
                        if (patRows.Length > 0)
                        {
                            patName = patRows[0]["FullName"].ToString();
                            patPhone = patRows[0]["Phone"].ToString();
                        }

                        string docName = "Specialist";
                        string docSpec = "General";
                        DataRow[] docRows = dtDocs.Select(string.Format("DoctorId = {0}", docId));
                        if (docRows.Length > 0)
                        {
                            docName = docRows[0]["DoctorName"].ToString();
                            docSpec = docRows[0]["Specialization"].ToString();
                        }

                        string docNotes = (app["DoctorNotes"] != null && app["DoctorNotes"] != DBNull.Value) ? app["DoctorNotes"].ToString() : "";

                        dt.Rows.Add(
                            appId,
                            patId,
                            docId,
                            patName,
                            patPhone,
                            docName,
                            docSpec,
                            Convert.ToDateTime(app["AppointmentDate"]),
                            app["TimeSlot"].ToString(),
                            app["Symptoms"].ToString(),
                            app["Status"].ToString(),
                            docNotes
                        );
                    }
                }
                return dt;
            }

            // 5. Manage Doctors View (Doctors + Email)
            if (query.IndexOf("Doctors D", StringComparison.OrdinalIgnoreCase) >= 0 && query.IndexOf("Users U", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("DoctorId", typeof(int));
                dt.Columns.Add("UserId", typeof(int));
                dt.Columns.Add("DoctorName", typeof(string));
                dt.Columns.Add("Specialization", typeof(string));
                dt.Columns.Add("Qualification", typeof(string));
                dt.Columns.Add("ExperienceYears", typeof(int));
                dt.Columns.Add("ConsultationFee", typeof(decimal));
                dt.Columns.Add("RoomNumber", typeof(string));
                dt.Columns.Add("AvailableDays", typeof(string));
                dt.Columns.Add("Email", typeof(string));

                DataTable dtDocs = ds.Tables["Doctors"];
                DataTable dtUsers = ds.Tables["Users"];

                foreach (DataRow d in dtDocs.Rows)
                {
                    int uId = Convert.ToInt32(d["UserId"]);
                    string email = "";
                    DataRow[] uRows = dtUsers.Select(string.Format("UserId = {0}", uId));
                    if (uRows.Length > 0) email = uRows[0]["Email"].ToString();

                    dt.Rows.Add(
                        d["DoctorId"],
                        d["UserId"],
                        d["DoctorName"],
                        d["Specialization"],
                        d["Qualification"],
                        d["ExperienceYears"],
                        d["ConsultationFee"],
                        d["RoomNumber"],
                        d["AvailableDays"],
                        email
                    );
                }
                return dt;
            }

            return new DataTable();
        }
        #endregion
    }
}
