<%@ Page Title="My Assets" Language="C#" MasterPageFile="~/SharedUI/Site.Master" AutoEventWireup="true" CodeBehind="MyAssets.aspx.cs" Inherits="ITAssetManagement.MyAssets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="h4 mb-0"><i class="bi bi-laptop me-2"></i>My Assigned Hardware</h2>
    </div>

    <div class="card shadow-sm border-0">
        <div class="card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvMyAssets" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover table-borderless align-middle mb-0" 
                    GridLines="None" OnRowCommand="gvMyAssets_RowCommand">
                    <HeaderStyle CssClass="table-light border-bottom" />
                    <Columns>
                        <asp:BoundField DataField="AssetID" HeaderText="Asset ID" ItemStyle-CssClass="fw-bold text-secondary" />
                        <asp:BoundField DataField="DeviceName" HeaderText="Device Name" />
                        <asp:BoundField DataField="Category" HeaderText="Category" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class="badge bg-success rounded-pill">Active</span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnViewDetails" runat="server" CommandName="ViewDetails" CommandArgument='<%# Eval("AssetID") %>' CssClass="btn btn-sm btn-outline-info me-2">
                                    <i class="bi bi-info-circle me-1"></i> Details
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnReportIssue" runat="server" CommandName="ReportIssue" CommandArgument='<%# Eval("AssetID") %>' CssClass="btn btn-sm btn-outline-danger">
                                    <i class="bi bi-exclamation-triangle me-1"></i> Report Issue
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Asset Details Modal (Hidden by default) -->
    <asp:Panel ID="pnlModal" runat="server" Visible="false" CssClass="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 1050;">
        <div class="card shadow-lg" style="width: 400px;">
            <div class="card-header bg-primary text-white d-flex justify-content-between">
                <h5 class="mb-0">Asset Details</h5>
                <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseModal_Click"><i class="bi bi-x-lg"></i></asp:LinkButton>
            </div>
            <div class="card-body">
                <ul class="list-group list-group-flush">
                    <li class="list-group-item"><strong>Serial Number:</strong> <asp:Label ID="lblSerial" runat="server"></asp:Label></li>
                    <li class="list-group-item"><strong>Warranty Status:</strong> <asp:Label ID="lblWarranty" runat="server"></asp:Label></li>
                    <li class="list-group-item"><strong>Specifications:</strong> <asp:Label ID="lblSpecs" runat="server"></asp:Label></li>
                </ul>
            </div>
        </div>
    </asp:Panel>
</asp:Content>