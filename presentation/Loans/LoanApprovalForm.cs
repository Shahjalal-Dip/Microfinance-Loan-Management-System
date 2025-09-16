using MicroFinance_Loan.presentation.Dashboards;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
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

namespace MicroFinance_Loan.presentation.Loans
{
    public partial class LoanApprovalForm : BaseForm
    {
        private Loan selectedLoan;
        public LoanApprovalForm()
        {
            InitializeComponent();
            LoadPendingLoans();
        }

        private void LoanApprovalForm_Load(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedLoan == null)
                {
                    ShowMessage("Please select a loan to approve.", "No Selection");
                    return;
                }

                if (!ValidateApprovalInputs())
                    return;

                if (!ConfirmAction($"Are you sure you want to approve this loan for ৳ {textBox1.Text}?", "Confirm Approval"))
                    return;

                decimal approvedAmount = Convert.ToDecimal(textBox1.Text);
                decimal interestRate = Convert.ToDecimal(textBox2.Text);

                LoanService loanService = new LoanService();
                if (loanService.ApproveLoan(selectedLoan.LoanID, approvedAmount, interestRate, CurrentUser.UserID))
                {
                    ShowMessage("Loan approved successfully!", "Approval Successful", MessageBoxIcon.Information);
                    LoadPendingLoans();
                    ClearDetailsPanel();
                }
                else
                {
                    ShowMessage("Failed to approve loan. Please try again.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error approving loan: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedLoan == null)
                {
                    ShowMessage("Please select a loan to reject.", "No Selection");
                    return;
                }

                if (!ConfirmAction("Are you sure you want to reject this loan application?", "Confirm Rejection"))
                    return;

                LoanService loanService = new LoanService();
                if (loanService.RejectLoan(selectedLoan.LoanID, CurrentUser.UserID))
                {
                    ShowMessage("Loan rejected.", "Rejection Successful", MessageBoxIcon.Information);
                    LoadPendingLoans();
                    ClearDetailsPanel();
                }
                else
                {
                    ShowMessage("Failed to reject loan. Please try again.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error rejecting loan: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                int loanID = Convert.ToInt32(selectedRow.Cells["LoanID"].Value);

                LoanRepository loanRepo = new LoanRepository();
                selectedLoan = loanRepo.GetById(loanID);

                if (selectedLoan != null)
                {
                    LoadLoanDetails(selectedLoan);
                    panel3.Enabled = true;
                }
            }
            else
            {
                panel3.Enabled = false;
            }
        }

        private void LoadLoanDetails(Loan loan)
        {
            MemberRepository memberRepo = new MemberRepository();
            Member member = memberRepo.GetById(loan.MemberID);

            string memberInfo = $"Name: {member?.Name ?? "N/A"}\n" +
                                $"NID: {member?.NID ?? "N/A"}\n" +
                                $"Income: {(member != null ? $"৳{member.Income:N0}" : "N/A")}";
            label6.Text = memberInfo;

            textBox1.Text = loan.AmountRequested.ToString();

            LoanPolicyRepository policyRepo = new LoanPolicyRepository();
            LoanPolicy policy = policyRepo.GetById(loan.PolicyID);
            textBox2.Text = policy?.InterestRate.ToString() ?? "0";
        }


        private bool ValidateApprovalInputs()
        {
            if (!decimal.TryParse(textBox1.Text, out decimal approvedAmount) || approvedAmount <= 0)
            {
                ShowMessage("Please enter a valid approved amount.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(textBox2.Text, out decimal interestRate) || interestRate < 0 || interestRate > 100)
            {
                ShowMessage("Please enter a valid interest rate (0-100).", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (approvedAmount > selectedLoan.AmountRequested * 1.1m) // Allow 10% more than requested
            {
                ShowMessage("Approved amount cannot be significantly higher than requested amount.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearDetailsPanel()
        {
            textBox1.Clear();
            textBox2.Clear();
            panel3.Enabled = false;
            selectedLoan = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OfficerDashboard officerDashboard = new OfficerDashboard();
            officerDashboard.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadPendingLoans();
        }
    }
}
