using MicroFinance_Loan.dataaccess.Repositories;
using MicroFinance_Loan.utilities;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Microfinance_Loan_Management_System.Presentation.Authentication
{
    public partial class SignUpForm : Form
    {
        public SignUpForm()
        {
            InitializeComponent();
            passwordTextBox.UseSystemPasswordChar = true;
            confirmtextBox.UseSystemPasswordChar = true;

            this.Text = "Sign Up - " + AppSettings.ApplicationName;
            string[] role = new string[] {
                "Member","LoanOfficer","Admin" };
            roleComboBox.DataSource = role;
            string[] dept = new string[] {
                "HR","Operation"};
            comboBox2.DataSource = dept;
            roleComboBox.SelectedIndex = 0; 
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SignUpForm_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AuthenticationService _userService = new AuthenticationService();
            MemberRepository memberRepository = new MemberRepository();
            AdminRepository adminRepository = new AdminRepository();
            LoanOfficerRepository loanOfficerRepository = new LoanOfficerRepository();

            try
            {
                if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(userNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(passwordTextBox.Text) ||
                    string.IsNullOrWhiteSpace(confirmtextBox.Text))
                {
                    MessageBox.Show("Please fill in all required fields!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (passwordTextBox.Text != confirmtextBox.Text)
                {
                    MessageBox.Show("Passwords do not match!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
 

                string role = roleComboBox.SelectedItem.ToString();
                User user = null;

                if (role == "Member")
                {
                    if (string.IsNullOrWhiteSpace(nidTextBox.Text) || string.IsNullOrWhiteSpace(incomeTextBox.Text))
                    {
                        MessageBox.Show("Please provide NID and Income for members!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(!ValidationHelper.IsValidNID(nidTextBox.Text))
                    {
                        MessageBox.Show("Invalid NID format!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if(!ValidationHelper.IsValidAmount(decimal.Parse(incomeTextBox.Text)))
                    {
                        MessageBox.Show("Income must be a valid Amount!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if(!ValidationHelper.IsValidPhoneNumber(phoneTextBox.Text))
                    {
                        MessageBox.Show("Invalid Phone Number!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (memberRepository.GetByNID(nidTextBox.Text))
                    {
                        MessageBox.Show("A member with this NID already exists!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    user = new Member
                    {
                        NID = nidTextBox.Text,
                        Phone = phoneTextBox.Text,
                        Address = addressTextBox.Text,
                        Income = decimal.Parse(incomeTextBox.Text)
                    };
                 

                }
                else if (role == "LoanOfficer")
                {
                    user = new LoanOfficer();
                   
                }
                else if (role == "Admin")
                {
                    user = new Admin
                    {
                        Department = comboBox2.SelectedItem.ToString()
                    };
                   
                }
                user.Name = nameTextBox.Text;
                user.Username = userNameTextBox.Text;
                string password = passwordTextBox.Text;
                user.Role = role;

                bool result = _userService.Register(user, password);
                if(role == "Member" && result)
                {
                    memberRepository.InsertMember((Member)user);
                }
                else if(role == "Admin" && result)
                {
                    adminRepository.Insert((Admin)user);
                }
                else
                {
                    if (result)
                    {
                        loanOfficerRepository.Insert((LoanOfficer)user);
                    }
                }

                if (result)
                {
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to register user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void passwordLable_Click(object sender, EventArgs e)
        {

        }

        private void roleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string role = roleComboBox.SelectedItem.ToString();

            nidLabel.Visible = false; nidTextBox.Visible = false;
            phoneLabel.Visible = false; phoneTextBox.Visible = false;
            label3.Visible = false; addressTextBox.Visible = false;
            label4.Visible = false; incomeTextBox.Visible = false;
            label5.Visible = false; comboBox2.Visible = false;

            if (role == "Member")
            {
                nidLabel.Visible = true; nidTextBox.Visible = true;
                phoneLabel.Visible = true; phoneTextBox.Visible = true;
                label3.Visible = true; addressTextBox.Visible = true;
                label4.Visible = true; incomeTextBox.Visible = true;
            }
            else if (role == "Admin")
            {
                label5.Visible = true; comboBox2.Visible = true;
            }
        }
    }
}
