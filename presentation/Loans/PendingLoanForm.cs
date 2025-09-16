using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.presentation.Base;
using Microfinance_Loan_Management_System.Presentation.Dashboards;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroFinance_Loan.presentation.Loans
{
    public partial class PendingLoanForm : BaseForm
    {
        public PendingLoanForm()
        {
            InitializeComponent();
            LoadPendingLoans();
        }

        private void PendingLoanForm_Load(object sender, EventArgs e)
        {

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

        private void button2_Click(object sender, EventArgs e)
        {
            LoadPendingLoans();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminDashboard adminDashboard = new AdminDashboard();  
            adminDashboard.Show();
            this.Hide();
            
        }
    }
}
