using System;
using System.Web.UI.WebControls;
using ITAssetManagement.BLL;
using ITAssetManagement.Models;

namespace ITAssetManagement
{
    public partial class MyAssets : System.Web.UI.Page
    {
        private AssetBLL assetBLL = new AssetBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUserAssets();
            }
        }

        private void BindUserAssets()
        {
            int userId = GetCurrentUserId();
            gvMyAssets.DataSource = assetBLL.GetUserAssets(userId);
            gvMyAssets.DataBind();
        }

        protected void gvMyAssets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (int.TryParse(e.CommandArgument.ToString(), out int assetId))
            {
                if (e.CommandName == "ReportIssue")
                {
                    Response.Redirect($"~/UI/Employee/SubmitTicket.aspx?AssetID={assetId}");
                }
                else if (e.CommandName == "ViewDetails")
                {
                    Asset asset = assetBLL.GetAssetById(assetId);
                    if (asset != null)
                    {
                        lblSerial.Text = string.IsNullOrEmpty(asset.SerialNumber) ? "N/A" : asset.SerialNumber;
                        lblWarranty.Text = asset.WarrantyExpiryDate.HasValue
                            ? asset.WarrantyExpiryDate.Value.ToString("MMM dd, yyyy")
                            : "No Warranty Info";
                        lblSpecs.Text = $"{asset.DeviceName} ({asset.Category})";
                        pnlModal.Visible = true;
                    }
                }
            }
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlModal.Visible = false;
        }

        private int GetCurrentUserId()
        {
            // Session fallback for authenticated user
            return Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 1;
        }
    }
}