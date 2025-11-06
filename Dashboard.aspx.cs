using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Expense_Tracker
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Komal\OneDrive\Important Folder\ExpenseTracker.mdf;Integrated Security=True;Connect Timeout=30";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Initialize ViewState for current month/year
                ViewState["CurrentMonth"] = DateTime.Now.Month;
                ViewState["CurrentYear"] = DateTime.Now.Year;

                LoadUserProfile();
                LoadDashboardData();
                LoadSpendingTrends();
                LoadSpendingHeatmap();
            }
        }

        private void LoadUserProfile()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Username, Email FROM Users WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string username = reader["Username"].ToString();
                    lblUsername.Text = username;
                    lblWelcomeUser.Text = username;
                    lblEmail.Text = reader["Email"].ToString();
                    lblUserInitial.Text = username.Substring(0, 1).ToUpper();
                }
                reader.Close();
            }
        }

        private void LoadDashboardData()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Get Total Income (Income linked to user's categories)
                string incomeQuery = @"SELECT ISNULL(SUM(i.Amount), 0) as TotalIncome, COUNT(*) as IncomeCount 
                                      FROM Income i
                                      LEFT JOIN Categories c ON i.CategoryID = c.CategoryID
                                      WHERE c.UserID = @UserID OR i.CategoryID IS NULL";
                SqlCommand incomeCmd = new SqlCommand(incomeQuery, conn);
                incomeCmd.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader incomeReader = incomeCmd.ExecuteReader();

                if (incomeReader.Read())
                {
                    decimal totalIncome = Convert.ToDecimal(incomeReader["TotalIncome"]);
                    int incomeCount = Convert.ToInt32(incomeReader["IncomeCount"]);

                    lblTotalIncome.Text = "₹ " + totalIncome.ToString("N2");
                    lblIncomeCount.Text = incomeCount.ToString();
                }
                incomeReader.Close();

                // Get Total Budget from Categories
                string budgetQuery = "SELECT ISNULL(SUM(Amount), 0) as TotalBudget FROM Categories WHERE UserID = @UserID";
                SqlCommand budgetCmd = new SqlCommand(budgetQuery, conn);
                budgetCmd.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader budgetReader = budgetCmd.ExecuteReader();

                if (budgetReader.Read())
                {
                    decimal totalBudget = Convert.ToDecimal(budgetReader["TotalBudget"]);

                    lblTotalBudget.Text = "₹ " + totalBudget.ToString("N2");
                }
                budgetReader.Close();

                // Calculate Balance (Income - Budget)
                decimal balance = Convert.ToDecimal(lblTotalIncome.Text.Replace("₹ ", "").Replace(",", "")) -
                                  Convert.ToDecimal(lblTotalBudget.Text.Replace("₹ ", "").Replace(",", ""));

                lblBalance.Text = "₹ " + balance.ToString("N2");
                lblBalanceStatus.Text = balance >= 0 ? "Healthy balance" : "Over budget";

                // Load Recent Income
                LoadRecentIncome(userId);

                // Load Top Categories
                LoadTopCategories(userId);
            }
        }

        private void LoadRecentIncome(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 5 i.Source, i.Amount, i.DateReceived 
                                FROM Income i
                                LEFT JOIN Categories c ON i.CategoryID = c.CategoryID
                                WHERE c.UserID = @UserID OR i.CategoryID IS NULL
                                ORDER BY i.DateReceived DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvRecentIncome.DataSource = dt;
                    gvRecentIncome.DataBind();
                    lblNoIncome.Visible = false;
                }
                else
                {
                    gvRecentIncome.Visible = false;
                    lblNoIncome.Visible = true;
                }
            }
        }

        private void LoadTopCategories(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 5 CategoryName, Amount 
                                FROM Categories 
                                WHERE UserID = @UserID 
                                ORDER BY Amount DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvTopCategories.DataSource = dt;
                    gvTopCategories.DataBind();
                    lblNoCategories.Visible = false;
                }
                else
                {
                    gvTopCategories.Visible = false;
                    lblNoCategories.Visible = true;
                }
            }
        }

        private void LoadSpendingTrends()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            List<SpendingTrend> trends = new List<SpendingTrend>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Get spending for last 7 days from Categories (using CreatedDate as proxy for spending date)
                string query = @"SELECT 
                                    DATEADD(day, -number, CAST(GETDATE() AS DATE)) as SpendingDate,
                                    ISNULL(SUM(c.Amount), 0) as DailySpending
                                FROM master..spt_values
                                LEFT JOIN Categories c ON CAST(c.CreatedDate AS DATE) = DATEADD(day, -number, CAST(GETDATE() AS DATE))
                                    AND c.UserID = @UserID
                                WHERE type = 'P' AND number BETWEEN 0 AND 6
                                GROUP BY DATEADD(day, -number, CAST(GETDATE() AS DATE))
                                ORDER BY SpendingDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                decimal maxAmount = 0;
                while (reader.Read())
                {
                    decimal amount = Convert.ToDecimal(reader["DailySpending"]);
                    if (amount > maxAmount) maxAmount = amount;

                    trends.Add(new SpendingTrend
                    {
                        Date = Convert.ToDateTime(reader["SpendingDate"]),
                        Amount = amount
                    });
                }
                reader.Close();

                // Calculate percentages
                foreach (var trend in trends)
                {
                    trend.Percentage = maxAmount > 0 ? (trend.Amount / maxAmount * 100) : 0;
                }

                if (trends.Count > 0 && trends.Any(t => t.Amount > 0))
                {
                    rptSpendingTrends.DataSource = trends;
                    rptSpendingTrends.DataBind();
                    lblNoTrends.Visible = false;
                }
                else
                {
                    rptSpendingTrends.Visible = false;
                    lblNoTrends.Visible = true;
                }
            }
        }

        private void LoadSpendingHeatmap()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            // Get month and year from ViewState (defaults to current month)
            int month = ViewState["CurrentMonth"] != null ? (int)ViewState["CurrentMonth"] : DateTime.Now.Month;
            int year = ViewState["CurrentYear"] != null ? (int)ViewState["CurrentYear"] : DateTime.Now.Year;

            DateTime targetDate = new DateTime(year, month, 1);
            DateTime firstDayOfMonth = new DateTime(year, month, 1);

            lblHeatmapMonth.Text = targetDate.ToString("MMMM yyyy");

            List<CalendarDay> calendarDays = new List<CalendarDay>();
            Dictionary<int, decimal> dailySpending = new Dictionary<int, decimal>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Get spending by day for selected month
                string query = @"SELECT 
                                    DAY(CreatedDate) as DayNumber,
                                    SUM(Amount) as TotalSpending
                                FROM Categories
                                WHERE UserID = @UserID 
                                    AND MONTH(CreatedDate) = @Month 
                                    AND YEAR(CreatedDate) = @Year
                                GROUP BY DAY(CreatedDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int day = Convert.ToInt32(reader["DayNumber"]);
                    decimal spending = Convert.ToDecimal(reader["TotalSpending"]);
                    dailySpending[day] = spending;
                }
                reader.Close();
            }

            // Add empty cells for days before first day of month
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            // Adjust for Monday start (0 = Sunday in C#, we want Monday = 0)
            firstDayOfWeek = (firstDayOfWeek + 6) % 7;

            for (int i = 0; i < firstDayOfWeek; i++)
            {
                calendarDays.Add(new CalendarDay
                {
                    Day = "",
                    IntensityClass = "empty",
                    Tooltip = ""
                });
            }

            // Add all days of the month
            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                decimal spending = dailySpending.ContainsKey(day) ? dailySpending[day] : 0;
                string intensityClass = GetIntensityClass(spending);
                string tooltip = $"{targetDate.ToString("MMMM")} {day}, {year}: ₹{spending:N2}";

                calendarDays.Add(new CalendarDay
                {
                    Day = day.ToString(),
                    IntensityClass = intensityClass,
                    Tooltip = tooltip
                });
            }

            rptCalendarDays.DataSource = calendarDays;
            rptCalendarDays.DataBind();
        }

        private string GetIntensityClass(decimal amount)
        {
            if (amount == 0) return "empty";
            if (amount < 1000) return "low";
            if (amount < 3000) return "medium";
            if (amount < 5000) return "high";
            return "very-high";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnPrevMonth_Click(object sender, EventArgs e)
        {
            int currentMonth = (int)ViewState["CurrentMonth"];
            int currentYear = (int)ViewState["CurrentYear"];

            currentMonth--;
            if (currentMonth < 1)
            {
                currentMonth = 12;
                currentYear--;
            }

            ViewState["CurrentMonth"] = currentMonth;
            ViewState["CurrentYear"] = currentYear;

            LoadSpendingHeatmap();
        }

        protected void btnNextMonth_Click(object sender, EventArgs e)
        {
            int currentMonth = (int)ViewState["CurrentMonth"];
            int currentYear = (int)ViewState["CurrentYear"];

            currentMonth++;
            if (currentMonth > 12)
            {
                currentMonth = 1;
                currentYear++;
            }

            ViewState["CurrentMonth"] = currentMonth;
            ViewState["CurrentYear"] = currentYear;

            LoadSpendingHeatmap();
        }

        // Helper classes
        public class SpendingTrend
        {
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public decimal Percentage { get; set; }
            public string DayName => Date.ToString("ddd");
        }

        public class CalendarDay
        {
            public string Day { get; set; }
            public string IntensityClass { get; set; }
            public string Tooltip { get; set; }
        }
    }
}