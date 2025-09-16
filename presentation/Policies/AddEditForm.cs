using MicroFinance_Loan.utilities;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
using Microfinance_Loan_Management_System.presentation.Base;
using Microfinance_Loan_Management_System.Presentation.Dashboards;
using System;
using System.Windows.Forms;

namespace MicroFinance_Loan.presentation.Policies
{
    public partial class AddEditForm : BaseForm
    {
        private bool isEditMode;
        private int editPolicyID;
        public AddEditForm(int policyID = 0)
        {
            InitializeComponent();
            isEditMode = policyID > 0;
            editPolicyID = policyID;
            this.Text = isEditMode ? "Edit Policy" : "Add New Policy";

            if (isEditMode)
            {
                LoadPolicyData();
            }
        }
        private void LoadPolicyData()
        {
            try
            {
                if (editPolicyID > 0)
                {
                    LoanPolicyRepository policyRepo = new LoanPolicyRepository();
                    LoanPolicy policy = policyRepo.GetById(editPolicyID);

                    if (policy != null)
                    {
                        textBox2.Text = policy.LoanType;
                        textBox3.Text = policy.MaxAmount.ToString();
                        textBox4.Text = policy.InterestRate.ToString();
                        textBox5.Text = policy.PenaltyRate.ToString();
                        textBox6.Text = policy.MinDurationMonths.ToString();
                        textBox7.Text = policy.MaxDurationMonths.ToString();
                        textBox1.Text = policy.IsActive.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading policy data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        private void AddEditForm_Load(object sender, EventArgs e)
        {
            label1.Text = isEditMode ? "Edit Policy" : "Add New Policy";
            button2.Text = isEditMode ? "Update" : "Submit";
            label8.Visible = isEditMode;
            textBox1.Visible = isEditMode;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;
                LoanPolicy policy = new LoanPolicy
                {
                    LoanType = textBox2.Text.Trim(),
                    MaxAmount = decimal.Parse(textBox3.Text.Trim()),
                    InterestRate = decimal.Parse(textBox4.Text.Trim()),
                    PenaltyRate = decimal.Parse(textBox5.Text.Trim()),
                    MinDurationMonths = int.Parse(textBox6.Text.Trim()),
                    MaxDurationMonths = int.Parse(textBox7.Text.Trim()),
                    IsActive = isEditMode ? bool.Parse(textBox1.Text.Trim()) : true,
                    CreatedBy = CurrentUser.UserID,
                };
                LoanPolicyRepository policyRepo = new LoanPolicyRepository();
                bool success;
                if (isEditMode)
                {
                    policy.PolicyID = editPolicyID;
                    success = policyRepo.Update(policy);
                }
                else
                {
                    policy.CreatedDate = DateTime.Now;
                    policy.IsActive = true;
                    success = policyRepo.Insert(policy);
                }
                if (success)
                {
                    ShowMessage(isEditMode ? "Policy updated successfully." : "Policy added successfully.", "Success", MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowMessage("Failed to save Policy. Please try again.", "Error", MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving Policy: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(textBox7.Text))
            {
                ShowMessage("All fields are required.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }
            if (!decimal.TryParse(textBox3.Text, out decimal maxAmount) || maxAmount <= 0)
            {
                ShowMessage("Max Amount must be a positive number.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }
            if(ValidationHelper.IsValidAmount(maxAmount) == false)
            {
                ShowMessage("Max Amount must be a valid amount (up to 999,999,999.99).", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }
            if (!decimal.TryParse(textBox4.Text, out decimal interestRate) || interestRate < 0)
            {
                ShowMessage("Interest Rate must be a non-negative number.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }
            if(ValidationHelper.IsValidInterestRate(interestRate) == false)
            {
                ShowMessage("Interest Rate must be between 0 and 100.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }
            if (!decimal.TryParse(textBox5.Text, out decimal penaltyRate) || penaltyRate < 0)
            {
                ShowMessage("Penalty Rate must be a non-negative number.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }
            if (!int.TryParse(textBox6.Text, out int minDuration) || minDuration <= 0)
            {
                ShowMessage("Min Duration must be a positive integer.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }
            if (!int.TryParse(textBox7.Text, out int maxDuration) || maxDuration <= 0 || maxDuration < minDuration)
            {
                ShowMessage("Max Duration must be a positive integer and greater than or equal to Min Duration.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
           LoanPolicyForm loanPolicyForm = new LoanPolicyForm();
            loanPolicyForm.ShowDialog();
            this.Close();
        }
    }
}
