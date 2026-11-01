<%@ Page Title="My Appointments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyAppointments.aspx.cs" Inherits="ClinicManagement.MyAppointments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .patient-kpi-card {
            border-radius: 14px;
            background: #ffffff;
            border: 1px solid #e2e8f0;
            padding: 1.25rem;
            transition: transform 0.2s;
        }
        .patient-kpi-card:hover {
            transform: translateY(-2px);
        }
        .kpi-icon-sm {
            width: 44px;
            height: 44px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.2rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Header Banner -->
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
        <div>
            <h3 class="fw-bold text-dark mb-1">
                <i class="fa-solid fa-hospital-user text-primary me-2"></i> Patient Health Portal
            </h3>
            <p class="text-muted small mb-0">Welcome, <strong><asp:Literal ID="litPatientName" runat="server"></asp:Literal></strong>. Track your visits, prescriptions, and upcoming doctor appointments.</p>
        </div>
        <div>
            <a href="BookAppointment.aspx" class="btn btn-primary fw-semibold px-3 shadow-sm">
                <i class="fa-solid fa-calendar-plus me-1"></i> Book New Appointment
            </a>
        </div>
    </div>

    <!-- Alert Panel -->
    <asp:Panel ID="pnlNotice" runat="server" Visible="false" CssClass="alert alert-info d-flex align-items-center gap-2 mb-4">
        <i class="fa-solid fa-circle-info fa-lg"></i>
        <div><asp:Literal ID="litNoticeMsg" runat="server"></asp:Literal></div>
    </asp:Panel>

    <!-- KPI Summary Cards -->
    <div class="row g-3 mb-4">
        <div class="col-md-4">
            <div class="patient-kpi-card d-flex align-items-center gap-3 shadow-sm">
                <div class="kpi-icon-sm bg-primary-subtle text-primary">
                    <i class="fa-solid fa-calendar-check"></i>
                </div>
                <div>
                    <h4 class="fw-bold mb-0 text-dark"><asp:Label ID="lblUpcomingCount" runat="server" Text="0"></asp:Label></h4>
                    <span class="text-muted small">Upcoming Scheduled Visits</span>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="patient-kpi-card d-flex align-items-center gap-3 shadow-sm">
                <div class="kpi-icon-sm bg-success-subtle text-success">
                    <i class="fa-solid fa-file-prescription"></i>
                </div>
                <div>
                    <h4 class="fw-bold mb-0 text-dark"><asp:Label ID="lblCompletedCount" runat="server" Text="0"></asp:Label></h4>
                    <span class="text-muted small">Completed Consultations</span>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="patient-kpi-card d-flex align-items-center gap-3 shadow-sm">
                <div class="kpi-icon-sm bg-secondary-subtle text-secondary">
                    <i class="fa-solid fa-clock-rotate-left"></i>
                </div>
                <div>
                    <h4 class="fw-bold mb-0 text-dark"><asp:Label ID="lblTotalVisits" runat="server" Text="0"></asp:Label></h4>
                    <span class="text-muted small">Total Bookings on Record</span>
                </div>
            </div>
        </div>
    </div>

    <!-- Appointments Card -->
    <div class="card shadow-sm border" style="border-radius: 16px;">
        <div class="card-header bg-white py-3 d-flex justify-content-between align-items-center">
            <span class="fw-semibold text-dark">
                <i class="fa-solid fa-list-check text-primary me-2"></i> Consultation History &amp; Prescriptions
            </span>
            <asp:Label ID="lblRecordCount" runat="server" CssClass="badge bg-secondary-subtle text-secondary border"></asp:Label>
        </div>
        <div class="card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvAppointments" runat="server" AutoGenerateColumns="false" 
                    CssClass="table table-hover align-middle mb-0" 
                    EmptyDataText="No appointments scheduled yet. Click 'Book New Appointment' above to schedule your visit."
                    EmptyDataRowStyle-CssClass="text-center text-muted p-5"
                    OnRowCommand="gvAppointments_RowCommand"
                    DataKeyNames="AppointmentId">
                    <Columns>
                        <asp:BoundField DataField="AppointmentId" HeaderText="#" ItemStyle-CssClass="fw-semibold text-muted text-center" HeaderStyle-CssClass="text-center" />
                        
                        <asp:TemplateField HeaderText="Doctor & Specialty">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><%# Eval("DoctorName") %></div>
                                <span class="badge bg-primary-subtle text-primary border small"><%# Eval("Specialization") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Date & Slot">
                            <ItemTemplate>
                                <div class="fw-semibold text-dark"><i class="fa-regular fa-calendar me-1 text-secondary"></i> <%# Eval("AppointmentDate", "{0:dd-MMM-yyyy}") %></div>
                                <div class="small text-muted"><i class="fa-regular fa-clock me-1 text-secondary"></i> <%# Eval("TimeSlot") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Symptoms" HeaderText="Symptoms / Notes" ItemStyle-CssClass="small text-muted" />

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='badge <%# GetStatusBadgeClass(Eval("Status").ToString()) %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Doctor's Prescription & Clinical Advice">
                            <ItemTemplate>
                                <div class="small">
                                    <%# string.IsNullOrEmpty(Eval("DoctorNotes") as string) 
                                        ? "<span class='text-muted fst-italic'>Pending consultation</span>" 
                                        : "<div class='text-dark bg-success-subtle p-2 rounded border border-success-subtle'><i class='fa-solid fa-file-prescription text-success me-1'></i><strong>Prescription:</strong> " + Eval("DoctorNotes") + "</div>" %>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                            <ItemTemplate>
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="CancelAppointment" 
                                    CommandArgument='<%# Eval("AppointmentId") %>' 
                                    CssClass="btn btn-outline-danger btn-sm"
                                    Visible='<%# Eval("Status").ToString() == "Confirmed" || Eval("Status").ToString() == "Pending" %>'
                                    OnClientClick="return confirm('Are you sure you want to cancel this appointment?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
