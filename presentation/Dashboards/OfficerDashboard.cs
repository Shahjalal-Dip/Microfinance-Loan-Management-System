using MicroFinance_Loan.presentation.Loans;
using MicroFinance_Loan.presentation.Payments;
using MicroFinance_Loan.presentation.Reports;
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
    public partial class OfficerDashboard : BaseForm
    {
        public OfficerDashboard()
        {
            InitializeComponent();
            LoadPendingLoans();
            UpdateRealTimeData();
            LoadTodayCollections();
            this.Text = "LoanOfficer Dashboard - " + AppSettings.ApplicationName;
        }

        private void OfficerDashboard_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = $"Welcome, {CurrentUser?.Name}     ";
            toolStripStatusLabel2.Text = $"Role: {CurrentUser?.Role}     ";
            toolStripStatusLabel3.Text = $"Date: {DateTime.Now:dd/MM/yyyy}";
        }

        private void LoadPendingLoans()
        {
            try
            {
                LoanService loanService = new LoanService();
                List<Loan> pendingLoans = loanService.GetPendingLoans();
                var selectedColumns = pendingLoans.Select(loan => new
                {
                    LoanID = loan.LoanID,
                    MemberName = loan.MemberName,
                    PolicyType = loan.PolicyType,
                    AmountRequested = $"৳ {loan.AmountRequested:N0}",
                    DurationMonths = loan.DurationMonths,
                    LoanPurpose = loan.LoanPurpose,
                    ApplicationDate = loan.ApplicationDate.ToString("dd/MM/yyyy")

                }).ToList();
                dataGridView1.DataSource = selectedColumns;

            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading pending loans: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadTodayCollections()
        {
            try
            {
                LoanService loanService = new LoanService();
                dataGridView2.DataSource = loanService.TodayCollection();
                
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading today's collections: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConfirmAction("Are you sure you want to logout?", "Logout"))
            {
                SessionManager.EndSession();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void UpdateRealTimeData()
        {
            try
            {
                LoanService loanService = new LoanService();
                int approvedLoans = loanService.GetActiveLoansCount();
                decimal todayCollections = loanService.GetTodayCollections();
                List<Loan> pendingLoans = loanService.GetPendingLoans();
                label7.Text = todayCollections.ToString();
                label6.Text = approvedLoans.ToString();
                label8.Text = pendingLoans.Count.ToString();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating real-time data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            LoanApprovalForm loanApprovalForm = new LoanApprovalForm();
            loanApprovalForm.Show();
            this.Close();
        }

        private void reviewApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoanApprovalForm loanApprovalForm = new LoanApprovalForm();
            loanApprovalForm.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PaymentCollectionForm paymentCollectionForm = new PaymentCollectionForm();
            paymentCollectionForm.Show();
            this.Close();
        }

        private void collectPaymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PaymentCollectionForm paymentCollectionForm = new PaymentCollectionForm();
            paymentCollectionForm.Show();
            this.Close();
        }

        private void overdueReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OverdueLoanReport overdueLoanReport = new OverdueLoanReport(2);
            overdueLoanReport.ShowDialog();
            this.Close();
        }

        private void allLoansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllLoansForm allLoansForm = new AllLoansForm(1);
            allLoansForm.Show();
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
