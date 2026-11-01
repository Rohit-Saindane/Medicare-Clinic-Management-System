<%@ Page Title="Manage Doctors" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageDoctors.aspx.cs" Inherits="ClinicManagement.ManageDoctors" %>
<%@ Import Namespace="ClinicManagement.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
        <div>
            <h3 class="fw-bold text-dark mb-1">
                <i class="fa-solid fa-user-doctor text-primary me-2"></i> Doctors &amp; Specialists Directory
            </h3>
            <p class="text-muted small mb-0">View clinic staff profiles, consultation charges, and clinical room assignments.</p>
        </div>
        <asp:PlaceHolder ID="phAddDoctorBtn" runat="server">
            <button type="button" class="btn btn-primary fw-semibold" data-bs-toggle="collapse" data-bs-target="#collapseAddDoctor">
                <i class="fa-solid fa-plus me-1"></i> Register New Doctor
            </button>
        </asp:PlaceHolder>
    </div>

    <!-- Alert Message -->
    <asp:Panel ID="pnlNotice" runat="server" Visible="false" CssClass="alert alert-info d-flex align-items-center gap-2 mb-4">
        <i class="fa-solid fa-circle-info fa-lg"></i>
        <div><asp:Literal ID="litNoticeMsg" runat="server"></asp:Literal></div>
    </asp:Panel>

    <!-- Collapsible Add Doctor Form (Admin Only) -->
    <div class="collapse mb-4" id="collapseAddDoctor">
        <div class="card card-body shadow-sm border p-4">
            <h5 class="fw-bold text-primary mb-3"><i class="fa-solid fa-user-plus me-2"></i> Onboard New Specialist Doctor</h5>
            
            <div class="row g-3">
                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Full Name</label>
                    <asp:TextBox ID="txtNewDocName" runat="server" CssClass="form-control form-control-sm" placeholder="e.g. Dr. Ananya Sen"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Email (Login ID)</label>
                    <asp:TextBox ID="txtNewDocEmail" runat="server" CssClass="form-control form-control-sm" placeholder="doctor@clinic.com" TextMode="Email"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Password</label>
                    <asp:TextBox ID="txtNewDocPassword" runat="server" CssClass="form-control form-control-sm" placeholder="Enter password" TextMode="Password"></asp:TextBox>
                </div>

                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Specialization</label>
                    <asp:TextBox ID="txtNewDocSpec" runat="server" CssClass="form-control form-control-sm" placeholder="e.g. Pediatrics, Neurology, Orthopedics"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Qualification</label>
                    <asp:TextBox ID="txtNewDocQual" runat="server" CssClass="form-control form-control-sm" placeholder="e.g. MBBS, MD, MS"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Experience (Years)</label>
                    <asp:TextBox ID="txtNewDocExp" runat="server" CssClass="form-control form-control-sm" TextMode="Number" Text="5"></asp:TextBox>
                </div>

                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Consultation Fee (₹)</label>
                    <asp:TextBox ID="txtNewDocFee" runat="server" CssClass="form-control form-control-sm" TextMode="Number" Text="600"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Room / Chamber Number</label>
                    <asp:TextBox ID="txtNewDocRoom" runat="server" CssClass="form-control form-control-sm" placeholder="e.g. Room 202"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label small fw-semibold text-dark">Available Schedule Days</label>
                    <asp:TextBox ID="txtNewDocDays" runat="server" CssClass="form-control form-control-sm" placeholder="e.g. Mon, Wed, Fri (10 AM - 2 PM)"></asp:TextBox>
                </div>
            </div>

            <div class="mt-3 text-end">
                <asp:Button ID="btnSaveDoctor" runat="server" Text="Save Doctor Profile" CssClass="btn btn-primary btn-sm fw-semibold px-4" OnClick="btnSaveDoctor_Click" />
            </div>
        </div>
    </div>

    <!-- Doctors GridView -->
    <div class="card shadow-sm border-0">
        <div class="card-header d-flex justify-content-between align-items-center">
            <span class="fw-semibold text-dark">
                <i class="fa-solid fa-list-ul text-primary me-2"></i> Registered Doctors List
            </span>
            <asp:Label ID="lblDoctorCount" runat="server" CssClass="badge bg-secondary-subtle text-secondary border"></asp:Label>
        </div>
        <div class="card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvDoctors" runat="server" AutoGenerateColumns="false" 
                    CssClass="table table-hover align-middle mb-0" 
                    EmptyDataText="No doctors registered yet."
                    OnRowCommand="gvDoctors_RowCommand"
                    DataKeyNames="DoctorId">
                    <Columns>
                        <asp:BoundField DataField="DoctorId" HeaderText="#" ItemStyle-CssClass="fw-semibold text-muted text-center" HeaderStyle-CssClass="text-center" />
                        
                        <asp:TemplateField HeaderText="Doctor Name">
                            <ItemTemplate>
                                <div class="fw-bold text-dark"><%# Eval("DoctorName") %></div>
                                <div class="small text-muted"><%# Eval("Email") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Specialty">
                            <ItemTemplate>
                                <span class="badge bg-primary-subtle text-primary border"><%# Eval("Specialization") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Qualification" HeaderText="Qualifications" ItemStyle-CssClass="small" />

                        <asp:TemplateField HeaderText="Experience">
                            <ItemTemplate>
                                <span class="fw-semibold"><%# Eval("ExperienceYears") %></span> <span class="text-muted small">years</span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Fee">
                            <ItemTemplate>
                                <span class="fw-bold text-success">₹<%# Eval("ConsultationFee", "{0:N0}") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="RoomNumber" HeaderText="Room" ItemStyle-CssClass="small text-muted" />
                        <asp:BoundField DataField="AvailableDays" HeaderText="Schedule" ItemStyle-CssClass="small text-muted" />

                        <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                            <ItemTemplate>
                                <asp:Button ID="btnDeleteDoc" runat="server" Text="Delete" CommandName="DeleteDoctor" 
                                    CommandArgument='<%# Eval("UserId") %>' 
                                    CssClass="btn btn-outline-danger btn-sm"
                                    Visible='<%# ClinicManagement.Helpers.SessionHelper.IsAdmin %>'
                                    OnClientClick="return confirm('Are you sure you want to remove this doctor and their associated account?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>

