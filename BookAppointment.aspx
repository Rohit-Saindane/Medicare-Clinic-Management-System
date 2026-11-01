<%@ Page Title="Book Appointment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BookAppointment.aspx.cs" Inherits="ClinicManagement.BookAppointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .booking-card {
            max-width: 720px;
            margin: 1.5rem auto;
            border-radius: 16px;
        }
        .doctor-preview-box {
            background-color: #f1f5f9;
            border-left: 4px solid #0d6efd;
            border-radius: 8px;
            padding: 1rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="booking-card card shadow-sm p-4 p-md-5">
        <div class="d-flex align-items-center gap-3 mb-4">
            <div class="stat-icon bg-primary text-white rounded-circle" style="width: 52px; height: 52px;">
                <i class="fa-solid fa-calendar-plus fa-lg"></i>
            </div>
            <div>
                <h3 class="fw-bold text-dark mb-0">Schedule a Clinic Consultation</h3>
                <p class="text-muted small mb-0">Choose your specialist, date, and preferred time slot.</p>
            </div>
        </div>

        <!-- Feedback Alerts -->
        <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success d-flex align-items-center justify-content-between mb-4">
            <div class="d-flex align-items-center gap-2">
                <i class="fa-solid fa-circle-check fa-lg"></i>
                <span><asp:Literal ID="litSuccessMsg" runat="server"></asp:Literal></span>
            </div>
            <a href="MyAppointments.aspx" class="btn btn-sm btn-success fw-semibold">View Appointments</a>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger d-flex align-items-center gap-2 mb-4">
            <i class="fa-solid fa-triangle-exclamation fa-lg"></i>
            <div><asp:Literal ID="litErrorMsg" runat="server"></asp:Literal></div>
        </asp:Panel>

        <div>
            <!-- Specialization & Doctor Selection (Cascading Dropdowns) -->
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label for="ddlSpecialization" class="form-label fw-semibold text-dark">Medical Specialty</label>
                    <asp:DropDownList ID="ddlSpecialization" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSpecialization_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="col-md-6 mb-3">
                    <label for="ddlDoctor" class="form-label fw-semibold text-dark">Select Doctor</label>
                    <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDoctor_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvDoctor" runat="server" ControlToValidate="ddlDoctor"
                        InitialValue="" ErrorMessage="Please select a doctor" Display="Dynamic" CssClass="text-danger small mt-1" />
                </div>
            </div>

            <!-- Doctor Live Info Box -->
            <asp:Panel ID="pnlDoctorDetails" runat="server" Visible="false" CssClass="doctor-preview-box mb-4">
                <div class="row">
                    <div class="col-sm-6 mb-2 mb-sm-0">
                        <span class="text-muted small d-block">Qualification &amp; Experience:</span>
                        <strong class="text-dark"><asp:Literal ID="litDocDetails" runat="server"></asp:Literal></strong>
                    </div>
                    <div class="col-sm-3 mb-2 mb-sm-0">
                        <span class="text-muted small d-block">Room No:</span>
                        <strong class="text-dark"><asp:Literal ID="litDocRoom" runat="server"></asp:Literal></strong>
                    </div>
                    <div class="col-sm-3">
                        <span class="text-muted small d-block">Fee:</span>
                        <strong class="text-success fs-6"><asp:Literal ID="litDocFee" runat="server"></asp:Literal></strong>
                    </div>
                </div>
                <div class="mt-2 pt-2 border-top small text-muted">
                    <i class="fa-solid fa-clock me-1 text-primary"></i> Availability: <asp:Literal ID="litDocDays" runat="server"></asp:Literal>
                </div>
            </asp:Panel>

            <!-- Date & Time Slot Selection -->
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label for="txtAppointmentDate" class="form-label fw-semibold text-dark">Appointment Date</label>
                    <asp:TextBox ID="txtAppointmentDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtAppointmentDate"
                        ErrorMessage="Appointment date is required" Display="Dynamic" CssClass="text-danger small mt-1" />
                </div>
                <div class="col-md-6 mb-3">
                    <label for="ddlTimeSlot" class="form-label fw-semibold text-dark">Preferred Time Slot</label>
                    <asp:DropDownList ID="ddlTimeSlot" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="-- Select Time Slot --"></asp:ListItem>
                        <asp:ListItem Value="09:00 AM - 09:30 AM" Text="09:00 AM - 09:30 AM"></asp:ListItem>
                        <asp:ListItem Value="09:30 AM - 10:00 AM" Text="09:30 AM - 10:00 AM"></asp:ListItem>
                        <asp:ListItem Value="10:00 AM - 10:30 AM" Text="10:00 AM - 10:30 AM"></asp:ListItem>
                        <asp:ListItem Value="10:30 AM - 11:00 AM" Text="10:30 AM - 11:00 AM"></asp:ListItem>
                        <asp:ListItem Value="11:30 AM - 12:00 PM" Text="11:30 AM - 12:00 PM"></asp:ListItem>
                        <asp:ListItem Value="12:00 PM - 12:30 PM" Text="12:00 PM - 12:30 PM"></asp:ListItem>
                        <asp:ListItem Value="02:00 PM - 02:30 PM" Text="02:00 PM - 02:30 PM"></asp:ListItem>
                        <asp:ListItem Value="03:00 PM - 03:30 PM" Text="03:00 PM - 03:30 PM"></asp:ListItem>
                        <asp:ListItem Value="04:00 PM - 04:30 PM" Text="04:00 PM - 04:30 PM"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvTimeSlot" runat="server" ControlToValidate="ddlTimeSlot"
                        InitialValue="" ErrorMessage="Please choose a time slot" Display="Dynamic" CssClass="text-danger small mt-1" />
                </div>
            </div>

            <!-- Symptoms & Chief Complaint -->
            <div class="mb-4">
                <label for="txtSymptoms" class="form-label fw-semibold text-dark">Symptoms / Reason for Visit</label>
                <asp:TextBox ID="txtSymptoms" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" 
                    placeholder="Briefly describe your symptoms or reason for visit (e.g. fever for 2 days, back pain)..."></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSymptoms" runat="server" ControlToValidate="txtSymptoms"
                    ErrorMessage="Please provide a brief description of symptoms" Display="Dynamic" CssClass="text-danger small mt-1" />
            </div>

            <!-- Submit Button -->
            <div class="d-flex justify-content-between align-items-center">
                <a href="MyAppointments.aspx" class="text-muted text-decoration-none small">
                    <i class="fa-solid fa-arrow-left me-1"></i> Back to My Appointments
                </a>
                <asp:Button ID="btnConfirmBooking" runat="server" Text="Confirm & Book Appointment" CssClass="btn btn-primary px-4 py-2 fw-semibold" OnClick="btnConfirmBooking_Click" />
            </div>
        </div>
    </div>
</asp:Content>

