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

namespace MicroFinance_Loan.presentation.Reports
{
    public partial class ReportsMainForm : BaseForm
    {
        public ReportsMainForm()
        {
            InitializeComponent();
        }

        private void ReportsMainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoanSummaryReport loanSummaryReport = new LoanSummaryReport(0);
            loanSummaryReport.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CollectionReport collectionReport = new CollectionReport(0);
            collectionReport.ShowDialog();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProfitLossReport profitLossReport = new ProfitLossReport(0);
            profitLossReport.ShowDialog();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OverdueLoanReport overdueLoanReport = new OverdueLoanReport(0);
            overdueLoanReport.ShowDialog();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AdminDashboard adminDashboard = new AdminDashboard();
            adminDashboard.Show();
            this.Close();
            return;
        }
    }
}
