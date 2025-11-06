<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MLPredictions.aspx.cs" Inherits="Expense_Tracker.MLPredictions" %>

<!DOCTYPE html>
<html>
<head>
    <title>Smart Predictions - Expense Tracker</title>
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

        /* Header */
        .header {
            background: white;
            padding: 25px 30px;
            border-radius: 15px;
            margin-bottom: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
        }

        .header-top {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
        }

        .header-left h1 {
            color: #1E293B;
            font-size: 32px;
            margin-bottom: 5px;
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .ml-badge {
            background: linear-gradient(135deg, #06B6D4, #0891B2);
            color: white;
            padding: 6px 14px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
            letter-spacing: 1px;
        }

        .header-left p {
            color: #64748B;
            font-size: 15px;
        }

        .btn-logout {
            padding: 10px 25px;
            background: #EF4444;
            color: white;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            transition: all 0.3s;
        }

        .btn-logout:hover {
            background: #DC2626;
            transform: translateY(-2px);
        }

        /* ML Status Bar */
        .ml-status {
            background: linear-gradient(135deg, #10B981, #059669);
            color: white;
            padding: 12px 20px;
            border-radius: 10px;
            text-align: center;
            font-size: 15px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
        }

        .ml-status-icon {
            font-size: 20px;
            animation: pulse 2s infinite;
        }

        @keyframes pulse {
            0%, 100% { transform: scale(1); }
            50% { transform: scale(1.1); }
        }

        /* Info Banner */
        .info-banner {
            background: linear-gradient(135deg, #DBEAFE, #BFDBFE);
            padding: 20px;
            border-radius: 12px;
            margin-bottom: 25px;
            border-left: 4px solid #3B82F6;
        }

        .info-banner h3 {
            color: #1E40AF;
            font-size: 16px;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .info-banner p {
            color: #1E3A8A;
            font-size: 14px;
            line-height: 1.6;
        }

        /* Grid Layout */
        .ml-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 25px;
            margin-bottom: 30px;
        }

        .ml-grid-full {
            grid-column: 1 / -1;
        }

        .ml-grid-single {
            grid-column: span 1;
        }

        /* Card Styles */
        .card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
            transition: transform 0.3s, box-shadow 0.3s;
        }

        .card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 15px rgba(0,0,0,0.1);
        }

        .card-header {
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid #F1F5F9;
        }

        .card-icon {
            font-size: 36px;
            filter: drop-shadow(0 2px 4px rgba(0,0,0,0.1));
        }

        .card-title {
            flex: 1;
        }

        .card-title h2 {
            color: #1E293B;
            font-size: 22px;
            margin-bottom: 5px;
        }

        .card-title p {
            color: #64748B;
            font-size: 13px;
        }

        .ml-model-badge {
            background: #DBEAFE;
            color: #1D4ED8;
            padding: 5px 12px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: bold;
        }

        /* Explanation Box */
        .explanation-box {
            background: #F0F9FF;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            border-left: 3px solid #0284C7;
        }

        .explanation-box h4 {
            color: #0C4A6E;
            font-size: 14px;
            margin-bottom: 8px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .explanation-box p {
            color: #0369A1;
            font-size: 13px;
            line-height: 1.5;
        }

        /* Time Series Forecasting */
        .forecast-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 15px;
            margin-bottom: 20px;
        }

        .forecast-card {
            background: linear-gradient(135deg, #DBEAFE, #BFDBFE);
            padding: 20px;
            border-radius: 12px;
            text-align: center;
            border: 2px solid #93C5FD;
        }

        .forecast-label {
            color: #1E40AF;
            font-size: 13px;
            margin-bottom: 10px;
            font-weight: 600;
        }

        .forecast-amount {
            font-size: 32px;
            font-weight: bold;
            color: #1E3A8A;
            margin-bottom: 5px;
        }

        .forecast-subtitle {
            font-size: 12px;
            color: #3B82F6;
        }

        .ml-metrics {
            display: flex;
            justify-content: space-around;
            padding: 20px 0;
            border-top: 1px solid #F1F5F9;
            margin-top: 20px;
        }

        .metric-item {
            text-align: center;
        }

        .metric-value {
            font-size: 24px;
            font-weight: bold;
            color: #0891B2;
        }

        .metric-label {
            font-size: 12px;
            color: #64748B;
            margin-top: 5px;
        }

        /* Monthly Trend Styles */
        .trend-stats {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 12px;
            margin-bottom: 15px;
        }

        .trend-stat-card {
            background: linear-gradient(135deg, #FEF3C7, #FDE68A);
            padding: 12px;
            border-radius: 12px;
            text-align: center;
            border: 2px solid #FDE047;
        }

        .trend-stat-label {
            color: #78350F;
            font-size: 11px;
            margin-bottom: 6px;
            font-weight: 600;
        }

        .trend-stat-value {
            font-size: 20px;
            font-weight: bold;
            color: #92400E;
        }

        .trend-up {
            color: #DC2626;
            font-weight: bold;
        }

        .trend-down {
            color: #16A34A;
            font-weight: bold;
        }

        .trend-bars {
            margin: 15px 0;
        }

        .trend-month {
            display: flex;
            align-items: center;
            margin-bottom: 10px;
            gap: 10px;
        }

        .trend-month-name {
            width: 70px;
            font-size: 12px;
            font-weight: 600;
            color: #475569;
        }

        .trend-bar-container {
            flex: 1;
            height: 24px;
            background: #F1F5F9;
            border-radius: 12px;
            overflow: hidden;
            position: relative;
        }

        .trend-bar {
            height: 100%;
            background: linear-gradient(90deg, #F59E0B, #D97706);
            border-radius: 12px;
            transition: width 1s ease;
            display: flex;
            align-items: center;
            justify-content: flex-end;
            padding-right: 8px;
            color: white;
            font-size: 10px;
            font-weight: bold;
        }

        /* Anomaly Detection */
        .anomaly-list {
            max-height: 400px;
            overflow-y: auto;
        }

        .anomaly-item {
            display: flex;
            align-items: center;
            gap: 15px;
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 12px;
            background: linear-gradient(135deg, #FEF2F2, #FEE2E2);
            border-left: 4px solid #EF4444;
        }

        .anomaly-icon {
            font-size: 28px;
        }

        .anomaly-details {
            flex: 1;
        }

        .anomaly-date {
            font-weight: 600;
            color: #1E293B;
            margin-bottom: 4px;
        }

        .anomaly-type {
            font-size: 12px;
            color: #64748B;
        }

        .anomaly-amount {
            font-size: 20px;
            font-weight: bold;
            color: #DC2626;
        }

        /* Clustering */
        .cluster-card {
            background: linear-gradient(135deg, #F5F3FF, #EDE9FE);
            padding: 20px;
            border-radius: 12px;
            margin-bottom: 15px;
            border-left: 5px solid #8B5CF6;
        }

        .cluster-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 12px;
        }

        .cluster-name {
            font-size: 18px;
            font-weight: 600;
            color: #1E293B;
        }

        .cluster-categories {
            color: #475569;
            margin-bottom: 10px;
            padding: 10px;
            background: rgba(255,255,255,0.5);
            border-radius: 8px;
            font-size: 14px;
        }

        .cluster-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding-top: 12px;
            border-top: 1px solid rgba(0,0,0,0.1);
        }

        .cluster-spending {
            font-size: 18px;
            font-weight: bold;
            color: #7C3AED;
        }

        .cluster-insight {
            font-size: 12px;
            color: #64748B;
            font-style: italic;
            margin-top: 8px;
        }

        /* Risk Prediction */
        .risk-container {
            text-align: center;
            padding: 30px;
            border-radius: 15px;
            margin-bottom: 20px;
        }

        .risk-score-circle {
            width: 120px;
            height: 120px;
            border-radius: 50%;
            background: white;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 20px;
            border: 4px solid;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        }

        .risk-score {
            font-size: 32px;
            font-weight: bold;
        }

        .risk-status {
            font-size: 24px;
            margin-bottom: 10px;
            font-weight: 600;
        }

        .risk-recommendation {
            font-size: 15px;
            margin-top: 15px;
            padding: 15px;
            background: white;
            border-radius: 10px;
            color: #1E293B;
        }

        /* Info Box */
        .info-box {
            background: linear-gradient(135deg, #DBEAFE, #BFDBFE);
            padding: 15px 20px;
            border-radius: 10px;
            margin-top: 20px;
            border-left: 4px solid #2563EB;
        }

        .info-box h4 {
            color: #1E40AF;
            margin-bottom: 8px;
            font-size: 14px;
        }

        .info-box p {
            color: #1E3A8A;
            font-size: 13px;
            line-height: 1.6;
        }

        /* Action Buttons */
        .action-buttons {
            display: flex;
            justify-content: center;
            gap: 15px;
            margin-top: 30px;
        }

        .btn-retrain {
            background: linear-gradient(135deg, #06B6D4, #0891B2);
            color: white;
            padding: 14px 35px;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            cursor: pointer;
            transition: all 0.3s;
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 600;
            box-shadow: 0 4px 15px rgba(6,182,212,0.3);
        }

        .btn-retrain:hover {
            transform: translateY(-3px);
            box-shadow: 0 6px 20px rgba(6,182,212,0.4);
        }

        /* Count Badge */
        .count-badge {
            display: inline-block;
            background: #FEE2E2;
            padding: 10px 20px;
            border-radius: 20px;
            margin-bottom: 20px;
        }

        .count-label {
            font-size: 14px;
            color: #64748B;
        }

        .count-value {
            font-size: 20px;
            font-weight: bold;
            color: #DC2626;
        }

        /* Responsive */
        @media (max-width: 768px) {
            .sidebar {
                display: none;
            }
            
            .main-content {
                margin-left: 0;
            }
            
            .ml-grid {
                grid-template-columns: 1fr;
            }
            
            .forecast-grid, .trend-stats {
                grid-template-columns: 1fr;
            }

            .header-top {
                flex-direction: column;
                gap: 15px;
            }
        }
    </style>
</head>
<body>
    <form runat="server">
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
                    <a href="Reports.aspx" class="nav-item">
                        <div class="nav-icon">📈</div>
                        <span>Reports</span>
                    </a>
                    <a href="MLPredictions.aspx" class="nav-item active">
                        <div class="nav-icon">🤖</div>
                        <span>Advance(ML)</span>
                    </a>
                </div>
            </div>

            <!-- Main Content -->
            <div class="main-content">
                <!-- Header -->
                <div class="header">
                    <div class="header-top">
                        <div class="header-left">
                            <h1>
                                🤖 Smart Predictions
                                <span class="ml-badge">AI POWERED</span>
                            </h1>
                            <p>AI analyzing your spending to help you save money</p>
                        </div>
                        <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn-logout" OnClick="btnLogout_Click" />
                    </div>
                    
                    <div class="ml-status">
                        <span class="ml-status-icon">🧠</span>
                        <asp:Label ID="lblMLStatus" runat="server" Text="Loading your predictions..."></asp:Label>
                    </div>
                </div>

                <!-- Info Banner -->
                <div class="info-banner">
                    <h3>💡 What is Machine Learning (ML)?</h3>
                    <p>Machine Learning is like having a smart assistant that learns from your spending habits. It analyzes your past expenses to predict future spending, detect unusual transactions, and group similar expenses together. This helps you make better financial decisions!</p>
                </div>

                <!-- ML Grid -->
                <div class="ml-grid">
                    <!-- 1. Time Series Forecasting -->
                    <asp:Panel ID="pnlMLPrediction" runat="server" CssClass="card ml-grid-full">
                        <div class="card-header">
                            <div class="card-icon">📈</div>
                            <div class="card-title">
                                <h2>Spending Forecast</h2>
                                <p>Your predicted expenses based on past spending</p>
                            </div>
                            <span class="ml-model-badge">AI Powered</span>
                        </div>
                        
                        <div class="explanation-box">
                            <h4>ℹ️ How does this work?</h4>
                            <p>The AI looks at your spending patterns from the last 90 days and predicts how much you'll spend tomorrow, next week, and next month. It's like having a crystal ball for your wallet!</p>
                        </div>
                        
                        <div class="forecast-grid">
                            <div class="forecast-card">
                                <div class="forecast-label">📅 Tomorrow</div>
                                <div class="forecast-amount">
                                    <asp:Label ID="lblNextDayPrediction" runat="server"></asp:Label>
                                </div>
                                <div class="forecast-subtitle">Next Day Prediction</div>
                            </div>
                            
                            <div class="forecast-card">
                                <div class="forecast-label">📆 Next Week</div>
                                <div class="forecast-amount">
                                    <asp:Label ID="lblNext7DayPrediction" runat="server"></asp:Label>
                                </div>
                                <div class="forecast-subtitle">7-Day Forecast</div>
                            </div>
                            
                            <div class="forecast-card">
                                <div class="forecast-label">📅 Next Month</div>
                                <div class="forecast-amount">
                                    <asp:Label ID="lblNext30DayPrediction" runat="server"></asp:Label>
                                </div>
                                <div class="forecast-subtitle">30-Day Forecast</div>
                            </div>
                        </div>
                        
                        <div class="ml-metrics">
                            <div class="metric-item">
                                <div class="metric-value"><asp:Label ID="lblMLConfidence" runat="server"></asp:Label></div>
                                <div class="metric-label">Prediction Accuracy</div>
                            </div>
                            <div class="metric-item">
                                <div class="metric-value"><asp:Label ID="lblTrainingDataPoints" runat="server"></asp:Label></div>
                                <div class="metric-label">Days Analyzed</div>
                            </div>
                        </div>

                        <div class="info-box">
                            <h4>💡 Smart Tip:</h4>
                            <p><asp:Label ID="lblSmartTip" runat="server"></asp:Label></p>
                        </div>
                    </asp:Panel>

                    <!-- 2. Monthly Spending Trend -->
                    <asp:Panel ID="pnlSpendingTrend" runat="server" CssClass="card ml-grid-single">
                        <div class="card-header">
                            <div class="card-icon">📊</div>
                            <div class="card-title">
                                <h2>Monthly Spending Trend</h2>
                                <p>6-month spending comparison</p>
                            </div>
                            <span class="ml-model-badge">Trend Analysis</span>
                        </div>
                        
                        <div class="explanation-box">
                            <h4>ℹ️ What am I seeing?</h4>
                            <p>This shows your spending over the last 6 months. Are you spending more or less? The bars make it easy to spot trends!</p>
                        </div>
                        
                        <div class="trend-stats">
                            <div class="trend-stat-card">
                                <div class="trend-stat-label">Current Month</div>
                                <div class="trend-stat-value">₹<asp:Label ID="lblCurrentMonth" runat="server"></asp:Label></div>
                            </div>
                            
                            <div class="trend-stat-card">
                                <div class="trend-stat-label">Last Month</div>
                                <div class="trend-stat-value">₹<asp:Label ID="lblLastMonth" runat="server"></asp:Label></div>
                            </div>
                            
                            <div class="trend-stat-card">
                                <div class="trend-stat-label">6-Month Average</div>
                                <div class="trend-stat-value">₹<asp:Label ID="lblSixMonthAvg" runat="server"></asp:Label></div>
                            </div>
                        </div>

                        <div style="text-align: center; margin-bottom: 15px;">
                            <div style="display: inline-block; background: #F1F5F9; padding: 8px 16px; border-radius: 20px;">
                                <span style="color: #64748B; font-size: 12px;">Trend: </span>
                                <asp:Label ID="lblCurrentTrend" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="trend-bars">
                            <asp:Repeater ID="rptMonthlyTrend" runat="server">
                                <ItemTemplate>
                                    <div class="trend-month">
                                        <div class="trend-month-name"><%# Eval("MonthName") %></div>
                                        <div class="trend-bar-container">
                                            <div class="trend-bar" style="width: <%# Eval("Percentage") %>%">
                                                ₹<%# ((float)Eval("Amount")).ToString("N0") %>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <div class="info-box">
                            <h4>💡 Smart Tip:</h4>
                            <p><asp:Label ID="lblTrendTip" runat="server"></asp:Label></p>
                        </div>
                    </asp:Panel>

                    <!-- 3. Anomaly Detection -->
                    <asp:Panel ID="pnlAnomalies" runat="server" CssClass="card">
                        <div class="card-header">
                            <div class="card-icon">🔍</div>
                            <div class="card-title">
                                <h2>Unusual Spending Detected</h2>
                                <p>Transactions that don't match your pattern</p>
                            </div>
                        </div>
                        
                        <div class="explanation-box">
                            <h4>ℹ️ What are anomalies?</h4>
                            <p>These are expenses that are unusually high or low compared to your normal spending. They might be one-time purchases or unexpected bills.</p>
                        </div>
                        
                        <div style="text-align: center;">
                            <div class="count-badge">
                                <span class="count-label">Found: </span>
                                <span class="count-value">
                                    <asp:Label ID="lblAnomalyCount" runat="server" Text="0"></asp:Label>
                                </span>
                            </div>
                        </div>

                        <div class="anomaly-list">
                            <asp:Repeater ID="rptAnomalies" runat="server">
                                <ItemTemplate>
                                    <div class="anomaly-item">
                                        <div class="anomaly-icon">⚠️</div>
                                        <div class="anomaly-details">
                                            <div class="anomaly-date"><%# ((DateTime)Eval("Date")).ToString("MMMM dd, yyyy") %></div>
                                            <div class="anomaly-type"><%# Eval("Type") %></div>
                                        </div>
                                        <div class="anomaly-amount">₹<%# ((float)Eval("Amount")).ToString("N2") %></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <div class="info-box">
                            <h4>💡 Smart Tip:</h4>
                            <p><asp:Label ID="lblAnomalyTip" runat="server"></asp:Label></p>
                        </div>
                    </asp:Panel>

                    <!-- 4. K-Means Clustering -->
                    <asp:Panel ID="pnlClustering" runat="server" CssClass="card">
                        <div class="card-header">
                            <div class="card-icon">🎯</div>
                            <div class="card-title">
                                <h2>Spending Categories</h2>
                                <p>Your expenses grouped by patterns</p>
                            </div>
                        </div>
                        
                        <div class="explanation-box">
                            <h4>ℹ️ What is clustering?</h4>
                            <p>The AI groups your expenses into categories based on how much and how often you spend. This helps you see your spending habits clearly!</p>
                        </div>
                        
                        <asp:Repeater ID="rptClusters" runat="server">
                            <ItemTemplate>
                                <div class="cluster-card">
                                    <div class="cluster-header">
                                        <div class="cluster-name"><%# Eval("ClusterName") %></div>
                                    </div>
                                    <div class="cluster-categories">
                                        <%# Eval("Categories") %>
                                    </div>
                                    <div class="cluster-footer">
                                        <div class="cluster-spending">₹<%# ((float)Eval("TotalSpending")).ToString("N2") %></div>
                                    </div>
                                    <div class="cluster-insight">💡 <%# Eval("Insight") %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <div class="info-box">
                            <h4>💡 Smart Tip:</h4>
                            <p><asp:Label ID="lblClusteringTip" runat="server"></asp:Label></p>
                        </div>
                    </asp:Panel>

                    <!-- 5. Risk Prediction -->
                    <asp:Panel ID="pnlRiskPrediction" runat="server" CssClass="card">
                        <div class="card-header">
                            <div class="card-icon">⚡</div>
                            <div class="card-title">
                                <h2>Overspending Risk</h2>
                                <p>Will you exceed your budget this month?</p>
                            </div>
                        </div>
                        
                        <div class="explanation-box">
                            <h4>ℹ️ Understanding risk score</h4>
                            <p>This score predicts if you'll overspend this month. Higher percentage = higher risk. The AI compares your current spending with your income and past patterns.</p>
                        </div>
                        
                        <div class="risk-container">
                            <div class="risk-score-circle">
                                <div class="risk-score">
                                    <asp:Label ID="lblRiskScore" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="risk-status">
                                <asp:Label ID="lblRiskStatus" runat="server"></asp:Label>
                            </div>
                            <div class="risk-recommendation">
                                💬 <asp:Label ID="lblRiskRecommendation" runat="server"></asp:Label>
                            </div>
                        </div>
                    </asp:Panel>
                </div>

                <!-- Quick Guide Section -->
                <div class="card" style="margin-bottom: 30px;">
                    <div class="card-header">
                        <div class="card-icon">📚</div>
                        <div class="card-title">
                            <h2>Quick Guide: Understanding Your AI Insights</h2>
                            <p>Here's what each section means in simple terms</p>
                        </div>
                    </div>
                    
                    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 15px;">
                        <div style="padding: 15px; background: #F0F9FF; border-radius: 8px; border-left: 3px solid #0284C7;">
                            <h4 style="color: #0C4A6E; margin-bottom: 8px; font-size: 14px;">📈 Spending Forecast</h4>
                            <p style="color: #0369A1; font-size: 13px;">Tells you how much you might spend in the future based on your past habits.</p>
                        </div>
                        
                        <div style="padding: 15px; background: #FFFBEB; border-radius: 8px; border-left: 3px solid #F59E0B;">
                            <h4 style="color: #78350F; margin-bottom: 8px; font-size: 14px;">📊 Monthly Trend</h4>
                            <p style="color: #92400E; font-size: 13px;">Shows if you're spending more or less each month. Look for patterns!</p>
                        </div>
                        
                        <div style="padding: 15px; background: #FEF2F2; border-radius: 8px; border-left: 3px solid #EF4444;">
                            <h4 style="color: #7F1D1D; margin-bottom: 8px; font-size: 14px;">🔍 Unusual Spending</h4>
                            <p style="color: #991B1B; font-size: 13px;">Highlights expenses that are different from your normal spending.</p>
                        </div>
                        
                        <div style="padding: 15px; background: #F5F3FF; border-radius: 8px; border-left: 3px solid #8B5CF6;">
                            <h4 style="color: #5B21B6; margin-bottom: 8px; font-size: 14px;">🎯 Spending Groups</h4>
                            <p style="color: #6D28D9; font-size: 13px;">Groups similar expenses together so you see where your money goes.</p>
                        </div>
                        
                        <div style="padding: 15px; background: #ECFDF5; border-radius: 8px; border-left: 3px solid #10B981;">
                            <h4 style="color: #065F46; margin-bottom: 8px; font-size: 14px;">⚡ Risk Score</h4>
                            <p style="color: #047857; font-size: 13px;">Predicts if you'll overspend this month. Take action if the risk is high!</p>
                        </div>
                        
                        <div style="padding: 15px; background: #FFF7ED; border-radius: 8px; border-left: 3px solid #F97316;">
                            <h4 style="color: #7C2D12; margin-bottom: 8px; font-size: 14px;">🔄 Update Button</h4>
                            <p style="color: #9A3412; font-size: 13px;">Click this to refresh all predictions with your latest expense data.</p>
                        </div>
                    </div>
                </div>

                <!-- Action Buttons -->
                <div class="action-buttons">
                    <asp:Button ID="btnRetrain" runat="server" Text="🔄 Update Predictions" 
                        CssClass="btn-retrain" OnClick="btnRetrain_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>