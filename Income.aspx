<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Income.aspx.cs" Inherits="Expense_Tracker.Income" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Income - Expense Tracker</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #F8FAFC;
            min-height: 100vh;
        }

        .container {
            display: flex;
            min-height: 100vh;
        }

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

        .user-info h3 {
            font-size: 15px;
            margin-bottom: 3px;
        }

        .user-info p {
            font-size: 12px;
            color: #94A3B8;
        }

        .nav-section {
            margin-bottom: 25px;
        }

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

        .nav-item:hover {
            background: #334155;
            color: white;
        }

        .nav-item.active {
            background: #06B6D4;
            color: white;
        }

        .nav-icon {
            width: 20px;
            height: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        /* Main Content Styles */
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

        .page-title {
            font-size: 26px;
            color: #1E293B;
            font-weight: 600;
        }

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

        .logout-btn:hover {
            background: #DC2626;
            transform: translateY(-1px);
        }

        .info-banner {
            background: linear-gradient(135deg, #10B981 0%, #059669 100%);
            color: white;
            padding: 20px 30px;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(16, 185, 129, 0.3);
            margin-bottom: 25px;
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .info-banner-icon {
            font-size: 32px;
        }

        .info-banner-text h3 {
            font-size: 18px;
            margin-bottom: 5px;
        }

        .info-banner-text p {
            font-size: 14px;
            opacity: 0.95;
        }

        .form-card {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 30px;
        }

        .form-card h3 {
            color: #1E293B;
            margin-bottom: 25px;
            font-size: 20px;
        }

        .input-group {
            display: flex;
            flex-direction: column;
            margin-bottom: 20px;
        }

        .input-group.hidden {
            display: none;
        }

        .input-group label {
            font-weight: 600;
            margin-bottom: 8px;
            color: #495057;
            font-size: 14px;
        }

        .input-group input[type="text"],
        .input-group input[type="number"],
        .input-group textarea {
            padding: 12px 15px;
            font-size: 14px;
            border-radius: 8px;
            border: 2px solid #E2E8F0;
            transition: all 0.3s;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .input-group textarea {
            resize: vertical;
            min-height: 80px;
        }

        .input-group input:focus,
        .input-group textarea:focus {
            border-color: #10B981;
            box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.1);
            outline: none;
        }

        .input-group input:read-only {
            background-color: #e9ecef;
            cursor: not-allowed;
        }

        .currency-input {
            position: relative;
        }

        .currency-input::before {
            content: '₹';
            position: absolute;
            left: 15px;
            top: 50%;
            transform: translateY(-50%);
            color: #495057;
            font-weight: 600;
            font-size: 16px;
        }

        .currency-input input {
            padding-left: 35px !important;
        }

        .btn-group {
            display: flex;
            gap: 15px;
            margin-top: 25px;
        }

        .btn {
            flex: 1;
            padding: 13px 0;
            font-size: 15px;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            color: #fff;
            transition: all 0.3s;
            font-weight: 600;
        }

        .btn-add {
            background: linear-gradient(135deg, #10B981, #059669);
        }

        .btn-add:hover {
            background: linear-gradient(135deg, #059669, #047857);
            transform: translateY(-1px);
        }

        .btn-update {
            background: linear-gradient(135deg, #007bff, #00c6ff);
        }

        .btn-update:hover {
            background: linear-gradient(135deg, #0056b3, #00a0cc);
            transform: translateY(-1px);
        }

        .btn-clear {
            background: linear-gradient(135deg, #6c757d, #adb5bd);
        }

        .btn-clear:hover {
            background: linear-gradient(135deg, #5a6268, #9aa0a6);
            transform: translateY(-1px);
        }

        .grid-card {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            overflow-x: auto;
        }

        .grid-card h3 {
            color: #1E293B;
            margin-bottom: 20px;
            font-size: 20px;
        }

        #GridView1 {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
        }

        #GridView1 th,
        #GridView1 td {
            padding: 14px 16px;
            border: 1px solid #dee2e6;
            text-align: left;
        }

        #GridView1 th {
            color: #fff;
            font-weight: 600;
            background: #10B981;
        }

        #GridView1 tr:nth-child(even) {
            background-color: #f8f9fa;
        }

        #GridView1 tr:hover {
            background-color: #e2e6ea;
        }

        .edit-button,
        .delete-button {
            color: #fff;
            padding: 8px 15px;
            border-radius: 6px;
            border: none;
            cursor: pointer;
            transition: 0.3s;
            margin: 0 5px;
            font-size: 13px;
            font-weight: 600;
        }

        .edit-button {
            background: #3b82f6;
        }

        .edit-button:hover {
            background: #2563eb;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
        }

        .delete-button {
            background: #ef4444;
        }

        .delete-button:hover {
            background: #dc2626;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(239, 68, 68, 0.4);
        }

        .amount-display {
            color: #059669;
            font-weight: 600;
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
                    <a href="Dashboard.aspx" class="nav-item">
                        <div class="nav-icon">📊</div>
                        <span>Dashboard</span>
                    </a>
                   <a href="Categories.aspx" class="nav-item">
                        <div class="nav-icon">💸</div>
                        <span>Expenses</span>
                    </a>
                    <a href="Income.aspx" class="nav-item active">
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
                    <h1 class="page-title">💰 Income Management</h1>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
                </div>

                <!-- Info Banner -->
                <div class="info-banner">
                    <div class="info-banner-icon">🎓</div>
                    <div class="info-banner-text">
                        <h3>Student Income Tracker</h3>
                        <p>Track your pocket money, scholarships, part-time earnings, and all other income sources here!</p>
                    </div>
                </div>

                <!-- Form Card -->
                <div class="form-card">
                    <h3>Add/Edit Income</h3>

                    <div class="input-group hidden">
                        <asp:TextBox ID="txtIncomeID" runat="server" ReadOnly="true"></asp:TextBox>
                    </div>

                    <div class="input-group">
                        <label for="txtSource">Income Source:</label>
                        <asp:TextBox ID="txtSource" runat="server" placeholder="e.g., Pocket Money, Part-time Job, Scholarship"></asp:TextBox>
                    </div>

                    <div class="input-group">
                        <label for="txtAmount">Amount (₹):</label>
                        <div class="currency-input">
                            <asp:TextBox ID="txtAmount" runat="server" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                        </div>
                    </div>

                    <div class="input-group">
                        <label for="txtDescription">Description (Optional):</label>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" placeholder="Brief description about this income"></asp:TextBox>
                    </div>

                    <div class="btn-group">
                        <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-add" Text="➕ Add Income" OnClick="btnAdd_Click" />
                        <asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-update" Text="✏️ Update Income" OnClick="btnUpdate_Click" Visible="false" />
                        <asp:Button ID="btnClear" runat="server" CssClass="btn btn-clear" Text="🔄 Clear" OnClick="btnClear_Click" />
                    </div>
                </div>

                <!-- Grid Card -->
                <div class="grid-card">
                    <h3>Your Income Records</h3>
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="IncomeID" HeaderText="ID" ReadOnly="True" />
                            <asp:BoundField DataField="Source" HeaderText="Income Source" />
                            <asp:TemplateField HeaderText="Amount">
                                <ItemTemplate>
                                    <span class="amount-display">₹ <%# String.Format("{0:N2}", Eval("Amount")) %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="DateReceived" HeaderText="Date Received" DataFormatString="{0:dd-MMM-yyyy}" ReadOnly="True" />
                            <asp:BoundField DataField="Description" HeaderText="Description" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnEdit" runat="server"
                                        CommandArgument='<%# Eval("IncomeID") %>'
                                        Text="Edit"
                                        CssClass="edit-button"
                                        OnClick="btnEdit_Click" />
                                    <asp:Button ID="btnDelete" runat="server"
                                        CommandArgument='<%# Eval("IncomeID") %>'
                                        Text="Delete"
                                        CssClass="delete-button"
                                        OnClick="btnDelete_Click"
                                        OnClientClick="return confirm('Are you sure you want to delete this income record?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>