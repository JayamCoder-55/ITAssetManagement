<%@ Page Title="Manage Assets" Language="C#" MasterPageFile="~/SharedUI/Site.Master" AutoEventWireup="true" CodeBehind="ManageAssets.aspx.cs" Inherits="ITAssetManagement.UI.Admin.ManageAssets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="h4 mb-0"><i class="bi bi-boxes me-2"></i>Inventory Management</h2>
        <asp:LinkButton ID="btnAddAsset" runat="server" CssClass="btn btn-primary" OnClick="btnAddAsset_Click">
            <i class="bi bi-plus-lg me-1"></i> Add New Asset
        </asp:LinkButton>
    </div>

    <!-- Alert Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success d-flex align-items-center" role="alert">
        <i class="bi bi-info-circle-fill me-2"></i>
        <div><asp:Label ID="lblMessage" runat="server"></asp:Label></div>
    </asp:Panel>

    <!-- Search / Filter Section -->
    <div class="card shadow-sm border-0 mb-4">
        <div class="card-body p-3 bg-light rounded d-flex gap-3 align-items-end flex-wrap">
            <div>
                <label class="form-label small fw-bold mb-1">Search Asset</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control form-control-sm" placeholder="ID, Tag, or Name"></asp:TextBox>
            </div>
            <div>
                <label class="form-label small fw-bold mb-1">Category</label>
                <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-select form-select-sm" Width="150px">
                    <asp:ListItem Text="All Categories" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Laptop" Value="Laptop"></asp:ListItem>
                    <asp:ListItem Text="Desktop" Value="Desktop"></asp:ListItem>
                    <asp:ListItem Text="Peripheral" Value="Peripheral"></asp:ListItem>
                    <asp:ListItem Text="Mobile" Value="Mobile"></asp:ListItem>
                    <asp:ListItem Text="Accessory" Value="Accessory"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div>
                <label class="form-label small fw-bold mb-1">Assignment State</label>
                <asp:DropDownList ID="ddlFilterState" runat="server" CssClass="form-select form-select-sm" Width="150px">
                    <asp:ListItem Text="All States" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Available" Value="Available"></asp:ListItem>
                    <asp:ListItem Text="Assigned" Value="Assigned"></asp:ListItem>
                    <asp:ListItem Text="Under Repair" Value="Under Repair"></asp:ListItem>
                    <asp:ListItem Text="Retired" Value="Retired"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div>
                <asp:Button ID="btnSearchFilter" runat="server" Text="Apply Filter" CssClass="btn btn-sm btn-secondary" OnClick="btnSearchFilter_Click" />
            </div>
        </div>
    </div>

    <!-- Inventory Grid -->
    <div class="card shadow-sm border-0">
        <div class="card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvAssets" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover table-borderless align-middle mb-0" 
                    GridLines="None" OnRowCommand="gvAssets_RowCommand" DataKeyNames="AssetID">
                    <HeaderStyle CssClass="table-light border-bottom" />
                    <Columns>
                        <asp:BoundField DataField="AssetTag" HeaderText="Asset Tag" ItemStyle-CssClass="fw-bold" />
                        <asp:BoundField DataField="DeviceName" HeaderText="Device Name" />
                        <asp:BoundField DataField="Category" HeaderText="Category" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:BoundField DataField="AssignedToName" HeaderText="Assigned To" NullDisplayText="--" />
                        
                        <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditAsset" CommandArgument='<%# Eval("AssetID") %>' CssClass="btn btn-sm btn-outline-secondary me-1" ToolTip="Edit Asset">
                                    <i class="bi bi-pencil"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnAssign" runat="server" CommandName="AssignAsset" CommandArgument='<%# Eval("AssetID") %>' CssClass="btn btn-sm btn-outline-primary me-1" ToolTip="Assign to User">
                                    <i class="bi bi-person-plus"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnRetire" runat="server" CommandName="RetireAsset" CommandArgument='<%# Eval("AssetID") %>' CssClass="btn btn-sm btn-outline-danger" ToolTip="Retire / Decommission">
                                    <i class="bi bi-trash"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Add/Edit Asset Modal -->
    <asp:Panel ID="pnlAssetModal" runat="server" Visible="false" CssClass="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 1050;">
        <div class="card shadow-lg" style="width: 500px;">
            <div class="card-header bg-primary text-white d-flex justify-content-between">
                <h5 class="mb-0"><asp:Label ID="lblAssetModalTitle" runat="server" Text="Add New Asset"></asp:Label></h5>
                <asp:LinkButton ID="btnCloseAssetModal" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseModal_Click" CausesValidation="false"><i class="bi bi-x-lg"></i></asp:LinkButton>
            </div>
            <div class="card-body">
                <asp:HiddenField ID="hfEditAssetID" runat="server" />
                
                <label class="form-label">Asset Tag / Serial Number</label>
                <asp:TextBox ID="txtAssetTag" runat="server" CssClass="form-control mb-3" placeholder="e.g. AST-10042"></asp:TextBox>

                <label class="form-label">Device Name</label>
                <asp:TextBox ID="txtDeviceName" runat="server" CssClass="form-control mb-3" placeholder="e.g. Dell Latitude 7420"></asp:TextBox>
                
                <label class="form-label">Category</label>
                <asp:DropDownList ID="ddlEditCategory" runat="server" CssClass="form-select mb-3">
                    <asp:ListItem Text="Laptop" Value="Laptop"></asp:ListItem>
                    <asp:ListItem Text="Desktop" Value="Desktop"></asp:ListItem>
                    <asp:ListItem Text="Peripheral" Value="Peripheral"></asp:ListItem>
                    <asp:ListItem Text="Mobile" Value="Mobile"></asp:ListItem>
                    <asp:ListItem Text="Accessory" Value="Accessory"></asp:ListItem>
                </asp:DropDownList>

                <label class="form-label">Warranty Expiry Date</label>
                <asp:TextBox ID="txtWarranty" runat="server" CssClass="form-control mb-3" TextMode="Date"></asp:TextBox>

                <div class="text-end">
                    <asp:Button ID="btnSaveAsset" runat="server" Text="Save Asset" CssClass="btn btn-primary" OnClick="btnSaveAsset_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Assign Asset Modal -->
    <asp:Panel ID="pnlAssignModal" runat="server" Visible="false" CssClass="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 1050;">
        <div class="card shadow-lg" style="width: 400px;">
            <div class="card-header bg-info text-white d-flex justify-content-between">
                <h5 class="mb-0">Assign Asset</h5>
                <asp:LinkButton ID="btnCloseAssignModal" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseModal_Click" CausesValidation="false"><i class="bi bi-x-lg"></i></asp:LinkButton>
            </div>
            <div class="card-body">
                <asp:HiddenField ID="hfAssignAssetID" runat="server" />
                <p class="mb-3 text-muted">Select an employee to assign <strong id="lblAssignTitle" runat="server"></strong> to.</p>
                
                <label class="form-label">Select User / Employee</label>
                <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-select mb-4">
                </asp:DropDownList>

                <div class="text-end">
                    <asp:Button ID="btnSaveAssignment" runat="server" Text="Confirm Assignment" CssClass="btn btn-info text-white" OnClick="btnSaveAssignment_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Retire Asset Modal -->
    <asp:Panel ID="pnlRetireModal" runat="server" Visible="false" CssClass="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 1050;">
        <div class="card shadow-lg" style="width: 400px;">
            <div class="card-header bg-danger text-white d-flex justify-content-between">
                <h5 class="mb-0">Retire Asset</h5>
                <asp:LinkButton ID="btnCloseRetireModal" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseModal_Click" CausesValidation="false"><i class="bi bi-x-lg"></i></asp:LinkButton>
            </div>
            <div class="card-body text-center p-4">
                <asp:HiddenField ID="hfRetireAssetID" runat="server" />
                <i class="bi bi-exclamation-triangle-fill text-warning" style="font-size: 3rem;"></i>
                <h5 class="mt-3">Are you sure?</h5>
                <p class="text-muted">You are about to retire asset ID: <strong><asp:Label ID="lblRetireID" runat="server"></asp:Label></strong>. It will be unassigned and marked as retired.</p>
                
                <div class="mt-4">
                    <asp:Button ID="btnCancelRetire" runat="server" Text="Cancel" CssClass="btn btn-outline-secondary me-2" OnClick="btnCloseModal_Click" CausesValidation="false" />
                    <asp:Button ID="btnConfirmRetire" runat="server" Text="Yes, Retire Asset" CssClass="btn btn-danger" OnClick="btnConfirmRetire_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

</asp:Content>