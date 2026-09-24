using System;
using System.Data;
using System.Web.UI.WebControls;
using ITAssetManagement.BLL;
using ITAssetManagement.Models;

namespace ITAssetManagement.UI.Admin
{
    public partial class ManageUsers : System.Web.UI.Page
    {
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
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            DataTable dt = userBLL.GetAllUsers();
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }

        protected void btnCreateUser_Click(object sender, EventArgs e)
        {
            lblUserModalTitle.Text = "Create New User";
            hfEditUserID.Value = string.Empty;
            txtEmail.Text = string.Empty;
            txtFullName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            pnlPassword.Visible = true;
            ddlRole.SelectedIndex = 0;

            pnlUserModal.Visible = true;
            pnlMessage.Visible = false;
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Safe parse to prevent FormatException if CommandArgument is empty or null
            if (e.CommandArgument == null || !int.TryParse(e.CommandArgument.ToString(), out int userId))
            {
                return;
            }

            if (e.CommandName == "EditRole")
            {
                User user = userBLL.GetUserById(userId);
                if (user != null)
                {
                    lblUserModalTitle.Text = "Edit User Details";
                    hfEditUserID.Value = user.UserId.ToString();
                    txtEmail.Text = user.Email;
                    txtFullName.Text = user.FullName;
                    ddlRole.SelectedValue = user.RoleId.ToString();

                    // Bind Department if control exists
                    if (ddlDepartment != null && !string.IsNullOrEmpty(user.Department))
                    {
                        ddlDepartment.SelectedValue = user.Department;
                    }

                    pnlPassword.Visible = false; // Do not display password field on edit
                    pnlUserModal.Visible = true;
                    pnlMessage.Visible = false;
                }
            }
            else if (e.CommandName == "ResetPassword")
            {
                string tempPassword = userBLL.ResetPassword(userId);
                lblMessage.Text = $"Password successfully reset for User #{userId}. Temporary Password: <strong>{tempPassword}</strong>";
                pnlMessage.CssClass = "alert alert-success d-flex align-items-center";
                pnlMessage.Visible = true;
            }
            else if (e.CommandName == "Deactivate")
            {
                // Opens deactivation confirmation modal
                hfDeactivateUserID.Value = userId.ToString();
                pnlDeactivateModal.Visible = true;
                pnlMessage.Visible = false;
            }
            else if (e.CommandName == "Activate")
            {
                // Re-activates user directly and displays success message
                bool isUpdated = userBLL.UpdateUserStatus(userId, true); // Sets IsActive = 1

                if (isUpdated)
                {
                    lblMessage.Text = $"User #{userId} has been successfully reactivated.";
                    pnlMessage.CssClass = "alert alert-success d-flex align-items-center";
                }
                else
                {
                    lblMessage.Text = "Failed to reactivate the account. Please try again.";
                    pnlMessage.CssClass = "alert alert-danger d-flex align-items-center";
                }

                pnlMessage.Visible = true;
                LoadUsers(); // Refresh GridView list
            }
        }

        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            User newUser = new User
            {
                Email = txtEmail.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                RoleId = Convert.ToInt32(ddlRole.SelectedValue),
                Department = ddlDepartment.SelectedValue // Captures selected department
            };

            string defaultPassword = txtPassword.Text.Trim();

            bool isSuccess = userBLL.CreateUser(newUser, defaultPassword);

            if (isSuccess)
            {
                // Reset form controls
                txtEmail.Text = string.Empty;
                txtFullName.Text = string.Empty;
                txtPassword.Text = string.Empty;
                ddlDepartment.SelectedIndex = 0;

                LoadUsers(); // Refresh GridView
            }
            else
            {
                // Show error alert
            }
        }


        protected void btnConfirmDeactivate_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(hfDeactivateUserID.Value);
            userBLL.DeactivateUser(userId);

            pnlDeactivateModal.Visible = false;
            lblMessage.Text = "User account deactivated.";
            pnlMessage.CssClass = "alert alert-warning d-flex align-items-center";
            pnlMessage.Visible = true;
            LoadUsers();
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlUserModal.Visible = false;
            pnlDeactivateModal.Visible = false;
        }


    }
}