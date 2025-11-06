using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Expense_Tracker
{
    public partial class Income : System.Web.UI.Page
    {
        // UPDATE THIS connection string to match your database path
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Komal\OneDrive\Important Folder\ExpenseTracker.mdf;Integrated Security=True;Connect Timeout=30";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
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

        // READ - Load all income records (no UserID filter as table doesn't have it)
        void BindGrid()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(
                        "SELECT i.IncomeID, i.Source, i.Amount, i.DateReceived, i.Description " +
                        "FROM Income i ORDER BY i.DateReceived DESC", con);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error loading income records: " + ex.Message + "');</script>");
            }
        }

        // CREATE - Add new income record (CategoryID set to NULL)
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtSource.Text))
                {
                    Response.Write("<script>alert('Please enter income source!');</script>");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    Response.Write("<script>alert('Please enter amount!');</script>");
                    return;
                }

                decimal amount = Convert.ToDecimal(txtAmount.Text);

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Income (CategoryID, Source, Amount, Description, DateReceived) " +
                    "VALUES (NULL, @Source, @Amount, @Description, @DateReceived)", con))
                {
                    cmd.Parameters.AddWithValue("@Source", txtSource.Text.Trim());
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@DateReceived", DateTime.Now);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                ClearFields();
                BindGrid();
                Response.Write("<script>alert('Income added successfully!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error adding income: " + ex.Message + "');</script>");
            }
        }

        // UPDATE - Update existing income record
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtIncomeID.Text))
                {
                    Response.Write("<script>alert('Please select an income record to update!');</script>");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSource.Text))
                {
                    Response.Write("<script>alert('Please enter income source!');</script>");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    Response.Write("<script>alert('Please enter amount!');</script>");
                    return;
                }

                int userID = Convert.ToInt32(Session["UserID"]);
                decimal amount = Convert.ToDecimal(txtAmount.Text);

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Income SET Source=@Source, Amount=@Amount, Description=@Description " +
                    "WHERE IncomeID=@IncomeID AND UserID=@UserID", con))
                {
                    cmd.Parameters.AddWithValue("@IncomeID", Convert.ToInt32(txtIncomeID.Text));
                    cmd.Parameters.AddWithValue("@Source", txtSource.Text.Trim());
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Response.Write("<script>alert('Income updated successfully!');</script>");
                        ClearFields();
                        BindGrid();
                        btnUpdate.Visible = false;
                        btnAdd.Visible = true;
                    }
                    else
                    {
                        Response.Write("<script>alert('No income record was updated.');</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error updating income: " + ex.Message + "');</script>");
            }
        }

        // DELETE - Remove income record
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                int incomeID = Convert.ToInt32(btn.CommandArgument);

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Income WHERE IncomeID=@IncomeID", con))
                {
                    cmd.Parameters.AddWithValue("@IncomeID", incomeID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                BindGrid();
                Response.Write("<script>alert('Income deleted successfully!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error deleting income: " + ex.Message + "');</script>");
            }
        }

        // EDIT - Load income data into form
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                int incomeID = Convert.ToInt32(btn.CommandArgument);

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Income WHERE IncomeID=@IncomeID", con))
                {
                    cmd.Parameters.AddWithValue("@IncomeID", incomeID);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtIncomeID.Text = reader["IncomeID"].ToString();
                        txtSource.Text = reader["Source"].ToString();
                        txtAmount.Text = reader["Amount"].ToString();
                        txtDescription.Text = reader["Description"].ToString();

                        btnUpdate.Visible = true;
                        btnAdd.Visible = false;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error loading income: " + ex.Message + "');</script>");
            }
        }

        // Clear all input fields
        void ClearFields()
        {
            txtIncomeID.Text = "";
            txtSource.Text = "";
            txtAmount.Text = "";
            txtDescription.Text = "";

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