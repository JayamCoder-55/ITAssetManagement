using System;
using System.Web.UI.WebControls;
using ITAssetManagement.BLL;
using ITAssetManagement.Models;

namespace ITAssetManagement
{
    public partial class SubmitTicket : System.Web.UI.Page
    {
        private TicketBLL ticketBLL = new TicketBLL();
        private AssetBLL assetBLL = new AssetBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserAssets();

                // Pre-select asset if passed via QueryString (e.g., SubmitTicket.aspx?AssetID=5)
                if (Request.QueryString["AssetID"] != null)
                {
                    string queryAssetId = Request.QueryString["AssetID"].ToString();
                    ListItem item = ddlAssets.Items.FindByValue(queryAssetId);
                    if (item != null)
                    {
                        ddlAssets.SelectedValue = queryAssetId;
                    }
                }
            }
        }

        private void LoadUserAssets()
        {
            int userId = GetCurrentUserId();

            // Populate the DropDownList with assets assigned to the current user
            var userAssets = assetBLL.GetAssetsByUserId(userId);

            ddlAssets.DataSource = userAssets;
            ddlAssets.DataTextField = "DeviceName"; // Displays device name in dropdown
            ddlAssets.DataValueField = "AssetID";   // Submits AssetId to backend
            ddlAssets.DataBind();

            // Insert default optional choice at index 0
            ddlAssets.Items.Insert(0, new ListItem("-- No Specific Asset (General Issue) --", "0"));
        }
        private void ClearInputFields()
        {
            if (ddlAssets.Items.Count > 0) ddlAssets.SelectedIndex = 0;
            txtTitle.Text = string.Empty;
            txtDescription.Text = string.Empty;
            ddlCategory.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 1; // Resets to Medium
        }
        protected void btnSubmitTicket_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int selectedAssetId = Convert.ToInt32(ddlAssets.SelectedValue);
                int? assetId = selectedAssetId > 0 ? selectedAssetId : (int?)null;
                int priorityPolicyId = Convert.ToInt32(ddlPriority.SelectedValue);

                ServiceTicket ticket = new ServiceTicket
                {
                    Subject = txtTitle.Text.Trim(),
                    Description = $"[{ddlCategory.SelectedValue}] {txtDescription.Text.Trim()}",
                    AssetId = assetId,
                    ReportedByUserId = GetCurrentUserId()
                };

                // Passes selected priority level ID dynamically
                bool isCreated = ticketBLL.CreateTicket(ticket, priorityLevelId: priorityPolicyId);

                if (isCreated)
                {
                    lblMessage.Text = $"Ticket successfully submitted! Ticket Ref: <strong>{ticket.TicketNumber}</strong>";
                    pnlMessage.CssClass = "alert alert-success d-flex align-items-center";
                    pnlMessage.Visible = true;
                    ClearInputFields();
                }
                else
                {
                    lblMessage.Text = "Failed to submit ticket. Please try again or contact IT support.";
                    pnlMessage.CssClass = "alert alert-danger d-flex align-items-center";
                    pnlMessage.Visible = true;
                }
            }
        }

        protected void btnClearForm_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            pnlMessage.Visible = false;
        }



        private int GetCurrentUserId()
        {
            if (Session["UserId"] != null) return Convert.ToInt32(Session["UserId"]);
            if (Session["UserID"] != null) return Convert.ToInt32(Session["UserID"]);
            return 1;
        }
    }
}