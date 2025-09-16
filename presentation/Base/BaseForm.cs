using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.Presentation.Authentication;
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

namespace Microfinance_Loan_Management_System.presentation.Base
{
    public partial class BaseForm : Form
    {
        protected User CurrentUser { get; set; }
        protected Timer sessionTimer;
        public BaseForm()
        {
            InitializeComponent();
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);
            this.Text = AppSettings.ApplicationName;
            SetupSessionTimer();
            SetupSecurity();
        }

        protected virtual void SetupSecurity()
        {
            if (SessionManager.IsLoggedIn())
            {
                CurrentUser = SessionManager.CurrentUser;
            }
        }

        protected virtual void LoadUserData() { }

        private void SetupSessionTimer()
        {
            sessionTimer = new Timer();
            sessionTimer.Interval = 60000; // Check every minute
            sessionTimer.Tick += SessionTimer_Tick;
            sessionTimer.Start();
        }

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            if (SessionManager.IsLoggedIn())
            {
                TimeSpan sessionDuration = DateTime.Now - SessionManager.LoginTime;
                if (sessionDuration.TotalMinutes > AppSettings.SessionTimeoutMinutes)
                {
                    MessageBox.Show("Session has expired. Please log in again.", "Session Timeout",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SessionManager.EndSession();

                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    this.Close();
                }
            }
        }

        protected void ShowMessage(string message, string title = "Information", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        protected bool ConfirmAction(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void BaseForm_Load(object sender, EventArgs e)
        {

        }
    }
}
