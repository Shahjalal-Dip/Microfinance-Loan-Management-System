using MicroFinance_Loan.presentation.Dashboards;
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
    public partial class AllLoansForm : BaseForm
    {
        private int n;
        public AllLoansForm(int n = 0)
        {
            InitializeComponent();
            this.n = n;
            AllLoans();
        }

        private void AllLoansForm_Load(object sender, EventArgs e)
        {

        }
        private void AllLoans()
        {
            try
            {
                if (n == 2)
                {
                    LoanService loanService = new LoanService();
                    List<Loan> memberLoans = loanService.GetLoansByMember(CurrentUser.UserID);
                    var selectedColumns = memberLoans.Select(loan => new
                    {
                        LoanID = loan.LoanID,
                        MemberName = loan.MemberName,
                        PolicyType = loan.PolicyType,
                        AmountRequested = $"৳ {loan.AmountRequested:N0}",
                        ApprovedAmmount = $"৳ {loan.ApprovedAmount:N0}",
                        ActionBy = loan.OfficerName,
                        DurationMonths = loan.DurationMonths,
                        LoanPurpose = loan.LoanPurpose,
                        loan.Status,
                        ApplicationDate = loan.ApplicationDate.ToString("dd/MM/yyyy")

                    }).ToList();
                    dataGridView1.DataSource = selectedColumns;
                }
                else
                {

                    LoanRepository loanRepository = new LoanRepository();
                    List<Loan> allLoans = loanRepository.GetAll();
                    var selectedColumns = allLoans.Select(loan => new
                    {
                        LoanID = loan.LoanID,
                        MemberName = loan.MemberName,
                        PolicyType = loan.PolicyType,
                        AmountRequested = $"৳ {loan.AmountRequested:N0}",
                        ApprovedAmmount = $"৳ {loan.ApprovedAmount:N0}",
                        ActionBy = loan.OfficerName,
                        DurationMonths = loan.DurationMonths,
                        LoanPurpose = loan.LoanPurpose,
                        loan.Status,
                        ApplicationDate = loan.ApplicationDate.ToString("dd/MM/yyyy")
                    }).ToList();
                    dataGridView1.DataSource = selectedColumns;
                }

                }
            catch (Exception ex)
            {
                ShowMessage($"Error loading All loans: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AllLoans();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (n == 1)
            {
                OfficerDashboard officerDashboard = new OfficerDashboard();
                officerDashboard.Show();
                this.Close();
            }
            else if (n == 2)
            {
                MemberDashboard memberDashboard = new MemberDashboard();
                memberDashboard.Show();
                this.Close();
            }
            else
            {
                AdminDashboard adminDashboard = new AdminDashboard();
                adminDashboard.Show();
                this.Close();
            }
        }
    }
}
