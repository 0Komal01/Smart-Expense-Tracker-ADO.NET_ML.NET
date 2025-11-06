<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Categories.aspx.cs" Inherits="Expense_Tracker.Categories" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Category Management - Expense Tracker</title>
    <style>
        /* Copy exact styles from Dashboard for full consistency */
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

        /* Form & Grid */
        .form-card, .grid-card {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            margin-bottom: 25px;
        }

        .input-group { margin-bottom: 20px; display: flex; flex-direction: column; }
        .input-group.hidden { display: none; }
        label { font-weight: 600; color: #334155; margin-bottom: 8px; }

        input, select, textarea {
            padding: 12px 15px;
            border: 2px solid #E2E8F0;
            border-radius: 8px;
            font-size: 14px;
        }

        .btn-group { display: flex; gap: 15px; }
        .btn {
            flex: 1;
            padding: 13px 0;
            border: none;
            border-radius: 8px;
            color: #fff;
            font-weight: 600;
            cursor: pointer;
        }
        .btn-add { background: linear-gradient(135deg, #16a34a, #22c55e); }
        .btn-update { background: linear-gradient(135deg, #2563eb, #3b82f6); }
        .btn-clear { background: linear-gradient(135deg, #6b7280, #9ca3af); }

        #GridView1 { width: 100%; border-collapse: collapse; }
        #GridView1 th, #GridView1 td { padding: 12px; border: 1px solid #E2E8F0; }
        #GridView1 th { background: #06B6D4; color: white; }
        .amount-display { color: #16a34a; font-weight: 600; }

        /* Action Buttons in Grid */
        #GridView1 .btn {
            flex: none;
            padding: 8px 16px;
            margin: 0 5px;
            font-size: 13px;
            border-radius: 6px;
            transition: all 0.3s;
        }

        #GridView1 .btn-update {
            background: #3b82f6;
            border: none;
        }

        #GridView1 .btn-update:hover {
            background: #2563eb;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
        }

        #GridView1 .btn-clear {
            background: #ef4444;
            border: none;
        }

        #GridView1 .btn-clear:hover {
            background: #dc2626;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(239, 68, 68, 0.4);
        }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <div class="container">
        <!-- Sidebar  -->
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
                <a href="MLPredictions.aspx" class="nav-item">
                    <div class="nav-icon">🤖</div>
                    <span>Advance(ML)</span>
                </a>
            </div>
        </div>

        <!-- Main Content -->
        <div class="main-content">
            <div class="top-bar">
                <h1 class="page-title">💸 Expense Management</h1>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
            </div>

            <!-- Add Category Form -->
            <div class="form-card">
                <h3>Add/Edit Category</h3>
                <div class="input-group hidden">
                    <asp:TextBox ID="txtCategoryID" runat="server" ReadOnly="true"></asp:TextBox>
                </div>

                <div class="input-group">
                    <label>Category Name:</label>
                    <asp:DropDownList ID="ddlCategoryName" runat="server">
                        <asp:ListItem Text="-- Select Category --" Value=""></asp:ListItem>
                        <asp:ListItem Text="Food & Groceries" Value="Food & Groceries"></asp:ListItem>
                        <asp:ListItem Text="Rent / Housing" Value="Rent / Housing"></asp:ListItem>
                        <asp:ListItem Text="Transportation" Value="Transportation"></asp:ListItem>
                        <asp:ListItem Text="Entertainment" Value="Entertainment"></asp:ListItem>
                        <asp:ListItem Text="Health & Medical" Value="Health & Medical"></asp:ListItem>
                        <asp:ListItem Text="Education" Value="Education"></asp:ListItem>
                        <asp:ListItem Text="Shopping" Value="Shopping"></asp:ListItem>
                        <asp:ListItem Text="Utilities & Bills" Value="Utilities & Bills"></asp:ListItem>
                        <asp:ListItem Text="Savings & Investments" Value="Savings & Investments"></asp:ListItem>
                        <asp:ListItem Text="Insurance" Value="Insurance"></asp:ListItem>
                        <asp:ListItem Text="Personal Care" Value="Personal Care"></asp:ListItem>
                        <asp:ListItem Text="Charity / Donations" Value="Charity / Donations"></asp:ListItem>
                        <asp:ListItem Text="Travel & Vacation" Value="Travel & Vacation"></asp:ListItem>
                        <asp:ListItem Text="Kids / Family" Value="Kids / Family"></asp:ListItem>
                        <asp:ListItem Text="Miscellaneous" Value="Miscellaneous"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="input-group">
                    <label>Description:</label>
                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" placeholder="Brief description of this category"></asp:TextBox>
                </div>

                <div class="input-group">
                    <label>Budget Amount (₹):</label>
                    <asp:TextBox ID="txtAmount" runat="server" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                </div>

                <div class="btn-group">
                    <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-add" Text="➕ Add Category" OnClick="btnAdd_Click" />
                    <asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-update" Text="✏️ Update Category" Visible="false" OnClick="btnUpdate_Click" />
                    <asp:Button ID="btnClear" runat="server" CssClass="btn btn-clear" Text="🔄 Clear" OnClick="btnClear_Click" />
                </div>
            </div>

            <!-- Category Grid -->
            <div class="grid-card">
                <h3>Your Categories</h3>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="CategoryID" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="CategoryName" HeaderText="Category Name" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:TemplateField HeaderText="Budget Amount">
                            <ItemTemplate>
                                <span class="amount-display">₹ <%# String.Format("{0:N2}", Eval("Amount")) %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandArgument='<%# Eval("CategoryID") %>' CssClass="btn btn-update" OnClick="btnEdit_Click" />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandArgument='<%# Eval("CategoryID") %>' CssClass="btn btn-clear" OnClick="btnDelete_Click" OnClientClick="return confirm('Delete this category?');" />
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