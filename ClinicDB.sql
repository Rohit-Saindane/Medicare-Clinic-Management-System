-- =====================================================================
-- Clinic Appointment & Patient Record Management System Database Script
-- Compatible with SQL Server 2014 / 2016 / 2017 / 2019 / 2022 / LocalDB / Express
-- =====================================================================

-- 1. Create Database if it doesn't already exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ClinicDB')
BEGIN
    CREATE DATABASE ClinicDB;
END
GO

USE ClinicDB;
GO

-- 2. Drop tables in foreign key order if re-running
IF OBJECT_ID('Appointments', 'U') IS NOT NULL DROP TABLE Appointments;
IF OBJECT_ID('Doctors', 'U') IS NOT NULL DROP TABLE Doctors;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO

-- 3. Users Table (Patients, Doctors, Admins)
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Patient', 'Doctor', 'Admin')),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 4. Doctors Profile Table
CREATE TABLE Doctors (
    DoctorId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    DoctorName NVARCHAR(100) NOT NULL,
    Specialization NVARCHAR(100) NOT NULL,
    Qualification NVARCHAR(100) NOT NULL,
    ExperienceYears INT NOT NULL DEFAULT 1,
    ConsultationFee DECIMAL(10,2) NOT NULL DEFAULT 500.00,
    RoomNumber NVARCHAR(20) NOT NULL,
    AvailableDays NVARCHAR(100) DEFAULT 'Mon - Sat',
    CONSTRAINT FK_Doctors_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

-- 5. Appointments Table
CREATE TABLE Appointments (
    AppointmentId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    AppointmentDate DATE NOT NULL,
    TimeSlot NVARCHAR(50) NOT NULL,
    Symptoms NVARCHAR(500) NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Completed', 'Cancelled')),
    DoctorNotes NVARCHAR(1000) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Appointments_Patient FOREIGN KEY (PatientId) REFERENCES Users(UserId),
    CONSTRAINT FK_Appointments_Doctor FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId)
);
GO

-- 6. Insert Seed Users
-- Passwords shown in plain text for academic demonstration / viva inspection
INSERT INTO Users (FullName, Email, Password, Phone, Role)
VALUES 
('System Administrator', 'admin@clinic.com', 'Admin@123', '9876543210', 'Admin'),
('Dr. Rajesh Sharma', 'dr.sharma@clinic.com', 'Doctor@123', '9876543211', 'Doctor'),
('Dr. Priya Nair', 'dr.priya@clinic.com', 'Doctor@123', '9876543212', 'Doctor'),
('Dr. Rahul Verma', 'dr.rahul@clinic.com', 'Doctor@123', '9876543213', 'Doctor'),
('Amit Patel', 'amit@example.com', 'Patient@123', '9876543220', 'Patient'),
('Neha Singh', 'neha@example.com', 'Patient@123', '9876543221', 'Patient');
GO

-- 7. Insert Seed Doctors
INSERT INTO Doctors (UserId, DoctorName, Specialization, Qualification, ExperienceYears, ConsultationFee, RoomNumber, AvailableDays)
VALUES
(2, 'Dr. Rajesh Sharma', 'Cardiology', 'MBBS, MD, DM (Cardio)', 14, 800.00, 'Room 101', 'Mon, Wed, Fri (10 AM - 4 PM)'),
(3, 'Dr. Priya Nair', 'Dermatology', 'MBBS, MD (Dermatology)', 9, 600.00, 'Room 204', 'Tue, Thu, Sat (11 AM - 5 PM)'),
(4, 'Dr. Rahul Verma', 'General Medicine', 'MBBS, MD (Internal Med)', 11, 500.00, 'Room 105', 'Mon - Sat (09 AM - 2 PM)');
GO

-- 8. Insert Seed Appointments
INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, TimeSlot, Symptoms, Status, DoctorNotes)
VALUES
(5, 1, DATEADD(DAY, 1, CAST(GETDATE() AS DATE)), '10:00 AM - 10:30 AM', 'Mild chest discomfort after running, fatigue', 'Confirmed', 'Advised initial ECG and fasting blood sugar test.'),
(6, 2, DATEADD(DAY, 2, CAST(GETDATE() AS DATE)), '11:30 AM - 12:00 PM', 'Skin rash and itching on arms', 'Pending', NULL),
(5, 3, CAST(GETDATE() AS DATE), '09:30 AM - 10:00 AM', 'Persistent seasonal fever and dry cough', 'Completed', 'Viral flu. Prescribed Paracetamol 650mg and Cetirizine. Rest for 3 days.');
GO

-- 9. Verification Query
SELECT 
    A.AppointmentId,
    P.FullName AS PatientName,
    D.DoctorName,
    D.Specialization,
    A.AppointmentDate,
    A.TimeSlot,
    A.Status,
    A.DoctorNotes
FROM Appointments A
INNER JOIN Users P ON A.PatientId = P.UserId
INNER JOIN Doctors D ON A.DoctorId = D.DoctorId
ORDER BY A.AppointmentDate DESC;
GO

