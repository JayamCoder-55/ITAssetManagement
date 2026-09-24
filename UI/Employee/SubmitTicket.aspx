<%@ Page Title="Submit Ticket" Language="C#" MasterPageFile="~/SharedUI/Site.Master" AutoEventWireup="true" CodeBehind="SubmitTicket.aspx.cs" Inherits="ITAssetManagement.SubmitTicket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="h4 mb-0"><i class="bi bi-ticket me-2"></i>Report an Issue</h2>
    </div>

    <div class="card shadow-sm border-0" style="max-width: 800px;">
        <div class="card-body p-4">
            
            <!-- Message Panel -->
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success d-flex align-items-center" role="alert">
                <i class="bi bi-check-circle-fill me-2"></i>
                <div><asp:Label ID="lblMessage" runat="server"></asp:Label></div>
            </asp:Panel>

           <div class="row g-3">
    <!-- Asset Selection DropDown -->
    <div class="col-md-4">
        <label class="form-label fw-semibold">Select Asset (Optional)</label>
        <asp:DropDownList ID="ddlAssets" runat="server" CssClass="form-select">
            <asp:ListItem Text="-- No Specific Asset (General Issue) --" Value="0"></asp:ListItem>
        </asp:DropDownList>
    </div>
    
    <!-- Issue Category -->
    <div class="col-md-4">
        <label class="form-label fw-semibold">Issue Category</label>
        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
            <asp:ListItem Text="Hardware (Broken Device)" Value="Hardware"></asp:ListItem>
            <asp:ListItem Text="Software (App crashing, login issue)" Value="Software"></asp:ListItem>
            <asp:ListItem Text="Network (No internet, VPN issues)" Value="Network"></asp:ListItem>
            <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
        </asp:DropDownList>
    </div>

    <!-- Priority Selection -->
    <div class="col-md-4">
        <label class="form-label fw-semibold">Priority Level</label>
        <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-select">
            <asp:ListItem Text="Low" Value="1"></asp:ListItem>
            <asp:ListItem Text="Medium" Value="2" Selected="True"></asp:ListItem>
            <asp:ListItem Text="High" Value="3"></asp:ListItem>
            <asp:ListItem Text="Critical" Value="4"></asp:ListItem>
        </asp:DropDownList>
    </div>

    <div class="col-12 mt-3">
        <label class="form-label fw-semibold">Issue Title</label>
        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Brief summary of the problem"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle" ErrorMessage="Title is required" CssClass="text-danger small" Display="Dynamic"></asp:RequiredFieldValidator>
    </div>

    <div class="col-12 mt-3">
        <label class="form-label fw-semibold">Detailed Description</label>
        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" placeholder="Please describe what happened and any error messages you see..."></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ErrorMessage="Description is required" CssClass="text-danger small" Display="Dynamic"></asp:RequiredFieldValidator>
    </div>

    <!-- Action Buttons -->
    <div class="col-12 mt-4 text-end">
        <asp:Button ID="btnClearForm" runat="server" Text="Clear Form" CssClass="btn btn-outline-secondary me-2" OnClick="btnClearForm_Click" CausesValidation="false" />
        <asp:Button ID="btnSubmitTicket" runat="server" Text="Submit Ticket" CssClass="btn btn-primary" OnClick="btnSubmitTicket_Click" />
    </div>
</div>
           
            
        </div>
    </div>
</asp:Content>