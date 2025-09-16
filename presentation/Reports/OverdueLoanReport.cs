using MicroFinance_Loan.presentation.Dashboards;
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
    public partial class OverdueLoanReport : BaseForm
    {
        private int n;
        public OverdueLoanReport(int n = 0)
        {
            InitializeComponent();
            this.n = n;
            GenerateReport();
        }

        private void OverdueLoanReport_Load(object sender, EventArgs e)
        {

        }

        private void GenerateReport()
        {
            ReportService reportService = new ReportService();
            DataTable dt = reportService.GetOverdueLoansReport();
            dataGridView1.DataSource = dt;
        }

        private string ConvertDataTableToText(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("----------------------OverDue Loan Report--------------------------\n");
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

        private void button1_Click(object sender, EventArgs e)
        {
            ReportService reportService = new ReportService();
            DataTable dt = reportService.GetOverdueLoansReport();

            string reportText = ConvertDataTableToText(dt);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog.FileName = "OverDueReport.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, reportText);
                MessageBox.Show("Report saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(n == 0) 
            {
                ReportsMainForm reportsMainForm = new ReportsMainForm();
                reportsMainForm.Show();
                this.Hide();
                return;
            }else if(n == 1)
            {
                AdminDashboard adminDashboard = new AdminDashboard();
                adminDashboard.Show();
                this.Hide();
                return;
            }
            else
            {
                OfficerDashboard officerDashboard = new OfficerDashboard();
                officerDashboard.Show();
                this.Hide();
                return;
            }
            
        }
    }
}
