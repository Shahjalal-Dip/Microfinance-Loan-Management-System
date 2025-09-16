using MicroFinance_Loan.presentation.Users;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
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

namespace MicroFinance_Loan.presentation.Policies
{
    public partial class LoanPolicyForm : BaseForm
    {
        public LoanPolicyForm()
        {
            InitializeComponent();
            LoadPolicies();
        }

        private void LoadPolicies()
        {
            try
            {
                LoanPolicyRepository repository = new LoanPolicyRepository();
                List<LoanPolicy> policies = repository.GetAll();
                dataGridView1.DataSource = policies;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading policies: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        private void LoanPolicyForm_Load(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminDashboard adminDashboard = new AdminDashboard();
            adminDashboard.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddEditForm addForm = new AddEditForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadPolicies();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int policyID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PolicyID"].Value);
                AddEditForm editForm = new AddEditForm(policyID);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadPolicies();
                }
            }
            else
            {
                ShowMessage("Please select a policy to edit.", "No Selection");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string type = dataGridView1.SelectedRows[0].Cells["LoanType"].Value.ToString();
                int policyID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PolicyID"].Value);

                if (ConfirmAction($"Are you sure you want to delete policy '{type}'?", "Confirm Deletion"))
                {
                    try
                    {
                        LoanPolicyRepository policyRepo = new LoanPolicyRepository();
                        if (policyRepo.Delete (policyID))
                        {
                            ShowMessage("Policy delete successfully.", "Success");
                            LoadPolicies();
                        }
                        else
                        {
                            ShowMessage("Failed to delete policy.", "Error", MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Error delete policy: {ex.Message}", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                ShowMessage("Please select a policy to deactivate.", "No Selection");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string type = dataGridView1.SelectedRows[0].Cells["LoanType"].Value.ToString();
                int policyID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PolicyID"].Value);

                if (ConfirmAction($"Are you sure you want to deactivate policy '{type}'?", "Confirm Deactivation"))
                {
                    try
                    {
                        LoanPolicyRepository policyRepo = new LoanPolicyRepository();
                        if (policyRepo.DeactivatePolicy(policyID))
                        {
                            ShowMessage("Policy deactivated successfully.", "Success");
                            LoadPolicies();
                        }
                        else
                        {
                            ShowMessage("Failed to deactivate policy.", "Error", MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Error deactivating policy: {ex.Message}", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                ShowMessage("Please select a policy to deactivate.", "No Selection");
            }
        }
    }
}
