using MicroFinance_Loan.presentation.Dashboards;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.presentation.Base;
using Microfinance_Loan_Management_System.Presentation.Dashboards;
using Microfinance_Loan_Management_System.utilities;
using Microfinance_Loan_Management_System.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microfinance_Loan_Management_System.Presentation.Authentication
{
    public partial class LoginForm : BaseForm
    {
        public LoginForm()
        {
            InitializeComponent();
            passwordTextBox.UseSystemPasswordChar = true;

            this.Text = "Login - " + AppSettings.ApplicationName;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void loginButton_Click_1(object sender, EventArgs e)
        {
            string username = userNameTextBox.Text;
            string password = passwordTextBox.Text;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            loginButton.Enabled = false;
            loginButton.Text = "Logging in...";
            AuthenticationService authService = new AuthenticationService();
            try
            {
                User user = authService.Login(username, password);
                if (user != null)
                {
                    SessionManager.StartSession(user);
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Form dashboard = DashboardFactory.CreateDashboard(user);
                    if (dashboard != null)
                    {
                        dashboard.Show();
                        this.Hide();
                    }
                }
                else
                {
                    ShowMessage("Invalid Information Try Again", "Login Failed", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Login error: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
            finally
            {
                loginButton.Enabled = true;
                loginButton.Text = "Login";
            }
        }

        private void exitButton_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void signUpLabel_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            SignUpForm signUpForm = new SignUpForm();
            signUpForm.Show();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {
            ShowMessage("Please contact your administrator to reset your password.", "Password Reset");
        }
    }
}
