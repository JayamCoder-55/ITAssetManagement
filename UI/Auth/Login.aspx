<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ITAssetManagement.UI.Auth.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Sign In - IT Asset Management</title>
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet" />
    
    <style>
        body { background-color: #e9ecef; display: flex; align-items: center; justify-content: center; height: 100vh; margin: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        .login-card { border: none; border-radius: 12px; box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1); overflow: hidden; width: 100%; max-width: 420px; background: #fff; }
        .login-header { background: linear-gradient(135deg, #0d6efd 0%, #0a58ca 100%); color: white; padding: 40px 20px 30px; text-align: center; }
        .login-header i { font-size: 3.5rem; margin-bottom: 15px; display: inline-block; }
        .login-body { padding: 40px 30px; }
        .form-control { padding: 12px; font-size: 1rem; border-left: none; }
        .form-control:focus { box-shadow: none; border-color: #dee2e6; }
        .input-group-text { background-color: #fff; color: #6c757d; border-right: none; }
        .input-group:focus-within .input-group-text, .input-group:focus-within .form-control { border-color: #0d6efd; }
        .btn-login { border-radius: 6px; padding: 12px; font-weight: 600; font-size: 1.1rem; letter-spacing: 0.5px; transition: 0.3s; }
        .btn-login:hover { transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 110, 253, 0.3); }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            
            <div class="login-header">
                <i class="bi bi-shield-lock-fill"></i>
                <h4 class="mb-0 fw-bold">IT Asset Management</h4>
                <p class="text-white-50 mt-2 mb-0 small">Sign in to start your session</p>
            </div>
            
            <div class="login-body">
                <!-- Error / Info Alert Box -->
                <asp:Panel ID="pnlAlert" runat="server" CssClass="alert alert-danger d-flex align-items-center" Visible="false" role="alert">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i>
                    <div><asp:Label ID="lblError" runat="server"></asp:Label></div>
                </asp:Panel>
                
                <!-- Email Input -->
<div class="mb-4">
    <label class="form-label text-muted fw-semibold small text-uppercase">Email Address</label>
    <div class="input-group">
        <span class="input-group-text"><i class="bi bi-envelope"></i></span>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="user@company.com" AutoCompleteType="Disabled"></asp:TextBox>
    </div>
    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
        ControlToValidate="txtEmail" 
        ErrorMessage="Email is required." 
        CssClass="text-danger small mt-1" 
        Display="Dynamic" 
        ValidationGroup="LoginGroup"></asp:RequiredFieldValidator>
</div>

<!-- Password Input -->
<div class="mb-4">
    <div class="d-flex justify-content-between">
        <label class="form-label text-muted fw-semibold small text-uppercase">Password</label>
        <asp:LinkButton ID="lnkForgotPassword" runat="server" CssClass="text-decoration-none small" OnClick="lnkForgotPassword_Click" CausesValidation="false">Forgot?</asp:LinkButton>
    </div>
    <div class="input-group">
        <span class="input-group-text"><i class="bi bi-key"></i></span>
        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
    </div>
    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
        ControlToValidate="txtPassword" 
        ErrorMessage="Password is required." 
        CssClass="text-danger small mt-1" 
        Display="Dynamic" 
        ValidationGroup="LoginGroup"></asp:RequiredFieldValidator>
</div>
                
                <!-- Submit Action -->
                <div class="mt-5">
                    <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn btn-primary w-100 btn-login" OnClick="btnSignIn_Click" ValidationGroup="LoginGroup" />
                </div>
                
            </div>
        </div>
    </form>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>