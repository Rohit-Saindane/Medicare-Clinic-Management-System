<%@ Page Title="Doctor Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DoctorDashboard.aspx.cs" Inherits="ClinicManagement.DoctorDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .consultation-panel {
            background-color: #ffffff;
            border: 2px solid #0d6efd;
            border-radius: 16px;
            box-shadow: 0 10px 25px rgba(13, 110, 253, 0.12);
        }
        .doc-kpi-card {
            border-radius: 14px;
            background: #ffffff;
            border: 1px solid #e2e8f0;
            padding: 1.25rem;
            transition: transform 0.2s;
        }
        .doc-kpi-card:hover {
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
    <!-- Header -->
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
        <div>
            <h3 class="fw-bold text-dark mb-1">
                <i class="fa-solid fa-stethoscope text-primary me-2"></i> <asp:Literal ID="litDashboardHeader" runat="server" Text="Clinical Queue &amp; Patient Records"></asp:Literal>
            </h3>
            <p class="text-muted small mb-0">Active clinical queue, patient symptoms review, and digital prescription dispensing.</p>
        </div>
        <div class="d-flex gap-2">
            <asp:HyperLink ID="hlManageDoctors" runat="server" NavigateUrl="~/ManageDoctors.aspx" CssClass="btn btn-outline-primary fw-semibold shadow-sm">
                <i class="fa-solid fa-user-doctor me-1"></i> Doctor Directory
            </asp:HyperLink>
        </div>
    </div>

    <!-- KPI Summary Cards -->
    <div class="row g-3 mb-4">
        <div class="col-md-3 col-sm-6">
            <div class="doc-kpi-card d-flex align-items-center gap-3 shadow-sm">
                <div class="kpi-icon-sm bg-warning-subtle text-warning">
                    <i class="fa-solid fa-calendar-day"></i>
                </div>
                <div>
                    <h4 class="fw-bold mb-0 text-dark"><asp:Label ID="lblDocToday" runat="server" Text="0"></asp:Label></h4>
                    <span class="text-muted small">Today's Scheduled</span>
                </div>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="doc-kpi-card d-flex align-items-center gap-3 shadow-sm">
                <div class="kpi-icon-sm bg-primary-subtle text-primary">
                    <i class="fa-solid fa-hourglass-half"></i>
                </div>
                <div>
                    <h4 class="fw-bold mb-0 text-dark"><asp:Label ID="lblDocPending" runat="server" Text="0"></asp:Label></h4>
                    <span class="text-muted small">Pending / Confirmed</span>
                </div>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="doc-kpi-card d-flex align-items-center gap-3 shadow-sm">
                <div class="kpi-icon-sm bg-success-subtle text-success">
                    <i class="fa-solid fa-circle-check"></i>
                </div>
                <div>
                    <h4 class="fw-bold mb-0 text-dark"><asp:Label ID="lblDocCompleted" runat="server" Text="0"></asp:Label></h4>
                    <span class="text-muted small">Completed Consults</span>
                </div>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="doc-kpi-card d-flex align-items-center gap-3 shadow-sm">
                <div class="kpi-icon-sm bg-info-subtle text-info">
                    <i class="fa-solid fa-clipboard-list"></i>
                </div>
                <div>
                    <h4 class="fw-bold mb-0 text-dark"><asp:Label ID="lblDocTotal" runat="server" Text="0"></asp:Label></h4>
                    <span class="text-muted small">Total Patient Visits</span>
                </div>
            </div>
        </div>
    </div>

    <!-- Alert Panel -->
    <asp:Panel ID="pnlNotice" runat="server" Visible="false" CssClass="alert alert-info d-flex align-items-center gap-2 mb-4">
        <i class="fa-solid fa-circle-info fa-lg"></i>
        <div><asp:Literal ID="litNoticeMsg" runat="server"></asp:Literal></div>
    </asp:Panel>

    <!-- Quick Filter Bar -->
    <div class="card p-3 mb-4 bg-white border" style="border-radius: 14px;">
        <div class="row g-3 align-items-end">
            <div class="col-md-3">
                <label for="ddlFilterDoctor" class="form-label small fw-semibold text-muted">Select Doctor</label>
                <asp:DropDownList ID="ddlFilterDoctor" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label for="ddlFilterStatus" class="form-label small fw-semibold text-muted">Filter by Status</label>
                <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                    <asp:ListItem Value="" Text="All Statuses"></asp:ListItem>
                    <asp:ListItem Value="Confirmed" Text="Confirmed"></asp:ListItem>
                    <asp:ListItem Value="Pending" Text="Pending"></asp:ListItem>
                    <asp:ListItem Value="Completed" Text="Completed"></asp:ListItem>
                    <asp:ListItem Value="Cancelled" Text="Cancelled"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label for="txtFilterDate" class="form-label small fw-semibold text-muted">Filter by Date</label>
                <asp:TextBox ID="txtFilterDate" runat="server" CssClass="form-control form-control-sm" TextMode="Date" AutoPostBack="true" OnTextChanged="FilterChanged"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnResetFilters" runat="server" Text="Reset Filters" CssClass="btn btn-sm btn-outline-secondary w-100" OnClick="btnResetFilters_Click" />
            </div>
        </div>
    </div>

    <!-- Inline Consultation & Prescription Edit Panel -->
    <asp:Panel ID="pnlPrescriptionBox" runat="server" Visible="false" CssClass="consultation-panel p-4 mb-4">
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-bold text-primary mb-0">
                <i class="fa-solid fa-file-prescription me-2"></i> Update Consultation &amp; Prescription for Appointment #<asp:Literal ID="litEditingAppId" runat="server"></asp:Literal>
            </h5>
            <asp:Button ID="btnClosePrescription" runat="server" Text="✕ Close" CssClass="btn btn-sm btn-light border" OnClick="btnClosePrescription_Click" />
        </div>
        <div class="row g-3">
            <div class="col-md-4">
                <div class="p-3 bg-light rounded-3 small">
                    <div class="mb-1"><span class="text-muted">Patient:</span> <strong><asp:Literal ID="litEditPatientName" runat="server"></asp:Literal></strong></div>
                    <div class="mb-1"><span class="text-muted">Phone:</span> <strong><asp:Literal ID="litEditPatientPhone" runat="server"></asp:Literal></strong></div>
                    <div><span class="text-muted">Symptoms:</span> <div class="text-danger fw-semibold mt-1"><asp:Literal ID="litEditSymptoms" runat="server"></asp:Literal></div></div>
                </div>
            </div>
            <div class="col-md-8">
                <div class="mb-3">
                    <label for="txtDoctorNotes" class="form-label fw-semibold text-dark small">Doctor Diagnosis, Prescription &amp; Clinical Advice:</label>
                    <asp:TextBox ID="txtDoctorNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" 
                        placeholder="Write prescription, prescribed medicines, dosages, and follow-up advice here..."></asp:TextBox>
                </div>
                <div class="d-flex gap-2">
                    <asp:Button ID="btnSavePrescription" runat="server" Text="Save & Mark as Completed" CssClass="btn btn-success fw-semibold shadow-sm" OnClick="btnSavePrescription_Click" />
                    <asp:Button ID="btnMarkCancelled" runat="server" Text="Mark Cancelled" CssClass="btn btn-outline-danger" OnClick="btnMarkCancelled_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Main GridView of Appointments -->
    <div class="card shadow-sm border" style="border-radius: 16px;">
        <div class="card-header bg-white py-3 d-flex justify-content-between align-items-center">
            <span class="fw-semibold text-dark">
                <i class="fa-solid fa-list-check text-primary me-2"></i> Patient Queue
            </span>
            <asp:Label ID="lblRecordCount" runat="server" CssClass="badge bg-secondary-subtle text-secondary border"></asp:Label>
        </div>
        <div class="card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvDoctorAppointments" runat="server" AutoGenerateColumns="false" 
                    CssClass="table table-hover align-middle mb-0" 
                    EmptyDataText="No appointments match the selected filter criteria."
                    EmptyDataRowStyle-CssClass="text-center text-muted p-5"
                    OnRowCommand="gvDoctorAppointments_RowCommand"
                    DataKeyNames="AppointmentId">
                    <Columns>
                        <asp:BoundField DataField="AppointmentId" HeaderText="#" ItemStyle-CssClass="fw-semibold text-muted text-center" HeaderStyle-CssClass="text-center" />
                        
                        <asp:TemplateField HeaderText="Patient Details">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><i class="fa-solid fa-circle-user text-secondary me-1"></i> <%# Eval("PatientName") %></div>
                                <div class="small text-muted"><i class="fa-solid fa-phone me-1"></i> <%# Eval("PatientPhone") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Doctor Assigned">
                            <ItemTemplate>
                                <div class="fw-semibold text-dark"><%# Eval("DoctorName") %></div>
                                <span class="badge bg-light text-primary border small"><%# Eval("Specialization") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Date & Slot">
                            <ItemTemplate>
                                <div class="fw-semibold text-dark"><i class="fa-regular fa-calendar me-1 text-secondary"></i> <%# Eval("AppointmentDate", "{0:dd-MMM-yyyy}") %></div>
                                <div class="small text-muted"><i class="fa-regular fa-clock me-1 text-secondary"></i> <%# Eval("TimeSlot") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Symptoms" HeaderText="Chief Complaint" ItemStyle-CssClass="small text-muted" />

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='badge <%# GetStatusBadgeClass(Eval("Status").ToString()) %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Prescription / Notes">
                            <ItemTemplate>
                                <div class="small text-truncate" style="max-width: 220px;">
                                    <%# string.IsNullOrEmpty(Eval("DoctorNotes") as string) 
                                        ? "<span class='text-muted fst-italic'>Pending consultation</span>" 
                                        : "<span class='text-success fw-medium'><i class='fa-solid fa-file-prescription me-1'></i>" + Eval("DoctorNotes") + "</span>" %>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                            <ItemTemplate>
                                <asp:Button ID="btnPrescribe" runat="server" Text="Prescribe / Update" CommandName="OpenPrescription" 
                                    CommandArgument='<%# Eval("AppointmentId") %>' 
                                    CssClass="btn btn-outline-primary btn-sm me-1 shadow-sm" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
