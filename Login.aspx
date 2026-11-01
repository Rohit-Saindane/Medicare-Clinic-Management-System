<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ClinicManagement.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .auth-card {
            max-width: 450px;
            margin: 2.5rem auto;
            border-radius: 20px;
            border: 1px solid #e2e8f0;
            background: #ffffff;
            box-shadow: 0 12px 35px -5px rgba(15, 23, 42, 0.08);
        }
        .form-control {
            border-radius: 0 10px 10px 0;
            padding: 0.65rem 0.9rem;
            border-color: #e2e8f0;
            font-size: 0.95rem;
        }
        .form-control:focus {
            border-color: #0d6efd;
            box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
        }
        .input-group-text {
            border-radius: 10px 0 0 10px;
            border-color: #e2e8f0;
            background-color: #f8fafc;
            width: 44px;
            justify-content: center;
        }
        .demo-pill {
            font-size: 0.8rem;
            border-radius: 20px;
            padding: 0.35rem 0.75rem;
            cursor: pointer;
            transition: all 0.2s ease;
            background-color: #ffffff;
        }
        .demo-pill:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 10px rgba(0,0,0,0.08);
            border-color: #0d6efd;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-card p-4 p-md-5">
        <div class="text-center mb-4">
            <div class="avatar-circle bg-primary text-white mx-auto mb-3">
                <i class="fa-solid fa-lock" style="font-size: 1.4rem;"></i>
            </div>
            <h3 class="fw-bold text-dark mb-1">Welcome Back</h3>
            <p class="text-muted small">Sign in to manage clinic appointments and records</p>
        </div>

        <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-danger d-flex align-items-center gap-2 mb-4" role="alert">
            <i class="fa-solid fa-circle-exclamation text-danger"></i>
            <div class="small"><asp:Literal ID="litAlertMsg" runat="server"></asp:Literal></div>
        </asp:Panel>

        <div>
            <!-- Email -->
            <div class="mb-3">
                <label for="txtEmail" class="form-label small fw-semibold text-dark">Email Address</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fa-solid fa-envelope text-muted"></i></span>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter your email" TextMode="Email"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="Email address is required" Display="Dynamic" CssClass="text-danger small mt-1" />
            </div>

            <!-- Password -->
            <div class="mb-4">
                <label for="txtPassword" class="form-label small fw-semibold text-dark">Password</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fa-solid fa-key text-muted"></i></span>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter your password" TextMode="Password"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required" Display="Dynamic" CssClass="text-danger small mt-1" />
            </div>

            <!-- Submit Button -->
            <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn btn-primary w-100 py-2 fw-semibold mb-3 shadow-sm" OnClick="btnLogin_Click" />

            <div class="text-center small text-muted">
                Don't have an account? <a href="Register.aspx" class="text-primary fw-semibold text-decoration-none">Register as Patient</a>
            </div>
        </div>

        <!-- Quick Demo Accounts for Viva Evaluator -->
        <div class="mt-4 pt-3 border-top">
            <div class="small fw-semibold text-muted text-uppercase mb-2 text-center" style="letter-spacing: 0.05em; font-size: 0.72rem;">
                Click to Auto-Fill Credentials
            </div>
            <div class="d-flex flex-wrap gap-2 justify-content-center">
                <button type="button" class="btn btn-sm btn-outline-secondary demo-pill" onclick="fillLogin('dr.sharma@clinic.com', 'Doctor@123')">
                    <i class="fa-solid fa-user-doctor me-1 text-primary"></i> Doctor (Dr. Sharma)
                </button>
                <button type="button" class="btn btn-sm btn-outline-secondary demo-pill" onclick="fillLogin('amit@example.com', 'Patient@123')">
                    <i class="fa-solid fa-hospital-user me-1 text-info"></i> Patient (Amit)
                </button>
                <button type="button" class="btn btn-sm btn-outline-secondary demo-pill" onclick="fillLogin('admin@clinic.com', 'Admin@123')">
                    <i class="fa-solid fa-shield-halved me-1 text-danger"></i> Admin
                </button>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function fillLogin(email, pass) {
            document.getElementById('<%= txtEmail.ClientID %>').value = email;
            document.getElementById('<%= txtPassword.ClientID %>').value = pass;
        }
    </script>
</asp:Content>
