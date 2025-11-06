<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Expense_Tracker.Dashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard - Expense Tracker</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #F8FAFC;
            min-height: 100vh;
        }

        .container { display: flex; min-height: 100vh; }

        /* Sidebar Styles */
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

        .welcome-card {
            background: linear-gradient(135deg, #06B6D4 0%, #0891B2 100%);
            color: white;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(6, 182, 212, 0.3);
            margin-bottom: 30px;
        }

        .welcome-card h2 { font-size: 28px; margin-bottom: 10px; }
        .welcome-card p { font-size: 16px; opacity: 0.9; }

        .dashboard-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
            margin-bottom: 30px;
        }

        .card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
        }

        .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
        }

        .card-title { font-size: 14px; color: #64748B; font-weight: 500; }

        .card-icon {
            width: 40px;
            height: 40px;
            background: #DBEAFE;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #2563EB;
            font-size: 20px;
        }

        .card-value {
            font-size: 32px;
            font-weight: 700;
            color: #1E293B;
            margin-bottom: 5px;
        }

        .card-info { font-size: 13px; color: #10B981; }

        /* Spending Trends Card */
        .trends-card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 30px;
        }

        .trends-header {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid #F1F5F9;
        }

        .trends-header h3 {
            font-size: 18px;
            color: #1E293B;
            font-weight: 600;
        }

        .trend-row {
            display: flex;
            align-items: center;
            padding: 15px 0;
            border-bottom: 1px solid #F1F5F9;
        }

        .trend-day {
            width: 120px;
            font-weight: 600;
            color: #475569;
            font-size: 14px;
        }

        .trend-date {
            font-size: 12px;
            color: #94A3B8;
            display: block;
            margin-top: 2px;
        }

        .trend-bar-container {
            flex: 1;
            height: 30px;
            background: #F1F5F9;
            border-radius: 6px;
            overflow: hidden;
            margin: 0 15px;
            position: relative;
        }

        .trend-bar {
            height: 100%;
            background: linear-gradient(90deg, #06B6D4, #0891B2);
            border-radius: 6px;
            transition: width 0.6s ease;
            display: flex;
            align-items: center;
            justify-content: flex-end;
            padding-right: 10px;
        }

        .trend-amount {
            font-weight: 700;
            color: #1E293B;
            font-size: 14px;
            min-width: 100px;
            text-align: right;
        }

        /* Heatmap Card */
        .heatmap-card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 30px;
        }

        .heatmap-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid #F1F5F9;
        }

        .heatmap-title {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .heatmap-title h3 {
            font-size: 18px;
            color: #1E293B;
            font-weight: 600;
        }

        .month-navigation {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .nav-btn {
            width: 32px;
            height: 32px;
            border: 1px solid #E2E8F0;
            background: white;
            border-radius: 6px;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            transition: all 0.3s;
        }

        .nav-btn:hover {
            background: #F1F5F9;
            border-color: #CBD5E1;
        }

        .current-month-btn {
            padding: 6px 12px;
            background: #06B6D4;
            color: white;
            border: none;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .current-month-btn:hover {
            background: #0891B2;
        }

        .calendar-grid {
            display: grid;
            grid-template-columns: repeat(7, 1fr);
            gap: 8px;
            margin-top: 15px;
        }

        .calendar-day-label {
            text-align: center;
            font-size: 12px;
            font-weight: 600;
            color: #64748B;
            padding: 8px 0;
        }

        .calendar-day {
            aspect-ratio: 1;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            position: relative;
        }

        .calendar-day:hover {
            transform: scale(1.1);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        }

        .calendar-day.empty {
            background: transparent;
            cursor: default;
        }

        .calendar-day.empty:hover {
            transform: none;
            box-shadow: none;
        }

        .calendar-day.low {
            background: #D1FAE5;
            color: #065F46;
        }

        .calendar-day.medium {
            background: #FEF3C7;
            color: #92400E;
        }

        .calendar-day.high {
            background: #FECACA;
            color: #7F1D1D;
        }

        .calendar-day.very-high {
            background: #EF4444;
            color: white;
        }

        .heatmap-legend {
            display: flex;
            align-items: center;
            gap: 20px;
            margin-top: 20px;
            padding-top: 15px;
            border-top: 1px solid #F1F5F9;
        }

        .legend-item {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 12px;
            color: #64748B;
        }

        .legend-box {
            width: 20px;
            height: 20px;
            border-radius: 4px;
        }

        .data-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

        .data-table th,
        .data-table td {
            padding: 12px;
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

        .amount-positive { color: #10B981; font-weight: 600; }
        .amount-negative { color: #EF4444; font-weight: 600; }

        .no-data {
            text-align: center;
            padding: 30px;
            color: #94A3B8;
            font-size: 14px;
        }

        .section-title {
            color: #1E293B;
            margin-bottom: 15px;
            font-size: 18px;
            font-weight: 600;
        }

        .tooltip {
            position: absolute;
            background: #1E293B;
            color: white;
            padding: 8px 12px;
            border-radius: 6px;
            font-size: 12px;
            pointer-events: none;
            opacity: 0;
            transition: opacity 0.3s;
            z-index: 1000;
            white-space: nowrap;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Sidebar -->
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
                    <a href="Dashboard.aspx" class="nav-item active">
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
                    <a href="Reports.aspx" class="nav-item">
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
                    <h1 class="page-title">Dashboard</h1>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>

                <div class="welcome-card">
                    <h2>Welcome back, <asp:Label ID="lblWelcomeUser" runat="server"></asp:Label>!</h2>
                    <p>Here's your expense tracking overview</p>
                </div>

                <!-- Stats Cards -->
                <div class="dashboard-grid">
                    <div class="card">
                        <div class="card-header">
                            <div class="card-title">Expense</div>
                            <div class="card-icon">💰</div>
                        </div>
                        <div class="card-value"><asp:Label ID="lblBalance" runat="server">₹ 0.00</asp:Label></div>
                        <div class="card-info"><asp:Label ID="lblBalanceStatus" runat="server">Loading...</asp:Label></div>
                    </div>

                    <div class="card">
                        <div class="card-header">
                            <div class="card-title">Total Income</div>
                            <div class="card-icon">📈</div>
                        </div>
                        <div class="card-value"><asp:Label ID="lblTotalIncome" runat="server">₹ 0.00</asp:Label></div>
                        <div class="card-info"><asp:Label ID="lblIncomeCount" runat="server">0</asp:Label> income records</div>
                    </div>

                    <div class="card">
                        <div class="card-header">
                            <div class="card-title">Total Budget</div>
                            <div class="card-icon">📉</div>
                        </div>
                        <div class="card-value"><asp:Label ID="lblTotalBudget" runat="server">₹ 0.00</asp:Label></div>
                        <div class="card-info">From categories</div>
                    </div>
                </div>

                <!-- Spending Trends (Last 7 Days) -->
                <div class="trends-card">
                    <div class="trends-header">
                        <span style="font-size: 24px;">📈</span>
                        <h3>Spending Trends (Last 7 Days)</h3>
                    </div>
                    <asp:Repeater ID="rptSpendingTrends" runat="server">
                        <ItemTemplate>
                            <div class="trend-row">
                                <div class="trend-day">
                                    <%# Eval("DayName") %>
                                    <span class="trend-date"><%# ((DateTime)Eval("Date")).ToString("MMM dd") %></span>
                                </div>
                                <div class="trend-bar-container">
                                    <div class="trend-bar" style='width: <%# Eval("Percentage") %>%;'></div>
                                </div>
                                <div class="trend-amount">₹ <%# String.Format("{0:N2}", Eval("Amount")) %></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblNoTrends" runat="server" CssClass="no-data" 
                        Text="No spending data for the last 7 days" Visible="false"></asp:Label>
                </div>

                <!-- Spending Heatmap -->
                <div class="heatmap-card">
                    <div class="heatmap-header">
                        <div class="heatmap-title">
                            <span style="font-size: 24px;">🔥</span>
                            <h3>Spending Heatmap - <asp:Label ID="lblHeatmapMonth" runat="server"></asp:Label></h3>
                        </div>
                        <div class="month-navigation">
                            <asp:Button ID="btnPrevMonth" runat="server" Text="◀" CssClass="nav-btn" 
                                OnClick="btnPrevMonth_Click" />
                            <asp:Button ID="btnNextMonth" runat="server" Text="▶" CssClass="nav-btn" 
                                OnClick="btnNextMonth_Click" />
                        </div>
                    </div>
                    
                    <div class="calendar-grid">
                        <!-- Day Labels -->
                        <div class="calendar-day-label">Mon</div>
                        <div class="calendar-day-label">Tue</div>
                        <div class="calendar-day-label">Wed</div>
                        <div class="calendar-day-label">Thu</div>
                        <div class="calendar-day-label">Fri</div>
                        <div class="calendar-day-label">Sat</div>
                        <div class="calendar-day-label">Sun</div>
                        
                        <!-- Calendar Days -->
                        <asp:Repeater ID="rptCalendarDays" runat="server">
                            <ItemTemplate>
                                <div class='calendar-day <%# Eval("IntensityClass") %>' 
                                     title='<%# Eval("Tooltip") %>'>
                                    <%# Eval("Day") %>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="heatmap-legend">
                        <span style="font-weight: 600; color: #475569;">Intensity:</span>
                        <div class="legend-item">
                            <div class="legend-box" style="background: #D1FAE5;"></div>
                            <span>Low (₹0-1k)</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-box" style="background: #FEF3C7;"></div>
                            <span>Medium (₹1k-3k)</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-box" style="background: #FECACA;"></div>
                            <span>High (₹3k-5k)</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-box" style="background: #EF4444;"></div>
                            <span>Very High (₹5k+)</span>
                        </div>
                    </div>
                </div>

                <!-- Recent Activity Section -->
                <div class="dashboard-grid" style="grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));">
                    <div class="card">
                        <h3 class="section-title">Recent Income</h3>
                        <asp:GridView ID="gvRecentIncome" runat="server" AutoGenerateColumns="False" 
                            CssClass="data-table" GridLines="None" ShowHeader="True">
                            <Columns>
                                <asp:BoundField DataField="Source" HeaderText="Source" />
                                <asp:TemplateField HeaderText="Amount">
                                    <ItemTemplate>
                                        <span class="amount-positive">₹ <%# String.Format("{0:N2}", Eval("Amount")) %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="DateReceived" HeaderText="Date" DataFormatString="{0:dd-MMM}" />
                            </Columns>
                        </asp:GridView>
                        <asp:Label ID="lblNoIncome" runat="server" CssClass="no-data" 
                            Text="No income records yet. Add your first income!" Visible="false"></asp:Label>
                    </div>

                    <div class="card">
                        <h3 class="section-title">Top Categories</h3>
                        <asp:GridView ID="gvTopCategories" runat="server" AutoGenerateColumns="False" 
                            CssClass="data-table" GridLines="None" ShowHeader="True">
                            <Columns>
                                <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                                <asp:TemplateField HeaderText="Budget">
                                    <ItemTemplate>
                                        <span class="amount-negative">₹ <%# String.Format("{0:N2}", Eval("Amount")) %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Label ID="lblNoCategories" runat="server" CssClass="no-data" 
                            Text="No categories yet. Create your first category!" Visible="false"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>