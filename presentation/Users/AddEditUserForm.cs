using MicroFinance_Loan.dataaccess.Repositories;
using MicroFinance_Loan.utilities;
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
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MicroFinance_Loan.presentation.Users
{
    public partial class AddEditUserForm : BaseForm
    {
        private bool isEditMode;
        private int editUserID;
        public AddEditUserForm(int userID = 0)
        {
            InitializeComponent();
            isEditMode = userID > 0;
            editUserID = userID;
            string[] role = new string[] {
                "Member","LoanOfficer","Admin" };
            roleComboBox.DataSource = role;
            string[] dept = new string[] {
                "HR","Operation"};
            comboBox2.DataSource = dept;
            roleComboBox.SelectedIndex = 0;

            this.Text = isEditMode ? "Edit User" : "Add New User";

            if (isEditMode)
            {
                LoadUserData();
            }
        }
        private void LoadUserData()
        {
            try
            {
                if (editUserID > 0)
                {
                    UserRepository userRepo = new UserRepository();
                    User user = userRepo.GetById(editUserID);

                    if (user != null)
                    {
                        nameTextBox.Text = user.Name;
                        userNameTextBox.Text = user.Username;
                        userNameTextBox.ReadOnly = true;
                        roleComboBox.Text = user.Role;
                        roleComboBox.Enabled = false;

                        passwordTextBox.Visible = false;
                        confirmtextBox.Visible = false;

                        if (user.Role == "Member")
                        {
                            MemberRepository memberRepo = new MemberRepository();
                            Member member = memberRepo.GetById(editUserID);
                            if (member != null)
                            {
                                nidTextBox.Text = member.NID;
                                addressTextBox.Text = member.Address;
                                phoneTextBox.Text = member.Phone;
                                incomeTextBox.Text = member.Income.ToString();

                            }
                        }
                        if(user.Role == "Admin")
                        {
                            AdminRepository adminRepository = new AdminRepository();
                            Admin admin = adminRepository.GetById(editUserID);
                            if (admin != null)
                            {
                                comboBox2.Text = admin.Department;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading user data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void AddEditUserForm_Load(object sender, EventArgs e)
        {
            label6.Text = isEditMode ? "Edit User" : "Add New User";
            passwordTextBox.PasswordChar = '●';
            confirmtextBox.PasswordChar = '●';
            button1.Text = isEditMode ? "Update" : "Save";
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            UserManagementForm umf = new UserManagementForm();
            this.Hide();
            umf.Show();
        }

        private void roleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Visible = roleComboBox.Text == "Member";
            panel3.Visible = roleComboBox.Text == "Admin";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                if (roleComboBox.Text == "Member")
                {
                    SaveMember();
                }else if (roleComboBox.Text == "Admin")
                {
                    SaveAdmin();
                }
                else
                {
                    SaveLoanOfficer();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving user: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        private void SaveMember()
        {
            Member member = new Member
            {
                Name = nameTextBox.Text.Trim(),
                Username = userNameTextBox.Text.Trim(),
                Role = "Member",
                NID = nidTextBox.Text.Trim(),
                Address = addressTextBox.Text.Trim(),
                Phone = phoneTextBox.Text.Trim(),
                Income = Convert.ToDecimal(incomeTextBox.Text),
            };

            UserService userService = new UserService();
            bool success;

            if (isEditMode)
            {
                MemberRepository memberRepository = new MemberRepository();
                Member existingMember = memberRepository.GetById(editUserID);
                member.UserID = editUserID;
                member.IsActive = existingMember.IsActive;
                member.PasswordHash = existingMember.PasswordHash;
                success = userService.UpdateMember(member);
            }
            else
            {
                success = userService.CreateMember(member, passwordTextBox.Text);
            }

            if (success)
            {
                ShowMessage($"Member {(isEditMode ? "updated" : "created")} successfully!", "Success");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowMessage($"Failed to {(isEditMode ? "update" : "create")} member.", "Error", MessageBoxIcon.Error);
            }
        }

        private void SaveAdmin()
        {
            Admin admin = new Admin
            {
                Department = comboBox2.Text.Trim(),
                Name = nameTextBox.Text.Trim(),
                Username = userNameTextBox.Text.Trim(),
                Role = "Admin",
            };
            UserService userService = new UserService();
            bool success;

            if (isEditMode)
            {
                AdminRepository adminRepository = new AdminRepository();
                UserRepository userRepository = new UserRepository();
                User user = userRepository.GetById(editUserID);
                admin.UserID = editUserID;
                admin.IsActive = user.IsActive;
                admin.PasswordHash = user.PasswordHash;
                success = adminRepository.Update(admin) && userRepository.Update(user);
            }
            else
            {
                success = userService.CreateAdmin(admin, passwordTextBox.Text);
            }

            if (success)
            {
                ShowMessage($"Admin {(isEditMode ? "updated" : "created")} successfully!", "Success");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowMessage($"Failed to {(isEditMode ? "update" : "create")} admin.", "Error", MessageBoxIcon.Error);
            }
        }

        private void SaveLoanOfficer()
        {

            LoanOfficer lo = new LoanOfficer
            {
                Name = nameTextBox.Text.Trim(),
                Username = userNameTextBox.Text.Trim(),
                Role = "LoanOfficer",
            };
            UserRepository userRepository = new UserRepository();
            LoanOfficerRepository loanOfficerRepository = new LoanOfficerRepository();
            bool success;
            if (isEditMode)
            {
                User user = userRepository.GetById(editUserID);
             
                if (user != null) {
                    lo.UserID = editUserID;
                    lo.IsActive = user.IsActive;
                    lo.PasswordHash = user.PasswordHash;
                    lo.HireDate = loanOfficerRepository.GetById(editUserID).HireDate;
                }
                success = userRepository.Update(lo) && loanOfficerRepository.Update(lo);
            }
            else
            {
                AuthenticationService authService = new AuthenticationService();
                lo.PasswordHash = authService.HashPassword(passwordTextBox.Text);
                lo.CreatedDate = DateTime.Now;
                lo.IsActive = true;
                success = userRepository.Insert(lo) && loanOfficerRepository.Insert(lo);
            }

            if (success)
            {
                ShowMessage($"LoanOfficer {(isEditMode ? "updated" : "created")} successfully!", "Success");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowMessage($"Failed to {(isEditMode ? "update" : "create")} LoanOfficer.", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                ShowMessage("Name is required.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(userNameTextBox.Text))
            {
                ShowMessage("Username is required.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (!isEditMode)
            {
                if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
                {
                    ShowMessage("Password is required.", "Validation Error", MessageBoxIcon.Warning);
                    return false;
                }

                if (passwordTextBox.Text != confirmtextBox.Text)
                {
                    ShowMessage("Passwords do not match.", "Validation Error", MessageBoxIcon.Warning);
                    return false;
                }

                if (!ValidationHelper.IsStrongPassword(passwordTextBox.Text))
                {
                    ShowMessage("Password must be at least 8 characters with uppercase, lowercase, digit, and special character.",
                        "Validation Error", MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (roleComboBox.SelectedItem == null)
            {
                ShowMessage("Please select a role.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (roleComboBox.Text == "Member")
            {
                if (!ValidateMemberFields())
                    return false;
            }

            if(roleComboBox.Text == "Admin")
            {
                if (string.IsNullOrWhiteSpace(comboBox2.Text))
                {
                    ShowMessage("Please select a department for Admin.", "Validation Error", MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private bool ValidateMemberFields()
        {
            if (string.IsNullOrWhiteSpace(nidTextBox.Text))
            {
                ShowMessage("NID is required for members.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (!ValidationHelper.IsValidNID(nidTextBox.Text))
            {
                ShowMessage("Please enter a valid NID (10, 13, or 17 digits).", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(phoneTextBox.Text) && !ValidationHelper.IsValidPhoneNumber(phoneTextBox.Text))
            {
                ShowMessage("Please enter a valid phone number (01XXXXXXXXX).", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(incomeTextBox.Text, out decimal income) || income < 0)
            {
                ShowMessage("Please enter a valid income amount.", "Validation Error", MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}

