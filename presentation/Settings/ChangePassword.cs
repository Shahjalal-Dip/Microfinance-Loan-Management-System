using MicroFinance_Loan.presentation.Dashboards;
using MicroFinance_Loan.utilities;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.presentation.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroFinance_Loan.presentation.Settings
{
    public partial class ChangePassword : BaseForm
    {
        public ChangePassword()
        {
            InitializeComponent();
            label1.Text = $"Welcome {CurrentUser.Name} Change Your Password";
        }

        private void ChangePassword_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MemberDashboard memberDashboard = new MemberDashboard();
            memberDashboard.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AuthenticationService authService = new AuthenticationService();
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                ShowMessage("All fields are required", "Validation Error", MessageBoxIcon.Warning);
                return;
            }
            
            if (ValidationHelper.IsStrongPassword(textBox2.Text))
            {
                ShowMessage("Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.", "Validation Error", MessageBoxIcon.Warning);
            }

            if (authService.ChangePassword(CurrentUser.UserID, textBox1.Text, textBox2.Text))
            {
                ShowMessage("Password changed successfully", "Success", MessageBoxIcon.Information);
            }
            else
            {
                ShowMessage("Failed to change password. Please try again.", "Error", MessageBoxIcon.Error);
            }
        }
    }
}
