<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClinicManagement.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .hero-section {
            background: linear-gradient(135deg, #0d6efd 0%, #0a58ca 100%);
            color: #ffffff;
            border-radius: 20px;
            padding: 3.5rem 2.5rem;
            margin-bottom: 2.5rem;
            box-shadow: 0 12px 30px -5px rgba(13, 110, 253, 0.35);
        }
        .stat-card {
            border-radius: 16px;
            background: #ffffff;
            border: 1px solid #e2e8f0;
            padding: 1.5rem;
            transition: transform 0.2s, box-shadow 0.2s;
        }
        .stat-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 10px 22px rgba(0,0,0,0.06);
        }
        .stat-icon {
            width: 52px;
            height: 52px;
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.4rem;
        }
        .doctor-card {
            transition: all 0.25s ease;
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            background: #ffffff;
            overflow: hidden;
        }
        .doctor-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 14px 28px -6px rgba(15, 23, 42, 0.1);
            border-color: #0d6efd;
        }
        .doctor-avatar-box {
            width: 56px;
            height: 56px;
            border-radius: 14px;
            background: linear-gradient(135deg, #e0f2fe 0%, #bae6fd 100%);
            color: #0284c7;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Hero Section -->
    <div class="hero-section text-center text-lg-start">
        <div class="row align-items-center">
            <div class="col-lg-8">
                <span class="badge bg-light text-primary px-3 py-2 rounded-pill fw-semibold mb-3">
                    <i class="fa-solid fa-sparkles me-1"></i> Smart Healthcare Scheduling System
                </span>
                <h1 class="display-5 fw-bold mb-3">Streamlined Patient Care &amp; Doctor Scheduling</h1>
                <p class="lead mb-4 opacity-90">
                    Book appointments with certified specialist doctors in seconds. Zero waiting lines, organized patient records, and instant doctor visit tracking.
                </p>
                <div class="d-flex flex-wrap gap-2 justify-content-center justify-content-lg-start">
                    <asp:HyperLink ID="hlHeroAction" runat="server" NavigateUrl="~/BookAppointment.aspx" CssClass="btn btn-light btn-lg px-4 fw-semibold text-primary shadow-sm">
                        <i class="fa-solid fa-calendar-check me-2"></i> Book an Appointment
                    </asp:HyperLink>
                    <a href="#doctorsSection" class="btn btn-outline-light btn-lg px-4 fw-semibold">
                        <i class="fa-solid fa-user-doctor me-2"></i> View Our Doctors
                    </a>
                </div>
            </div>
            <div class="col-lg-4 text-center mt-4 mt-lg-0 d-none d-lg-block">
                <i class="fa-solid fa-hospital-user" style="font-size: 9.5rem; opacity: 0.9;"></i>
            </div>
        </div>
    </div>

    <!-- Live Statistics Cards -->
    <div class="row g-4 mb-5">
        <div class="col-md-3 col-sm-6">
            <div class="stat-card d-flex align-items-center gap-3">
                <div class="stat-icon bg-primary-subtle text-primary">
                    <i class="fa-solid fa-user-doctor"></i>
                </div>
                <div>
                    <h3 class="fw-bold mb-0 text-dark"><asp:Label ID="lblTotalDoctors" runat="server" Text="4"></asp:Label></h3>
                    <span class="text-muted small">Specialist Doctors</span>
                </div>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card d-flex align-items-center gap-3">
                <div class="stat-icon bg-info-subtle text-info">
                    <i class="fa-solid fa-hospital-user"></i>
                </div>
                <div>
                    <h3 class="fw-bold mb-0 text-dark"><asp:Label ID="lblTotalPatients" runat="server" Text="48"></asp:Label></h3>
                    <span class="text-muted small">Registered Patients</span>
                </div>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card d-flex align-items-center gap-3">
                <div class="stat-icon bg-warning-subtle text-warning">
                    <i class="fa-solid fa-calendar-day"></i>
                </div>
                <div>
                    <h3 class="fw-bold mb-0 text-dark"><asp:Label ID="lblTodayAppointments" runat="server" Text="6"></asp:Label></h3>
                    <span class="text-muted small">Today's Appointments</span>
                </div>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card d-flex align-items-center gap-3">
                <div class="stat-icon bg-success-subtle text-success">
                    <i class="fa-solid fa-circle-check"></i>
                </div>
                <div>
                    <h3 class="fw-bold mb-0 text-dark"><asp:Label ID="lblCompletedVisits" runat="server" Text="1,240"></asp:Label></h3>
                    <span class="text-muted small">Completed Consults</span>
                </div>
            </div>
        </div>
    </div>

    <!-- Doctors Directory / Available Specialists -->
    <div id="doctorsSection" class="mb-5">
        <div class="d-flex justify-content-between align-items-end mb-4">
            <div>
                <h3 class="fw-bold text-dark mb-1">Meet Our Medical Specialists</h3>
                <p class="text-muted mb-0">Browse through our qualified doctors across multiple specialties.</p>
            </div>
            <asp:HyperLink ID="hlManageDoctorsLink" runat="server" NavigateUrl="~/ManageDoctors.aspx" CssClass="btn btn-outline-primary btn-sm fw-semibold" Visible="false">
                <i class="fa-solid fa-gear me-1"></i> Manage Doctors
            </asp:HyperLink>
        </div>

        <div class="row g-4">
            <asp:Repeater ID="rptDoctors" runat="server">
                <ItemTemplate>
                    <div class="col-lg-6 col-xl-6">
                        <div class="doctor-card p-4 h-100 d-flex flex-column">
                            <div class="d-flex align-items-start justify-content-between gap-3 mb-3">
                                <div class="d-flex align-items-center gap-3">
                                    <div class="doctor-avatar-box">
                                        <i class="fa-solid fa-user-doctor"></i>
                                    </div>
                                    <div>
                                        <h5 class="fw-bold mb-1 text-dark"><%# Eval("DoctorName") %></h5>
                                        <div class="d-flex flex-wrap gap-2 align-items-center">
                                            <span class="badge bg-primary-subtle text-primary border"><%# Eval("Specialization") %></span>
                                            <span class="badge bg-light text-secondary border small">
                                                <i class="fa-solid fa-award text-warning me-1"></i><%# Eval("ExperienceYears") %>+ Yrs Exp
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class="text-end">
                                    <span class="badge bg-warning-subtle text-dark border px-2 py-1">
                                        <i class="fa-solid fa-star text-warning me-1"></i><%# Eval("Rating") %>
                                    </span>
                                </div>
                            </div>

                            <!-- Doctor Credentials & Patient Count -->
                            <div class="bg-light rounded-3 p-3 mb-3 small">
                                <div class="mb-2 text-dark">
                                    <i class="fa-solid fa-graduation-cap text-primary me-2"></i>
                                    <strong><%# Eval("Qualification") %></strong>
                                </div>
                                <div class="d-flex justify-content-between flex-wrap text-muted">
                                    <div><i class="fa-solid fa-users text-success me-1"></i> <strong><%# Eval("PatientCount") %></strong> Treated</div>
                                    <div><i class="fa-solid fa-door-closed text-secondary me-1"></i> <%# Eval("RoomNumber") %></div>
                                </div>
                            </div>

                            <div class="small text-muted mb-3">
                                <i class="fa-regular fa-calendar-check text-primary me-2"></i><strong>Schedule:</strong> <%# Eval("AvailableDays") %>
                            </div>

                            <!-- Bottom: Fee & Action -->
                            <div class="mt-auto pt-3 border-top d-flex justify-content-between align-items-center">
                                <div>
                                    <span class="text-muted small d-block">Consultation Fee</span>
                                    <span class="fw-bold text-success fs-5">₹<%# Eval("ConsultationFee", "{0:N0}") %></span>
                                </div>
                                <a href='BookAppointment.aspx?docId=<%# Eval("DoctorId") %>' class="btn btn-primary btn-sm px-3 fw-semibold">
                                    <i class="fa-solid fa-calendar-plus me-1"></i> Book Consultation
                                </a>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Quick Features Highlight -->
    <div class="card p-4 p-md-5 bg-white border" style="border-radius: 18px;">
        <h4 class="fw-bold text-dark mb-4 text-center">Why Choose Our Smart Clinic System?</h4>
        <div class="row g-4 text-center">
            <div class="col-md-4">
                <div class="stat-icon bg-light text-primary mx-auto mb-3">
                    <i class="fa-solid fa-calendar-check"></i>
                </div>
                <h5 class="fw-semibold">Conflict-Free Booking</h5>
                <p class="text-muted small">Automated slot validation checks that prevent doctor double-booking in real time.</p>
            </div>
            <div class="col-md-4">
                <div class="stat-icon bg-light text-success mx-auto mb-3">
                    <i class="fa-solid fa-file-prescription"></i>
                </div>
                <h5 class="fw-semibold">Prescriptions &amp; Notes</h5>
                <p class="text-muted small">Doctors can write diagnosis notes and prescriptions directly accessible to the patient.</p>
            </div>
            <div class="col-md-4">
                <div class="stat-icon bg-light text-info mx-auto mb-3">
                    <i class="fa-solid fa-database"></i>
                </div>
                <h5 class="fw-semibold">Robust ADO.NET Architecture</h5>
                <p class="text-muted small">Implements both Connected (`SqlDataReader`) and Disconnected (`DataTable`/`SqlDataAdapter`) models.</p>
            </div>
        </div>
    </div>
</asp:Content>
