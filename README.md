# 🏥 MediCare — Clinic Appointment & Patient Record Management System

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET](https://img.shields.io/badge/ASP.NET-Web%20Forms-blue?style=for-the-badge&logo=microsoft)](https://learn.microsoft.com/en-us/aspnet/web-forms/)
[![C#](https://img.shields.io/badge/Language-C%23%205.0-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Database](https://img.shields.io/badge/Database-SQL%20Server%20%2F%20XML-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![UI Framework](https://img.shields.io/badge/UI-Bootstrap%205%20%2B%20FontAwesome%206-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

> **Academic Target**: B.Tech 5th / 6th Semester — *.NET & Web Technologies Practical Lab / Project Presentation*  
> **Author & Developer**: [Rohit Saindane](https://github.com/Rohit-Saindane) (`rohitsaindane36@gmail.com`)

---

## 📋 Table of Contents
1. [Project Overview](#-project-overview)
2. [Key Capabilities & Workflows](#-key-capabilities--workflows)
3. [System Architecture](#-system-architecture)
4. [Dual-Engine Data Access (ADO.NET)](#-dual-engine-data-access-adonet)
5. [Database Schema & ER Model](#-database-schema--er-model)
6. [Pre-configured Demo Accounts](#-pre-configured-demo-accounts)
7. [Repository File Structure](#-repository-file-structure)
8. [Setup & Running Locally](#-setup--running-locally)
9. [Free Cloud Hosting Guide (MonsterASP.NET)](#-free-cloud-hosting-guide-monsteraspnet)
10. [B.Tech Viva & Interview Cheat Sheet](#-btech-viva--interview-cheat-sheet)

---

## 🌟 Project Overview

**MediCare Clinic Management System** is a full-featured, intermediate-to-advanced healthcare web application developed using **ASP.NET Web Forms (.NET Framework 4.8)**, **C#**, and **ADO.NET**. 

Designed to overcome waiting-room congestion and fragmented medical records in outpatient clinics, MediCare delivers an end-to-end digital clinical workflow across three distinct personas:
- **Patients** discover medical specialists, check availability, book conflict-free appointments, and review prescription notes.
- **Doctors** inspect patient complaints in real-time, diagnose symptoms, issue prescriptions, and update consultation statuses.
- **Clinic Administrators** onboard licensed physicians, oversee clinic-wide statistics, and manage medical staff profiles.

---

## 🚀 Key Capabilities & Workflows

### 1. 🧑‍🦱 Patient Portal
- **Self-Registration & Authentication**: Secure sign-up with client-side and server-side validation.
- **Live Doctor Directory**: Real-time browsing of specialists complete with qualifications, room numbers, experience years, patient counts, and consultation fees.
- **Conflict-Free Smart Booking**: Real-time validation algorithm prevents duplicate bookings for the same doctor and time slot.
- **Appointment History & Status Tracker**: Filterable dashboard with visual status indicators (*Pending*, *Confirmed*, *Completed*, *Cancelled*).
- **Digital Medical Advice**: Direct access to doctor consultation notes and medication regimens.
- **Self-Service Cancellation**: Allows patients to release booked slots if unable to attend.

### 2. 👨‍⚕️ Doctor / Clinical Specialist Portal
- **Live Consultation Queue**: Automatic filtering of incoming appointments by date and status (*Today's Visits*, *Confirmed*, *Pending*, *Completed*).
- **Patient History & Complaint Inspection**: Review recorded symptoms prior to consultation.
- **Interactive Prescription Pad**: In-page clinical note editor allowing doctors to prescribe medications and mark consultations as *Completed*.

### 3. 🛡️ Clinic Administrator Portal
- **Executive Analytics Dashboard**: Live metrics for Total Doctors, Registered Patients, Active Bookings, and Completed Consultations.
- **Doctor Onboarding System**: Atomic, dual-table registration that simultaneously provisions secure authentication credentials in `Users` and clinical metadata in `Doctors`.
- **Medical Staff Management**: Directory oversight with cascading delete safeguards.

---

## 🏗️ System Architecture

MediCare is engineered following a clean **3-Tier Enterprise Architecture**:

```
┌─────────────────────────────────────────────────────────────┐
│                 PRESENTATION TIER (UI / Web)                │
│   Site.Master | Default.aspx | Login.aspx | Register.aspx   │
│   BookAppointment.aspx | MyAppointments.aspx                │
│   DoctorDashboard.aspx | ManageDoctors.aspx                 │
│   [Bootstrap 5 + FontAwesome 6 + CSS3 Flex/Grid System]     │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│             BUSINESS LOGIC & SECURITY HELPERS               │
│   SessionHelper.cs  — State, role authorization & auth guards│
│   DBHelper.cs       — Query sanitation, parameter binding   │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│           ADO.NET DUAL-ENGINE DATA ACCESS LAYER             │
│   Connected Model:    SqlConnection, SqlCommand, SqlReader  │
│   Disconnected Model: SqlDataAdapter, DataSet, DataTable    │
│   ───────────────────────────────────────────────────────   │
│   PRIMARY ENGINE:      SQL Server (LocalDB / Express / Azure)│
│   AUTONOMOUS ENGINE:   Self-Contained XML / DataSet Fallback │
│                        (Zero-configuration, instant run)    │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔄 Dual-Engine Data Access (ADO.NET)

To ensure this project runs **flawlessly in any examination, laboratory, or cloud environment** without failing due to missing SQL Server instances:

1. **Primary Engine (SQL Server)**:
   - Connects to SQL Server via `System.Data.SqlClient`.
   - Uses strict parameterized queries (`@Param`) to prevent SQL Injection.
2. **Autonomous Fallback Engine (`App_Data/ClinicDB.xml`)**:
   - If SQL Server is offline or unreachable (e.g. Network Error 40 on lab machines), `DBHelper.cs` transparently switches to the embedded XML / `DataTable` engine.
   - All `SELECT`, `INSERT`, `UPDATE`, and `DELETE` queries run against the in-memory `DataSet` and persist back to XML with atomic thread safety (`lock`).
   - **Result**: Zero runtime crashes, zero external database setup required during evaluation!

---

## 🗄️ Database Schema & ER Model

MediCare uses a 3NF normalized schema designed for high relational integrity:

```sql
                      ┌──────────────────────┐
                      │        USERS         │
                      ├──────────────────────┤
                      │ PK  UserId           │
                      │     FullName         │
                      │     Email (UNIQUE)   │
                      │     Password         │
                      │     Phone            │
                      │     Role             │
                      │     CreatedAt        │
                      └──────────┬───────────┘
                                 │
         ┌───────────────────────┴───────────────────────┐
         │ 1:1                                           │ 1:N
┌────────▼─────────────┐                       ┌─────────▼────────────┐
│       DOCTORS        │                       │     APPOINTMENTS     │
├──────────────────────┤                       ├──────────────────────┤
│ PK  DoctorId         │ 1                   N │ PK  AppointmentId    │
│ FK  UserId           │───────────────────────│ FK  DoctorId         │
│     DoctorName       │                       │ FK  PatientId        │
│     Specialization   │                       │     AppointmentDate  │
│     Qualification    │                       │     TimeSlot         │
│     ExperienceYears  │                       │     Symptoms         │
│     ConsultationFee  │                       │     Status           │
│     RoomNumber       │                       │     DoctorNotes      │
│     AvailableDays    │                       │     CreatedAt        │
└──────────────────────┘                       └──────────────────────┘
```

---

## 🔑 Pre-configured Demo Accounts

The project comes pre-seeded with sample data across all user roles:

| Role | Email Address | Password | Role Description & Assignments |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@clinic.com` | `Admin@123` | Master clinic administration & staff onboarding |
| **Doctor** | `dr.sharma@clinic.com` | `Doctor@123` | Cardiologist — Room 101, 12 yrs exp, ₹800 fee |
| **Doctor** | `dr.priya@clinic.com` | `Doctor@123` | Dermatologist — Room 204, 8 yrs exp, ₹600 fee |
| **Doctor** | `dr.rahul@clinic.com` | `Doctor@123` | General Physician — Room 105, 10 yrs exp, ₹500 fee |
| **Patient** | `amit@example.com` | `Patient@123` | Pre-booked appointments with clinical history |
| **Patient** | `neha@example.com` | `Patient@123` | Registered patient account |

> 💡 **Quick Login**: The `Login.aspx` page includes 1-click credential filler buttons for lightning-fast lab demonstrations!

---

## 📁 Repository File Structure

```text
d:\NET Project\
│
├── App_Data/
│   └── ClinicDB.xml                # Embedded self-contained XML database engine
│
├── Helpers/
│   ├── DBHelper.cs                 # Central ADO.NET dual-engine database helper
│   └── SessionHelper.cs            # Session management and role-based access control
│
├── Properties/
│   └── AssemblyInfo.cs             # Application metadata and assembly versioning
│
├── BookAppointment.aspx            # Patient appointment booking with conflict validation
├── BookAppointment.aspx.cs         # Code-behind logic (ADO.NET slot checks & booking)
│
├── Default.aspx                    # Homepage & Medical Specialists catalog
├── Default.aspx.cs                 # Code-behind logic (Specialists data-binding)
│
├── DoctorDashboard.aspx            # Doctor consultation queue & prescription pad
├── DoctorDashboard.aspx.cs         # Code-behind logic (Status updates & clinical notes)
│
├── Login.aspx                      # Multi-role authentication page with 1-click demo buttons
├── Login.aspx.cs                   # Code-behind logic (Connected ADO.NET authentication)
│
├── Logout.aspx                     # Session termination & clean redirection
├── Logout.aspx.cs                  # Session abandons & cache clearing
│
├── ManageDoctors.aspx              # Admin dashboard & doctor onboarding form
├── ManageDoctors.aspx.cs           # Code-behind logic (Staff management & statistics)
│
├── MyAppointments.aspx             # Patient appointment history & prescription viewer
├── MyAppointments.aspx.cs          # Code-behind logic (Appointment cancellation & tracking)
│
├── Register.aspx                   # New patient self-registration
├── Register.aspx.cs                # Code-behind logic (Account creation & validation)
│
├── Site.Master                     # Master page (Navigation bar, styling, minimal footer)
├── Site.Master.cs                  # Dynamic navbar state based on active user session
│
├── ClinicDB.sql                    # Production MS SQL Server database generation script
├── ClinicManagement.csproj         # Visual Studio C# Project configuration
├── ClinicManagement.sln            # Visual Studio Solution file
├── Web.config                      # IIS handlers, modules, and database connection strings
└── README.md                       # Comprehensive documentation & Viva Q&A
```

---

## 💻 Setup & Running Locally

### Option A: Open with Visual Studio (Recommended)
1. Clone the repository:
   ```bash
   git clone https://github.com/Rohit-Saindane/Medicare-Clinic-Management-System.git
   ```
2. Open `ClinicManagement.sln` in **Visual Studio 2022 / 2019**.
3. (Optional) Run `ClinicDB.sql` in SQL Server Management Studio (SSMS). If skipped, the system automatically uses the embedded XML engine with zero errors!
4. Right-click `Default.aspx` $\rightarrow$ select **"Set As Start Page"**.
5. Press **F5** or **Ctrl + F5** to run in IIS Express.

### Option B: Build via MSBuild / Roslyn Command Line
```powershell
# Compile the web application library
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\Roslyn\csc.exe" `
  /target:library `
  /out:bin\ClinicManagement.dll `
  /r:System.dll,System.Core.dll,System.Data.dll,System.Web.dll,System.Xml.dll,System.Configuration.dll `
  /recurse:*.cs

# Serve with IIS Express
& "C:\Program Files\IIS Express\iisexpress.exe" /path:"$PWD" /port:8080
```
Then visit `http://localhost:8080/Default.aspx`.

---

## ☁️ Free Cloud Hosting Guide (MonsterASP.NET)

You can host this project 100% free with no credit card required:

1. Create a free account on [MonsterASP.NET](https://www.monsterasp.net/).
2. Create a new website with runtime configured to **.NET Framework 4.8**.
3. Open the **File Manager** $\rightarrow$ navigate into `wwwroot/`.
4. Upload `ClinicManagement_Deploy.zip` (or upload the `.aspx` files, `Web.config`, `Site.Master`, `bin/`, and `App_Data/`).
5. Extract the ZIP in `wwwroot/`.
6. Visit your free domain (e.g. `https://your-site.monsterasp.net/Default.aspx`). The self-contained XML engine runs out of the box!

---

## 🎓 B.Tech Viva & Interview Cheat Sheet

### 1. What is ADO.NET and how is it used in this project?
> **Answer**: ADO.NET (ActiveX Data Objects .NET) is Microsoft’s data access architecture in .NET. In this project, all database operations are decoupled inside `Helpers/DBHelper.cs` using core ADO.NET classes: `SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlDataAdapter`, and `DataTable`.

### 2. What is the difference between Connected and Disconnected Architecture in ADO.NET?
> **Answer**:
> - **Connected Architecture**: Requires a live, continuous database connection while reading records. Implemented using **`SqlDataReader`** (e.g. `Login.aspx.cs` for fast, lightweight, forward-only stream authentication).
> - **Disconnected Architecture**: Connects, fetches the data into client memory (`DataTable`/`DataSet`), and immediately releases the connection. Implemented using **`SqlDataAdapter`** (e.g. `DoctorDashboard.aspx.cs` and `Default.aspx.cs` to bind data to UI controls).

### 3. Explain `ExecuteNonQuery`, `ExecuteScalar`, and `ExecuteReader`.
> **Answer**:
> - `ExecuteNonQuery()`: Executes statements that mutate state (`INSERT`, `UPDATE`, `DELETE`) and returns the number of rows affected.
> - `ExecuteScalar()`: Executes a query and returns only the single value in the first column of the first row (used in `SELECT COUNT(*)` for conflict checking).
> - `ExecuteReader()`: Returns a forward-only `SqlDataReader` stream for reading complete row sets sequentially.

### 4. How does the system prevent SQL Injection attacks?
> **Answer**: By strictly using **Parameterized Queries** (`cmd.Parameters.Add(new SqlParameter(...))`). Parameters treat user inputs strictly as literal values instead of executable SQL fragments, eliminating SQL injection vulnerability.

### 5. Why is `if (!IsPostBack)` checked inside `Page_Load`?
> **Answer**: `IsPostBack` indicates whether the page is being rendered for the first time or posted back to the server due to a user control event (such as a dropdown selection or button click). Checking `!IsPostBack` ensures heavy database calls and control initializations occur only once, preserving ViewState across round trips.

### 6. How does the slot conflict-prevention algorithm work?
> **Answer**: Before saving an appointment, `BookAppointment.aspx.cs` executes an ADO.NET scalar validation:
> ```sql
> SELECT COUNT(*) FROM Appointments 
> WHERE DoctorId = @DocId AND AppointmentDate = @AppDate 
>   AND TimeSlot = @TimeSlot AND Status <> 'Cancelled'
> ```
> If the count is greater than 0, the booking is rejected with an on-screen warning, guaranteeing zero double-booking.

### 7. What is a Master Page (`Site.Master`) and why is it used?
> **Answer**: A Master Page provides a uniform layout, navigation bar, footer, and styling across all web forms. Content pages (`.aspx`) inject their unique content inside `<asp:Content ContentPlaceHolderID="..." runat="server">` blocks without duplicating header and footer code.

### 8. What is the difference between `Session` and `ViewState`?
> **Answer**:
> - `Session`: Stored on the **server**, persists across multiple pages throughout a user's entire visit (used in `SessionHelper.cs` to track logged-in user credentials and roles).
> - `ViewState`: Stored on the **client** inside a hidden HTML input (`__VIEWSTATE`), preserves control properties between round trips on the **same single page**.

---

## 📜 License
This project is open-source and available under the [MIT License](LICENSE).
Created for educational purposes by **Rohit Saindane**.
