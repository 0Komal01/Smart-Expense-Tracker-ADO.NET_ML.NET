using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Expense_Tracker
{
    public partial class Budget : System.Web.UI.Page
    {
        string cs = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Komal\OneDrive\Important Folder\ExpenseTracker.mdf;Integrated Security=True;Connect Timeout=30";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Username"] == null || Session["Email"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserInfo();
                BindMonthYearDropdowns();
                BindBudgetGrid();
                LoadBudgetChart();
            }
        }

        void LoadUserInfo()
        {
            lblUsername.Text = Session["Username"]?.ToString() ?? "";
            lblEmail.Text = Session["Email"]?.ToString() ?? "";
            lblUserInitial.Text = !string.IsNullOrEmpty(lblUsername.Text)
                ? lblUsername.Text.Substring(0, 1).ToUpper()
                : "?";
        }

        void BindMonthYearDropdowns()
        {
            ddlMonth.Items.Clear();
            for (int i = 1; i <= 12; i++)
                ddlMonth.Items.Add(new ListItem(new DateTime(2000, i, 1).ToString("MMMM"), i.ToString()));

            ddlYear.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear - 1; y <= currentYear + 1; y++)
                ddlYear.Items.Add(new ListItem(y.ToString(), y.ToString()));

            ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
            ddlYear.SelectedValue = DateTime.Now.Year.ToString();
        }

        void BindBudgetGrid()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        b.BudgetID,
                        DATENAME(MONTH, DATEFROMPARTS(b.Year, b.Month, 1)) AS MonthName,
                        b.Month,
                        b.Year,
                        b.BudgetAmount,
                        ISNULL(SUM(c.Amount), 0) AS ActualSpent,
                        (b.BudgetAmount - ISNULL(SUM(c.Amount), 0)) AS Remaining
                    FROM Budget b
                    LEFT JOIN Categories c
                        ON b.UserID = c.UserID
                        AND MONTH(c.CreatedDate) = b.Month 
                        AND YEAR(c.CreatedDate) = b.Year
                    WHERE b.UserID = @UserID
                    GROUP BY b.BudgetID, b.Month, b.Year, b.BudgetAmount
                    ORDER BY b.Year DESC, b.Month DESC", con);

                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvBudgets.DataSource = dt;
                gvBudgets.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtBudgetAmount.Text, out decimal amount))
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        IF EXISTS (SELECT 1 FROM Budget WHERE UserID=@UserID AND Month=@Month AND Year=@Year)
                            UPDATE Budget SET BudgetAmount=@Amount 
                            WHERE UserID=@UserID AND Month=@Month AND Year=@Year
                        ELSE
                            INSERT INTO Budget(UserID, BudgetAmount, Month, Year) 
                            VALUES(@UserID, @Amount, @Month, @Year)", con);

                    cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);
                    cmd.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                    cmd.ExecuteNonQuery();
                }

                txtBudgetAmount.Text = "";
                BindBudgetGrid();
                LoadBudgetChart();
            }
        }

        protected void gvBudgets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int budgetId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "DeleteBudget")
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Budget WHERE BudgetID=@ID AND UserID=@UserID", con);
                    cmd.Parameters.AddWithValue("@ID", budgetId);
                    cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    cmd.ExecuteNonQuery();
                }
                BindBudgetGrid();
                LoadBudgetChart();
            }
            else if (e.CommandName == "EditBudget")
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Budget WHERE BudgetID=@ID AND UserID=@UserID", con);
                    cmd.Parameters.AddWithValue("@ID", budgetId);
                    cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        txtBudgetAmount.Text = rdr["BudgetAmount"].ToString();
                        ddlMonth.SelectedValue = rdr["Month"].ToString();
                        ddlYear.SelectedValue = rdr["Year"].ToString();
                    }
                }
            }
        }

        protected void gvBudgets_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                decimal remaining = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Remaining"));
                if (remaining < 0)
                {
                    e.Row.Cells[4].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[4].Font.Bold = true;
                }
                else
                {
                    e.Row.Cells[4].ForeColor = System.Drawing.Color.Green;
                    e.Row.Cells[4].Font.Bold = true;
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        // ✅ UPDATED: Monthly Comparison Chart (Bar Chart)
        private void LoadBudgetChart()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        DATENAME(MONTH, DATEFROMPARTS(b.Year, b.Month, 1)) + ' ' + CAST(b.Year AS VARCHAR) AS MonthYear,
                        b.BudgetAmount,
                        ISNULL(SUM(c.Amount), 0) AS ActualSpent,
                        b.Month,
                        b.Year
                    FROM Budget b
                    LEFT JOIN Categories c
                        ON b.UserID = c.UserID
                        AND MONTH(c.CreatedDate) = b.Month 
                        AND YEAR(c.CreatedDate) = b.Year
                    WHERE b.UserID = @UserID
                    GROUP BY b.BudgetID, b.Month, b.Year, b.BudgetAmount
                    ORDER BY b.Year ASC, b.Month ASC", con);

                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                System.Collections.Generic.List<string> labels = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<decimal> budgetData = new System.Collections.Generic.List<decimal>();
                System.Collections.Generic.List<decimal> spentData = new System.Collections.Generic.List<decimal>();

                while (rdr.Read())
                {
                    labels.Add(rdr["MonthYear"].ToString());
                    budgetData.Add(Convert.ToDecimal(rdr["BudgetAmount"]));
                    spentData.Add(Convert.ToDecimal(rdr["ActualSpent"]));
                }
                rdr.Close();

                if (labels.Count > 0)
                {
                    string labelsJson = "['" + string.Join("','", labels) + "']";
                    string budgetJson = "[" + string.Join(",", budgetData) + "]";
                    string spentJson = "[" + string.Join(",", spentData) + "]";

                    string script = $@"
                        var ctx = document.getElementById('budgetBarChart').getContext('2d');
                        new Chart(ctx, {{
                            type: 'bar',
                            data: {{
                                labels: {labelsJson},
                                datasets: [
                                    {{
                                        label: 'Budget Amount',
                                        data: {budgetJson},
                                        backgroundColor: '#06B6D4',
                                        borderColor: '#0891B2',
                                        borderWidth: 2
                                    }},
                                    {{
                                        label: 'Actual Spent',
                                        data: {spentJson},
                                        backgroundColor: '#EF4444',
                                        borderColor: '#DC2626',
                                        borderWidth: 2
                                    }}
                                ]
                            }},
                            options: {{
                                responsive: true,
                                maintainAspectRatio: true,
                                plugins: {{
                                    legend: {{ 
                                        position: 'top',
                                        labels: {{
                                            font: {{ size: 14, weight: 'bold' }},
                                            padding: 15
                                        }}
                                    }},
                                    title: {{ 
                                        display: true, 
                                        text: 'Monthly Budget vs Actual Spending',
                                        font: {{ size: 16, weight: 'bold' }},
                                        padding: 20
                                    }}
                                }},
                                scales: {{
                                    y: {{
                                        beginAtZero: true,
                                        ticks: {{
                                            callback: function(value) {{
                                                return '₹' + value.toLocaleString();
                                            }}
                                        }}
                                    }}
                                }}
                            }}
                        }});";

                    ClientScript.RegisterStartupScript(this.GetType(), "budgetChart", script, true);
                }
                else
                {
                    // No data available - show empty chart message
                    string script = @"
                        var ctx = document.getElementById('budgetBarChart').getContext('2d');
                        new Chart(ctx, {
                            type: 'bar',
                            data: {
                                labels: ['No Data'],
                                datasets: [{
                                    label: 'No Budget Data Available',
                                    data: [0],
                                    backgroundColor: '#CBD5E1'
                                }]
                            },
                            options: {
                                responsive: true,
                                plugins: {
                                    legend: { display: false },
                                    title: { 
                                        display: true, 
                                        text: 'Please add budget data to see comparison'
                                    }
                                }
                            }
                        });";
                    ClientScript.RegisterStartupScript(this.GetType(), "budgetChart", script, true);
                }
            }
        }
    }
}