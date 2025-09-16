using MicroFinance_Loan.presentation.Dashboards;
using MicroFinance_Loan.utilities;
using Microfinance_Loan_Management_System.BusinessLogic.Models;
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

namespace MicroFinance_Loan.presentation.Settings
{
    public partial class UserProfileForm : BaseForm
    {
        public UserProfileForm()
        {
            InitializeComponent();
           
        }

        private void UserProfileForm_Load(object sender, EventArgs e)
        {
            MemberRepository memberRepo = new MemberRepository();
            Member member = memberRepo.GetById(CurrentUser.UserID);
            if (member != null)
            {
                nameTextBox.Text = member.Name;
                userNameTextBox.Text = member.Username;
                phoneTextBox.Text = member.Phone;
                nidTextBox.Text = member.NID;
                addressTextBox.Text = member.Address;
                incomeTextBox.Text = member.Income.ToString();
            }
            else
            {
              ShowMessage($"Error loading user profile. ID: {CurrentUser.UserID}", "Error", MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserRepository userRepo = new UserRepository();
            User us = userRepo.GetByUsername(userNameTextBox.Text);
            if (us != null && us.UserID != CurrentUser.UserID)
            {
                ShowMessage("Username already taken. Please choose a different username.", "Error", MessageBoxIcon.Error);
                return;
            }
            if(!ValidationHelper.IsValidPhoneNumber(phoneTextBox.Text))
            {
                ShowMessage("Invalid phone number format.", "Error", MessageBoxIcon.Error);
                return;
            }
            if(!ValidationHelper.IsValidNID(nidTextBox.Text))
            {
                ShowMessage("Invalid NID format.", "Error", MessageBoxIcon.Error);
                return;
            }
            if(string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(userNameTextBox.Text)
                || string.IsNullOrWhiteSpace(phoneTextBox.Text) || string.IsNullOrWhiteSpace(nidTextBox.Text)
                || string.IsNullOrWhiteSpace(addressTextBox.Text) || string.IsNullOrWhiteSpace(incomeTextBox.Text))
            {
                ShowMessage("All fields are required.", "Error", MessageBoxIcon.Error);
                return;
            }
            User user = new Member
            {
                UserID = CurrentUser.UserID,
                Name = nameTextBox.Text,
                Username = userNameTextBox.Text,
                PasswordHash = CurrentUser.PasswordHash,
                IsActive = CurrentUser.IsActive,
                Phone = phoneTextBox.Text,
                NID = nidTextBox.Text,
                Address = addressTextBox.Text,
                Income = decimal.TryParse(incomeTextBox.Text, out decimal income) ? income : 0
            };
            try
            {
                MemberRepository memberRepo = new MemberRepository();
                memberRepo.Update((Member)user);
                ShowMessage("Profile updated successfully.", "Success", MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating profile: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            MemberDashboard memberDashboard = new MemberDashboard();
            memberDashboard.Show();
            this.Close();
        }
    }
}
