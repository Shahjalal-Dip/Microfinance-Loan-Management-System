using MicroFinance_Loan.presentation.Loans;
using MicroFinance_Loan.presentation.Payments;
using MicroFinance_Loan.presentation.Settings;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.presentation.Base;
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

namespace MicroFinance_Loan.presentation.Dashboards
{
    public partial class MemberDashboard : BaseForm
    {
        public MemberDashboard()
        {
            InitializeComponent();
            LoadMemberData();

            this.Text = "Member Dashboard - " + AppSettings.ApplicationName;
        }

        private void MemberDashboard_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = $"Welcome, {CurrentUser?.Name}     ";
            toolStripStatusLabel2.Text = $"Role: {CurrentUser?.Role}     ";
            toolStripStatusLabel3.Text = $"Date: {DateTime.Now:dd/MM/yyyy}";
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConfirmAction("Are you sure you want to logout?", "Logout"))
            {
                SessionManager.EndSession();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
        private void LoadMemberData()
        {
            try
            {
                LoanService loanService = new LoanService();
                List<Loan> memberLoans = loanService.GetLoansByMember(CurrentUser.UserID);

                dataGridView1.Rows.Clear();
                var displayList = memberLoans.Select(loan => new
                {
                    loan.LoanID,
                    loan.PolicyType,
                    Amount = loan.ApprovedAmount ?? loan.AmountRequested,
                    loan.Status,
                    Date = loan.ApplicationDate.ToString("dd/MM/yyyy")
                }).ToList();

                dataGridView1.DataSource = displayList;

                dataGridView2.DataSource = loanService.LoadLoanScheduleByUser(CurrentUser.UserID);

                UpdateLoanSummary(memberLoans);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading member data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateLoanSummary(List<Loan> loans)
        {
            int totalLoans = loans.Count;
            int activeLoans = loans.Count(l => l.Status == "Approved" || l.Status == "Disbursed");
            decimal totalBorrowed = loans.Where(l => l.ApprovedAmount.HasValue).Sum(l => l.ApprovedAmount.Value);
            
            label8.Text = totalLoans.ToString();
            label9.Text = activeLoans.ToString();
            label10.Text = $"৳ {totalBorrowed:0}";
        }

        private void updateProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserProfileForm userProfileForm = new UserProfileForm();
            userProfileForm.Show();
            this.Close();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePassword changePasswordForm = new ChangePassword();
            changePasswordForm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoanApplicationForm loanApplicationForm = new LoanApplicationForm();
            loanApplicationForm.Show();
            this.Close();
        }

        private void applyForLoansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoanApplicationForm loanApplicationForm = new LoanApplicationForm();
            loanApplicationForm.Show();
            this.Close();
        }

        private void makePaymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakePaymentForm makePaymentForm = new MakePaymentForm();
            makePaymentForm.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MakePaymentForm makePaymentForm = new MakePaymentForm();
            makePaymentForm.Show();
            this.Close();
        }

        

        private void viewAllLoansToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AllLoansForm allLoansForm = new AllLoansForm(2);
            allLoansForm.Show();
            this.Close();
        }

        private void paymentsHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
    
