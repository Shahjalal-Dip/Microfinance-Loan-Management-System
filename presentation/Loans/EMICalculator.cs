using Microfinance_Loan_Management_System.BusinessLogic.Services;
using Microfinance_Loan_Management_System.presentation.Base;
using Microfinance_Loan_Management_System.Presentation.Dashboards;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroFinance_Loan.presentation.Loans
{
    public partial class EMICalculator : BaseForm
    {
        public EMICalculator()
        {
            InitializeComponent();
        }

        private void EMICalculator_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            EMICalculator_Load();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            EMICalculator_Load();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            EMICalculator_Load();
        }

        private void EMICalculator_Load()
        {
            decimal principleAmount;
            decimal.TryParse(textBox1.Text, out principleAmount);     

            decimal annualInterestRate;
            decimal.TryParse(textBox2.Text, out annualInterestRate);
            
            int duration;
            int.TryParse(textBox3.Text, out duration);
            if (principleAmount <= 0 || annualInterestRate < 0 || duration <= 0)
            {
                label5.Text = "Please enter valid inputs for calculation.";
                return;
            }

            LoanService loanService = new LoanService();
            decimal emi = loanService.CalculateEMI(principleAmount, annualInterestRate, duration);
            decimal totalPayable = emi * duration;
            decimal totalInterest = totalPayable - principleAmount;

            label5.Text = $"Monthly EMI: ৳ {emi:N0}\n" +
                          $"Total Payable: ৳ {totalPayable:N0}\n" +
                          $"Total Interest: ৳ {totalInterest:N0}\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminDashboard adminDashboard = new AdminDashboard();
            adminDashboard.Show();
            this.Hide();
        }
    }
}
