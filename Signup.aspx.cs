using System;
using Expense_Tracker.Data;

namespace Expense_Tracker
{
    public partial class Signup : System.Web.UI.Page
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

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowError("All fields are required.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Passwords do not match.");
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Password must be at least 6 characters long.");
                return;
            }

            try
            {
                // Check if username exists
                if (DatabaseHelper.CheckUsernameExists(username))
                {
                    ShowError("Username already exists. Please choose another one.");
                    return;
                }

                // Check if email exists
                if (DatabaseHelper.CheckEmailExists(email))
                {
                    ShowError("Email already registered. Please use another email.");
                    return;
                }

                // Register user
                bool success = DatabaseHelper.RegisterUser(username, email, password);

                if (success)
                {
                    ShowSuccess("Registration successful! Redirecting to login...");
                    System.Threading.Thread.Sleep(1500);
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    ShowError("Registration failed. Please try again.");
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
            lblSuccess.CssClass = "success-message";
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.CssClass = "success-message show";
            lblError.CssClass = "error-message";
        }
    }
}