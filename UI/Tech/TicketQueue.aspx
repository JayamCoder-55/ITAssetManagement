<%@ Page Title="Ticket Queue" Language="C#" MasterPageFile="~/SharedUI/Site.Master" AutoEventWireup="true" CodeBehind="TicketQueue.aspx.cs" Inherits="ITAssetManagement.TicketQueue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="h4 mb-0"><i class="bi bi-list-task me-2"></i>Incident Ticket Queue</h2>
    </div>

    <!-- Filter Queue Section -->
    <div class="card shadow-sm border-0 mb-4">
        <div class="card-body p-3 bg-light rounded d-flex gap-3 align-items-end">
            <div>
                <label class="form-label small fw-bold mb-1">Status</label>
                <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-select form-select-sm" Width="150px">
                    <asp:ListItem Text="All Open" Value="Open"></asp:ListItem>
                    <asp:ListItem Text="In Progress" Value="In Progress"></asp:ListItem>
                    <asp:ListItem Text="On Hold" Value="On Hold"></asp:ListItem>
                    <asp:ListItem Text="Resolved" Value="Resolved"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div>
                <label class="form-label small fw-bold mb-1">Priority</label>
                <asp:DropDownList ID="ddlFilterPriority" runat="server" CssClass="form-select form-select-sm" Width="150px">
                    <asp:ListItem Text="All Priorities" Value="All"></asp:ListItem>
                    <asp:ListItem Text="High / Critical" Value="High"></asp:ListItem>
                    <asp:ListItem Text="Medium" Value="Medium"></asp:ListItem>
                    <asp:ListItem Text="Low" Value="Low"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div>
                <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn btn-sm btn-primary" OnClick="btnFilter_Click" />
            </div>
        </div>
    </div>

    <!-- Ticket Grid -->
    <div class="card shadow-sm border-0">
        <div class="card-body p-0">
            <div class="table-responsive">
                <asp:GridView ID="gvTickets" runat="server" AutoGenerateColumns="False" 
    CssClass="table table-hover table-borderless align-middle mb-0" 
    GridLines="None" OnRowCommand="gvTickets_RowCommand" DataKeyNames="TicketID">
    <HeaderStyle CssClass="table-light border-bottom" />
    <Columns>
    <%-- Dynamic Ticket Number display --%>
    <asp:BoundField DataField="TicketNumber" HeaderText="Ticket #" ItemStyle-CssClass="fw-bold text-primary" />
    <asp:BoundField DataField="Subject" HeaderText="Issue Summary" />
        <asp:BoundField DataField="PriorityLevel" HeaderText="Priority" />
        <asp:BoundField DataField="Status" HeaderText="Status" />
        <asp:BoundField DataField="AssignedTechnicianName" HeaderText="Assigned Tech" NullDisplayText="Unassigned" />
        
        <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <asp:LinkButton ID="btnAssign" runat="server" CommandName="AssignToMe" CommandArgument='<%# Eval("TicketID") %>' CssClass="btn btn-sm btn-outline-secondary me-1" ToolTip="Assign to Me">
                    <i class="bi bi-person-plus"></i>
                </asp:LinkButton>
                <asp:LinkButton ID="btnUpdate" runat="server" CommandName="UpdateStatus" CommandArgument='<%# Eval("TicketID") %>' CssClass="btn btn-sm btn-outline-primary me-1" ToolTip="Update Status">
                    <i class="bi bi-arrow-repeat"></i>
                </asp:LinkButton>
                <asp:LinkButton ID="btnAddNote" runat="server" CommandName="AddNote" CommandArgument='<%# Eval("TicketID") %>' CssClass="btn btn-sm btn-outline-info" ToolTip="Add Note">
                    <i class="bi bi-chat-text"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
            </div>
        </div>
    </div>

    <!-- Update Status Modal -->
    <asp:Panel ID="pnlStatusModal" runat="server" Visible="false" CssClass="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 1050;">
        <div class="card shadow-lg" style="width: 400px;">
            <div class="card-header bg-primary text-white d-flex justify-content-between">
                <h5 class="mb-0">Update Ticket Status</h5>
                <asp:LinkButton ID="btnCloseStatusModal" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseModal_Click"><i class="bi bi-x-lg"></i></asp:LinkButton>
            </div>
            <div class="card-body">
                <asp:HiddenField ID="hfStatusTicketID" runat="server" />
                <label class="form-label">New Status</label>
                <asp:DropDownList ID="ddlUpdateStatus" runat="server" CssClass="form-select mb-3">
                    <asp:ListItem Text="In Progress" Value="In Progress"></asp:ListItem>
                    <asp:ListItem Text="On Hold" Value="On Hold"></asp:ListItem>
                    <asp:ListItem Text="Resolved" Value="Resolved"></asp:ListItem>
                </asp:DropDownList>
                <div class="text-end">
                    <asp:Button ID="btnSaveStatus" runat="server" Text="Save Status" CssClass="btn btn-primary" OnClick="btnSaveStatus_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Add Note Modal -->
    <!-- Add Note Modal -->
<asp:Panel ID="pnlNoteModal" runat="server" Visible="false" CssClass="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 1050;">
    <div class="card shadow-lg" style="width: 500px;">
        <div class="card-header bg-info text-white d-flex justify-content-between">
            <h5 class="mb-0"><i class="bi bi-chat-left-text me-2"></i>Ticket History & Notes</h5>
            <asp:LinkButton ID="btnCloseNoteModal" runat="server" CssClass="text-white text-decoration-none" OnClick="btnCloseModal_Click"><i class="bi bi-x-lg"></i></asp:LinkButton>
        </div>
        <div class="card-body">
            <asp:HiddenField ID="hfNoteTicketID" runat="server" />
            
            <!-- Existing Notes History List -->
            <label class="form-label fw-bold small text-uppercase text-muted">Past Notes</label>
            <div class="p-2 border rounded bg-light mb-3" style="max-height: 180px; overflow-y: auto;">
                <asp:Repeater ID="rptNotesHistory" runat="server">
                    <ItemTemplate>
                        <div class="mb-2 p-2 bg-white rounded shadow-sm border-start border-3 border-info">
                            <div class="d-flex justify-content-between small text-muted mb-1">
                                <span class="fw-bold text-dark"><%# Eval("FullName") %></span>
                                <span><%# Eval("CreatedAt", "{0:MMM dd, yyyy hh:mm tt}") %></span>
                            </div>
                            <p class="mb-0 small"><%# Eval("CommentText") %></p>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoNotes" runat="server" Text="No notes added yet." CssClass="text-muted small italic d-block py-2 text-center" Visible="false"></asp:Label>
            </div>

            <!-- Add New Note Textbox -->
            <label class="form-label fw-semibold">Add New Comment / Progress Update</label>
            <asp:TextBox ID="txtNewNote" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control mb-3" placeholder="Enter notes here..."></asp:TextBox>
            <div class="text-end">
                <asp:Button ID="btnSaveNote" runat="server" Text="Add Note" CssClass="btn btn-info text-white" OnClick="btnSaveNote_Click" />
            </div>
        </div>
    </div>
</asp:Panel>

</asp:Content>