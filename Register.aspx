<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ClinicManagement.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .register-card {
            max-width: 540px;
            margin: 2rem auto;
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="register-card p-4 p-md-5">
        <div class="text-center mb-4">
            <div class="avatar-circle bg-primary text-white mx-auto mb-3">
                <i class="fa-solid fa-user-plus" style="font-size: 1.4rem;"></i>
            </div>
            <h3 class="fw-bold text-dark mb-1">Create Patient Account</h3>
            <p class="text-muted small">Register to schedule clinic consultations and track prescriptions</p>
        </div>

        <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-danger d-flex align-items-center gap-2 mb-4" role="alert">
            <i class="fa-solid fa-circle-exclamation text-danger"></i>
            <div class="small"><asp:Literal ID="litAlertMsg" runat="server"></asp:Literal></div>
        </asp:Panel>

        <div>
            <!-- Full Name -->
            <div class="mb-3">
                <label for="txtFullName" class="form-label small fw-semibold text-dark">Full Name</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fa-solid fa-user text-muted"></i></span>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Enter your full name"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ControlToValidate="txtFullName"
                    ErrorMessage="Full Name is required" Display="Dynamic" CssClass="text-danger small mt-1" />
            </div>

            <!-- Email -->
            <div class="mb-3">
                <label for="txtEmail" class="form-label small fw-semibold text-dark">Email Address</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fa-solid fa-envelope text-muted"></i></span>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter your email address" TextMode="Email"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="Email is required" Display="Dynamic" CssClass="text-danger small mt-1" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"
                    ErrorMessage="Please enter a valid email address" Display="Dynamic" CssClass="text-danger small mt-1" />
            </div>

            <!-- Phone Number -->
            <div class="mb-3">
                <label for="txtPhone" class="form-label small fw-semibold text-dark">Phone Number</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fa-solid fa-phone text-muted"></i></span>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Enter 10-digit mobile number" MaxLength="15"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone"
                    ErrorMessage="Phone number is required" Display="Dynamic" CssClass="text-danger small mt-1" />
            </div>

            <!-- Password -->
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label for="txtPassword" class="form-label small fw-semibold text-dark">Password</label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="fa-solid fa-key text-muted"></i></span>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Create password" TextMode="Password"></asp:TextBox>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                        ErrorMessage="Password required" Display="Dynamic" CssClass="text-danger small mt-1" />
                </div>
                <div class="col-md-6 mb-3">
                    <label for="txtConfirmPassword" class="form-label small fw-semibold text-dark">Confirm Password</label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="fa-solid fa-key text-muted"></i></span>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" placeholder="Confirm password" TextMode="Password"></asp:TextBox>
                    </div>
                    <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword"
                        ControlToCompare="txtPassword" ErrorMessage="Passwords do not match" Display="Dynamic" CssClass="text-danger small mt-1" />
                </div>
            </div>

            <!-- Submit -->
            <asp:Button ID="btnRegister" runat="server" Text="Complete Registration" CssClass="btn btn-primary w-100 py-2 fw-semibold mb-3 shadow-sm" OnClick="btnRegister_Click" />

            <div class="text-center small text-muted">
                Already registered? <a href="Login.aspx" class="text-primary fw-semibold text-decoration-none">Sign In here</a>
            </div>
        </div>
    </div>
</asp:Content>
