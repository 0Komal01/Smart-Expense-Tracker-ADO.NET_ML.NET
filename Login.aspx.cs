using System;
using System.Data;
using Expense_Tracker.Data;

namespace Expense_Tracker
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    Response.Redirect("Dashboard.aspx");
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter both username and password.");
                return;
            }

            try
            {
                DataRow user = DatabaseHelper.ValidateUser(username, password);

                if (user != null)
                {
                    Session["UserID"] = user["UserID"];
                    Session["Username"] = user["Username"];
                    Session["Email"] = user["Email"];
                    Response.Redirect("Dashboard.aspx");
                }
                else
                {
                    ShowError("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                ShowError("An error occurred: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.CssClass = "error-message show";
        }
    }
}