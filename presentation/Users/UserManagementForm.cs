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

namespace MicroFinance_Loan.presentation.Users
{
    public partial class UserManagementForm : BaseForm
    {
        public UserManagementForm()
        {
            InitializeComponent();
            LoadUsers();

            string[] role = new string[] { "All", "Admin", "LoanOfficer", "Member" };
            comboBox1.SelectedIndex = -1;
            comboBox1.DataSource = role;
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadUsers()
        {
            try
            {
                UserService userService = new UserService();
                List<User> users = userService.GetAllUsers();
                dataGridView1.DataSource = users;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading users: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void backbut_Click(object sender, EventArgs e)
        {
            AdminDashboard adminDashboard = new AdminDashboard();
            this.Hide();
            adminDashboard.Show();    
        }

        private void deletebut_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string userName = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
                int userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["UserID"].Value);

                if (ConfirmAction($"Are you sure you want to deactivate user '{userName}'?", "Confirm Deactivation"))
                {
                    try
                    {
                        UserService userService = new UserService();
                        if (userService.DeactivateUser(userID))
                        {
                            ShowMessage("User deactivated successfully.", "Success");
                            LoadUsers();
                        }
                        else
                        {
                            ShowMessage("Failed to deactivate user.", "Error", MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Error deactivating user: {ex.Message}", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                ShowMessage("Please select a user to deactivate.", "No Selection");
            }
        }

        private void searchBut_Click(object sender, EventArgs e)
        {
            
            try
            {
                UserRepository userRepository = new UserRepository();
                if(comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "All")
                {
                    string role = comboBox1.SelectedItem.ToString();
                    List<User> users = userRepository.GetByRole(role);
                    dataGridView1.DataSource = users;
                    return;
                }

                string searchValue = textBox1.Text.Trim();
                if (string.IsNullOrWhiteSpace(searchValue))
                {
                    MessageBox.Show("Please enter a search term.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                User user = userRepository.GetByUsername(searchValue);
                if (user != null)
                {
                    dataGridView1.DataSource = new List<User> { user };
                }
                else
                {
                    MessageBox.Show("No user found with that username.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


            }
            catch (Exception ex)
            {
                ShowMessage($"Error searching users: {ex.Message}", "Error", MessageBoxIcon.Error);

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "All")
            {
                LoadUsers();
            }
        }

        private void addbut_Click(object sender, EventArgs e)
        {
            AddEditUserForm addForm = new AddEditUserForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void editbut_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["UserID"].Value);
                AddEditUserForm editForm = new AddEditUserForm(userID);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
            else
            {
                ShowMessage("Please select a user to edit.", "No Selection");
            }
        }
    }
}
