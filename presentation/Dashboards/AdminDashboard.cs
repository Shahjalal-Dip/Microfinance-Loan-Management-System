using MicroFinance_Loan.presentation.Loans;
using MicroFinance_Loan.presentation.Policies;
using MicroFinance_Loan.presentation.Reports;
using MicroFinance_Loan.presentation.Users;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.presentation.Base;
using Microfinance_Loan_Management_System.Presentation.Authentication;
using Microfinance_Loan_Management_System.utilities;
using Microfinance_Loan_Management_System.Utilities;
using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Microfinance_Loan_Management_System.Presentation.Dashboards
{
    public partial class AdminDashboard : BaseForm
    {
        public AdminDashboard()
        {
            InitializeComponent();
            LoadDashboardData();
            //this.WindowState = FormWindowState.Maximized;
            this.Text = "Admin Dashboard - " + AppSettings.ApplicationName;
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = $"Welcome, {CurrentUser?.Name}     ";
            toolStripStatusLabel2.Text = $"Role: {CurrentUser?.Role}     ";
            toolStripStatusLabel3.Text = $"Date: {DateTime.Now:dd/MM/yyyy}";
        }
        private void LoadDashboardData()
        {
            try
            {
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadStatistics()
        {
            UserService userService = new UserService();
            LoanService loanService = new LoanService();
            RepaymentService repaymentService = new RepaymentService();

            var members = userService.GetAllMembers();
            var pendingLoans = loanService.GetPendingLoans();
            var overduePayments = repaymentService.GetOverdueRepayments();

            label1.Text = members.Count.ToString();
            label2.Text = pendingLoans.Count.ToString();
            label3.Text = overduePayments.Count.ToString();

            DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ReportService reportService = new ReportService();
            DataTable collectionData = reportService.GetCollectionReport(startOfMonth, DateTime.Now);

            decimal monthlyCollection = 0;
            foreach (DataRow row in collectionData.Rows)
            {
                monthlyCollection += Convert.ToDecimal(row["AmountPaid"]);
            }

            label4.Text = $"৳{ monthlyCollection:0}";
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConfirmAction("Are you sure you want to exit the application?", "Exit"))
            {
                Application.Exit();
            }
        }

        private void manageUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserManagementForm userManagementForm = new UserManagementForm();
            userManagementForm.ShowDialog();
            this.Close();
        }

        private void loanPolicysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoanPolicyForm loanPolicyForm = new LoanPolicyForm();
            loanPolicyForm.ShowDialog();
            this.Close();
        }

        private void pendingApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PendingLoanForm pendingLoanForm = new PendingLoanForm();
            pendingLoanForm.ShowDialog();
            this.Close();
        }

        private void allLoansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllLoansForm all = new AllLoansForm();
            all.ShowDialog();
            this.Close();
        }

        private void loanSummaryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoanSummaryReport loanSummaryReport = new LoanSummaryReport(1);
            loanSummaryReport.ShowDialog();
            this.Close();
        }

        private void overdueReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfitLossReport profitLossReport = new ProfitLossReport(1);
            profitLossReport.ShowDialog();
            this.Close();
        }

        private void overdueLoansReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OverdueLoanReport overdueLoanReport = new OverdueLoanReport(1);
            overdueLoanReport.ShowDialog();
            this.Close();
        }

        private void collectionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CollectionReport collectionReport = new CollectionReport(1);
            collectionReport.ShowDialog();
            this.Close();
        }     

        private void eMICalculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EMICalculator eMICalculator = new EMICalculator();
            eMICalculator.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserManagementForm userManagementForm = new UserManagementForm();
            userManagementForm.ShowDialog();
            this.Close();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            PendingLoanForm pendingLoanForm = new PendingLoanForm();
            pendingLoanForm.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoanPolicyForm loanPolicyForm = new LoanPolicyForm();
            loanPolicyForm.ShowDialog();
            this.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ReportsMainForm reportsMainForm = new ReportsMainForm();
            reportsMainForm.ShowDialog();
            this.Close();
        }
    }
}
