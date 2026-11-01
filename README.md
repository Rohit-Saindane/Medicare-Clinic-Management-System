# 🏥 MediCare - Clinic Appointment & Patient Record Management System
> **Tech Stack**: ASP.NET Web Forms (.NET Framework 4.8), C#, ADO.NET, SQL Server, Bootstrap 5, FontAwesome 6  
> **Target**: B.Tech 5th Semester — .NET & Web Technologies Practical Lab / Project

---

## 📌 Project Overview
**MediCare Clinic Management System** is an intermediate-level web application designed for healthcare clinics to streamline doctor scheduling, eliminate waiting room congestion, and maintain structured patient consultation records.

### 👥 User Roles & Features:
1. **Patient Portal**:
   - Self-registration and login with form validations.
   - Live doctor directory with specialties, qualifications, room numbers, and consultation fees.
   - Conflict-free appointment booking (prevents double-booking same doctor at the same slot).
   - "My Appointments" tracker with real-time status badges (*Pending*, *Confirmed*, *Completed*, *Cancelled*).
   - Doctor's prescription notes and clinical advice view.
   - Self-service appointment cancellation.

2. **Doctor / Specialist Portal**:
   - Dedicated Clinical Queue with dynamic filtering by status (*Confirmed, Pending, Completed, Cancelled*) and date.
   - Detailed patient complaint / symptom inspection.
   - In-page Consultation & Prescription form to record medications and mark consultations as *Completed*.

3. **Admin Dashboard**:
   - Global overview of clinic statistics (Total Doctors, Registered Patients, Today's Appointments, Completed Visits).
   - Onboard new specialist doctors (automatically creates both security credentials in `Users` and medical profile in `Doctors`).
   - Manage and delete staff profiles with cascading integrity.

---

## 🗄️ Database Architecture (`ClinicDB.sql`)

The database consists of 3 normalized relational tables:

```
[Users] (UserId, FullName, Email, Password, Phone, Role, CreatedAt)
   │
   ├── (1:1 / 1:N) ────> [Doctors] (DoctorId, UserId, DoctorName, Specialization, 
   │                                Qualification, ExperienceYears, ConsultationFee, 
   │                                RoomNumber, AvailableDays)
   │
   └── (1:N) ──────────> [Appointments] (AppointmentId, PatientId, DoctorId, 
                                        AppointmentDate, TimeSlot, Symptoms, 
                                        Status, DoctorNotes, CreatedAt)
```

---

## 🔑 Pre-seeded Demo Credentials

Use any of these ready-to-test accounts (or create a new patient account via Register):

| Role | Email | Password | Details |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@clinic.com` | `Admin@123` | Full clinic management & onboarding |
| **Doctor** | `dr.sharma@clinic.com` | `Doctor@123` | Cardiologist (Room 101) |
| **Doctor** | `dr.priya@clinic.com` | `Doctor@123` | Dermatologist (Room 204) |
| **Doctor** | `dr.rahul@clinic.com` | `Doctor@123` | General Medicine (Room 105) |
| **Patient** | `amit@example.com` | `Patient@123` | Existing patient with booked visits |
| **Patient** | `neha@example.com` | `Patient@123` | Existing patient |

*(Note: The login page also features 1-click credential filler buttons for fast demonstration).*

---

## 🚀 Setup & Execution Guide

### Step 1: Initialize Database in SQL Server
1. Open **SQL Server Management Studio (SSMS)** or Visual Studio's **SQL Server Object Explorer**.
2. Connect to your SQL Server instance (e.g. `.` or `localhost` or `(localdb)\MSSQLLocalDB` or `.\SQLEXPRESS`).
3. Open and execute the script:
   ```
   d:\NET Project\ClinicDB.sql
   ```
4. Verify that `ClinicDB` database and sample records are created.

### Step 2: Configure Connection String
Open [Web.config](file:///d:/NET%20Project/Web.config) and verify the connection string matches your local SQL Server instance:
```xml
<connectionStrings>
  <!-- Default for local SQL Server / Windows Authentication: -->
  <add name="ClinicDB" 
       connectionString="Data Source=.;Initial Catalog=ClinicDB;Integrated Security=True;TrustServerCertificate=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```
*(If using SQL Express, change `Data Source=.` to `Data Source=.\SQLEXPRESS`).*

### Step 3: Run the Project in Visual Studio
1. Open `ClinicManagement.sln` in **Visual Studio 2022**.
2. Set `Default.aspx` as the Start Page (Right-click `Default.aspx` $\rightarrow$ *Set As Start Page*).
3. Press **F5** or **Ctrl + F5** to launch in IIS Express.

---

## 🎓 B.Tech Viva & Practical Exam Questions (Cheat Sheet)

### 1. What is ADO.NET and how is it used in this project?
> **Answer**: ADO.NET (ActiveX Data Objects .NET) is Microsoft's data access technology in the .NET Framework that enables communication between front-end web forms and relational databases like SQL Server. In our project, all database operations are encapsulated in `DBHelper.cs` using core ADO.NET classes: `SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlDataAdapter`, and `DataTable`.

### 2. What is the difference between Connected and Disconnected Architecture in ADO.NET?
> **Answer**:
> - **Connected Architecture**: Requires a persistent, active connection while reading data. Implemented via **`SqlDataReader`** (used in `Login.aspx.cs` to authenticate user streams). It is fast, lightweight, and forward-only.
> - **Disconnected Architecture**: Connects to the database, fetches the dataset into client memory, and immediately closes the connection. Implemented via **`SqlDataAdapter`** and **`DataTable`** (used in `Default.aspx.cs`, `MyAppointments.aspx.cs`, and `DoctorDashboard.aspx.cs` to populate `GridView` and `Repeater` controls).

### 3. Difference between `ExecuteNonQuery`, `ExecuteScalar`, and `ExecuteReader`?
> **Answer**:
> - `ExecuteNonQuery()`: Executes queries that modify state (`INSERT`, `UPDATE`, `DELETE`) and returns the **number of rows affected** (integer).
> - `ExecuteScalar()`: Executes a query that returns a **single scalar value** (the first column of the first row), commonly used with aggregates like `COUNT(*)`, `SUM()`, or `SELECT SCOPE_IDENTITY()`.
> - `ExecuteReader()`: Returns a forward-only, read-only **`SqlDataReader`** stream for reading result sets row-by-row.

### 4. How does this project prevent SQL Injection attacks?
> **Answer**: By strictly using **Parameterized Queries** (`cmd.Parameters.Add(new SqlParameter(...))`). User inputs are treated solely as literal parameter values rather than executable SQL commands, completely neutralizing SQL injection attempts.

### 5. Why do we check `if (!IsPostBack)` in `Page_Load`?
> **Answer**: `IsPostBack` indicates whether the page is being rendered for the first time or is being posted back in response to a client event (like a button click or dropdown change). Checking `!IsPostBack` prevents costly database re-fetching and prevents overwriting form control states on user actions.

### 6. How does the conflict-prevention algorithm work in `BookAppointment.aspx`?
> **Answer**: Before running the `INSERT` statement, the system runs an ADO.NET `ExecuteScalar` query checking:
> ```sql
> SELECT COUNT(*) FROM Appointments 
> WHERE DoctorId = @DocId AND AppointmentDate = @AppDate AND TimeSlot = @TimeSlot AND Status <> 'Cancelled'
> ```
> If the count is $> 0$, the request is stopped with an alert to the user, ensuring no doctor is booked twice for the same time slot.

