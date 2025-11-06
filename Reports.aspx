<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="Expense_Tracker.Reports" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reports - Expense Tracker</title>
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #F8FAFC;
            min-height: 100vh;
        }

        .container { display: flex; min-height: 100vh; }

        /* Sidebar Styles - Same as Dashboard */
        .sidebar {
            width: 260px;
            background: #1E293B;
            color: white;
            padding: 20px;
            position: fixed;
            height: 100vh;
            overflow-y: auto;
        }

        .sidebar-header {
            display: flex;
            align-items: center;
            justify-content: center;
            padding-bottom: 20px;
            border-bottom: 1px solid #334155;
            margin-bottom: 20px;
        }

        .logo-image {
            max-width: 200px;
            height: auto;
            display: block;
        }

        .user-profile {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 15px;
            background: #334155;
            border-radius: 10px;
            margin-bottom: 25px;
        }

        .user-avatar {
            width: 45px;
            height: 45px;
            background: #06B6D4;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            font-weight: bold;
        }

        .user-info h3 { font-size: 15px; margin-bottom: 3px; }
        .user-info p { font-size: 12px; color: #94A3B8; }

        .nav-section { margin-bottom: 25px; }

        .nav-section-title {
            font-size: 11px;
            text-transform: uppercase;
            color: #06B6D4;
            font-weight: 600;
            margin-bottom: 10px;
            letter-spacing: 0.5px;
        }

        .nav-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 12px 15px;
            color: #CBD5E1;
            text-decoration: none;
            border-radius: 8px;
            margin-bottom: 5px;
            transition: all 0.3s;
            cursor: pointer;
        }

        .nav-item:hover { background: #334155; color: white; }
        .nav-item.active { background: #06B6D4; color: white; }

        .nav-icon {
            width: 20px;
            height: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        /* Main Content */
        .main-content {
            margin-left: 260px;
            flex: 1;
            padding: 30px;
        }

        .top-bar {
            background: white;
            padding: 20px 30px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .page-title { font-size: 26px; color: #1E293B; font-weight: 600; }

        .logout-btn {
            padding: 10px 20px;
            background: #EF4444;
            color: white;
            border: none;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .logout-btn:hover { background: #DC2626; transform: translateY(-1px); }

        .summary-cards {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 25px;
        }
        
        .card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
        }
        
        .card h3 {
            font-size: 14px;
            color: #64748B;
            margin-bottom: 10px;
            text-transform: uppercase;
            font-weight: 600;
            letter-spacing: 0.5px;
        }
        
        .card .amount {
            font-size: 32px;
            font-weight: bold;
            color: #1E293B;
        }
        
        .income-card .amount { color: #10b981; }
        .expense-card .amount { color: #ef4444; }
        .balance-card .amount { color: #3b82f6; }
        
        .charts-section {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
            gap: 20px;
            margin-bottom: 25px;
        }
        
        .chart-card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
        }
        
        .chart-card h2 {
            color: #1E293B;
            margin-bottom: 20px;
            font-size: 18px;
            font-weight: 600;
        }

        .chart-card.full-width {
            grid-column: 1 / -1;
        }
        
        .tips-section {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 25px;
        }
        
        .tips-section h2 {
            color: #1E293B;
            margin-bottom: 20px;
            font-size: 20px;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 10px;
        }
        
        .tips-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 15px;
        }
        
        .tip-card {
            background: linear-gradient(135deg, #06B6D4 0%, #0891B2 100%);
            padding: 20px;
            border-radius: 10px;
            color: white;
            font-size: 14px;
            line-height: 1.6;
        }
        
        .tip-card::before {
            content: "💡";
            font-size: 24px;
            display: block;
            margin-bottom: 10px;
        }
        
        .transactions-section {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 25px;
        }
        
        .transactions-section h2 {
            color: #1E293B;
            margin-bottom: 20px;
            font-size: 20px;
            font-weight: 600;
        }
        
        .data-table {
            width: 100%;
            border-collapse: collapse;
        }
        
        .data-table th,
        .data-table td {
            padding: 12px 15px;
            text-align: left;
            border-bottom: 1px solid #E2E8F0;
        }
        
        .data-table th {
            background: #F8FAFC;
            color: #475569;
            font-weight: 600;
            font-size: 13px;
        }
        
        .data-table td { color: #334155; font-size: 14px; }
        
        .data-table tr:hover {
            background: #F8FAFC;
        }
        
        .pdf-section {
            text-align: center;
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
        }
        
        .btn-pdf {
            padding: 15px 40px;
            background: linear-gradient(135deg, #06B6D4 0%, #0891B2 100%);
            color: white;
            border: none;
            border-radius: 10px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 600;
            transition: all 0.3s;
            box-shadow: 0 4px 15px rgba(6, 182, 212, 0.3);
        }
        
        .btn-pdf:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(6, 182, 212, 0.4);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Sidebar - Same as Dashboard -->
            <div class="sidebar">
                <div class="sidebar-header">
                    <img src="Image/logo.png" alt="Expense Tracker" class="logo-image" />
                </div>

                <div class="user-profile">
                    <div class="user-avatar">
                        <asp:Label ID="lblUserInitial" runat="server"></asp:Label>
                    </div>
                    <div class="user-info">
                        <h3><asp:Label ID="lblUsername" runat="server"></asp:Label></h3>
                        <p><asp:Label ID="lblEmail" runat="server"></asp:Label></p>
                    </div>
                </div>

                <div class="nav-section">
                    <div class="nav-section-title">GENERAL</div>
                    <a href="Dashboard.aspx" class="nav-item">
                        <div class="nav-icon">📊</div>
                        <span>Dashboard</span>
                    </a>
                    <a href="Categories.aspx" class="nav-item">
                        <div class="nav-icon">💸</div>
                        <span>Expenses</span>
                    </a>
                    <a href="Income.aspx" class="nav-item">
                        <div class="nav-icon">💵</div>
                        <span>Income</span>
                    </a>
                    <a href="Budget.aspx" class="nav-item">
                        <div class="nav-icon">🪙</div>
                        <span>Budget</span>
                    </a>
                </div>

                <div class="nav-section">
                    <div class="nav-section-title">EXTRAS</div>
                    <a href="Reports.aspx" class="nav-item active">
                        <div class="nav-icon">📈</div>
                        <span>Reports</span>
                    </a>
                    <a href="MLPredictions.aspx" class="nav-item">
                        <div class="nav-icon">🤖</div>
                        <span>Advance(ML)</span>
                    </a>
                </div>
            </div>

            <!-- Main Content -->
            <div class="main-content">
                <div class="top-bar">
                    <h1 class="page-title">📈 Financial Reports</h1>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>

                <div class="summary-cards">
                    <div class="card income-card">
                        <h3>Total Income</h3>
                        <asp:Label ID="lblTotalIncome" runat="server" CssClass="amount">₹0.00</asp:Label>
                    </div>
                    <div class="card expense-card">
                        <h3>Total Expenses</h3>
                        <asp:Label ID="lblTotalExpenses" runat="server" CssClass="amount">₹0.00</asp:Label>
                    </div>
                    <div class="card balance-card">
                        <h3>Balance</h3>
                        <asp:Label ID="lblBalance" runat="server" CssClass="amount">₹0.00</asp:Label>
                    </div>
                </div>

                <div class="charts-section">
                    <div class="chart-card">
                        <h2>📊 Expense Breakdown</h2>
                        <div id="expenseChart" style="width: 100%; height: 400px;"></div>
                    </div>
                    <div class="chart-card">
                        <h2>💰 Income Sources</h2>
                        <div id="incomeChart" style="width: 100%; height: 400px;"></div>
                    </div>
                </div>

                <!-- Budget Comparison Chart -->
                <div class="charts-section">
                    <div class="chart-card full-width">
                        <h2>🪙 Budget vs Actual Spending (Monthly)</h2>
                        <div id="budgetChart" style="width: 100%; height: 400px;"></div>
                    </div>
                </div>

                <div class="tips-section">
                    <h2>💡 Financial Management Tips</h2>
                    <div class="tips-grid">
                        <div class="tip-card">
                            Compare prices and look for deals before making purchases to maximize your purchasing power.
                        </div>
                    </div>
                </div>

                <div class="transactions-section">
                    <h2>📝 Recent Transactions</h2>
                    <asp:GridView ID="gvTransactions" runat="server" CssClass="data-table" 
                                  AutoGenerateColumns="False" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="TransactionName" HeaderText="Description" />
                            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="₹{0:N2}" />
                            <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
                            <asp:BoundField DataField="Type" HeaderText="Type" />
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="pdf-section">
                    <asp:Button ID="btnGeneratePDF" runat="server" Text="📄 Generate PDF Report" 
                                CssClass="btn-pdf" OnClick="btnGeneratePDF_Click" />
                </div>

                <asp:HiddenField ID="hiddenExpenseData" runat="server" />
                <asp:HiddenField ID="hiddenIncomeData" runat="server" />
                <asp:HiddenField ID="hiddenBudgetData" runat="server" />
            </div>
        </div>
    </form>

    <script type="text/javascript">
        google.charts.load('current', { 'packages': ['corechart', 'bar'] });
        google.charts.setOnLoadCallback(drawCharts);

        function drawCharts() {
            drawExpenseChart();
            drawIncomeChart();
            drawBudgetChart();
        }

        function drawExpenseChart() {
            var expenseDataStr = document.getElementById('<%= hiddenExpenseData.ClientID %>').value;

            if (expenseDataStr === '[]' || !expenseDataStr) {
                document.getElementById('expenseChart').innerHTML = '<p style="text-align:center; padding:50px; color:#94A3B8;">No expense data available</p>';
                return;
            }

            var expenseData = eval(expenseDataStr);
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Category');
            data.addColumn('number', 'Amount');
            data.addRows(expenseData);

            var options = {
                title: 'Expenses by Category',
                pieHole: 0.4,
                colors: ['#ef4444', '#f59e0b', '#10b981', '#3b82f6', '#8b5cf6', '#ec4899'],
                chartArea: { width: '90%', height: '75%' },
                legend: { position: 'bottom' },
                fontSize: 13
            };

            var chart = new google.visualization.PieChart(document.getElementById('expenseChart'));
            chart.draw(data, options);
        }

        function drawIncomeChart() {
            var incomeDataStr = document.getElementById('<%= hiddenIncomeData.ClientID %>').value;

            if (incomeDataStr === '[]' || !incomeDataStr) {
                document.getElementById('incomeChart').innerHTML = '<p style="text-align:center; padding:50px; color:#94A3B8;">No income data available</p>';
                return;
            }

            var incomeData = eval(incomeDataStr);
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Source');
            data.addColumn('number', 'Amount');
            data.addRows(incomeData);

            var options = {
                title: 'Income by Source',
                pieHole: 0.4,
                colors: ['#10b981', '#059669', '#047857', '#065f46', '#064e3b'],
                chartArea: { width: '90%', height: '75%' },
                legend: { position: 'bottom' },
                fontSize: 13
            };

            var chart = new google.visualization.PieChart(document.getElementById('incomeChart'));
            chart.draw(data, options);
        }

        function drawBudgetChart() {
            var budgetDataStr = document.getElementById('<%= hiddenBudgetData.ClientID %>').value;

            if (budgetDataStr === '[]' || !budgetDataStr) {
                document.getElementById('budgetChart').innerHTML = '<p style="text-align:center; padding:50px; color:#94A3B8;">No budget data available</p>';
                return;
            }

            var budgetDataArray = eval(budgetDataStr);

            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Month');
            data.addColumn('number', 'Budget Amount');
            data.addColumn('number', 'Actual Spent');
            data.addRows(budgetDataArray);

            var options = {
                title: 'Monthly Budget vs Actual Spending',
                colors: ['#06B6D4', '#EF4444'],
                chartArea: { width: '85%', height: '70%' },
                legend: { position: 'top' },
                fontSize: 13,
                vAxis: {
                    title: 'Amount (₹)',
                    format: '₹#,###'
                },
                hAxis: {
                    title: 'Month'
                },
                bar: { groupWidth: '75%' }
            };

            var chart = new google.visualization.ColumnChart(document.getElementById('budgetChart'));
            chart.draw(data, options);
        }

        window.addEventListener('resize', function () {
            drawCharts();
        });
    </script>
</body>
</html>