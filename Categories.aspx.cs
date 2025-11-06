using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Expense_Tracker
{
    public partial class Categories : System.Web.UI.Page
    {
        // ✅ FIXED connection string (wrapped in quotes)
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Komal\OneDrive\Important Folder\ExpenseTracker.mdf;Integrated Security=True;Connect Timeout=30";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                string username = Session["Username"].ToString();
                string email = Session["Email"].ToString();

                lblUsername.Text = username;
                lblEmail.Text = email;
                lblUserInitial.Text = username.Substring(0, 1).ToUpper();

                BindGrid();
            }
        }

        // READ - Load all categories for logged-in user
        void BindGrid()
        {
            try
            {
                int userID = Convert.ToInt32(Session["UserID"]);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(
                        "SELECT CategoryID, CategoryName, Description, Amount, CreatedDate FROM Categories WHERE UserID = @UserID ORDER BY CreatedDate DESC", con);

                    da.SelectCommand.Parameters.AddWithValue("@UserID", userID);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error loading categories: " + ex.Message + "');</script>");
            }
        }

        // CREATE - Add new category
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ddlCategoryName.SelectedValue))
                {
                    Response.Write("<script>alert('Please select a category name!');</script>");
                    return;
                }

                int userID = Convert.ToInt32(Session["UserID"]);
                decimal amount = 0;

                if (!string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    amount = Convert.ToDecimal(txtAmount.Text);
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Categories (CategoryName, Description, Amount, UserID, CreatedDate) VALUES (@CategoryName, @Description, @Amount, @UserID, GETDATE())", con))
                {
                    cmd.Parameters.AddWithValue("@CategoryName", ddlCategoryName.SelectedValue);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                ClearFields();
                BindGrid();
                Response.Write("<script>alert('Category added successfully!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error adding category: " + ex.Message + "');</script>");
            }
        }

        // UPDATE - Update existing category
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCategoryID.Text))
                {
                    Response.Write("<script>alert('Please select a category to update!');</script>");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlCategoryName.SelectedValue))
                {
                    Response.Write("<script>alert('Please select a category name!');</script>");
                    return;
                }

                int userID = Convert.ToInt32(Session["UserID"]);
                decimal amount = 0;

                if (!string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    amount = Convert.ToDecimal(txtAmount.Text);
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Categories SET CategoryName=@CategoryName, Description=@Description, Amount=@Amount WHERE CategoryID=@CategoryID AND UserID=@UserID", con))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", Convert.ToInt32(txtCategoryID.Text));
                    cmd.Parameters.AddWithValue("@CategoryName", ddlCategoryName.SelectedValue);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Response.Write("<script>alert('Category updated successfully!');</script>");
                        ClearFields();
                        BindGrid();
                        btnUpdate.Visible = false;
                        btnAdd.Visible = true;
                    }
                    else
                    {
                        Response.Write("<script>alert('No category was updated.');</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error updating category: " + ex.Message + "');</script>");
            }
        }

        // DELETE - Remove category
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                int categoryID = Convert.ToInt32(btn.CommandArgument);
                int userID = Convert.ToInt32(Session["UserID"]);

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Categories WHERE CategoryID=@CategoryID AND UserID=@UserID", con))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                BindGrid();
                Response.Write("<script>alert('Category deleted successfully!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error deleting category: " + ex.Message + "');</script>");
            }
        }

        // EDIT - Load category data into form
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                int categoryID = Convert.ToInt32(btn.CommandArgument);
                int userID = Convert.ToInt32(Session["UserID"]);

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Categories WHERE CategoryID=@CategoryID AND UserID=@UserID", con))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtCategoryID.Text = reader["CategoryID"].ToString();

                        string categoryValue = reader["CategoryName"].ToString();
                        if (ddlCategoryName.Items.FindByValue(categoryValue) != null)
                            ddlCategoryName.SelectedValue = categoryValue;
                        else
                            ddlCategoryName.SelectedIndex = 0;

                        txtDescription.Text = reader["Description"].ToString();
                        txtAmount.Text = reader["Amount"].ToString();

                        btnUpdate.Visible = true;
                        btnAdd.Visible = false;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error loading category: " + ex.Message + "');</script>");
            }
        }

        // Clear all input fields
        void ClearFields()
        {
            txtCategoryID.Text = "";
            ddlCategoryName.SelectedIndex = 0;
            txtDescription.Text = "";
            txtAmount.Text = "";

            btnAdd.Visible = true;
            btnUpdate.Visible = false;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
