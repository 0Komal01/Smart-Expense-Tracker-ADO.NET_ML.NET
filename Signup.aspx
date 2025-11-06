<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="Expense_Tracker.Signup" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sign Up - Expense Tracker</title>
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
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .signup-container {
            background: white;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
            width: 100%;
            max-width: 420px;
        }

        .signup-header {
            text-align: center;
            margin-bottom: 35px;
        }

        .logo {
            width: 150px;
            height: auto;
            margin: 0 auto 20px;
            display: block;
        }

        .signup-header h2 {
            color: #1E293B;
            font-size: 26px;
            margin-bottom: 8px;
        }

        .signup-header p {
            color: #64748B;
            font-size: 14px;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            color: #1E293B;
            font-weight: 500;
            margin-bottom: 8px;
            font-size: 14px;
        }

        .form-group input {
            width: 100%;
            padding: 12px 15px;
            border: 2px solid #E2E8F0;
            border-radius: 8px;
            font-size: 14px;
            transition: all 0.3s;
        }

        .form-group input:focus {
            outline: none;
            border-color: #06B6D4;
            box-shadow: 0 0 0 3px rgba(6, 182, 212, 0.1);
        }

        .btn-signup {
            width: 100%;
            padding: 13px;
            background: #06B6D4;
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            margin-top: 10px;
        }

        .btn-signup:hover {
            background: #0891B2;
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(6, 182, 212, 0.3);
        }

        .login-link {
            text-align: center;
            margin-top: 20px;
            color: #64748B;
            font-size: 14px;
        }

        .login-link a {
            color: #06B6D4;
            text-decoration: none;
            font-weight: 600;
        }

        .login-link a:hover {
            text-decoration: underline;
        }

        .error-message {
            background: #FEE2E2;
            color: #991B1B;
            padding: 12px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-size: 14px;
            display: none;
        }

        .error-message.show {
            display: block;
        }

        .success-message {
            background: #D1FAE5;
            color: #065F46;
            padding: 12px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-size: 14px;
            display: none;
        }

        .success-message.show {
            display: block;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="signup-container">
            <div class="signup-header">
                <img src="Image/logo.png" alt="Expense Tracker" class="logo" />
                <h2>Create Account</h2>
                <p>Sign up to start tracking your expenses</p>
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="error-message"></asp:Label>
            <asp:Label ID="lblSuccess" runat="server" CssClass="success-message"></asp:Label>

            <div class="form-group">
                <label>Username</label>
                <asp:TextBox ID="txtUsername" runat="server" placeholder="Choose a username"></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="Enter your email"></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Create a password"></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Confirm Password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder="Confirm your password"></asp:TextBox>
            </div>

            <asp:Button ID="btnSignup" runat="server" Text="Sign Up" CssClass="btn-signup" OnClick="btnSignup_Click" />

            <div class="login-link">
                Already have an account? <a href="Login.aspx">Login</a>
            </div>
        </div>
    </form>
</body>
</html>