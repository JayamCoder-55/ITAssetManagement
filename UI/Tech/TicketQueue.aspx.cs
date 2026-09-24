using System;
using System.Web.UI.WebControls;
using ITAssetManagement.BLL;

namespace ITAssetManagement
{
    public partial class TicketQueue : System.Web.UI.Page
    {
        private TicketBLL ticketBLL = new TicketBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTicketQueue();
            }
        }

        private void LoadTicketQueue()
        {
            string status = ddlFilterStatus.SelectedValue;
            string priority = ddlFilterPriority.SelectedValue;

            gvTickets.DataSource = ticketBLL.GetFilteredTickets(status, priority);
            gvTickets.DataBind();
        }

        protected void gvTickets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (int.TryParse(e.CommandArgument.ToString(), out int ticketId))
            {
                if (e.CommandName == "AssignToMe")
                {
                    int currentTechId = GetCurrentUserId();
                    ticketBLL.AssignTicketToTechnician(ticketId, currentTechId);
                    LoadTicketQueue();
                }
                else if (e.CommandName == "UpdateStatus")
                {
                    hfStatusTicketID.Value = ticketId.ToString();
                    pnlStatusModal.Visible = true;
                }
                else if (e.CommandName == "AddNote")
                {
                    hfNoteTicketID.Value = ticketId.ToString();
                    txtNewNote.Text = string.Empty;
                    LoadTicketNotes(ticketId); // Load past notes history
                    pnlNoteModal.Visible = true;
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadTicketQueue();
        }

        protected void btnSaveStatus_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hfStatusTicketID.Value, out int ticketId))
            {
                string newStatus = ddlUpdateStatus.SelectedValue;
                int currentTechId = GetCurrentUserId();

                ticketBLL.UpdateStatus(ticketId, newStatus, currentTechId);
                pnlStatusModal.Visible = false;
                LoadTicketQueue();
            }
        }

        //protected void btnSaveNote_Click(object sender, EventArgs e)
        //{
        //    if (int.TryParse(hfNoteTicketID.Value, out int ticketId))
        //    {
        //        string note = txtNewNote.Text.Trim();
        //        int currentTechId = GetCurrentUserId();

        //        if (!string.IsNullOrEmpty(note))
        //        {
        //            ticketBLL.AddComment(ticketId, currentTechId, note);
        //        }

        //        pnlNoteModal.Visible = false;
        //    }
        //}

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlStatusModal.Visible = false;
            pnlNoteModal.Visible = false;
        }

        private int GetCurrentUserId()
        {
            return Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 1;
        }

        // Helper method to load comments into the repeater
        private void LoadTicketNotes(int ticketId)
        {
            // Assuming TicketBLL calls TicketDAL.GetTicketComments(ticketId)
            // Or call DAL directly if BLL doesn't have it yet:
            ITAssetManagement.DAL.TicketDAL dal = new ITAssetManagement.DAL.TicketDAL();
            var dtNotes = dal.GetTicketComments(ticketId);

            rptNotesHistory.DataSource = dtNotes;
            rptNotesHistory.DataBind();

            lblNoNotes.Visible = (dtNotes.Rows.Count == 0);
        }

       
        protected void btnSaveNote_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hfNoteTicketID.Value, out int ticketId))
            {
                string note = txtNewNote.Text.Trim();
                int currentTechId = GetCurrentUserId();

                if (!string.IsNullOrEmpty(note))
                {
                    ticketBLL.AddComment(ticketId, currentTechId, note);
                    txtNewNote.Text = string.Empty;
                    LoadTicketNotes(ticketId); // Refresh notes history list
                }
            }
        }
    }
}