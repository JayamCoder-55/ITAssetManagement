using System;
using System.Data;
using System.Web.UI.WebControls;
using ITAssetManagement.BLL;
using ITAssetManagement.Models;

namespace ITAssetManagement.UI.Admin
{
    public partial class ManageAssets : System.Web.UI.Page
    {
        private AssetBLL assetBLL = new AssetBLL();
        private UserBLL userBLL = new UserBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Authorization Check
            if (Session["RoleName"] == null || Session["RoleName"].ToString() != "Admin")
            {
                Response.Redirect("~/UI/Auth/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadInventory();
                LoadUserDropdown();
            }
        }

        private void LoadInventory()
        {
            string searchQuery = txtSearch.Text.Trim();
            string category = ddlFilterCategory.SelectedValue;
            string state = ddlFilterState.SelectedValue;

            DataTable dt = assetBLL.GetFilteredAssets(searchQuery, category, state);
            gvAssets.DataSource = dt;
            gvAssets.DataBind();
        }

        private void LoadUserDropdown()
        {
            DataTable dtUsers = userBLL.GetAllUsers();
            ddlUsers.DataSource = dtUsers;
            ddlUsers.DataTextField = "FullName";
            ddlUsers.DataValueField = "UserID";
            ddlUsers.DataBind();

            // Add unassign option
            ddlUsers.Items.Insert(0, new ListItem("-- Unassigned / Return to Inventory --", "0"));
        }

        protected void btnSearchFilter_Click(object sender, EventArgs e)
        {
            LoadInventory();
        }

        protected void btnAddAsset_Click(object sender, EventArgs e)
        {
            lblAssetModalTitle.Text = "Add New Asset";
            hfEditAssetID.Value = string.Empty;
            txtAssetTag.Text = string.Empty;
            txtDeviceName.Text = string.Empty;
            txtWarranty.Text = DateTime.Now.AddYears(3).ToString("yyyy-MM-dd");

            pnlAssetModal.Visible = true;
            pnlMessage.Visible = false;
        }

        protected void gvAssets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            int assetId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditAsset")
            {
                Asset asset = assetBLL.GetAssetById(assetId);
                if (asset != null)
                {
                    lblAssetModalTitle.Text = $"Edit Asset #{asset.AssetTag}";
                    hfEditAssetID.Value = asset.AssetID.ToString();
                    txtAssetTag.Text = asset.AssetTag;
                    txtDeviceName.Text = asset.DeviceName;
                    ddlEditCategory.SelectedValue = asset.Category;
                    txtWarranty.Text = asset.WarrantyExpiry?.ToString("yyyy-MM-dd");

                    pnlAssetModal.Visible = true;
                    pnlMessage.Visible = false;
                }
            }
            else if (e.CommandName == "AssignAsset")
            {
                hfAssignAssetID.Value = assetId.ToString();
                lblAssignTitle.InnerText = $"ID: #{assetId}";
                pnlAssignModal.Visible = true;
                pnlMessage.Visible = false;
            }
            else if (e.CommandName == "RetireAsset")
            {
                hfRetireAssetID.Value = assetId.ToString();
                lblRetireID.Text = assetId.ToString();
                pnlRetireModal.Visible = true;
                pnlMessage.Visible = false;
            }
        }

        protected void btnSaveAsset_Click(object sender, EventArgs e)
        {
            Asset asset = new Asset
            {
                AssetTag = txtAssetTag.Text.Trim(),
                DeviceName = txtDeviceName.Text.Trim(),
                Category = ddlEditCategory.SelectedValue,
                WarrantyExpiry = DateTime.TryParse(txtWarranty.Text, out DateTime wDate) ? (DateTime?)wDate : null
            };

            if (string.IsNullOrEmpty(hfEditAssetID.Value))
            {
                assetBLL.AddAsset(asset);
                lblMessage.Text = "Asset successfully created.";
            }
            else
            {
                asset.AssetID = Convert.ToInt32(hfEditAssetID.Value);
                assetBLL.UpdateAsset(asset);
                lblMessage.Text = "Asset details updated.";
            }

            pnlAssetModal.Visible = false;
            pnlMessage.Visible = true;
            LoadInventory();
        }

        protected void btnSaveAssignment_Click(object sender, EventArgs e)
        {
            int assetId = Convert.ToInt32(hfAssignAssetID.Value);
            int selectedUserId = Convert.ToInt32(ddlUsers.SelectedValue);

            assetBLL.AssignAsset(assetId, selectedUserId);

            pnlAssignModal.Visible = false;
            lblMessage.Text = selectedUserId == 0 ? "Asset successfully unassigned." : "Asset successfully assigned.";
            pnlMessage.Visible = true;
            LoadInventory();
        }

        protected void btnConfirmRetire_Click(object sender, EventArgs e)
        {
            int assetId = Convert.ToInt32(hfRetireAssetID.Value);
            assetBLL.RetireAsset(assetId);

            pnlRetireModal.Visible = false;
            lblMessage.Text = "Asset marked as retired.";
            pnlMessage.CssClass = "alert alert-warning d-flex align-items-center";
            pnlMessage.Visible = true;
            LoadInventory();
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlAssetModal.Visible = false;
            pnlAssignModal.Visible = false;
            pnlRetireModal.Visible = false;
        }
    }
}