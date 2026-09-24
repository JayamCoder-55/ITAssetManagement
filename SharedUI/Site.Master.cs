using System;

namespace ITAssetManagement
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/UI/Auth/Login.aspx");
                    return;
                }

                lblUserName.Text = Session["Name"]?.ToString() ?? "User";
                string role = Session["Role"]?.ToString() ?? "";
                lblUserRole.Text = string.IsNullOrEmpty(role) ? "Guest" : role;

                pnlAdminMenu.Visible = false;
                pnlTechMenu.Visible = false;
                pnlEmployeeMenu.Visible = false;

                if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    pnlAdminMenu.Visible = true;
                    pnlTechMenu.Visible = true;
                    pnlEmployeeMenu.Visible = true;
                }
                else if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
                {
                    pnlTechMenu.Visible = true;
                    pnlEmployeeMenu.Visible = true;
                }
                else if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase))
                {
                    pnlEmployeeMenu.Visible = true;
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/UI/Auth/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}