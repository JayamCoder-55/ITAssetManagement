using System;
using ITAssetManagement.BLL;
using ITAssetManagement.Models;

namespace ITAssetManagement.UI.Auth
{
    public partial class Login : System.Web.UI.Page
    {
        private UserBLL userBLL = new UserBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlAlert.Visible = false;

                // Redirect if user is already logged in
                if (Session["UserID"] != null || Session["UserId"] != null)
                {
                    string role = (Session["Role"] ?? Session["RoleName"])?.ToString();
                    if (!string.IsNullOrEmpty(role))
                    {
                        RedirectBasedOnRole(role);
                    }
                }
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            // Validate against SQL Database using UserBLL
            User user = userBLL.ValidateLogin(email, password);

            if (user != null)
            {
                SetSessionAndRedirect(user);
            }
            else
            {
                lblError.Text = "Invalid email or password.";
                pnlAlert.CssClass = "alert alert-danger d-flex align-items-center";
                pnlAlert.Visible = true;

                txtPassword.Text = string.Empty;
                txtPassword.Focus();
            }
        }

        private void SetSessionAndRedirect(User user)
        {
            // Primary session keys required by Site.Master
            Session["UserID"] = user.UserId;
            Session["Name"] = user.FullName;
            Session["Role"] = user.RoleName;

            // Secondary session keys to maintain compatibility across other pages
            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["RoleId"] = user.RoleId;
            Session["RoleName"] = user.RoleName;

            RedirectBasedOnRole(user.RoleName);
        }

        private void RedirectBasedOnRole(string role)
        {
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                Response.Redirect("~/UI/Admin/ManageAssets.aspx");
            else if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
                Response.Redirect("~/UI/Tech/TicketQueue.aspx");
            else
                Response.Redirect("~/UI/Employee/MyAssets.aspx");
        }

        protected void lnkForgotPassword_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string newPassword = txtPassword.Text;

            // Validate email entry
            if (string.IsNullOrEmpty(email))
            {
                lblError.Text = "Please enter your Email Address first.";
                pnlAlert.CssClass = "alert alert-warning d-flex align-items-center";
                pnlAlert.Visible = true;
                txtEmail.Focus();
                return;
            }

            // Validate new password entry
            if (string.IsNullOrEmpty(newPassword))
            {
                lblError.Text = "Please type your new password into the Password box, then click 'Forgot?' to update it.";
                pnlAlert.CssClass = "alert alert-warning d-flex align-items-center";
                pnlAlert.Visible = true;
                txtPassword.Focus();
                return;
            }

            // Update password directly in database
            bool isUpdated = userBLL.UpdatePasswordByEmail(email, newPassword);

            if (isUpdated)
            {
                lblError.Text = "Your password has been successfully updated! You can now click <strong>Sign In</strong>.";
                pnlAlert.CssClass = "alert alert-success d-flex align-items-center";
                pnlAlert.Visible = true;
            }
            else
            {
                lblError.Text = "No active account found matching that email address.";
                pnlAlert.CssClass = "alert alert-danger d-flex align-items-center";
                pnlAlert.Visible = true;
            }
        }
    }
}