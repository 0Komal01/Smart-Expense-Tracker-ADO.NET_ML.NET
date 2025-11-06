using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Expense_Tracker
{
    public partial class Reports : Page
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Komal\OneDrive\Important Folder\ExpenseTracker.mdf;Integrated Security=True;Connect Timeout=30";
        private int userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    userId = Convert.ToInt32(Session["UserID"]);
                    Session["ReportUserID"] = userId;
                    LoadUserInfo();
                    LoadReportData();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void LoadUserInfo()
        {
            userId = Convert.ToInt32(Session["ReportUserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Username, Email FROM Users WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string username = reader["Username"].ToString();
                        string email = reader["Email"].ToString();

                        lblUsername.Text = username;
                        lblEmail.Text = email;
                        lblUserInitial.Text = username.Substring(0, 1).ToUpper();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Handle error
                }
            }
        }

        private void LoadReportData()
        {
            userId = Convert.ToInt32(Session["ReportUserID"]);

            // Get summary data
            decimal totalIncome = GetTotalIncome();
            decimal totalExpenses = GetTotalExpenses();
            decimal balance = totalIncome - totalExpenses;

            lblTotalIncome.Text = "₹" + totalIncome.ToString("N2");
            lblTotalExpenses.Text = "₹" + totalExpenses.ToString("N2");
            lblBalance.Text = "₹" + balance.ToString("N2");

            if (balance >= 0)
            {
                lblBalance.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblBalance.ForeColor = System.Drawing.Color.Red;
            }

            // Load data for charts
            LoadCategoryExpenses();
            LoadIncomeBreakdown();
            LoadBudgetComparison();
            LoadRecentTransactions();
        }

        private decimal GetTotalIncome()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT ISNULL(SUM(i.Amount), 0) 
                                FROM Income i 
                                LEFT JOIN Categories c ON i.CategoryID = c.CategoryID 
                                WHERE c.UserID = @UserID OR i.CategoryID IS NULL";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        private decimal GetTotalExpenses()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT ISNULL(SUM(Amount), 0) 
                                FROM Categories 
                                WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
                catch
                {
                    return 0;
                }
            }
        }

        private void LoadCategoryExpenses()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT CategoryName, Amount 
                                FROM Categories 
                                WHERE UserID = @UserID AND Amount > 0
                                ORDER BY Amount DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    StringBuilder chartData = new StringBuilder();
                    chartData.Append("[");

                    bool first = true;
                    while (reader.Read())
                    {
                        if (!first) chartData.Append(",");
                        chartData.AppendFormat("['{0}', {1}]",
                            reader["CategoryName"],
                            reader["Amount"]);
                        first = false;
                    }

                    chartData.Append("]");
                    hiddenExpenseData.Value = chartData.ToString();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    hiddenExpenseData.Value = "[]";
                }
            }
        }

        private void LoadIncomeBreakdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT i.Source, SUM(i.Amount) as TotalAmount 
                                FROM Income i
                                LEFT JOIN Categories c ON i.CategoryID = c.CategoryID
                                WHERE c.UserID = @UserID OR i.CategoryID IS NULL
                                GROUP BY i.Source
                                ORDER BY TotalAmount DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    StringBuilder chartData = new StringBuilder();
                    chartData.Append("[");

                    bool first = true;
                    while (reader.Read())
                    {
                        if (!first) chartData.Append(",");
                        chartData.AppendFormat("['{0}', {1}]",
                            reader["Source"],
                            reader["TotalAmount"]);
                        first = false;
                    }

                    chartData.Append("]");
                    hiddenIncomeData.Value = chartData.ToString();
                    reader.Close();
                }
                catch
                {
                    hiddenIncomeData.Value = "[]";
                }
            }
        }

        // NEW: Load Budget Comparison Data
        private void LoadBudgetComparison()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        DATENAME(MONTH, DATEFROMPARTS(b.Year, b.Month, 1)) + ' ' + CAST(b.Year AS VARCHAR) AS MonthYear,
                        b.BudgetAmount,
                        ISNULL(SUM(c.Amount), 0) AS ActualSpent
                    FROM Budget b
                    LEFT JOIN Categories c
                        ON b.UserID = c.UserID
                        AND MONTH(c.CreatedDate) = b.Month 
                        AND YEAR(c.CreatedDate) = b.Year
                    WHERE b.UserID = @UserID
                    GROUP BY b.BudgetID, b.Month, b.Year, b.BudgetAmount
                    ORDER BY b.Year ASC, b.Month ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    StringBuilder chartData = new StringBuilder();
                    chartData.Append("[");

                    bool first = true;
                    while (reader.Read())
                    {
                        if (!first) chartData.Append(",");
                        chartData.AppendFormat("['{0}', {1}, {2}]",
                            reader["MonthYear"],
                            reader["BudgetAmount"],
                            reader["ActualSpent"]);
                        first = false;
                    }

                    chartData.Append("]");
                    hiddenBudgetData.Value = chartData.ToString();
                    reader.Close();
                }
                catch
                {
                    hiddenBudgetData.Value = "[]";
                }
            }
        }

        private void LoadRecentTransactions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 10 
                                    i.Source as TransactionName,
                                    i.Amount,
                                    i.DateReceived as TransactionDate,
                                    'Income' as Type
                                FROM Income i
                                LEFT JOIN Categories c ON i.CategoryID = c.CategoryID
                                WHERE c.UserID = @UserID OR i.CategoryID IS NULL
                                ORDER BY i.DateReceived DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvTransactions.DataSource = dt;
                    gvTransactions.DataBind();
                }
                catch (Exception ex)
                {
                    // Handle error
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnGeneratePDF_Click(object sender, EventArgs e)
        {
            try
            {
                userId = Convert.ToInt32(Session["ReportUserID"]);

                // Get user details
                string username = GetUsername();

                // Create PDF document
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                MemoryStream ms = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Add title
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.DARK_GRAY);
                Paragraph title = new Paragraph("Financial Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10f;
                document.Add(title);

                // Add user info
                Font regularFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                document.Add(new Paragraph("Generated for: " + username, regularFont));
                document.Add(new Paragraph("Report Date: " + DateTime.Now.ToString("dd MMM yyyy"), regularFont));
                document.Add(new Paragraph(" "));

                // Add summary section
                Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DARK_GRAY);
                Paragraph summaryTitle = new Paragraph("Financial Summary", sectionFont);
                summaryTitle.SpacingBefore = 10f;
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 100;
                summaryTable.AddCell(CreateCell("Total Income:", regularFont, BaseColor.LIGHT_GRAY));
                summaryTable.AddCell(CreateCell(lblTotalIncome.Text, regularFont, BaseColor.WHITE));
                summaryTable.AddCell(CreateCell("Total Expenses:", regularFont, BaseColor.LIGHT_GRAY));
                summaryTable.AddCell(CreateCell(lblTotalExpenses.Text, regularFont, BaseColor.WHITE));
                summaryTable.AddCell(CreateCell("Balance:", regularFont, BaseColor.LIGHT_GRAY));
                summaryTable.AddCell(CreateCell(lblBalance.Text, regularFont, BaseColor.WHITE));
                document.Add(summaryTable);

                // NEW: Add Budget Comparison Section
                document.Add(new Paragraph(" "));
                Paragraph budgetTitle = new Paragraph("Budget vs Actual Spending (Monthly)", sectionFont);
                budgetTitle.SpacingBefore = 10f;
                budgetTitle.SpacingAfter = 10f;
                document.Add(budgetTitle);

                PdfPTable budgetTable = new PdfPTable(3);
                budgetTable.WidthPercentage = 100;
                budgetTable.SetWidths(new float[] { 2f, 1.5f, 1.5f });
                budgetTable.AddCell(CreateCell("Month", regularFont, BaseColor.LIGHT_GRAY, true));
                budgetTable.AddCell(CreateCell("Budget Amount", regularFont, BaseColor.LIGHT_GRAY, true));
                budgetTable.AddCell(CreateCell("Actual Spent", regularFont, BaseColor.LIGHT_GRAY, true));

                DataTable budgetData = GetBudgetData();
                foreach (DataRow row in budgetData.Rows)
                {
                    budgetTable.AddCell(CreateCell(row["MonthYear"].ToString(), regularFont, BaseColor.WHITE));
                    budgetTable.AddCell(CreateCell("₹" + Convert.ToDecimal(row["BudgetAmount"]).ToString("N2"), regularFont, BaseColor.WHITE));

                    decimal actualSpent = Convert.ToDecimal(row["ActualSpent"]);
                    decimal budgetAmount = Convert.ToDecimal(row["BudgetAmount"]);

                    PdfPCell spentCell = CreateCell("₹" + actualSpent.ToString("N2"), regularFont, BaseColor.WHITE);
                    if (actualSpent > budgetAmount)
                    {
                        spentCell.BackgroundColor = new BaseColor(254, 202, 202); // Light red
                    }
                    budgetTable.AddCell(spentCell);
                }
                document.Add(budgetTable);

                // Add category breakdown
                document.Add(new Paragraph(" "));
                Paragraph categoryTitle = new Paragraph("Expense Breakdown by Category", sectionFont);
                categoryTitle.SpacingBefore = 10f;
                categoryTitle.SpacingAfter = 10f;
                document.Add(categoryTitle);

                PdfPTable categoryTable = new PdfPTable(2);
                categoryTable.WidthPercentage = 100;
                categoryTable.AddCell(CreateCell("Category", regularFont, BaseColor.LIGHT_GRAY, true));
                categoryTable.AddCell(CreateCell("Amount", regularFont, BaseColor.LIGHT_GRAY, true));

                DataTable categories = GetCategoryData();
                foreach (DataRow row in categories.Rows)
                {
                    categoryTable.AddCell(CreateCell(row["CategoryName"].ToString(), regularFont, BaseColor.WHITE));
                    categoryTable.AddCell(CreateCell("₹" + Convert.ToDecimal(row["Amount"]).ToString("N2"), regularFont, BaseColor.WHITE));
                }
                document.Add(categoryTable);

                // Add income breakdown
                document.Add(new Paragraph(" "));
                Paragraph incomeTitle = new Paragraph("Income Breakdown by Source", sectionFont);
                incomeTitle.SpacingBefore = 10f;
                incomeTitle.SpacingAfter = 10f;
                document.Add(incomeTitle);

                PdfPTable incomeTable = new PdfPTable(2);
                incomeTable.WidthPercentage = 100;
                incomeTable.AddCell(CreateCell("Source", regularFont, BaseColor.LIGHT_GRAY, true));
                incomeTable.AddCell(CreateCell("Amount", regularFont, BaseColor.LIGHT_GRAY, true));

                DataTable incomes = GetIncomeData();
                foreach (DataRow row in incomes.Rows)
                {
                    incomeTable.AddCell(CreateCell(row["Source"].ToString(), regularFont, BaseColor.WHITE));
                    incomeTable.AddCell(CreateCell("₹" + Convert.ToDecimal(row["TotalAmount"]).ToString("N2"), regularFont, BaseColor.WHITE));
                }
                document.Add(incomeTable);

                // Add financial tips
                document.Add(new Paragraph(" "));
                Paragraph tipsTitle = new Paragraph("Financial Management Tips", sectionFont);
                tipsTitle.SpacingBefore = 15f;
                tipsTitle.SpacingAfter = 10f;
                document.Add(tipsTitle);

                string[] tips = GetFinancialTips();
                iTextSharp.text.List tipsList = new iTextSharp.text.List(iTextSharp.text.List.UNORDERED);
                tipsList.SetListSymbol("•");
                tipsList.IndentationLeft = 20f;

                foreach (string tip in tips)
                {
                    iTextSharp.text.ListItem item = new iTextSharp.text.ListItem(tip, regularFont);
                    item.SpacingAfter = 5f;
                    tipsList.Add(item);
                }
                document.Add(tipsList);

                // Close document
                document.Close();
                writer.Close();

                // Download PDF
                byte[] bytes = ms.ToArray();
                ms.Close();

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition",
                    "attachment; filename=FinancialReport_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
                Response.Buffer = true;
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
            }
            catch (Exception ex)
            {
                // Handle error
                Response.Write("<script>alert('Error generating PDF: " + ex.Message + "');</script>");
            }
        }

        private PdfPCell CreateCell(string text, Font font, BaseColor bgColor, bool isHeader = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = bgColor;
            cell.Padding = 8f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;

            if (isHeader)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
            }

            return cell;
        }

        private string GetUsername()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Username FROM Users WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString() ?? "User";
                }
                catch
                {
                    return "User";
                }
            }
        }

        // NEW: Get Budget Data for PDF
        private DataTable GetBudgetData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        DATENAME(MONTH, DATEFROMPARTS(b.Year, b.Month, 1)) + ' ' + CAST(b.Year AS VARCHAR) AS MonthYear,
                        b.BudgetAmount,
                        ISNULL(SUM(c.Amount), 0) AS ActualSpent
                    FROM Budget b
                    LEFT JOIN Categories c
                        ON b.UserID = c.UserID
                        AND MONTH(c.CreatedDate) = b.Month 
                        AND YEAR(c.CreatedDate) = b.Year
                    WHERE b.UserID = @UserID
                    GROUP BY b.BudgetID, b.Month, b.Year, b.BudgetAmount
                    ORDER BY b.Year ASC, b.Month ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        private DataTable GetCategoryData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT CategoryName, Amount 
                                FROM Categories 
                                WHERE UserID = @UserID AND Amount > 0
                                ORDER BY Amount DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        private DataTable GetIncomeData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT i.Source, SUM(i.Amount) as TotalAmount 
                                FROM Income i
                                LEFT JOIN Categories c ON i.CategoryID = c.CategoryID
                                WHERE c.UserID = @UserID OR i.CategoryID IS NULL
                                GROUP BY i.Source
                                ORDER BY TotalAmount DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        private string[] GetFinancialTips()
        {
            decimal totalIncome = 0;
            decimal totalExpenses = 0;

            try
            {
                totalIncome = Convert.ToDecimal(lblTotalIncome.Text.Replace("₹", "").Replace(",", ""));
                totalExpenses = Convert.ToDecimal(lblTotalExpenses.Text.Replace("₹", "").Replace(",", ""));
            }
            catch { }

            decimal balance = totalIncome - totalExpenses;

            List<string> tips = new List<string>();

            if (totalExpenses > totalIncome)
            {
                tips.Add("Your expenses exceed your income. Consider reducing unnecessary spending.");
                tips.Add("Review your categories and identify areas where you can cut costs.");
            }
            else if (totalIncome > 0)
            {
                tips.Add("Great job! You're spending less than you earn. Keep it up!");
                decimal savingsPercent = (balance / totalIncome) * 100;
                tips.Add("Consider saving or investing " + savingsPercent.ToString("N0") + "% of your income.");
            }

            // Add budget-specific tips
            DataTable budgetData = GetBudgetData();
            foreach (DataRow row in budgetData.Rows)
            {
                decimal budgetAmount = Convert.ToDecimal(row["BudgetAmount"]);
                decimal actualSpent = Convert.ToDecimal(row["ActualSpent"]);

                if (actualSpent > budgetAmount)
                {
                    tips.Add("You exceeded your budget in " + row["MonthYear"].ToString() +
                            ". Review spending in this period to identify improvement areas.");
                    break; // Only show one budget warning
                }
            }

            tips.Add("Follow the 50/30/20 rule: 50% needs, 30% wants, 20% savings.");
            tips.Add("Create an emergency fund covering 3-6 months of expenses.");
            tips.Add("Track every expense, no matter how small, to identify spending patterns.");
            tips.Add("Set specific financial goals and review them monthly.");
            tips.Add("Automate your savings by setting up automatic transfers.");
            tips.Add("Compare prices before making purchases to get the best deals.");

            return tips.ToArray();
        }
    }
}