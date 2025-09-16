using Microfinance_Loan_Management_System.BusinessLogic.Services;
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
    public partial class LoanSummaryReport : BaseForm
    {
        private int n;
        public LoanSummaryReport(int n = 0)
        {
            InitializeComponent();
            this.n = n;
            GenerateReport();
        }

        private void LoanSummaryReport_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReportService reportService = new ReportService();
            DataTable dt = reportService.GetLoanSummaryReport();

            string reportText = ConvertDataTableToText(dt);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog.FileName = "LoanSummaryReport.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, reportText);
                MessageBox.Show("Report saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void GenerateReport()
        {
            ReportService reportService = new ReportService();
            DataTable dt = reportService.GetLoanSummaryReport();
            dataGridView1.DataSource = dt;
        }

        private string ConvertDataTableToText(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("----------------------Loan Summary Report--------------------------\n");
            foreach (DataColumn col in dt.Columns)
            {
                sb.Append(col.ColumnName.PadRight(20));
            }
            sb.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    sb.Append(item.ToString().PadRight(20));
                }
                sb.AppendLine();
            }

            sb.Append("----------------------------------------------------------------------\n");

            return sb.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (n == 0)
            {
                ReportsMainForm reportsMainForm = new ReportsMainForm();
                reportsMainForm.Show();
                this.Close();
                return;
            }
            else
            {
                AdminDashboard adminDashboard = new AdminDashboard();
                adminDashboard.Show();
                this.Close();
                return;
            }
        }
    }
}
