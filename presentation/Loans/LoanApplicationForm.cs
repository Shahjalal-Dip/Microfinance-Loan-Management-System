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
    public partial class LoanApplicationForm : BaseForm
    {
        private List<LoanPolicy> policies;
        public LoanApplicationForm()
        {
            InitializeComponent();
            LoadLoanPolicies();
        }

        private void LoanApplicationForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadLoanPolicies()
        {
            try
            {
                LoanPolicyRepository policyRepo = new LoanPolicyRepository();
                policies = policyRepo.GetAll();

                comboBox1.DataSource = policies;
                comboBox1.DisplayMember = "LoanType";   
                comboBox1.ValueMember = "PolicyID";     

                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading loan policies: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue != null)
                {
                    int selectedPolicyId = Convert.ToInt32(comboBox1.SelectedValue);
                    LoanPolicy selectedPolicy = policies.FirstOrDefault(p => p.PolicyID == selectedPolicyId);

                    if (selectedPolicy != null)
                    {
                        comboBox2.Items.Clear();

                        for (int i = selectedPolicy.MinDurationMonths; i <= selectedPolicy.MaxDurationMonths; i += 3)
                        {
                            comboBox2.Items.Add(i.ToString());
                        }

                        if (comboBox2.Items.Count > 0)
                            comboBox2.SelectedIndex = 0;

                        UpdateEMIPreview();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating duration options: {ex.Message}", "Error", MessageBoxIcon.Error);
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateEMIPreview();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEMIPreview();
        }

        private void UpdateEMIPreview()
        {
            try
            {
                if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null &&
                    decimal.TryParse(textBox1.Text, out decimal amount) && amount > 0)
                {
                    int selectedPolicyId = Convert.ToInt32(comboBox1.SelectedValue);
                    LoanPolicy selectedPolicy = policies.FirstOrDefault(p => p.PolicyID == selectedPolicyId);

                    if (selectedPolicy != null)
                    {
                        int duration = Convert.ToInt32(comboBox2.SelectedItem);
                        decimal emi = selectedPolicy.CalculateEMI(amount, duration);
                        decimal totalPayable = emi * duration;
                        decimal totalInterest = totalPayable - amount;

                           label8.Text =   $"Monthly EMI: ৳ {emi:N0}\n" +
                                           $"Total Payable: ৳ {totalPayable:N0}\n" +
                                           $"Total Interest: ৳ {totalInterest:N0}\n" +
                                           $"Interest Rate: {selectedPolicy.InterestRate}% per annum";
                    }
                }
                else
                {
                    label8.Text = "Please select loan type and enter amount";
                }
            }
            catch
            {
                label8.Text = "Invalid input for calculation";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                int selectedPolicyId = Convert.ToInt32(comboBox1.SelectedValue);

                Loan loan = new Loan
                {
                    MemberID = CurrentUser.UserID,
                    AmountRequested = Convert.ToDecimal(textBox1.Text),
                    DurationMonths = Convert.ToInt32(comboBox2.SelectedItem),
                    LoanPurpose = textBox2.Text.Trim(),
                    PolicyID = selectedPolicyId,
                    Status = "Pending",
                    ApplicationDate = DateTime.Now
                };

                LoanService loanService = new LoanService();
                if (loanService.ApplyForLoan(loan))
                {
                    ShowMessage("Loan application submitted successfully!\nYou will be notified once it's reviewed.",
                        "Application Submitted", MessageBoxIcon.Information);
                    MemberDashboard memberDashboard = new MemberDashboard();
                    memberDashboard.Show();
                    this.Close();
                }
                else
                {
                    ShowMessage("Failed to submit loan application. Please try again.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error submitting application: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (comboBox1.SelectedItem == null)
            {
                ShowMessage("Please select a loan type.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(textBox1.Text, out decimal amount) || amount <= 0)
            {
                ShowMessage("Please enter a valid loan amount.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            int selectedPolicyId = Convert.ToInt32(comboBox1.SelectedValue);
            LoanPolicy selectedPolicy = policies.FirstOrDefault(p => p.PolicyID == selectedPolicyId);

            if (amount > selectedPolicy.MaxAmount)
            {
                ShowMessage($"Amount exceeds maximum limit of ৳ {selectedPolicy.MaxAmount:N0} for {selectedPolicy.LoanType} loans.",
                    "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (comboBox2.SelectedItem == null)
            {
                ShowMessage("Please select loan duration.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                ShowMessage("Please enter the loan purpose.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {        
            MemberDashboard memberDashboard = new MemberDashboard();
            memberDashboard.Show();
            this.Hide();
        }
    }
}
