<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/SharedUI/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="ITAssetManagement.UI.Admin.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="h4 mb-0"><i class="bi bi-people me-2"></i>User Administration</h2>
        <asp:LinkButton ID="btnCreateUser" runat="server" CssClass="btn btn-primary" OnClick="btnCreateUser_Click">
            <i class="bi bi-person-plus-fill me-1"></i> Create User
        </asp:LinkButton>
    </div>

    <!-- Success Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success d-flex align-items-center" role="alert">
        <i class="bi bi-info-circle-fill me-2"></i>
        <div><asp:Label ID="lblMessage" runat="server"></asp:Label></div>
    </asp:Panel>

    <!-- Users Grid -->
    <div class="card shadow-sm border-0">
        <div class="card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover table-borderless align-middle mb-0" 
                    GridLines="None" OnRowCommand="gvUsers_RowCommand" DataKeyNames="UserID">
                    <HeaderStyle CssClass="table-light border-bottom" />
                    <Columns>
                        <asp:BoundField DataField="Email" HeaderText="Email Address" ItemStyle-CssClass="fw-bold" />
                        <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                        <asp:BoundField DataField="RoleName" HeaderText="Role" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <%# Eval("IsActive").ToString() == "True" 
                                    ? "<span class='badge bg-success rounded-pill'>Active</span>" 
                                    : "<span class='badge bg-secondary rounded-pill'>Inactive</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEditRole" runat="server" CommandName="EditRole" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-sm btn-outline-primary me-1" ToolTip="Edit Role / Info">
                                    <i class="bi bi-pencil-square"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnResetPass" runat="server" CommandName="ResetPassword" CommandArgument='<%# Eval("UserID") %>' CssClass="btn btn-sm btn-outline-warning me-1" ToolTip="Reset Password">
                                    <i class="bi bi-key"></i>
                                </asp:LinkButton>
                                <!-- Dynamic Activate / Deactivate Action Button -->
<asp:LinkButton ID="btnToggleStatus" runat="server" 
    CommandName='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' 
    CommandArgument='<%# Eval("UserId") %>' 
    CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-outline-danger btn-sm" : "btn btn-outline-success btn-sm" %>'
    ToolTip='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate User" : "Activate User" %>'>
    <i class='<%# Convert.ToBoolean(Eval("IsActive")) ? "bi bi-person-x" : "bi bi-person-check" %>'></i>
</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Create / Edit User Modal -->
    <asp:Panel ID="pnlUserModal" runat="server" Visible="false" CssClass="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 1050;">
        <div class="card shadow-lg" style="width: 450px;">
            <div class="card-header bg-primary text-white d-flex justify-content-between">
                <h5 class="mb-0"><asp:Label ID="lblUserModalTitle" runat="server" Text="Create User"></asp:Label></h5>
                <asp:LinkButton ID="btnCloseUserModal" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseModal_Click" CausesValidation="false"><i class="bi bi-x-lg"></i></asp:LinkButton>
            </div>
            <div class="card-body">
                <asp:HiddenField ID="hfEditUserID" runat="server" />
                
                <label class="form-label">Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control mb-3" placeholder="user@company.com"></asp:TextBox>
                
                <label class="form-label">Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control mb-3" placeholder="e.g. John Doe"></asp:TextBox>
                
                <asp:Panel ID="pnlPassword" runat="server">
                    <label class="form-label">Default Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control mb-3" TextMode="Password" placeholder="Enter default password"></asp:TextBox>
                </asp:Panel>

                <label class="form-label">System Role</label>
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select mb-4">
                    <asp:ListItem Text="Employee" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Technician" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Admin" Value="1"></asp:ListItem>
                </asp:DropDownList>

                <!-- Department Dropdown -->
<div class="mb-3">
    <label class="form-label text-muted fw-semibold small text-uppercase">Department</label>
    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select">
        <asp:ListItem Text="IT Support" Value="IT Support"></asp:ListItem>
        <asp:ListItem Text="Human Resources" Value="Human Resources"></asp:ListItem>
        <asp:ListItem Text="Finance" Value="Finance"></asp:ListItem>
        <asp:ListItem Text="Operations" Value="Operations"></asp:ListItem>
        <asp:ListItem Text="Engineering" Value="Engineering"></asp:ListItem>
        <asp:ListItem Text="General" Value="General" Selected="True"></asp:ListItem>
    </asp:DropDownList>
</div>

                <div class="text-end">
                    <asp:Button ID="btnSaveUser" runat="server" Text="Save User" CssClass="btn btn-primary" OnClick="btnSaveUser_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Deactivate Confirmation Modal -->
    <asp:Panel ID="pnlDeactivateModal" runat="server" Visible="false" CssClass="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 1050;">
        <div class="card shadow-lg" style="width: 400px;">
            <div class="card-header bg-danger text-white d-flex justify-content-between">
                <h5 class="mb-0">Deactivate Account</h5>
                <asp:LinkButton ID="btnCloseDeactivateModal" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseModal_Click" CausesValidation="false"><i class="bi bi-x-lg"></i></asp:LinkButton>
            </div>
            <div class="card-body text-center p-4">
                <asp:HiddenField ID="hfDeactivateUserID" runat="server" />
                <i class="bi bi-person-x-fill text-danger" style="font-size: 3rem;"></i>
                <h5 class="mt-3">Suspend User Access?</h5>
                <p class="text-muted">This will prevent the user from logging in. Are you sure?</p>
                
                <div class="mt-4">
                    <asp:Button ID="btnCancelDeactivate" runat="server" Text="Cancel" CssClass="btn btn-outline-secondary me-2" OnClick="btnCloseModal_Click" CausesValidation="false" />
                    <asp:Button ID="btnConfirmDeactivate" runat="server" Text="Yes, Deactivate" CssClass="btn btn-danger" OnClick="btnConfirmDeactivate_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

</asp:Content>